using System;
using System.Threading.Tasks;

using FormattableSql.Core;

namespace Anvil.Services.Data.Migrations
{
    public sealed class Migration201603261910AddConfigurationTable : Migration
    {
        public Migration201603261910AddConfigurationTable()
            : base(
                "Add Configuration table",
                new DateTimeOffset(2016, 03, 26, 19, 10, 0, TimeSpan.Zero))
        {
        }

        public override Task Up(IFormattableSqlProvider sql)
        {
            return sql.ExecuteAsync($@"
CREATE TABLE Configuration
([Key] TEXT NOT NULL PRIMARY KEY ON CONFLICT REPLACE,
 Value BLOB,
 Type  TEXT NOT NULL
);
");
        }
    }
}
