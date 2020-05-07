using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using JetBrains.Application.Progress;
using JetBrains.Application.Settings;
using JetBrains.Application.UI.Controls.BulbMenu.Anchors;
using JetBrains.ProjectModel;
using JetBrains.ProjectModel.DataContext;
using JetBrains.ReSharper.Feature.Services.Bulbs;
using JetBrains.ReSharper.Feature.Services.ContextActions;
using JetBrains.ReSharper.Feature.Services.Intentions;
using JetBrains.ReSharper.Feature.Services.Xaml.Bulbs;
using JetBrains.ReSharper.Psi;
using JetBrains.ReSharper.Psi.Files;
using JetBrains.ReSharper.Psi.Xaml;
using JetBrains.TextControl;
using JetBrains.Util;
using Xavalon.XamlStyler.Core;

namespace ReSharperPlugin.XamlStyler.dotUltimate
{
    [ContextAction(
        Name = "XamlStyler.Reformat",
        Description = "Formats the document(s) using XAML Styler.",
        Group = CommonContextActions.GroupID,
        Disabled = false,
        Priority = 100)]
    public class XamlStylerReformatContextAction : ContextActionBase
    {
        [NotNull] private readonly XamlContextActionDataProvider _dataProvider;
        [NotNull] private readonly string _text;
        private readonly ActionAppliesTo _actionAppliesTo;

        public XamlStylerReformatContextAction([NotNull] XamlContextActionDataProvider dataProvider)
            : this(dataProvider, "Format with XAML Styler", ActionAppliesTo.File)
        {
        }
        
        private XamlStylerReformatContextAction(
            [NotNull] XamlContextActionDataProvider dataProvider, 
            [NotNull] string text,
            ActionAppliesTo actionAppliesTo)
        {
            _dataProvider = dataProvider ?? throw new ArgumentNullException(nameof(dataProvider));
            _text = text ?? throw new ArgumentNullException(nameof(text));
            _actionAppliesTo = actionAppliesTo;
        }
 
        public override string Text => _text;
        
        public override bool IsAvailable(IUserDataHolder cache) => _dataProvider.Document != null;

        public override IEnumerable<IntentionAction> CreateBulbItems()
        {
            var mainAnchor = new SubmenuAnchor(IntentionsAnchors.ContextActionsAnchor, 
                new SubmenuBehavior(text: null, icon: null, executable: true, removeFirst: true));
            var subAnchor2 = new InvisibleAnchor(mainAnchor);
            var subAnchor3 = subAnchor2.CreateNext(separate: true);

            IntentionAction Create(string text, ActionAppliesTo appliesTo, IAnchor anchor)
            {
                return new XamlStylerReformatContextAction(_dataProvider, text, appliesTo)
                    .ToContextActionIntention(anchor, null);
            }
            
            return new[]
            {
                Create(_text, _actionAppliesTo, subAnchor3),
                Create("Format XAML file", ActionAppliesTo.File, subAnchor3),
                Create("Format XAML files in project", ActionAppliesTo.Project, subAnchor3),
                Create("Format XAML files in solution", ActionAppliesTo.Solution, subAnchor3)
            };
        } 

        protected override Action<ITextControl> ExecutePsiTransaction(
            [NotNull] ISolution solution, 
            [NotNull] IProgressIndicator progress)
        {
            // Fetch settings
            var lifetime = solution.GetLifetime();
            var settings = solution.GetSettingsStore().SettingsStore.BindToContextLive(lifetime, ContextRange.Smart(solution.ToDataContext()));
            var stylerOptions = StylerOptionsFactory.FromSettings(
                settings,
                solution, 
                _dataProvider.Project,
                _actionAppliesTo == ActionAppliesTo.File 
                    ? _dataProvider.SourceFile as IPsiSourceFileWithLocation // Traverse config chain from file path
                    : null  // Traverse config chain from project path
            );

            // Bail out early if needed
            if (stylerOptions.SuppressProcessing) return null;
            
            // Perform styling
            var styler = new StylerService(stylerOptions);
            
            var psiSourceFiles = 
                _actionAppliesTo == ActionAppliesTo.File ? _dataProvider.Document.GetPsiSourceFiles(solution).AsIReadOnlyList()
                    : _actionAppliesTo == ActionAppliesTo.Project ? _dataProvider.Project.GetAllProjectFiles(it => it.LanguageType.Is<XamlProjectFileType>()).SelectMany(file => file.ToSourceFiles().AsIReadOnlyList())
                        : _dataProvider.Solution.GetAllProjects().SelectMany(project => project.GetAllProjectFiles(it => it.LanguageType.Is<XamlProjectFileType>()).SelectMany(file => file.ToSourceFiles().AsIReadOnlyList()));

            foreach (var prjItem in psiSourceFiles)
            {
                foreach (var file in prjItem.GetPsiFiles<XamlLanguage>())
                {
                    var sourceFile = file.GetSourceFile();
                    if (sourceFile?.Document != null)
                    {
                        var oldText = sourceFile.Document.GetText();
                        var newText = styler.StyleDocument(oldText).Replace("\r\n", "\n");
                        file.ReParse(new TreeTextRange(new TreeOffset(0), new TreeOffset(oldText.Length)), newText);
                    }
                }
            }

            return null;
        }

        private enum ActionAppliesTo
        {
            File,
            Project,
            Solution
        }
    }
}