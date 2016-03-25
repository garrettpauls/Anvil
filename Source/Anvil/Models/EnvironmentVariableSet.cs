using System;
using System.Collections.Generic;

namespace Anvil.Models
{
    public sealed class EnvironmentVariableSet : Dictionary<string, string>
    {
        public EnvironmentVariableSet()
            : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public EnvironmentVariableSet(IDictionary<string, string> dictionary)
            : base(dictionary, StringComparer.OrdinalIgnoreCase)
        {
        }
    }
}
