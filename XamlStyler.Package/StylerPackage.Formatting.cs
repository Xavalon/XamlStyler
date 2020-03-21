﻿// © Xavalon. All rights reserved.

using EnvDTE;
using Microsoft.VisualStudio.Shell;
using System;
using Xavalon.XamlStyler.Core;
using Xavalon.XamlStyler.Core.Options;
using Xavalon.XamlStyler.Package.Extensions;

namespace Xavalon.XamlStyler.Package
{
    public sealed partial class StylerPackage : AsyncPackage
    {
        private void FormatDocument(ProjectItem projectItem)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
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

                    Document document = projectItem.Document;
                    if (document != null)
                    {
                        document.Activate();
                        this.FormatDocument(document);

                        if (!wasOpen)
                        {
                            document.Close(vsSaveChanges.vsSaveChangesYes);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageBox(ex);
            }
        }

        private void FormatDocument(Document document, IStylerOptions stylerOptions = null)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            try
            {
                if (document.IsFormatable())
                {
                    SetupFormatDocumentContinuation(document, stylerOptions ?? this.optionsHelper.GetDocumentStylerOptions(document))()();
                }
            }
            catch (Exception ex)
            {
                this.ShowMessageBox(ex);
            }
        }

        private static Func<Action> SetupFormatDocumentContinuation(Document document, IStylerOptions stylerOptions)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            var textDocument = (TextDocument)document.Object("TextDocument");
            EditPoint startPoint = textDocument.StartPoint.CreateEditPoint();
            EditPoint endPoint = textDocument.EndPoint.CreateEditPoint();
            string xamlSource = startPoint.GetText(endPoint);

            return () =>
            {
                // This part can be executed in parallel.
                var styler = new StylerService(stylerOptions);
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