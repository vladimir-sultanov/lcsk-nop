using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.LiveChat.LCSK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nop.Plugin.Widgets.LiveChat
{
    public class SignalRSHStartUpTask : IStartupTask
    {
        #region fields

        #endregion
        public void Execute()
        {
            string url = EngineContext.Current.Resolve<ILiveChatService>().GetSignaRStartPath();
            SignalRSelfHost.SetUrl(url);
            SignalRSelfHost.Start();    
        }
        public int Order
        {
            //ensure that this task is run first 
            get { return 1; }
        }
    }
}
