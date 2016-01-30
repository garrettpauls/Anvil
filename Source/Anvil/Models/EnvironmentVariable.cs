using DynamicData;

using ReactiveUI;

namespace Anvil.Models
{
    public sealed class EnvironmentVariable : Model, IKey<long>
    {
        private long mId;
        private string mKey;
        private long mParentId;
        private string mValue;

        public long Id
        {
            get { return mId; }
            set { this.RaiseAndSetIfChanged(ref mId, value); }
        }

        public string Key
        {
            get { return mKey; }
            set { this.RaiseAndSetIfChanged(ref mKey, value); }
        }

        long IKey<long>.Key => Id;

        public long ParentId
        {
            get { return mParentId; }
            set { this.RaiseAndSetIfChanged(ref mParentId, value); }
        }

        public string Value
        {
            get { return mValue; }
            set { this.RaiseAndSetIfChanged(ref mValue, value); }
        }
    }
}