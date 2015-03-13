using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace XamlStyler.Core.Model
{
    public class SetterNodeCollection
    {
        public List<XNode> Nodes { get; private set; }
        public int BlockIndex { get; set; }
        public string Property { get; set; }
        public string TargetName { get; set; }

        public SetterNodeCollection()
        {
            Nodes = new List<XNode>();
        }

        public override string ToString()
        {
            return string.Format("B{0} {1}", BlockIndex, String.Join("|", Nodes));
        }
    }
}