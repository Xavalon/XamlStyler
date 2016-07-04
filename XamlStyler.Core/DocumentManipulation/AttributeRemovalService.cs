// © Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
    public class AttributeRemovalService : IProcessElementService
    {
        private List<AttributeSelector> initializedAttributes;

        public List<NameSelector> NodeNames { get; } = new List<NameSelector>();

        public List<XNamespace> NamespaceDeclarations { get; } = new List<XNamespace>();

        public List<AttributeSelector> Attributes { get; } = new List<AttributeSelector>();

        public bool IsEnabled { get; set; } = true;

        public void Initialize(XElement element)
        {
            this.initializedAttributes = this.Attributes
                .Select(_ => new AttributeSelector(
                    _.Name, 
                    (element.GetNamespaceOfPrefix(_.Namespace)?.NamespaceName ?? _.Namespace),
                    _.Value))
                .ToList();
        }

        public void ProcessElement(XElement element)
        {
            if (this.initializedAttributes == null)
            {
                throw new InvalidOperationException("AttributeRemovalService not initialized");
            }

            if (!this.IsEnabled)
            {
                return;
            }

            if (!element.HasAttributes)
            {
                return;
            }

            // Process all nodes if NodeNames is empty, otherwise process only specified nodes.
            if ((this.NodeNames.Count == 0) || this.NodeNames.Any(_ => _.IsMatch(element.Name)))
            {
                var elementAttributeList = element.Attributes().ToList();
                var removedAttributeList = new List<XAttribute>();

                foreach (XAttribute elementAttribute in elementAttributeList)
                {
                    if (elementAttribute.IsNamespaceDeclaration)
                    {
                        // Handle namespace declarations separately from other attributes.
                        if (this.NamespaceDeclarations.Any(_ => (_.NamespaceName == elementAttribute.Name.ToString())))
                        {
                            removedAttributeList.Add(elementAttribute);
                        }
                    }
                    else
                    {
                        foreach (AttributeSelector attribute in this.initializedAttributes)
                        {
                            if (attribute.IsMatch(elementAttribute)
                                && (String.IsNullOrEmpty(attribute.Value) || attribute.Value.Equals(elementAttribute.Value)))
                            {
                                removedAttributeList.Add(elementAttribute);
                                continue;
                            }
                        }
                    }
                }

                foreach (XAttribute removedAttribute in removedAttributeList)
                {
                    elementAttributeList.Remove(removedAttribute);
                }

                element.ReplaceAttributes(elementAttributeList);
            }
        }
    }
}