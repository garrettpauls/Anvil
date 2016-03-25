using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using Anvil.Framework.QuickGraph.Algorithms;

using QuickGraph;

namespace Anvil.Services
{
    public interface IEnvironmentVariableExpander : IService
    {
        IDictionary<string, string> Expand(IDictionary<string, string> environmentVariables);

        string ExpandValue(IDictionary<string, string> environmentVariables, string value);
    }

    public sealed class EnvironmentVariableExpander : IEnvironmentVariableExpander
    {
        private static readonly Regex mEnvVarRegex = new Regex(@"%(?<key>[^%]+)%");

        public IDictionary<string, string> Expand(IDictionary<string, string> environmentVariables)
        {
            var expanded = new Dictionary<string, string>(environmentVariables, StringComparer.OrdinalIgnoreCase);
            var referenceGraph = _CreateReferenceGraph(expanded);

            var keysByLeastDependencies = referenceGraph.TopologicalSortSupportingCycles().Reverse();

            foreach(var key in keysByLeastDependencies)
            {
                string value;
                if(expanded.TryGetValue(key, out value))
                {
                    expanded[key] = ExpandValue(expanded, value);
                }
            }

            return expanded;
        }

        public string ExpandValue(IDictionary<string, string> environmentVariables, string value)
        {
            return mEnvVarRegex.Replace(value, match =>
            {
                var key = match.Groups["key"].Value;
                string replacementValue;

                if(!environmentVariables.TryGetValue(key, out replacementValue))
                {
                    replacementValue = match.Value;
                }

                return replacementValue;
            });
        }

        private static AdjacencyGraph<string, Edge<string>> _CreateReferenceGraph(IDictionary<string, string> target)
        {
            var graph = new AdjacencyGraph<string, Edge<string>>();

            foreach(var key in target.Keys.Select(key => key.ToLowerInvariant()))
            {
                graph.AddVertex(key);

                foreach(var referencedKey in _GetReferencedKeys(target[key]))
                {
                    graph.AddVertex(referencedKey);
                    graph.AddEdge(new Edge<string>(key, referencedKey));
                }
            }

            return graph;
        }

        private static IEnumerable<string> _GetReferencedKeys(string value)
        {
            return mEnvVarRegex.Matches(value).Cast<Match>()
                               .Select(match => match.Groups["key"].Value.ToLowerInvariant());
        }
    }
}
