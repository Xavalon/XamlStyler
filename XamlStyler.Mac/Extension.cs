using Xavalon.XamlStyler.Mac.Services.XamlFiles;
using Xavalon.XamlStyler.Mac.Services.XamlFormatting;
using Xavalon.XamlStyler.Mac.Services.XamlStylerOptions;

namespace Xavalon.XamlStyler.Mac
{
    public class Extension
    {
        public void Initialize()
        {
            var container = Container.Instance;

            RegisterServices(container);
        }

        private void RegisterServices(Container container)
        {
            container.LazyRegisterSingleton<IXamlFormattingService, XamlFormattingService>();
            container.LazyRegisterSingleton<IXamlStylerOptionsService, XamlStylerOptionsService>();
            container.LazyRegisterSingleton<IXamlFilesService, XamlFilesService>();
        }
    }
}
