using System.Linq;
using System.Threading.Tasks;

using Anvil.Models;

using DynamicData;

namespace Anvil.Services.Data
{
    public interface IDataService
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

                mLaunchGroups.AddOrUpdate(new[]
                {
                    new LaunchGroup
                    {
                        Name = "One",
                        Id = 1
                    },
                    new LaunchGroup
                    {
                        Name = "Two",
                        Id = 2
                    },
                    new LaunchGroup
                    {
                        Name = "Three",
                        Id = 3,
                        ParentGroupId = 1
                    }
                });

                mLaunchItems.AddOrUpdate(new[]
                {
                    new LaunchItem
                    {
                        Name = "Item 1",
                        Id = 1,
                        ParentGroupId = 1
                    },
                    new LaunchItem
                    {
                        Name = "Item 2",
                        Id = 2,
                        ParentGroupId = 2
                    },
                    new LaunchItem
                    {
                        Name = "Item 3",
                        Id = 3,
                        ParentGroupId = 3
                    }
                });
            });
        }
    }
}