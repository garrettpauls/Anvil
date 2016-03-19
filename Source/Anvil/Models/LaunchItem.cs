using DynamicData;

using ReactiveUI;

namespace Anvil.Models
{
    public sealed class LaunchItem : Model, IKey<long>
    {
        private string mArguments = "";
        private long mId;
        private string mName = "";
        private long mParentGroupId;
        private string mPath = "";
        private string mWorkingDirectory = "";

        public string Arguments
        {
            get { return mArguments; }
            set { this.RaiseAndSetIfChanged(ref mArguments, value); }
        }

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

        public long ParentGroupId
        {
            get { return mParentGroupId; }
            set { this.RaiseAndSetIfChanged(ref mParentGroupId, value); }
        }

        public string Path
        {
            get { return mPath; }
            set { this.RaiseAndSetIfChanged(ref mPath, value); }
        }

        public string WorkingDirectory
        {
            get { return mWorkingDirectory; }
            set { this.RaiseAndSetIfChanged(ref mWorkingDirectory, value); }
        }
    }
}
