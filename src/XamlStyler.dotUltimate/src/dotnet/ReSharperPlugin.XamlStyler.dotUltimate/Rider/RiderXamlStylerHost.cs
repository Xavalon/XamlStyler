using System.Threading.Tasks;
using JetBrains.Annotations;
using JetBrains.Application.Settings;
using JetBrains.DocumentManagers;
using JetBrains.Lifetimes;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.Rd.Tasks;
using JetBrains.ReSharper.Host.Features;
using JetBrains.Rider.Model;
using Xavalon.XamlStyler.Core;

namespace ReSharperPlugin.XamlStyler.dotUltimate
{
    [SolutionComponent]
    public class RiderXamlStylerHost
    {
        [NotNull] private readonly Lifetime _lifetime;
        [NotNull] private readonly SolutionModel _solutionModel;
        [NotNull] private readonly ISolution _solution;
        [NotNull] private readonly DocumentManager _documentManager;

        public RiderXamlStylerHost(
            [NotNull] Lifetime lifetime,
            [NotNull] SolutionModel solutionModel, 
            [NotNull] ISolution solution,
            [NotNull] DocumentManager documentManager)
        {
            _lifetime = lifetime;
            _solutionModel = solutionModel;
            _solution = solution;
            _documentManager = documentManager;

            var rdSolutionModel = solutionModel.TryGetCurrentSolution();
            if (rdSolutionModel != null)
            {
                var rdXamlStylerModel = rdSolutionModel.GetXamlStylerModel();
                rdXamlStylerModel.PerformReformat.Set(PerformReformatHandler);
            }
        }

        private RdTask<RdXamlStylerFormattingResult> PerformReformatHandler(
            Lifetime requestLifetime, 
            RdXamlStylerFormattingRequest request)
        {
            return Task.Run(() =>
            {
                _lifetime.ThrowIfNotAlive();
            
                // Fetch settings
                var settings = _solution.GetSettingsStore().SettingsStore.BindToContextLive(_lifetime, ContextRange.Smart(_solution.ToDataContext()));
                var stylerOptions = StylerOptionsFactory.FromSettings(
                    settings,
                    _solution, 
                    null,
                    request.FilePath);

                // Bail out early if needed
                if (stylerOptions.SuppressProcessing || !stylerOptions.FormatOnSave) return new RdXamlStylerFormattingResult(false, false, "");
            
                // Perform styling
                var styler = new StylerService(stylerOptions);
                var formattedText = styler.StyleDocument(request.DocumentText).Replace("\r\n", "\n");

                if (request.DocumentText == formattedText) {
                    return new RdXamlStylerFormattingResult(true, false, "");
                }
                return new RdXamlStylerFormattingResult(true, true, formattedText);
            }, requestLifetime).ToRdTask();
        }
    }
}