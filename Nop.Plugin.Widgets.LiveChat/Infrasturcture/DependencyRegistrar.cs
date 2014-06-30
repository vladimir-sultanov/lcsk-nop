using System.Web;
using Autofac;
using Autofac.Core;
using Autofac.Integration.Mvc;
using Nop.Core.Caching;
using Nop.Core.Infrastructure;
using Nop.Core.Infrastructure.DependencyManagement;
using Nop.Plugin.Widgets.LiveChat.Controllers;
using Nop.Plugin.Widgets.LiveChat.LCSK;
using Owin;
using Nop.Data;
using Nop.Plugin.Widgets.LiveChat.Data;
using Nop.Core.Data;
using Nop.Web.Framework.Mvc;

namespace Nop.Plugin.Widgets.LiveChat.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        public virtual void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<LiveChatService>().As<ILiveChatService>().InstancePerHttpRequest();

            //data context
            this.RegisterPluginDataContext<LiveChatObjectContext>(builder, "nop_object_context_live_chatlog");

            //override required repository with our custom context
            builder.RegisterType<EfRepository<LiveChatLog>>()
                .As<IRepository<LiveChatLog>>()
                .WithParameter(ResolvedParameter.ForNamed<IDbContext>("nop_object_context_live_chatlog"))
                .InstancePerHttpRequest();
        }

        public int Order
        {
            get { return 2; }
        }
    }
}
