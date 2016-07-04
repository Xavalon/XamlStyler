// © Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Xavalon.XamlStyler.Core.Options;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public class DocumentManipulationService
    {
        private readonly IStylerOptions options;
        private readonly List<IProcessElementService> processElementServices;

        public DocumentManipulationService(IStylerOptions options)
        {
            this.options = options;
            this.processElementServices = new List<IProcessElementService>
            {
                new VSMReorderService() { Mode = this.options.ReorderVSM },
                new FormatThicknessService(this.options.ThicknessStyle, this.options.ThicknessAttributes),
                this.GetReorderGridChildrenService(),
                this.GetReorderCanvasChildrenService(),
                this.GetReorderSettersService()
            };
        }

        public string ManipulateDocument(XDocument xDocument)
        {
            var xmlDeclaration = xDocument.Declaration?.ToString() ?? string.Empty;
            var rootElement = xDocument.Root;

            if (rootElement != null)
            {
                this.processElementServices.Add(this.GetRemoveDesignTimeReferencesService(rootElement));
                this.HandleNode(rootElement);
            }

            return (xmlDeclaration + xDocument);
        }

        private AttributeRemovalService GetRemoveDesignTimeReferencesService(XElement element)
        {
            var removalService = new AttributeRemovalService() { IsEnabled = this.options.RemoveDesignTimeReferences };
            removalService.NamespaceDeclarations.Add(XNamespace.Get($"{{{XNamespace.Xmlns.NamespaceName}}}d"));
            removalService.NamespaceDeclarations.Add(XNamespace.Get($"{{{XNamespace.Xmlns.NamespaceName}}}mc"));
            removalService.Attributes.Add(new AttributeSelector("*", "d", null));
            removalService.Attributes.Add(new AttributeSelector("*", "mc", null));
            removalService.Initialize(element);
            return removalService;
        }

        private NodeReorderService GetReorderGridChildrenService()
        {
            var reorderService = new NodeReorderService { IsEnabled = this.options.ReorderGridChildren };
            reorderService.ParentNodeNames.Add(new NameSelector("Grid", null));
            reorderService.ChildNodeNames.Add(new NameSelector(null, null));
            reorderService.SortByAttributes.Add(new SortBy("Grid.Row", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Grid.Column", null, true));
            return reorderService;
        }

        private NodeReorderService GetReorderCanvasChildrenService()
        {
            var reorderService = new NodeReorderService { IsEnabled = this.options.ReorderCanvasChildren };
            reorderService.ParentNodeNames.Add(new NameSelector("Canvas", null));
            reorderService.ChildNodeNames.Add(new NameSelector(null, null));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Left", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Top", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Right", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Canvas.Bottom", null, true));
            return reorderService;
        }

        private NodeReorderService GetReorderSettersService()
        {
            var reorderService = new NodeReorderService();
            reorderService.ParentNodeNames.Add(new NameSelector("DataTrigger", null));
            reorderService.ParentNodeNames.Add(new NameSelector("MultiDataTrigger", null));
            reorderService.ParentNodeNames.Add(new NameSelector("MultiTrigger", null));
            reorderService.ParentNodeNames.Add(new NameSelector("Style", null));
            reorderService.ParentNodeNames.Add(new NameSelector("Trigger", null));
            reorderService.ChildNodeNames.Add(new NameSelector("Setter", "http://schemas.microsoft.com/winfx/2006/xaml/presentation"));

            switch (this.options.ReorderSetters)
            {
                case ReorderSettersBy.None:
                    reorderService.IsEnabled = false;
                    break;

                case ReorderSettersBy.Property:
                    reorderService.SortByAttributes.Add(new SortBy("Property", null, false));
                    break;

                case ReorderSettersBy.TargetName:
                    reorderService.SortByAttributes.Add(new SortBy("TargetName", null, false));
                    break;

                case ReorderSettersBy.TargetNameThenProperty:
                    reorderService.SortByAttributes.Add(new SortBy("TargetName", null, false));
                    reorderService.SortByAttributes.Add(new SortBy("Property", null, false));
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            return reorderService;
        }

        private void HandleNode(XNode node)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    XElement element = node as XElement;

                    // Handle children first.
                    if (element?.Nodes().Any() ?? false)
                    {
                        foreach (var childNode in element.Nodes())
                        {
                            this.HandleNode(childNode);
                        }
                    }

                    if (element != null)
                    {
                        foreach (var elementService in this.processElementServices)
                        {
                            elementService.ProcessElement(element);
                        }
                    }

                    break;
            }
        }
    }
}