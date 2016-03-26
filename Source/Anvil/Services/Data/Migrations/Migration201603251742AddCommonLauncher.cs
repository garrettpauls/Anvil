using System;
using System.Threading.Tasks;

using FormattableSql.Core;

namespace Anvil.Services.Data.Migrations
{
    public sealed class Migration201603251742AddCommonLauncher : Migration
    {
        public Migration201603251742AddCommonLauncher()
            : base("Updates the LaunchItem table to support common launchers",
                   new DateTimeOffset(2016, 03, 25, 17, 42, 0, TimeSpan.Zero))
        {
        }

        public override Task Up(IFormattableSqlProvider sql)
        {
            throw new NotImplementedException("TODO: create migration");
//            return sql.ExecuteAsync($@"
//");
        }
    }
}
