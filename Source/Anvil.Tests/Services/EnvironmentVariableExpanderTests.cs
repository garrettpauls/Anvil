using System.Collections.Generic;

using Anvil.Models;
using Anvil.Services;

using FluentAssertions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Anvil.Tests.Services
{
    [TestClass]
    public sealed class EnvironmentVariableExpanderTests
    {
        [TestMethod]
        public void BasicExpand()
        {
            var before = new EnvironmentVariableSet
            {
                ["logFile"] = @"%logdir%\error.log",
                ["temp"] = @"C:\temp",
                ["logdir"] = @"%temp%\%version%\logs"
            };
            var after = new EnvironmentVariableSet
            {
                ["logFile"] = @"C:\temp\%version%\logs\error.log",
                ["temp"] = @"C:\temp",
                ["logdir"] = @"C:\temp\%version%\logs"
            };

            _ExpandTest(before, after);
        }

        [TestMethod]
        public void ExpandValue()
        {
            var environment = new EnvironmentVariableSet
            {
                ["temp"] = @"C:\temp"
            };

            var result = new EnvironmentVariableExpander().ExpandValue(environment, @"%temp%\%version%\log.txt");

            result.Should().Be(@"C:\temp\%version%\log.txt");
        }

        [TestMethod]
        public void RecursiveExpansion()
        {
            var before = new EnvironmentVariableSet
            {
                ["RecA"] = @"%RecB%\RecA",
                ["RecB"] = @"%RecA%\RecB",
                ["RecSelf"] = @"%RecSelf%\forever"
            };
            var after = new EnvironmentVariableSet
            {
                ["RecA"] = @"%RecB%\RecA\RecB\RecA",
                ["RecB"] = @"%RecB%\RecA\RecB",
                ["RecSelf"] = @"%RecSelf%\forever\forever"
            };

            _ExpandTest(before, after);
        }

        private static void _ExpandTest(IDictionary<string, string> beforeExpansion, IDictionary<string, string> expectedAfterExpansion)
        {
            var expanded = new EnvironmentVariableExpander().Expand(beforeExpansion);

            expanded.Should().Equal(expectedAfterExpansion);
        }
    }
}
