// © Xavalon. All rights reserved.

using MonoDevelop.Components.Commands;

namespace Xavalon.XamlStyler.Extension.Mac.CommandHandlers
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