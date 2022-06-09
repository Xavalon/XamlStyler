// © Xavalon. All rights reserved.

using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Xavalon.XamlStyler.Options;
using Xavalon.XamlStyler.Extension.Windows.Extensions;

namespace Xavalon.XamlStyler.Extension.Windows
{
    public sealed partial class StylerPackage : AsyncPackage
    {
        private void FormatDocument(Document document, IStylerOptions stylerOptions = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                var xamlLanguageOptions = document.GetXamlLanguageOptions();
                if (xamlLanguageOptions.IsFormatable)
                {
                    SetupFormatDocumentContinuation(document, stylerOptions ?? this.optionsHelper.GetDocumentStylerOptions(document), xamlLanguageOptions)()();
                }
            }
            catch (Exception ex)
            {
                IStylerOptions options = this.optionsHelper.GetDocumentStylerOptions(document);
                if (options.ShowMessageBoxOnError)
                {
                    this.ShowMessageBox(ex);
                }
            }
        }

        private void FormatDocument(ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            Document document = projectItem.Document;

            try
            {
                if (projectItem.IsFormatable())
                {
                    // Open document if not already open.
                    bool wasOpen = projectItem.IsOpen[EnvDTE.Constants.vsViewKindTextView] || projectItem.IsOpen[EnvDTE.Constants.vsViewKindCode];

                    if (!wasOpen)
                    {
                        try
                        {
                            projectItem.Open(EnvDTE.Constants.vsViewKindTextView);
                        }
                        catch (Exception)
                        {
                            // Skip if file cannot be opened.
                        }
                    }
                    
                    if (document != null)
                    {
                        document.Activate();
                        this.FormatDocument(document);

                        IStylerOptions globalOptions = this.optionsHelper.GetGlobalStylerOptions();
                        IStylerOptions options = this.optionsHelper.GetDocumentStylerOptions(document);
                        if (!wasOpen && globalOptions.SaveAndCloseOnFormat && options.SaveAndCloseOnFormat)
                        {
                            document.Close(vsSaveChanges.vsSaveChangesYes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                IStylerOptions options = this.optionsHelper.GetDocumentStylerOptions(document);
                if (options.ShowMessageBoxOnError)
                {
                    this.ShowMessageBox(ex);
                }
            }
        }

        private void FormatDocuments(IEnumerable<ProjectItem> projectItems)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            foreach (ProjectItem projectItem in projectItems)
            {
                Debug.WriteLine($"Processing: {projectItem.GetFileName()}");
                this.FormatDocument(projectItem);
            }
        }

        private static Func<Action> SetupFormatDocumentContinuation(Document document, IStylerOptions stylerOptions, XamlLanguageOptions xamlLanguageOptions)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var textDocument = (TextDocument)document.Object("TextDocument");
            EditPoint startPoint = textDocument.StartPoint.CreateEditPoint();
            EditPoint endPoint = textDocument.EndPoint.CreateEditPoint();
            string xamlSource = startPoint.GetText(endPoint);

            return () =>
            {
                // This part can be executed in parallel.
                var styler = new StylerService(stylerOptions, xamlLanguageOptions);
                xamlSource = styler.StyleDocument(xamlSource);

                return () =>
                {
                    // This part should be executed sequentially.
                    ThreadHelper.ThrowIfNotOnUIThread();
                    startPoint.ReplaceText(endPoint, xamlSource, (int)vsEPReplaceTextOptions.vsEPReplaceTextKeepMarkers);
                };
            };
        }
    }
}