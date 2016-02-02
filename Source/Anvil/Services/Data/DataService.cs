using System;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Anvil.Framework.ComponentModel;
using Anvil.Framework.Reactive;
using Anvil.Models;

using DynamicData;

using ReactiveUI;

namespace Anvil.Services.Data
{
    public interface IDataService : IService
    {
        IObservableCache<LaunchGroup, long> LaunchGroups { get; }

        IObservableCache<LaunchItem, long> LaunchItems { get; }

        Task AddEnvironmentVariable(LaunchGroup group, EnvironmentVariable envVar);

        Task AddLaunchGroupAsync(LaunchGroup group);

        Task DeleteEnvironmentVariable(LaunchGroup group, EnvironmentVariable envVar);

        IObservableList<EnvironmentVariable> GetEnvironmentVariablesFor(LaunchGroup group);

        Task RefreshDataAsync();

        Task RemoveLaunchGroupAsync(LaunchGroup group);
    }

    public sealed class DataService : IDataService, IInitializableService
    {
        private readonly DisposableTracker mDisposables = new DisposableTracker();
        private readonly SourceCache<LaunchGroup, long> mLaunchGroups = new SourceCache<LaunchGroup, long>(lg => lg.Key);
        private readonly SourceList<EnvironmentVariable> mLaunchGroupVariables = new SourceList<EnvironmentVariable>();
        private readonly SourceCache<LaunchItem, long> mLaunchItems = new SourceCache<LaunchItem, long>(li => li.Key);
        private readonly SourceList<EnvironmentVariable> mLaunchItemVariables = new SourceList<EnvironmentVariable>();
        private readonly IPersistenceService mPersistenceService;

        public DataService(IPersistenceService persistenceService)
        {
            mPersistenceService = persistenceService;

            _TrackPropertyChanges(mLaunchGroups);
            _TrackPropertyChanges(mLaunchItems);
            _TrackPropertyChanges(mLaunchGroupVariables);
            _TrackPropertyChanges(mLaunchItemVariables);
        }

        public IObservableCache<LaunchGroup, long> LaunchGroups => mLaunchGroups;

        public IObservableCache<LaunchItem, long> LaunchItems => mLaunchItems;

        private void _SavePropertyValue<T>(PropertyChangedNotification<T> evt)
        {
            var grp = evt.Source as LaunchGroup;
            var item = evt.Source as LaunchItem;
            var envVar = evt.Source as EnvironmentVariable;

            if(grp != null)
            {
                mPersistenceService.Save(grp, evt.PropertyName);
            }
            else if(item != null)
            {
                mPersistenceService.Save(item, evt.PropertyName);
            }
            else if(envVar != null)
            {
                mPersistenceService.Save(envVar, evt.PropertyName);
            }
        }

        private void _TrackPropertyChanges<T>(SourceCache<T, long> cache)
            where T : IReactiveObject
        {
            cache.Connect()
                 .Transform(_TrackPropertyChanges).DisposeMany()
                 .Subscribe().TrackWith(mDisposables);
        }

        private void _TrackPropertyChanges<T>(SourceList<T> cache)
            where T : IReactiveObject
        {
            cache.Connect()
                 .Transform(_TrackPropertyChanges).DisposeMany()
                 .Subscribe().TrackWith(mDisposables);
        }

        private IDisposable _TrackPropertyChanges<T>(T value)
            where T : IReactiveObject
        {
            return value
                .ObservePropertyChanged()
                .Subscribe(_SavePropertyValue);
        }

        public async Task AddEnvironmentVariable(LaunchGroup group, EnvironmentVariable envVar)
        {
            envVar.ParentId = group.Id;
            await mPersistenceService.Add(envVar);
            await mPersistenceService.Assign(envVar, group);
            mLaunchGroupVariables.Add(envVar);
        }

        public Task AddLaunchGroupAsync(LaunchGroup group)
        {
            return Task.Factory.StartNew(() =>
            {
                mPersistenceService.Add(group);

                mLaunchGroups.AddOrUpdate(group);
            });
        }

        public async Task DeleteEnvironmentVariable(LaunchGroup group, EnvironmentVariable envVar)
        {
            await mPersistenceService.Remove(envVar);
            mLaunchGroupVariables.Remove(envVar);
        }

        public IObservableList<EnvironmentVariable> GetEnvironmentVariablesFor(LaunchGroup group)
        {
            return mLaunchGroupVariables
                .Connect()
                .Filter(envVar => envVar.ParentId == group.Id)
                .AsObservableList();
        }

        public Task InitializeAsync()
        {
            return RefreshDataAsync();
        }

        public Task RefreshDataAsync()
        {
            return Task.Factory.StartNew(() =>
            {
                mLaunchGroups.Clear();
                mLaunchItems.Clear();
                mLaunchGroupVariables.Clear();
                mLaunchItemVariables.Clear();

                var groups = mPersistenceService.GetLaunchGroups();
                var items = mPersistenceService.GetLaunchItems();
                var groupEnvVars = mPersistenceService.GetLaunchGroupEnvironmentVariables();
                var itemEnvVars = mPersistenceService.GetLaunchItemEnvironmentVariables();

                return Task.WhenAll(
                    groups.ForEachAsync(mLaunchGroups.AddOrUpdate),
                    items.ForEachAsync(mLaunchItems.AddOrUpdate),
                    groupEnvVars.ForEachAsync(mLaunchGroupVariables.Add),
                    itemEnvVars.ForEachAsync(mLaunchItemVariables.Add));
            });
        }

        public async Task RemoveLaunchGroupAsync(LaunchGroup group)
        {
            await mPersistenceService.Remove(group);
            mLaunchGroups.Remove(group);
        }
    }
}