using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XamlStyler.Core.Model
{

    public class CanvasNodeContainer : NodeContainer
    {

        public int Left { get; set; }
        public int Top { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }


        public CanvasNodeContainer(XNode node, int left, int top, int right, int bottom)
        {
            Node = node;
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
