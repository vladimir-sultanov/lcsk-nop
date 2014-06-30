using Microsoft.Owin;
using Microsoft.AspNet.SignalR;
using Owin;


//[assembly: OwinStartup(typeof(Nop.Plugin.Widgets.LiveChat.LCSK.SignalRStartup))]

namespace Nop.Plugin.Widgets.LiveChat.LCSK
{    
    public class SignalRStartup
    {
        //public static IAppBuilder App = null;

        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();        
           
        }
    }    
}
