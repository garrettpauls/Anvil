using System;
using System.Collections.Concurrent;
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
    }

    public sealed class DataService : IDataService, IInitializableService
    {
        private readonly DisposableTracker mDisposables = new DisposableTracker();

        private readonly ConcurrentDictionary<long, SourceList<EnvironmentVariable>> mLaunchGroupEnvVars =
            new ConcurrentDictionary<long, SourceList<EnvironmentVariable>>();

        private readonly SourceCache<LaunchGroup, long> mLaunchGroups = new SourceCache<LaunchGroup, long>(lg => lg.Key);
        private readonly SourceCache<LaunchItem, long> mLaunchItems = new SourceCache<LaunchItem, long>(li => li.Key);
        private readonly IPersistenceService mPersistenceService;

        public DataService(IPersistenceService persistenceService)
        {
            mPersistenceService = persistenceService;

            _TrackPropertyChanges(mLaunchGroups);
            _TrackPropertyChanges(mLaunchItems);
        }

        public IObservableCache<LaunchGroup, long> LaunchGroups => mLaunchGroups;

        public IObservableCache<LaunchItem, long> LaunchItems => mLaunchItems;

        private SourceList<EnvironmentVariable> _GetEnvironmentVariablesFor(LaunchGroup group)
        {
            return mLaunchGroupEnvVars.GetOrAdd(group.Id, id =>
            {
                var envVars = new SourceList<EnvironmentVariable>();

                //TODO: Load from database

                return envVars;
            });
        }

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

        private IDisposable _TrackPropertyChanges<T>(T value)
            where T : IReactiveObject
        {
            return value
                .ObservePropertyChanged()
                .Subscribe(_SavePropertyValue);
        }

        public Task AddEnvironmentVariable(LaunchGroup group, EnvironmentVariable envVar)
        {
            return Task.Factory.StartNew(() =>
            {
                //TODO: Save to database

                var envVars = _GetEnvironmentVariablesFor(group);
                envVars.Add(envVar);
            });
        }

        public Task AddLaunchGroupAsync(LaunchGroup group)
        {
            return Task.Factory.StartNew(() =>
            {
                mPersistenceService.Add(group);

                mLaunchGroups.AddOrUpdate(group);
            });
        }

        public Task DeleteEnvironmentVariable(LaunchGroup group, EnvironmentVariable envVar)
        {
            return Task.Factory.StartNew(() =>
            {
                //TODO: Delete from database

                var envVars = _GetEnvironmentVariablesFor(group);
                envVars.Remove(envVar);
            });
        }

        public IObservableList<EnvironmentVariable> GetEnvironmentVariablesFor(LaunchGroup group)
        {
            return _GetEnvironmentVariablesFor(group);
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

                var groups = mPersistenceService.GetLaunchGroups();
                var items = mPersistenceService.GetLaunchItems();

                return Task.WhenAll(
                    groups.ForEachAsync(mLaunchGroups.AddOrUpdate),
                    items.ForEachAsync(mLaunchItems.AddOrUpdate));
            });
        }
    }
}