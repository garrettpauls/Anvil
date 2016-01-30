using System;
using System.IO;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using Anvil.Framework;
using Anvil.Framework.Reactive;
using Anvil.Models;

namespace Anvil.Services.Data
{
    public interface IPersistenceService : IService
    {
        Task Add(LaunchGroup grp);

        IObservable<LaunchGroup> GetLaunchGroups();

        IObservable<LaunchItem> GetLaunchItems();

        Task Remove(LaunchGroup grp);

        Task Save(LaunchGroup grp, string changedPropertyName = null);

        Task Save(LaunchItem item, string changedPropertyName = null);

        Task Save(EnvironmentVariable envVar, string changedPropertyName = null);
    }

    public sealed class PersistenceService : IPersistenceService, IInitializableService
    {
        private readonly ISqlService mSql;

        public PersistenceService(ISqlService sql)
        {
            mSql = sql;
        }

        public Task Add(LaunchGroup grp)
        {
            throw new NotImplementedException();
        }

        public IObservable<LaunchGroup> GetLaunchGroups()
        {
            return mSql.Run(sql => sql.RxQueryAsync(
                $"SELECT Id, ParentId, Name FROM LaunchGroup",
                async row => new LaunchGroup
                {
                    Id = await row.GetValueAsync<long>("Id"),
                    ParentGroupId = await row.GetValueAsync<long?>("ParentId"),
                    Name = await row.GetValueAsync<string>("Name")
                }));
        }

        public IObservable<LaunchItem> GetLaunchItems()
        {
            var subject = new Subject<LaunchItem>();

            mSql.Run(sql => sql.RxQueryAsync(
                $"SELECT Id, ParentId, Name, Path, WorkingDirectory FROM LaunchItem",
                async row => new LaunchItem
                {
                    Id = await row.GetValueAsync<long>("Id"),
                    ParentGroupId = await row.GetValueAsync<long>("ParentId"),
                    Name = await row.GetValueAsync<string>("Name"),
                    Path = await row.GetValueAsync<string>("Path"),
                    WorkingDirectory = await row.GetValueAsync<string>("WorkingDirectory")
                }));

            return subject;
        }

        public async Task InitializeAsync()
        {
            await mSql.EnsureDatabaseExists();
            await mSql.Run(async sql =>
            {
                var launcherTableExists = await sql.ExecuteScalarAsync<string>($"SELECT name FROM sqlite_master WHERE type='table' AND name='LaunchItem'");
                if(!string.IsNullOrEmpty(launcherTableExists))
                {
                    return;
                }

                using(var sqlStream = this.OpenRelativeResourceStream(@"CreateTables.sql"))
                using(var sqlReader = new StreamReader(sqlStream))
                {
                    await sql.ExecuteAsync(FormattableStringFactory.Create(sqlReader.ReadToEnd()));
                }
            });
        }

        public Task Remove(LaunchGroup grp)
        {
            throw new NotImplementedException();
        }

        public Task Save(LaunchGroup grp, string changedPropertyName = null)
        {
            return mSql.Run(sql => sql.ExecuteAsync($@"
UPDATE LaunchGroup
   SET ParentId = {grp.ParentGroupId}
     , Name = {grp.Name}
 WHERE Id = {grp.Id}"));
        }

        public Task Save(LaunchItem item, string changedPropertyName = null)
        {
            return mSql.Run(sql => sql.ExecuteAsync($@"
UPDATE LaunchItem
   SET ParentId = {item.ParentGroupId}
     , Name = {item.Name}
     , Path = {item.Path}
     , WorkingDirectory = {item.WorkingDirectory}
 WHERE Id = {item.Id}"));
        }

        public Task Save(EnvironmentVariable envVar, string changedPropertyName = null)
        {
            return mSql.Run(sql => sql.ExecuteAsync($@"
UPDATE EnvironmentVariable
   SET Key = {envVar.Key}
     , Value = {envVar.Value}
 WHERE Id = {envVar.Id}"));
        }
    }
}