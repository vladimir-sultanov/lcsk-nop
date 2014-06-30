using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Core.Data;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Seo;
using Nop.Plugin.Widgets.LiveChat.Data;
using Nop.Services.Configuration;
using System.Xml.Serialization;
using System.IO;
using Nop.Plugin.ActiveForever.ActiveChat.Hubs;
using Nop.Core;

namespace Nop.Plugin.Widgets.LiveChat
{
    public class LiveChatService : ILiveChatService
    {
        #region Fields

        private readonly IRepository<LiveChatLog> _chatLogRepository;
        private readonly LiveChatSettings _liveChatSettings;
        private readonly ISettingService _settingService;

        #endregion

        #region Methods

        public LiveChatService(IRepository<LiveChatLog> chatLogRepository, ISettingService settingService, LiveChatSettings liveChatSettings)
        {
            this._chatLogRepository = chatLogRepository;
            this._liveChatSettings = liveChatSettings;
            this._settingService = settingService;
        }        

        #region Hidden methods
        
        #endregion
#endregion


        #region ILiveChatService Implements
        public IPagedList<LiveChatLog> GetAllChatLogs(int pageIndex, int pageSize)
        {
            var query = _chatLogRepository.Table
                        .Select(clr => clr)
                        .OrderBy(clr => clr.Id);            
            var records = new PagedList<LiveChatLog>(query, pageIndex, pageSize);
            return records;   
        }

        public IList<Data.LiveChatLog> GetRecentChatLogs()
        {
            var query = (_chatLogRepository.Table
                        .Select(clr => clr)
                        .OrderByDescending(clr => clr.Id)).Take(5);
            var records = query.ToList();
            return records;               
        }

        public void InsertChatLog(Data.LiveChatLog chatLog)
        {
            if (chatLog == null) 
                throw new ArgumentException("chatlog null");
            this._chatLogRepository.Insert(chatLog);
        }

        public Data.LiveChatLog GetChatLog(int chatLogId)
        {
            return this._chatLogRepository.GetById(chatLogId);
        }

        public string Import(string fileName)
        {
            throw new NotImplementedException();
        }

        public void Export()
        {
            throw new NotImplementedException();
        }

        public bool SaveChat(string id, ChatClient client)
        {
            bool isSaved = false;
            try
            {
                XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
                ns.Add(string.Empty, string.Empty);

                XmlSerializer serializer = new XmlSerializer(client.ChatMessages.GetType());
                string chatMessagesSerialized = "";
                using (StringWriter textWriter = new StringWriter())
                {
                    serializer.Serialize(textWriter, client.ChatMessages, ns);
                    chatMessagesSerialized = textWriter.ToString();
                }
                LiveChatLog chatLog = new LiveChatLog()
                {
                    SessionId = id,
                    Name = client.Name,
                    Messages = chatMessagesSerialized,
                    Email = client.Email,
                    ClientIp = client.Ip,
                    Url = client.Url,
                    OperatingSystem = client.OperatingSystem,
                    Browser = client.Browser,
                    ConnectedAt = client.ConnectedAt,
                };                
                InsertChatLog(chatLog);
                isSaved = true;
            }
            catch (Exception ex)
            {
                isSaved = false;
                throw ex;
            }
            return isSaved;
        }

        public string GetMessages(string serilizedMessages)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(List<Models.LiveChatMessage>));            
            List<Models.LiveChatMessage> list = null;
            using (StringReader reader = new StringReader(serilizedMessages))
            {
                list = serializer.Deserialize(reader) as List<Models.LiveChatMessage>;                
            }
            string chatMessages = "";
            if(list != null)
            {
                foreach (string s in list.Select(l => l.Message))
                    chatMessages += s + "<br />";
            }            
            return chatMessages;
        }

        #endregion
    }
}
