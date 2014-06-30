using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.LiveChat.Models
{
    public class LiveChatModel : BaseNopModel
    {
        #region constructors

        public LiveChatModel()
        {

        }

        #endregion

        #region methods
        #endregion

        #region Properties
        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.LiveChatResult")]
        public string LiveChatResult { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.EmailAccounts")]
        public IList<EmailAccount> EmailAccounts { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.SelectedEmailAccountId")]
        public int SelectedEmailAccountId { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.ReCaptchaPublicKey")]
        public string ReCaptchaPublicKey { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.ReCaptchaTheme")]
        public string ReCaptchaTheme { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.ChatClientIp")]
        public string ChatClientIp { get; set; }       

        #endregion
    }
}
