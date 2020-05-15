// (c) Xavalon. All rights reserved.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Xml.Linq;

namespace Xavalon.XamlStyler.DocumentManipulation
{
    // TODO: Fully implement IComparable interface.
    [SuppressMessage("Design", "CA1036:Override methods on comparable types", Justification = "Introduces breaking change")]
    public class NodeCollection : IComparable<NodeCollection>
    {
        /// <summary>
        /// This is a block of nodes which usually consist of whitespace, comment and optionally one Element node.
        /// </summary>
        public List<XNode> Nodes { get; private set; }

        /// <summary>
        /// Primary sort index. NodeCollections from different BlockIndexes never mix.
        /// </summary>
        public int BlockIndex { get; set; }

        /// <summary>
        /// This is the collection of attributes for this node
        /// </summary>
        public Collection<ISortableAttribute> SortAttributeValues { get; private set; }

        public void SetSortAttributeValues(Collection<ISortableAttribute> collection)
        {
            this.SortAttributeValues = collection;
        }

        public NodeCollection()
        {
            this.Nodes = new List<XNode>();
        }

        public int CompareTo(NodeCollection other)
        {
            if (this == other)
            {
                return 0;
            }

            var result = this.BlockIndex.CompareTo(other.BlockIndex);
            if (result == 0)
            {
                result = this.SortAttributeValues.Count.CompareTo(other.SortAttributeValues.Count);
                if (result == 0)
                {
                    for (int i = 0; i < this.SortAttributeValues.Count; i++)
                    {
                        result = this.SortAttributeValues[i].CompareTo(other.SortAttributeValues[i]);
                        if (result != 0)
                        {
                            break;
                        }
                    }
                }
            }

            return result;
        }

#if DEBUG
        public override string ToString()
        {
            return String.Format(
                CultureInfo.InvariantCulture,
                "B{0} A{1} N{2}",
                this.BlockIndex,
                String.Join("|", (IEnumerable<ISortableAttribute>)this.SortAttributeValues),
                String.Join("|", this.Nodes));
        }
#endif
    }
}