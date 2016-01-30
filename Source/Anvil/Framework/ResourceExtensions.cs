using System;
using System.IO;

namespace Anvil.Framework
{
    public static class ResourceExtensions
    {
        public static Stream OpenRelativeResourceStream(this object self, string relativePath)
        {
            Guard.AgainstNullArgument(nameof(self), self);
            Guard.AgainstNullArgument(nameof(relativePath), relativePath);

            var type = self.GetType();

            var stream = type.Assembly.GetManifestResourceStream(type, relativePath);
            if(stream == null)
            {
                throw new InvalidOperationException($"Could not find embedded resource {type.Namespace}.{relativePath}");
            }

            return stream;
        }
    }
}