using Nop.Plugin.Widgets.LiveChat.Models;
using System;
using System.Collections.Generic;

namespace Nop.Plugin.ActiveForever.ActiveChat.Hubs
{
  public class ChatClient
  {
    public Guid SessionId { get; set; }
    public string Name { get; set; }

    public string ConnectionId { get; set; }

    public string OperatorConnectionId { get; set; }

    public string ConnectedAt { get; set; }

    public string Email { get; set; }

    public string Browser { get; set; }

    public string OperatingSystem { get; set; }

    public string Url { get; set; }

    public string Ip { get; set; }

    public List<LiveChatMessage> ChatMessages { get; set; }
  }
}
