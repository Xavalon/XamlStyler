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
        private readonly IStylerOptions _options;
        private readonly List<IProcessElementService> _processElementServices;

        public DocumentManipulationService(IStylerOptions options)
        {
            _options = options;
            _processElementServices = new List<IProcessElementService>
            {
                new FormatThicknessService(_options.ThicknessStyle, _options.ThicknessAttributes),
                GetReorderGridChildrenService(),
                GetReorderCanvasChildrenService(),
                GetReorderSettersService()
            };
        }

        private NodeReorderService GetReorderGridChildrenService()
        {
            var reorderService = new NodeReorderService { IsEnabled = _options.ReorderGridChildren };
            reorderService.ParentNodeNames.Add(new NameSelector("Grid", null));
            reorderService.ChildNodeNames.Add(new NameSelector(null, null));
            reorderService.SortByAttributes.Add(new SortBy("Grid.Row", null, true));
            reorderService.SortByAttributes.Add(new SortBy("Grid.Column", null, true));
            return reorderService;
        }

        private NodeReorderService GetReorderCanvasChildrenService()
        {
            var reorderService = new NodeReorderService { IsEnabled = _options.ReorderCanvasChildren };
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

            switch (_options.ReorderSetters)
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

        public string ManipulateDocument(XDocument xDocument)
        {
            var xmlDeclaration = xDocument.Declaration?.ToString() ?? string.Empty;
            var rootElement = xDocument.Root;

            if (rootElement != null)
            {
                HandleNode(rootElement);
            }

            return xmlDeclaration + xDocument;
        }

        private void HandleNode(XNode node)
        {
            switch (node.NodeType)
            {
                case XmlNodeType.Element:
                    XElement element = node as XElement;

                    if (element != null && element.Nodes().Any())
                    {
                        // handle children first
                        foreach (var childNode in element.Nodes())
                        {
                            HandleNode(childNode);
                        }
                    }

                    if (element != null)
                    {
                        foreach (var elementService in _processElementServices)
                        {
                            elementService.ProcessElement(element);
                        }
                    }
                    break;
            }
        }
    }
}