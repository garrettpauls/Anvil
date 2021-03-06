﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

using Anvil.Framework.IO;
using Anvil.Models;
using Anvil.Services.Data;

using Autofac.Extras.NLog;

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
        private readonly IEnvironmentVariableExpander mExpander;
        private readonly ILogger mLog;
        private readonly INotificationService mNotificationService;

        public LaunchService(IDataService dataService, IEnvironmentVariableExpander expander, INotificationService notificationService, ILogger log)
        {
            mDataService = dataService;
            mExpander = expander;
            mNotificationService = notificationService;
            mLog = log;
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
                    environment[envVar.Key] = envVar.Value;
                }
            }

            var itemVars = mDataService.GetEnvironmentVariableSnapshotFor(item);
            foreach(var envVar in itemVars)
            {
                environment[envVar.Key] = envVar.Value;
            }

            var expanded = mExpander.Expand(environment);

            return new EnvironmentVariableSet(expanded);
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
            try
            {
                var environmentVariables = _GetEnvironmentVariables(item);

                var fileName = mExpander.ExpandValue(environmentVariables, item.Path);
                var arguments = mExpander.ExpandValue(environmentVariables, item.Arguments);
                var workingDirectory = mExpander.ExpandValue(environmentVariables, item.WorkingDirectory);

                var startInfo = new ProcessStartInfo(fileName, arguments);
                startInfo.WorkingDirectory = DirectoryEx.FirstExisting(workingDirectory, Path.GetDirectoryName(fileName));
                startInfo.UseShellExecute = false;

                foreach(var envVar in environmentVariables)
                {
                    startInfo.Environment[envVar.Key] = envVar.Value;
                }

                Process.Start(startInfo)?.Dispose();
            }
            catch(Exception ex)
            {
                mLog.Error($"Failed to launch {item.Name} ({item.Id})", ex);
                mNotificationService.ShowError($"Failed to launch {item.Name}", ex);
            }
        }
    }
}
