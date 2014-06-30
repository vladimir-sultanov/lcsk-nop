using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Nop.Plugin.Widgets.LiveChat.LCSK
{
    public class Agent
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public bool IsOnline { get; set; }
        public int ChatSessionsCount { get; set; }

        public Agent()
        {
        }
    }
}