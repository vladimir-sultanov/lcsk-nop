// Type: Nop.Plugin.ActiveForever.ActiveChat.Data.ClientChatLog
// Assembly: Nop.Plugin.ActiveForever.ActiveChat, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: D9B5B96A-2F21-4797-AA5E-CC11D3FF63A9
// Assembly location: C:\CurrentProjects\Flowershop\NopCommerceOthers\68199_6449_ActiveChat 1.80\ActiveChat 1.80\3.10\ActiveForever.ActiveChat\Nop.Plugin.ActiveForever.ActiveChat.dll

using Nop.Core;
using System;

namespace Nop.Plugin.Widgets.LiveChat.Data
{
  public class LiveChatLog : BaseEntity
  {
    public virtual string SessionId { get; set; }

    public virtual string Name { get; set; }

    public virtual string Email { get; set; }

    public virtual string ConnectedAt { get; set; }    

    public virtual string Url { get; set; }

    public virtual string Browser { get; set; }

    public virtual string OperatingSystem { get; set; }

    public virtual string ClientIp { get; set; }

    public virtual string Messages { get; set; }

  }
}
