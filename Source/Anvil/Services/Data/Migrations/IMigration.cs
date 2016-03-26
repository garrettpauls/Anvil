using System;
using System.Threading.Tasks;

using FormattableSql.Core;

namespace Anvil.Services.Data.Migrations
{
    public interface IMigration
    {
        string Description { get; }

        long Version { get; }

        Task Up(IFormattableSqlProvider sql);
    }

    public abstract class Migration : IMigration
    {
        protected Migration(string description, DateTimeOffset writtenInstant)
        {
            Description = description;
            Version = _GetVersionNumber(writtenInstant);
        }

        public string Description { get; }

        public long Version { get; }

        public abstract Task Up(IFormattableSqlProvider sql);

        private static long _GetVersionNumber(DateTimeOffset instant)
        {
            var utcInstant = instant.ToUniversalTime();

            return
                100000000L*utcInstant.Year +
                1000000L*utcInstant.Month +
                10000L*utcInstant.Day +
                100L*utcInstant.Hour +
                utcInstant.Minute;
        }
    }
}
