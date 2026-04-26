using System.Threading.Tasks;
using JetBrains.Application.Parts;
using JetBrains.Application.Settings;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.Rd.Tasks;
using JetBrains.Rider.Backend.Features;
using JetBrains.Rider.Model;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.Extension.Rider.Rider
{
    [SolutionComponent(Instantiation.ContainerAsyncPrimaryThread)]
    public class RiderXamlStylerHost
    {
        private readonly Lifetime _lifetime;
        private readonly SolutionModel _solutionModel;
        private readonly ISolution _solution;
        private readonly DocumentManager _documentManager;

        public RiderXamlStylerHost(
            Lifetime lifetime,
            SolutionModel solutionModel,
            ISolution solution,
            DocumentManager documentManager)
        {
            _lifetime = lifetime;
            _solutionModel = solutionModel;
            _solution = solution;
            _documentManager = documentManager;

            var rdSolutionModel = solutionModel.TryGetCurrentSolution();
            
            if (rdSolutionModel == null) return;
            
            var rdXamlStylerModel = rdSolutionModel.GetXamlStylerModel();
            rdXamlStylerModel.PerformReformat.SetAsync(PerformReformatHandler);
        }

        private Task<RdXamlStylerFormattingResult> PerformReformatHandler(
            Lifetime requestLifetime,
            RdXamlStylerFormattingRequest request)
        {
            return Task.Run(
                    () =>
                    {
                        _lifetime.ThrowIfNotAlive();

                        // Fetch settings
                        var settings = _solution
                            .GetSettingsStore()
                            .SettingsStore.BindToContextLive(
                                _lifetime,
                                ContextRange.Smart(_solution.ToDataContext())
                            );
                        var stylerOptions = StylerOptionsFactory.FromSettings(
                            settings,
                            _solution,
                            null,
                            request.FilePath
                        );

                        // Bail out early if needed
                        if (stylerOptions.SuppressProcessing || !stylerOptions.FormatOnSave)
                            return new RdXamlStylerFormattingResult(false, false, "");

                        // Perform styling
                        var styler = new StylerService(
                            stylerOptions,
                            new XamlLanguageOptions { IsFormatable = true }
                        );

                        var formattedText = styler
                            .StyleDocument(request.DocumentText)
                            .Replace("\r\n", "\n");

                        return request.DocumentText == formattedText ? new RdXamlStylerFormattingResult(true, false, "") : new RdXamlStylerFormattingResult(true, true, formattedText);
                    },
                    requestLifetime
                )
                .ToRdTask();
        }
    }
}