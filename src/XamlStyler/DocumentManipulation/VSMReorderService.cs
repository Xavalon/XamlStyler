// (c) Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Xavalon.XamlStyler.Options;

namespace Xavalon.XamlStyler.DocumentManipulation
{
    public class VSMReorderService : IProcessElementService
    {
        private readonly NameSelector VSMNode = new NameSelector("VisualStateManager.VisualStateGroups", null);

        public VisualStateManagerRule Mode { get; set; } = VisualStateManagerRule.None;

        public void ProcessElement(XElement element)
        {
            if (this.Mode == VisualStateManagerRule.None)
            {
                return;
            }

            if (!element.HasElements)
            {
                return;
            }

            if (this.VSMNode.IsMatch(element.Name))
            {
                if (element.Parent != null)
                {
                    // Reorder the parent of any element that contains a VSM.
                    this.ReorderChildNodes(element.Parent);
                }
            }
        }

        /// <summary>
        /// Move VSM to last child of element.
        /// </summary>
        /// <param name="element">Element that is getting its VSM moved to the end.</param>
        private void ReorderChildNodes(XElement element)
        {
            List<NodeCollection> nodeCollections = new List<NodeCollection>();
            List<NodeCollection> propertyElementCollection = new List<NodeCollection>();
            NodeCollection vsmNodeCollection = new NodeCollection();

            var parentName = element.Name;
            var children = element.Nodes().ToList();
            bool hasCollectionBeenAdded = false;

            NodeCollection currentNodeCollection = null;

            //remove last new line node to prevent new lines on every format
            if (this.Mode == VisualStateManagerRule.Last)
            {
                children.Remove(children.Last());
            }

            foreach (var child in children)
            {
                if (currentNodeCollection == null)
                {
                    currentNodeCollection = new NodeCollection();
                }

                currentNodeCollection.Nodes.Add(child);

                if (child.NodeType == XmlNodeType.Element)
                {
                    var childName = ((XElement)child).Name;

                    if (this.VSMNode.IsMatch(childName))
                    {
                        // Extract VSM for adding to end.
                        vsmNodeCollection = currentNodeCollection;
                        hasCollectionBeenAdded = true;
                    }
                    else if(childName.LocalName.StartsWith($"{parentName.LocalName}.", StringComparison.Ordinal))
                    {
                        // Extract property-element syntax nodes.
                        propertyElementCollection.Add(currentNodeCollection);
                        hasCollectionBeenAdded = true;
                    }
                    else if (!hasCollectionBeenAdded)
                    {
                        // Maintain all other nodes.
                        nodeCollections.Add(currentNodeCollection);
                        hasCollectionBeenAdded = true;
                    }

                    currentNodeCollection = null;
                    hasCollectionBeenAdded = false;
                }
            }

            // Add any trailing non-Element nodes (e.g., comments).
            if (currentNodeCollection != null)
            {
                nodeCollections.Add(currentNodeCollection);
            }

            var newNodes = ((this.Mode == VisualStateManagerRule.Last)
                ? propertyElementCollection.SelectMany(_ => _.Nodes)
                    .Concat(nodeCollections.SelectMany(_ => _.Nodes))
                    .Concat(vsmNodeCollection.Nodes)
                : propertyElementCollection.SelectMany(_ => _.Nodes)
                    .Concat(vsmNodeCollection.Nodes)
                    .Concat(nodeCollections.SelectMany(_ => _.Nodes))).ToList();

            var firstNode = newNodes.First() as XText;
            if ((this.Mode == VisualStateManagerRule.Last) 
                && firstNode != null 
                && string.IsNullOrWhiteSpace(firstNode.Value.Trim()))
            {
                newNodes.Remove(firstNode);
            }

            element.ReplaceNodes(newNodes);
        }
    }
}