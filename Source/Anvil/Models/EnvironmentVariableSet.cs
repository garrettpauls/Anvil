using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Anvil.Models
{
    public sealed class EnvironmentVariableSet : IEnumerable<KeyValuePair<string, string>>
    {
        private readonly Regex mEnvVarRegex = new Regex(@"%(?<key>[^%]+)%");
        private readonly Dictionary<string, string> mValues = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        public string this[string key]
        {
            get
            {
                string value;
                return mValues.TryGetValue(key, out value) ? value : null;
            }
            set { mValues[key] = value; }
        }

        public IEnumerable<string> Keys => mValues.Keys;

        public string Expand(string value)
        {
            return mEnvVarRegex.Replace(value, match =>
            {
                string replacement;

                var key = match.Groups["key"];
                if(!key.Success || !mValues.TryGetValue(key.Value, out replacement))
                {
                    replacement = match.Value;
                }

                return replacement;
            });
        }

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return mValues.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void MergeAdd(string key, string value)
        {
            this[key] = Expand(value);
        }
    }
}
