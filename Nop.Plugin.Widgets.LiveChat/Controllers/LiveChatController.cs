using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Security;
using Nop.Services.Messages;
using Nop.Plugin.Widgets.LiveChat.LCSK;
using Nop.Plugin.Widgets.LiveChat.Models;
using System.Net.Mail;
using Nop.Web.Framework.Kendoui;
using Nop.Services.Security;
using Nop.Plugin.Widgets.LiveChat.Data;
using System.Xml.Serialization;
using Nop.Web.Framework.UI.Captcha;
using System.Net;
using System.IO;
using Nop.Web.Framework.Localization;
using Nop.Services.Localization;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Widgets.LiveChat.Controllers
{
    public class LiveChatController : BasePluginController
    {
        //private readonly IImportService _importService;       
        private readonly ISettingService _settingService;
        private readonly IEmailSender _emailSender;
        private readonly IEmailAccountService _emailAccountService;
        private readonly IPermissionService _permissionService;
        private readonly ILiveChatService _liveChatService;
        private readonly LiveChatSettings _liveChatSettings;
        public readonly CaptchaSettings _captchaSettings;

        #region Ctors
        public LiveChatController(ISettingService settingService, IEmailSender emailSender
            , IEmailAccountService emailAccountService, LiveChatSettings liveChatSettings
            , IPermissionService permissionService, ILiveChatService liveChatService,
            CaptchaSettings captchaSettings)
        {
            this._settingService = settingService;
            this._emailAccountService = emailAccountService;
            this._emailSender = emailSender;
            this._liveChatSettings = liveChatSettings;
            this._permissionService = permissionService;
            this._liveChatService = liveChatService;
            this._captchaSettings = captchaSettings;
        }

        #endregion

        #region Web Methods

        [AdminAuthorize]
        [ChildActionOnly]
        public ActionResult Configure()
        {
            LiveChatModel liveChatModel = new LiveChatModel();
            liveChatModel.EmailAccounts = this._emailAccountService.GetAllEmailAccounts().Select
                (
                ea => new EmailAccount() { Id = ea.Id, Name = ea.DisplayName }
                ).ToList();

            return View("Nop.Plugin.Widgets.LiveChat.Views.LiveChat.Configure", liveChatModel);
        }

        [HttpPost]
        [ChildActionOnly]
        [FormValueRequired("save")]
        public ActionResult Configure(LiveChatModel model)
        {
            if (!ModelState.IsValid)
            {
                return Configure();
            }

            //save settings
            _liveChatSettings.SelectedEmailAccountId = model.SelectedEmailAccountId;            
            _settingService.SaveSetting(_liveChatSettings);

            //redisplay the form
            return Configure();
        }

        [ChildActionOnly]
        public ActionResult PublicInfo(string widgetZone)
        {
            LiveChatModel liveChatModel = new LiveChatModel();
            liveChatModel.ReCaptchaPublicKey = _captchaSettings.ReCaptchaPublicKey;
            liveChatModel.ReCaptchaTheme = _captchaSettings.ReCaptchaTheme;
            return View("Nop.Plugin.Widgets.LiveChat.Views.LiveChat.PublicInfo", liveChatModel);
        }

        [Authorize]
        public ActionResult Agent()
        {
            return View("Nop.Plugin.Widgets.LiveChat.Views.LiveChat.Agent");
        }

        [Authorize]
        public ActionResult ChatHistory()
        {
            return View("Nop.Plugin.Widgets.LiveChat.Views.LiveChat.ChatHistory");
        }

        public JsonResult SendEmailMessage(string toEmail, string message)
        {
            string subject = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Plugins.Widgets.LiveChat.Email.OfflineEmailMessageSubject");
            string body = string.Format(EngineContext.Current.Resolve<ILocalizationService>().GetResource("Plugins.Widgets.LiveChat.Email.OfflineEmailMessageBody"), Environment.NewLine, message);
            return SendToEmail(toEmail, subject, body);
        }
        public JsonResult SendChatHistoryToEmail(string toEmail, int chatId)
        {
            LiveChatLog chatLog = _liveChatService.GetChatLog(chatId);
            string subject = string.Format(EngineContext.Current.Resolve<ILocalizationService>().GetResource("Plugins.Widgets.LiveChat.Email.SupportChatHistoryFor"), chatLog.Name);
            string body = _liveChatService.GetMessages(chatLog.Messages);
            return SendToEmail(toEmail, subject, body);         
        }

        [HttpPost]
        public ActionResult ChatHistory(DataSourceRequest command)
        {
            var liveChatLogModels = new List<LiveChatLogModel>();
            foreach (LiveChatLog log in _liveChatService.GetAllChatLogs(command.Page - 1, command.PageSize))
                liveChatLogModels.Add(new LiveChatLogModel()
                {
                    Id = log.Id,
                    SessionId = log.SessionId,
                    Name = log.Name,
                    Email = log.Email,
                    ConnectedAt = log.ConnectedAt,
                    Url = log.Url,
                    ClientIp = log.ClientIp,
                    Browser = log.Browser,
                    OperatingSystem = log.OperatingSystem,                    
                    Messages = _liveChatService.GetMessages(log.Messages)
                });

            var gridModel = new DataSourceResult
            {
                Data = liveChatLogModels,
                Total = liveChatLogModels.Count 
            };
            return Json(gridModel);
        }
        
        [CaptchaValidator]  
        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult CreateComment(Int32 id, bool captchaValid)  
        {
            return
            this.Json((object)new
            {
                result = captchaValid
            }, JsonRequestBehavior.AllowGet); 
        }
        
        public JsonResult ValidateCaptcha(CaptchaRequest captcha)
        {           

            var cientIP = Request.ServerVariables["REMOTE_ADDR"];
            var privateKey = _captchaSettings.ReCaptchaPrivateKey;

            string data = string.Format("privatekey={0}&remoteip={1}&challenge={2}&response={3}", privateKey, cientIP, captcha.Challenge, captcha.Response);
            byte[] byteArray = new ASCIIEncoding().GetBytes(data);

            WebRequest request = WebRequest.Create("http://www.google.com/recaptcha/api/verify");
            request.Credentials = CredentialCache.DefaultCredentials;
            request.Method = "POST";
            request.ContentType = "application/x-www-form-urlencoded";
            request.ContentLength = byteArray.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(byteArray, 0, byteArray.Length);
            dataStream.Close();

            WebResponse response = request.GetResponse();
            var status = (((HttpWebResponse)response).StatusCode);
            dataStream = response.GetResponseStream();
            StreamReader reader = new StreamReader(dataStream);
            string responseFromServer = reader.ReadToEnd();
            reader.Close();
            dataStream.Close();
            response.Close();

            var responseLines = responseFromServer.Split(new string[] { "\n" }, StringSplitOptions.None);
            bool success = responseLines[0].Equals("true");

            return this.Json((object)new
            {
                result = success
            }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Hidden Methods

        private JsonResult SendToEmail(string toEmail, string subject, string body)
        {
            try
            {
                var emailAccount = _emailAccountService.GetEmailAccountById(_liveChatSettings.SelectedEmailAccountId);

                if (emailAccount == null) 
                    return this.Json((object)new
                    {
                        result = false,
                        message = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Plugins.Widgets.LiveChat.Email.EmailNotSent")
                    }, JsonRequestBehavior.AllowGet);

                var from = new MailAddress(emailAccount.Email);
                var to = new MailAddress(toEmail);
                _emailSender.SendEmail(emailAccount, subject, body, from, to);
            }
            catch
            {
            }
            
            return this.Json((object)new
            {
                result = true,
                message =  EngineContext.Current.Resolve<ILocalizationService>().GetResource("Plugins.Widgets.LiveChat.Email.EmailWasSent")
            }, JsonRequestBehavior.AllowGet);

        }

        #endregion

    }
}
