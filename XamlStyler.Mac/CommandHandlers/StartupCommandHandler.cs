using MonoDevelop.Components.Commands;
using Xavalon.XamlStyler.Mac.Services.XamlStyler;
using Xavalon.XamlStyler.Mac.Services.XamlStylerOptions;

namespace Xavalon.XamlStyler.Mac.CommandHandlers
{
    public class StartupCommandHandler : CommandHandler
    {
        protected override void Run()
        {
            var container = Container.Instance;

            container.LazyRegisterSingleton<IXamlStylerService, XamlStylerService>();
            container.LazyRegisterSingleton<IXamlStylerOptionsService, XamlStylerOptionsService>();
        }
    }
}