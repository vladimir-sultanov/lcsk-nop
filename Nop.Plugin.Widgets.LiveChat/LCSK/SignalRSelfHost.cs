using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Widgets.LiveChat.LCSK
{
    public class SignalRSelfHost
    {
        #region fields
        private static IDisposable signalr;        
        private static bool _stop = false;
        private static string _url;
        #endregion

        public static void SetUrl(string url)
        {
            _url = url;
        }
        public static void Start()
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses. 
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx 
            // for more information.             
            signalr = WebApp.Start<Startup>(_url);            
        }
        public static void Restart()
        {
            _stop = true;
            signalr.Dispose();
            Start();
        }
    }  
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HubConfiguration { EnableJSONP = true, 
                EnableDetailedErrors = true, 
                Resolver = new DefaultDependencyResolver() };

            app.UseCors(CorsOptions.AllowAll);

            app.MapSignalR(config);
        }
    }
}
