using System.Runtime.InteropServices;
using System.Threading.Tasks;
using FluentMigrator;
using Serilog;
using WinTenBot.Providers;

namespace WinTenBot.Migrations
{
    
    [Migration(202003141720)]
    public class RssMigration:Migration
    {
        public override void Up()
        {
            Create.Table("rss_settings2")
                .WithColumn("id").AsInt32().NotNullable().Identity()
                .WithColumn("chat_id").AsString(30).NotNullable()
                .WithColumn("from_id").AsString(20).NotNullable()
                .WithColumn("url_feed").AsString(255).NotNullable()
                .WithColumn("created_at").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                .WithColumn("updated_at").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);
        }

        public override void Down()
        {
            Delete.Table("rss_settings2");
        }
    }
}