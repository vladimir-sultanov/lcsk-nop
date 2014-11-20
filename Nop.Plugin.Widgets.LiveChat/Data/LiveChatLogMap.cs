using System.Data.Entity.ModelConfiguration;

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