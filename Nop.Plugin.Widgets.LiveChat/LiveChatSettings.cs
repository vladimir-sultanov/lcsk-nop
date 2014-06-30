using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.LiveChat
{
    public class LiveChatSettings : ISettings
    {
        /// <summary>
        /// Email scoount for Offline message
        /// </summary>
        public int SelectedEmailAccountId { get; set; }
    }
}