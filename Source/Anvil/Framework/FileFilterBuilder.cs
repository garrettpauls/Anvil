using System.Linq;
using System.Text;

namespace Anvil.Framework
{
    public sealed class FileFilterBuilder
    {
        private readonly StringBuilder mBuilder = new StringBuilder();

        private void _AppendFilter(string fullFilter)
        {
            if(mBuilder.Length > 0)
            {
                mBuilder.Append("|");
            }
            mBuilder.Append(fullFilter);
        }

        public string ToFileFilter()
        {
            return mBuilder.ToString();
        }

        public override string ToString()
        {
            return ToFileFilter();
        }

        public FileFilterBuilder With(string description, params string[] extensions)
        {
            var combinedExtensions = string.Join(";", extensions.Select(ext => $"*{ext}"));
            _AppendFilter($"{description}|{combinedExtensions}");
            return this;
        }

        public FileFilterBuilder WithAllFiles()
        {
            _AppendFilter("All files|*");
            return this;
        }
    }
}
