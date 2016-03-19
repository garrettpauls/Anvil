using System;
using System.IO;
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

        Task Add(EnvironmentVariable envVar);

        Task Add(LaunchItem item);

        Task Assign(EnvironmentVariable envVar, LaunchGroup group);

        Task Assign(EnvironmentVariable envVar, LaunchItem item);

        IObservable<EnvironmentVariable> GetLaunchGroupEnvironmentVariables();

        IObservable<LaunchGroup> GetLaunchGroups();

        IObservable<EnvironmentVariable> GetLaunchItemEnvironmentVariables();

        IObservable<LaunchItem> GetLaunchItems();

        Task Remove(LaunchGroup grp);

        Task Remove(EnvironmentVariable envVar);

        Task Remove(LaunchItem item);

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
            return mSql.Run(async sql => grp.Id = await sql.ExecuteScalarAsync<long>($@"
insert into LaunchGroup (Name      , ParentId)
                 values ({grp.Name}, {grp.ParentGroupId})
;select last_insert_rowid()"));
        }

        public Task Add(EnvironmentVariable envVar)
        {
            return mSql.Run(async sql => envVar.Id = await sql.ExecuteScalarAsync<long>($@"
insert into EnvironmentVariable (Key         , Value)
                         values ({envVar.Key}, {envVar.Value})
;select last_insert_rowid()"));
        }

        public Task Add(LaunchItem item)
        {
            return mSql.Run(async sql => item.Id = await sql.ExecuteScalarAsync<long>($@"
insert into LaunchItem (Name       , Path       , WorkingDirectory       , ParentId)
                values ({item.Name}, {item.Path}, {item.WorkingDirectory}, {item.ParentGroupId})
;select last_insert_rowid()"));
        }

        public Task Assign(EnvironmentVariable envVar, LaunchGroup @group)
        {
            return mSql.Run(sql => sql.ExecuteAsync($@"
delete from LaunchGroupVariables where EnvironmentVariableId = {envVar.Id}
;
insert into LaunchGroupVariables (LaunchGroupId, EnvironmentVariableId)
                          values ({group.Id}   , {envVar.Id})"));
        }

        public Task Assign(EnvironmentVariable envVar, LaunchItem item)
        {
            return mSql.Run(sql => sql.ExecuteAsync($@"
delete from LaunchItemVariables where EnvironmentVariableId = {envVar.Id}
;
insert into LaunchItemVariables (LaunchItemId, EnvironmentVariableId)
                         values ({item.Id}   , {envVar.Id})"));
        }

        public IObservable<EnvironmentVariable> GetLaunchGroupEnvironmentVariables()
        {
            return mSql.Run(sql => sql.RxQueryAsync($@"
select env.Id as EnvVarId, lgv.LaunchGroupId, env.Key, env.Value
  from LaunchGroupVariables lgv
  join EnvironmentVariable env on lgv.EnvironmentVariableId = env.Id
",
                                                    async row => new EnvironmentVariable
                                                    {
                                                        Id = await row.GetValueAsync<long>("EnvVarId"),
                                                        Key = await row.GetValueAsync<string>("Key"),
                                                        Value = await row.GetValueAsync<string>("Value"),
                                                        ParentId = await row.GetValueAsync<long>("LaunchGroupId")
                                                    }));
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

        public IObservable<EnvironmentVariable> GetLaunchItemEnvironmentVariables()
        {
            return mSql.Run(sql => sql.RxQueryAsync($@"
select env.Id as EnvVarId, liv.LaunchItemId, env.Key, env.Value
  from LaunchItemVariables liv
  join EnvironmentVariable env on liv.EnvironmentVariableId = env.Id
",
                                                    async row => new EnvironmentVariable
                                                    {
                                                        Id = await row.GetValueAsync<long>("EnvVarId"),
                                                        Key = await row.GetValueAsync<string>("Key"),
                                                        Value = await row.GetValueAsync<string>("Value"),
                                                        ParentId = await row.GetValueAsync<long>("LaunchItemId")
                                                    }));
        }

        public IObservable<LaunchItem> GetLaunchItems()
        {
            return mSql.Run(sql => sql.RxQueryAsync(
                $"SELECT Id, ParentId, Name, Path, WorkingDirectory FROM LaunchItem",
                async row => new LaunchItem
                {
                    Id = await row.GetValueAsync<long>("Id"),
                    ParentGroupId = await row.GetValueAsync<long>("ParentId"),
                    Name = await row.GetValueAsync<string>("Name"),
                    Path = await row.GetValueAsync<string>("Path"),
                    WorkingDirectory = await row.GetValueAsync<string>("WorkingDirectory")
                }));
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
            return mSql.Run(sql => sql.ExecuteAsync($"delete from LaunchGroup where Id = {grp.Id}"));
        }

        public Task Remove(EnvironmentVariable envVar)
        {
            return mSql.Run(sql => sql.ExecuteAsync($"delete from EnvironmentVariable where Id = {envVar.Id}"));
        }

        public Task Remove(LaunchItem item)
        {
            return mSql.Run(sql => sql.ExecuteAsync($"delete from LaunchItem where Id = {item.Id}"));
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
