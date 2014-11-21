using Nop.Core.Configuration;

namespace Nop.Plugin.Widgets.LiveChat
{
    public class LiveChatSettings : ISettings
    {
        /// <summary>
        /// Email scoount for Offline message
        /// </summary>
        public int SelectedEmailAccountId { get; set; }

        /// <summary>
        /// Signal R port
        /// </summary>
        public int SignalRPort { get; set; }
    }
}