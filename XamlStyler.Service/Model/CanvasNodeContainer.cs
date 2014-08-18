using System.Xml.Linq;

namespace XamlStyler.Core.Model
{

    public class CanvasNodeContainer : NodeContainer
    {

        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }


        public CanvasNodeContainer(XNode node, double left, double top, double right, double bottom)
        {
            Node = node;
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
