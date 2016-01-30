using System;
using System.Linq;
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

        Task AddLaunchGroupAsync(LaunchGroup group);

        Task RefreshDataAsync();
    }

    public sealed class DataService : IDataService, IInitializableService
    {
        private readonly DisposableTracker mDisposables = new DisposableTracker();
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

        public Task AddLaunchGroupAsync(LaunchGroup group)
        {
            return Task.Factory.StartNew(() =>
            {
                if(group.Id == 0)
                {
                    group.Id = mLaunchGroups.Keys.Max() + 1;
                }

                mLaunchGroups.AddOrUpdate(group);
            });
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