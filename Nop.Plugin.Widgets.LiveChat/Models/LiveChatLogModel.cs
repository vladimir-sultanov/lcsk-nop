using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.LiveChat.Models
{
    public class LiveChatLogModel : BaseNopModel
    {
        #region constructors

        public LiveChatLogModel()
        {

        }

        #endregion

        #region methods
        #endregion

        #region Properties
        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.Id")]
        public virtual int Id { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.SessionId")]
        public virtual string SessionId { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.Name")]
        public virtual string Name { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.Email")]
        public virtual string Email { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.ConnectedAt")]
        public virtual string ConnectedAt { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.Url")]
        public virtual string Url { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.ClientIp")]
        public virtual string ClientIp { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.OperatingSystem")]
        public virtual string OperatingSystem { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.Browser")]
        public virtual string Browser { get; set; }

        [NopResourceDisplayName("Nop.Plugin.Widgets.LiveChat.Models.LiveChatModel.Messages")]
        public virtual string Messages { get; set; }

        #endregion
    }
}
