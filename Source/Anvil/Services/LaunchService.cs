using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Anvil.Framework.IO;
using Anvil.Models;
using Anvil.Services.Data;

using DynamicData.Kernel;

namespace Anvil.Services
{
    public interface ILaunchService : IService
    {
        void Launch(LaunchItem item);
    }

    public sealed class LaunchService : ILaunchService
    {
        private readonly IDataService mDataService;

        public LaunchService(IDataService dataService)
        {
            mDataService = dataService;
        }

        private EnvironmentVariableSet _GetEnvironmentVariables(LaunchItem item)
        {
            var parents = _GetParentLaunchGroups(item).Reverse().ToArray();
            var environment = new EnvironmentVariableSet();

            foreach(DictionaryEntry envVar in Environment.GetEnvironmentVariables())
            {
                environment[(string) envVar.Key] = (string) envVar.Value;
            }

            foreach(var parent in parents)
            {
                var parentVars = mDataService.GetEnvironmentVariableSnapshotFor(parent);
                foreach(var envVar in parentVars)
                {
                    environment.MergeAdd(envVar.Key, envVar.Value);
                }
            }

            var itemVars = mDataService.GetEnvironmentVariableSnapshotFor(item);
            foreach(var envVar in itemVars)
            {
                environment.MergeAdd(envVar.Key, envVar.Value);
            }

            return environment;
        }

        private IEnumerable<LaunchGroup> _GetParentLaunchGroups(LaunchItem item)
        {
            var parent = mDataService.LaunchGroups.Lookup(item.ParentGroupId);

            while(parent.HasValue)
            {
                yield return parent.Value;
                parent = parent.Value.ParentGroupId.HasValue
                             ? mDataService.LaunchGroups.Lookup(parent.Value.ParentGroupId.Value)
                             : new Optional<LaunchGroup>();
            }
        }

        public void Launch(LaunchItem item)
        {
            var environmentVariables = _GetEnvironmentVariables(item);
            var fileName = environmentVariables.Expand(item.Path);
            var startInfo = new ProcessStartInfo(fileName, item.Arguments);
            startInfo.WorkingDirectory = DirectoryEx.FirstExisting(item.WorkingDirectory, Path.GetDirectoryName(item.Path));
            startInfo.UseShellExecute = false;

            foreach(var envVar in environmentVariables)
            {
                startInfo.Environment[envVar.Key] = envVar.Value;
            }

            Process.Start(startInfo)?.Dispose();
        }
    }
}
