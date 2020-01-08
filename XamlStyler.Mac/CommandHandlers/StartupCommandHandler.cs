using MonoDevelop.Components.Commands;

namespace Xavalon.XamlStyler.Mac.CommandHandlers
{
    public class StartupCommandHandler : CommandHandler
    {
        protected override void Run()
        {
            var extension = new Extension();
            extension.Initialize();
        }
    }
}