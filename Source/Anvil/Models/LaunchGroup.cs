using DynamicData;

using ReactiveUI;

namespace Anvil.Models
{
    public sealed class LaunchGroup : Model, IKey<long>
    {
        private long mId;
        private string mName;
        private long? mParentGroupId;

        public long Id
        {
            get { return mId; }
            set { this.RaiseAndSetIfChanged(ref mId, value); }
        }

        public long Key => Id;

        public string Name
        {
            get { return mName; }
            set { this.RaiseAndSetIfChanged(ref mName, value); }
        }

        public long? ParentGroupId
        {
            get { return mParentGroupId; }
            set { this.RaiseAndSetIfChanged(ref mParentGroupId, value); }
        }
    }
}