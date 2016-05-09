using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace Xavalon.XamlStyler.Core.DocumentManipulation
{
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
        public ISortableAttribute[] SortAttributeValues { get; set; }

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
                result = this.SortAttributeValues.Length.CompareTo(other.SortAttributeValues.Length);
                if (result == 0)
                {
                    for (int i = 0; i < this.SortAttributeValues.Length; i++)
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
            return string.Format(
                "B{0} A{1} N{2}",
                this.BlockIndex,
                String.Join("|", (IEnumerable<ISortableAttribute>)this.SortAttributeValues),
                String.Join("|", this.Nodes));
        }

#endif
    }
}