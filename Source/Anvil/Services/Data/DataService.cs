using System.Linq;
using System.Reactive.Linq;
using System.Threading.Tasks;

using Anvil.Models;

using DynamicData;

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
        private readonly SourceCache<LaunchGroup, long> mLaunchGroups = new SourceCache<LaunchGroup, long>(lg => lg.Key);
        private readonly SourceCache<LaunchItem, long> mLaunchItems = new SourceCache<LaunchItem, long>(li => li.Key);
        private readonly IPersistenceService mPersistenceService;

        public DataService(IPersistenceService persistenceService)
        {
            mPersistenceService = persistenceService;
        }

        public IObservableCache<LaunchGroup, long> LaunchGroups => mLaunchGroups;

        public IObservableCache<LaunchItem, long> LaunchItems => mLaunchItems;

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