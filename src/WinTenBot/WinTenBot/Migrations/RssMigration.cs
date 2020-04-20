using FluentMigrator;

namespace WinTenBot.Migrations
{
    [Migration(202003141720)]
    public class RssMigration : Migration
    {
        public override void Up()
        {
            if (!Schema.Table("rss_setting").Exists())
                Create.Table("rss_settings")
                    .WithColumn("id").AsInt32().NotNullable().PrimaryKey().Identity()
                    .WithColumn("chat_id").AsString(30).NotNullable()
                    .WithColumn("from_id").AsString(20).NotNullable()
                    .WithColumn("url_feed").AsString(255).NotNullable()
                    .WithColumn("created_at").AsDateTime().WithDefault(SystemMethods.CurrentDateTime)
                    .WithColumn("updated_at").AsDateTime().WithDefault(SystemMethods.CurrentDateTime);
        }

        public override void Down()
        {
            Delete.Table("rss_settings");
        }
    }
}