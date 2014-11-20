using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Threading.Tasks;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Net.Mail;
using System.Collections.Concurrent;
using Nop.Services.Messages;
using Nop.Plugin.ActiveForever.ActiveChat.Hubs;
using Nop.Plugin.Widgets.LiveChat.Models;
using Nop.Plugin.Widgets.LiveChat.Data;
using System.Xml.Serialization;
using Nop.Core.Infrastructure;
using Nop.Services.Localization;


namespace Nop.Plugin.Widgets.LiveChat.LCSK
{    
    public class ChatHub : Hub
    {

        #region Constants

        private string CONFIG_FILE = "lcsk.dat";

        private string CacheKeyChatClient = "lcsk_chatclients";
        private string CacheKeyAgents = "lcsk_agents";

        private string UiStringYouInvitedThisVisitorToChat = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.YouInvitedThisVisitorToChat"); 
        private string UiStringNoAgentAreCurrentlyAvailable = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.NoAgentAreCurrentlyAvailable");
        private string UiStringLetMeKnowIfYouHaveAnyQuestions = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.LetMeKnowIfYouHaveAnyQuestions");
        private string UiStringThisVisitorAppearToHaveLost = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.ThisVisitorAppearToHaveLost");
        private string UiStringTheAgentWasDisconnectedFromChat = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.TheAgentWasDisconnectedFromChat");
        private string UiStringWeWereUnableToSendYourMessage = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.WeWereUnableToSendYourMessage");
        private string UiStringTheAgentCloseTheChatSession = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.TheAgentCloseTheChatSession");
        private string UiStringTheVisitorCloseTheConnection = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.TheVisitorCloseTheConnection");
        private string UiStringSystem = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.System");
        private string UiStringVisitor = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.Visitor");
        private string UiStringYou = EngineContext.Current.Resolve<ILocalizationService>().GetResource("Nop.Plugin.Widgets.LiveChat.ChatHub.You");
        

        #endregion

        #region Fields

        private static Dictionary<string, Agent> _agents;

        private static Dictionary<string, ChatClient> _chatClients;

        private static Dictionary<string, Agent> Agents
        {
            get
            {
                if (_agents == null)
                    _agents = new Dictionary<string, Agent>();                                    
                    return _agents;                
            }
        }
        private Dictionary<string, ChatClient> ChatClients
        {
            get
            {
                if (_chatClients == null)
                    _chatClients = new Dictionary<string, ChatClient>();
                return _chatClients;                
            }
        }    

        private static ConcurrentDictionary<string, string> ChatSessions;

        private ILiveChatService _liveChatService;

        #endregion 

        #region Connection Work

        public void AgentConnect(string name)
        {
            if (ChatSessions == null)
                ChatSessions = new ConcurrentDictionary<string, string>();

            var agent = new Agent()
            {
                Id = Context.ConnectionId,
                Name = name,
                IsOnline = true
            };

            Agents.Add(agent.Id, agent);

            Clients.Caller.loginResult(true, agent.Id, agent.Name);

            Clients.All.onlineStatus(Agents.Values.Count(x => x.IsOnline) > 0);
        }

        public void ChangeStatus(bool online)
        {       
            Agent agent = CurrentAgent;
            if (agent != null)
            {
                agent.IsOnline = online;

                // TODO: Check if the agent was in chat sessions.

                Clients.All.onlineStatus(Agents.Values.Count(x => x.IsOnline) > 0);
            }
        }
        public void EngageVisitor(string connectionId)
        {
            var agent = CurrentAgent;
            if(agent != null)
            {
                ChatSessions.TryAdd(connectionId, agent.Id);

                Clients.Caller.newChat(connectionId);

                Clients.Client(connectionId).setChat(connectionId, agent.Name, false);

                Clients.Caller.addMessage(connectionId, UiStringSystem, UiStringYouInvitedThisVisitorToChat);

                Clients.Client(connectionId).addMessage(agent.Name, string.Format(UiStringLetMeKnowIfYouHaveAnyQuestions, agent.Name));
            }
        }
        public void LogVisit(string page, string referrer, string existingChatId)
        {
            Clients.Caller.onlineStatus(Agents.Values.Count(x => x.IsOnline) > 0);

            if (!string.IsNullOrEmpty(existingChatId) &&
                ChatSessions.ContainsKey(existingChatId))
            {
                var agentId = ChatSessions[existingChatId];
                Clients.Client(agentId).visitorSwitchPage(existingChatId, Context.ConnectionId, page);

                var agent = Agents.Values.SingleOrDefault(x => x.Id == agentId);

                if (agent != null)
                    Clients.Caller.setChat(Context.ConnectionId, agent.Name, true);

                string buffer = "";
                ChatSessions.TryRemove(existingChatId, out buffer);

                ChatSessions.TryAdd(Context.ConnectionId, agentId);
            }

            foreach (var agent in Agents.Values)
            {
                var chatWith = (from c in ChatSessions
                               join a in Agents on c.Value equals a.Value.Id
                               where c.Key == Context.ConnectionId
                               select a.Value.Name).SingleOrDefault();

                Clients.Client(agent.Id).newVisit(page, referrer, chatWith, Context.ConnectionId);
            }
        }

        public void RequestChat(ChatClient client, string message)
        {
            SendToChat(client, message, true);
        }
        public void Send(string data)
        {
            SendToChat(null, data, false);
        }
        public void OpSend(string id, string data)
        {
            if (string.IsNullOrEmpty(id)) return;
            var agent = CurrentAgent;
            if (agent == null)
            {
                Clients.Caller.addMessage(id, UiStringSystem, UiStringWeWereUnableToSendYourMessage);
                return;
            }

            if (id == "internal")
            {
                foreach (var a in Agents.Values.Where(x => x.IsOnline))
                    Clients.Client(a.Id).addMessage(id, agent.Name, data);
                        
            }
            else if (ChatSessions.ContainsKey(id))
            {                
                Clients.Caller.addMessage(id, "you", data);                
                Clients.Client(id).addMessage(agent.Name, data);
                ChatClient client = GetChatClient(id);
                LogMessage(client, agent.Name, string.Format("{0} : {1}", agent.Name,data));
            }
        }

        public void CloseChat(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            if (ChatSessions.ContainsKey(id))
            {
                Clients.Client(id).addMessage("", UiStringTheAgentCloseTheChatSession);
                string buffer = "";
                ChatSessions.TryRemove(id, out buffer);
            }
        }
        public void LeaveChat(string id)
        {
            if (string.IsNullOrEmpty(id)) return;
            // was it an agent
            var agent = Agents.ContainsKey(id) ? Agents[id] : null;
            if (agent != null)
            {
                Agents.Remove(id);

                var sessions = ChatSessions.Where(x => x.Value == agent.Id);
                if(sessions != null)
                {
                    foreach(var session in sessions)
                        Clients.Client(session.Key).addMessage("", UiStringTheAgentWasDisconnectedFromChat);
                }

                Clients.All.updateStatus(Agents.Values.Count(x => x.IsOnline) > 0);
            }

            // was it a visitor
            if (ChatSessions.ContainsKey(id))
            {
                var agentId = ChatSessions[id];
                Clients.Client(agentId).addMessage(id, UiStringSystem, UiStringTheVisitorCloseTheConnection);
                ChatClient client = GetChatClient(id);
                SaveChat(id, client);
                string buffer = "";
                ChatSessions.TryRemove(id, out buffer);
                ChatClients.Remove(id);
            }            
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            return Clients.All.leave(Context.ConnectionId);
        }

        #endregion

        #region Methods

        private Agent CurrentAgent
        {
            get
            {
                return Agents.ContainsKey(Context.ConnectionId)? Agents[Context.ConnectionId] : null;
            }
        }
        //private ChatClient GetNewChatClient(string connectionId, string name)
        //{
        //    ChatClient client = new ChatClient() 
        //                {
        //                    ConnectionId = connectionId,
        //                    Name = name,
        //                    ChatMessages = new List<LiveChatMessage>()
        //                };

        //    return client;
        //}
        
        private ChatClient GetChatClient(string connectionId)
        {
            ChatClient client = null;
            if(ChatClients.ContainsKey(connectionId))
            {
                client = ChatClients[connectionId];
            }

            return client;
        }

        private void LogMessage(ChatClient client, string agentName, string message)
        {
            client.ChatMessages.Add(new LiveChatMessage()
                                    {
                                        ClientName = client.Name,
                                        AgentName = agentName,
                                        Message = message,
                                        MessageTime = DateTime.Now
                                    });
        }

        private Agent GetLessBuzyAgent()
        {
            Agent lessBuzyAgent = null;
            // We assign the chat to the less buzy agent            
            var workload = Agents.Values.Where(a => a.IsOnline)
                                 .Select(a => new Agent()
                                {
                                    Id = a.Id,
                                    Name = a.Name,
                                    ChatSessionsCount = ChatSessions.Count(x => x.Value == a.Id)
                                });
            if (workload != null)
            {
                lessBuzyAgent = workload.OrderBy(x => x.ChatSessionsCount).FirstOrDefault();    
            }

            return lessBuzyAgent;
        }
        private void StartChat(string connectionId, string agentId, string agentName)
        {
            ChatSessions.TryAdd(connectionId, agentId);
            Clients.Client(agentId).newChat(connectionId);
            Clients.Caller.setChat(connectionId, agentName, false);
        }
        private void SendToChat(ChatClient client, string message, bool isRequest)
        {            
            Agent agent = null;            
            string connectionId = Context.ConnectionId;
            string agentId = "";

            if (!isRequest && ChatSessions.ContainsKey(connectionId))
            {
                agentId = ChatSessions[connectionId];
                agent = Agents[agentId];
                client = GetChatClient(connectionId);
            }
            else
            {
                if(client != null)
                {
                    //Add new chat client
                    client.ChatMessages = new List<LiveChatMessage>();
                    client.ConnectionId = connectionId;
                    if (!ChatClients.ContainsKey(connectionId))
                    {
                        ChatClients.Add(connectionId, client);
                    }
                    else
                    {
                        ChatClients[connectionId] = client;
                    }
                }
                agent = GetLessBuzyAgent();
                if (agent != null)
                {
                    StartChat(connectionId, agent.Id, agent.Name);                                 
                }
                else
                {
                    Clients.Caller.addMessage("", UiStringNoAgentAreCurrentlyAvailable);
                    return;
                }
            }

            Clients.Caller.addMessage(client.Name, message);
            LogMessage(client, agent.Name, string.Format("{0} : {1}", client.Name , message));

            if (!isRequest)
                Clients.Client(agent.Id).addMessage(connectionId, UiStringSystem, UiStringThisVisitorAppearToHaveLost);

            Clients.Client(agent.Id).addMessage(connectionId, UiStringVisitor, message);           
        }
        private bool SaveChat(string id, ChatClient client)
        {
            this._liveChatService = (ILiveChatService)EngineContext.Current.Resolve<ILiveChatService>();
            bool isSaved = this._liveChatService.SaveChat(id, client);
            return isSaved;            
        }

        #endregion

        #region Email Methods

        #endregion

        #region NopCommerceRelationMethods

        #endregion

        #region Install and config methods

        #endregion
    }
}