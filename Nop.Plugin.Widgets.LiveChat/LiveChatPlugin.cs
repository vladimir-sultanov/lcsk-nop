using System.Collections.Generic;
using System.Web.Routing;
using Nop.Core.Plugins;
using Nop.Plugin.Widgets.LiveChat.LCSK;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Messages;
using Nop.Plugin.Widgets.LiveChat.Data;
using Nop.Core.Infrastructure;

namespace Nop.Plugin.Widgets.LiveChat
{
    public class LiveChatPlugin : BasePlugin, IWidgetPlugin, IPlugin
    {
        private readonly ISettingService _settingService;
        private readonly LiveChatObjectContext _objectContext;

        public LiveChatPlugin(ISettingService settingService, LiveChatObjectContext objectContext)
        {
            this._settingService = settingService;
            this._objectContext = objectContext;            
        }

        #region Methods



        /// <summary>
        /// Gets a route for provider configuration
        /// </summary>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetConfigurationRoute(out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "Configure";
            controllerName = "LiveChat";
            routeValues = new RouteValueDictionary() { { "Namespaces", "Nop.Plugin.Widgets.LiveChat.Controllers" }, { "area", null } };
        }

        /// <summary>
        /// Gets a route for displaying widget
        /// </summary>
        /// <param name="widgetZone">Widget zone where it's displayed</param>
        /// <param name="actionName">Action name</param>
        /// <param name="controllerName">Controller name</param>
        /// <param name="routeValues">Route values</param>
        public void GetDisplayWidgetRoute(string widgetZone, out string actionName, out string controllerName, out RouteValueDictionary routeValues)
        {
            actionName = "PublicInfo";
            controllerName = "LiveChat";
            routeValues = new RouteValueDictionary()
            {
                {"Namespaces", "Nop.Plugin.Widgets.LiveChat.Controllers"},
                {"area", null},
                {"widgetZone", widgetZone}
            };
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string>()
            { 
                
                //desktop version (you can also replace it with "head_html_tag")
                "home_page_top"
                //"body_end_html_tag_before", 
                //mobile version
                //"mobile_body_end_html_tag_before" 
            };
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        {
            _objectContext.Install();
            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            _objectContext.Uninstall();
            base.Uninstall();
        }

        #endregion
    }
}
