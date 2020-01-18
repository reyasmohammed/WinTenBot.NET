using System.Threading.Tasks;
using Serilog;
using WinTenBot.Providers;

namespace WinTenBot.Migration
{
    public static class RssMigration
    {
        public static async Task MigrateRssHistory(this bool ifExist)
        {
            if (!ifExist)
            {
                var sqlCreate = "CREATE TABLE rss_history ( "
                                + "id         INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, " +
                                "chat_id      INTEGER, rss_source   TEXT, " +
                                "url          TEXT, " +
                                "title        TEXT, " +
                                "publish_date TEXT, " +
                                "author       TEXT, " +
                                "created_at   TEXT    DEFAULT 'CURRENT_TIMESTAMP');";
                Log.Debug("Migrating rss_history");

                await sqlCreate.ExecForSqLite();
            }
        }
    }
}