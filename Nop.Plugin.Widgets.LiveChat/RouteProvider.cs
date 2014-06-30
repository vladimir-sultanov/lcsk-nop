using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Mvc.Routes;

namespace Nop.Plugin.Widgets.LiveChat
{
    public partial class RouteProvider : IRouteProvider
    {
        public void RegisterRoutes(RouteCollection routes)
        {
            //routes.MapRoute("Plugin.ExternalAuth.Facebook.Login",
            //     "Plugins/ExternalAuthFacebook/Login",
            //     new { controller = "ExternalAuthFacebook", action = "Login" },
            //     new[] { "Nop.Plugin.ExternalAuth.Facebook.Controllers" }
            //);

            //routes.MapRoute("Plugin.ExternalAuth.Facebook.Login",
            //     "Plugins/ExternalAuthFacebook/Login",
            //     new { controller = "ExternalAuthFacebook", action = "Login" },
            //     new[] { "Nop.Plugin.ExternalAuth.Facebook.Controllers" }
            //);

            routes.MapRoute("Nop.Plugin.Widgets.LiveChat.SendEmailMessage",
                 "SendEmailMessage",
                 new { controller = "LiveChat", action = "SendEmailMessage" },
                 new[] { "Nop.Plugin.Widgets.LiveChat.Controllers" }
            );
            routes.MapRoute("Nop.Plugin.Widgets.LiveChat.ValidateCaptcha",
                 "ValidateCaptcha",
                 new { controller = "LiveChat", action = "ValidateCaptcha" },
                 new[] { "Nop.Plugin.Widgets.LiveChat.Controllers" }
            );
            routes.MapRoute("Nop.Plugin.Widgets.LiveChat.SendChatHistoryToEmail",
                 "SendChatHistoryToEmail",
                 new { controller = "LiveChat", action = "SendChatHistoryToEmail" },
                 new[] { "Nop.Plugin.Widgets.LiveChat.Controllers" }
            );
            
        }
        public int Priority
        {
            get
            {
                return 0;
            }
        }
    }
}
