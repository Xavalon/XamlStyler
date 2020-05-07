using System.Threading;
using JetBrains.Application.BuildScript.Application.Zones;
using JetBrains.ReSharper.TestFramework;
using JetBrains.TestFramework;
using JetBrains.TestFramework.Application.Zones;
using NUnit.Framework;

[assembly: Apartment(ApartmentState.STA)]

namespace ReSharperPlugin.XamlStyler.dotUltimate.Tests
{
    [ZoneDefinition]
    public interface IXamlStyler_dotUltimateTestZone : ITestsEnvZone, IRequire<PsiFeatureTestZone>
    {
    }

    [SetUpFixture]
    public class TestEnvironment : ExtensionTestEnvironmentAssembly<IXamlStyler_dotUltimateTestZone>
    {
    }
}