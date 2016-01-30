using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Threading.Tasks;

using Autofac.Extras.NLog;

using FormattableSql.Core;
using FormattableSql.Core.Data.Provider;

namespace Anvil.Services.Data
{
    public interface ISqlService : IService
    {
        Task EnsureDatabaseExists();

        Task Run(Func<IFormattableSqlProvider, Task> action);

        Task<TResult> Run<TResult>(Func<IFormattableSqlProvider, Task<TResult>> action);

        TResult Run<TResult>(Func<IFormattableSqlProvider, TResult> action);
    }

    public sealed class SqlService : ISqlService, IInitializableService
    {
        private readonly string mFile;
        private readonly ILogger mLog;
        private readonly Lazy<IFormattableSqlProvider> mSql;

        public SqlService(ILogger log)
        {
            mLog = log;
            mFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Anvil", "data.sqlite");
            mSql = new Lazy<IFormattableSqlProvider>(() =>
            {
                var dbProvider = DbProviderFactories.GetFactory("System.Data.SQLite");
                var connection = new SQLiteConnectionStringBuilder
                {
                    DataSource = mFile,
                    DateTimeKind = DateTimeKind.Utc,
                    ForeignKeys = true
                };

                return FormattableSqlFactory.For(new AdoNetSqlProvider(dbProvider, connection.ToString()));
            });
        }

        public Task EnsureDatabaseExists()
        {
            if(!File.Exists(mFile))
            {
                var directory = Path.GetDirectoryName(mFile);
                if(!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllBytes(mFile, new byte[0]);
            }

            return Task.CompletedTask;
        }

        public Task InitializeAsync()
        {
            return EnsureDatabaseExists();
        }

        public async Task Run(Func<IFormattableSqlProvider, Task> action)
        {
            try
            {
                await action(mSql.Value);
            }
            catch(Exception ex)
            {
                mLog.Error(ex);
                throw;
            }
        }

        public async Task<TResult> Run<TResult>(Func<IFormattableSqlProvider, Task<TResult>> action)
        {
            try
            {
                return await action(mSql.Value);
            }
            catch(Exception ex)
            {
                mLog.Error(ex);
                throw;
            }
        }

        public TResult Run<TResult>(Func<IFormattableSqlProvider, TResult> action)
        {
            try
            {
                return action(mSql.Value);
            }
            catch(Exception ex)
            {
                mLog.Error(ex);
                throw;
            }
        }
    }
}