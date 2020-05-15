﻿// (c) Xavalon. All rights reserved.

using Newtonsoft.Json;
using NUnit.Framework;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.UnitTests
{
    //Should these tests fail with a File -or Directory not found exception, disable Shadow-Copy assemblies in your
    //test runner (probably ReSharper, it searches for the config files in the wrong directory)

    [TestFixture]
    public sealed partial class Tests
    {
        [Test]
        public void TestConfigurationDefault()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations\Default.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations\SerializedDefault.json");

            Assert.IsTrue(stylerOptions.UseVisualStudioIndentWithTabs);
        }

        [Test]
        public void TestConfigurationEmpty()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations\Empty.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations\SerializedDefault.json");
        }

        [Test]
        public void TestConfigurationSingle()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations\Single.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations\Single.json");
        }

        [Test]
        public void TestConfigurationBadSetting()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations\BadSetting.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations\SerializedDefault.json");
        }

        [Test]
        public void TestConfigurationAllDifferent()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations\AllDifferent.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations\AllDifferent.json");
        }

        [Test]
        public void TestConfigurationIndentUsingTabs()
        {
            var stylerOptions = new StylerOptions(config: Tests.GetConfiguration(@"TestConfigurations\IndentWithTabsOverride.json"));
            this.TestConfig(stylerOptions, @"TestConfigurations\IndentWithTabsOverride.json");

            Assert.IsFalse(stylerOptions.UseVisualStudioIndentWithTabs); // IndentWithTabs is true
            Assert.IsFalse(stylerOptions.UseVisualStudioIndentSize); // IndentSize is set
        }

        private void TestConfig(StylerOptions stylerOptions, string expectedConfiguration)
        {
            var actualOptions = JsonConvert.SerializeObject(stylerOptions);
            var expectedOptions = File.ReadAllText(Tests.GetConfiguration(expectedConfiguration));

            Assert.That(Regex.Replace(actualOptions, @"\s+", ""), Is.EqualTo(Regex.Replace(expectedOptions, @"\s+", "")));
        }

        private static string GetConfiguration(string path)
        {
            return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), path);
        }
    }
}
