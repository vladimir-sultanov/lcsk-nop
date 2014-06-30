using System.Data.Entity.ModelConfiguration;
//using Nop.Plugin.Feed.Froogle.Domain;

namespace Nop.Plugin.Widgets.LiveChat.Data
{
    public partial class LiveChatLogMap : EntityTypeConfiguration<LiveChatLog>
    {
        public LiveChatLogMap()
        {
            this.ToTable("LiveChatLog");
            this.HasKey(x => x.Id);
        }
    }
}