using Nop.Core;
using Nop.Plugin.ActiveForever.ActiveChat.Hubs;
using Nop.Plugin.Widgets.LiveChat.Data;
using Nop.Services.Messages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Nop.Plugin.Widgets.LiveChat
{
    public interface ILiveChatService
    {
        IPagedList<LiveChatLog> GetAllChatLogs(int pageIndex, int pageSize);

        IList<LiveChatLog> GetRecentChatLogs();

        void InsertChatLog(LiveChatLog chatLog);

        LiveChatLog GetChatLog(int chatLogId);

        string Import(string fileName);

        void Export();
        bool SaveChat(string id, ChatClient client);
        string GetMessages(string serilizedMessages);
    }
}
