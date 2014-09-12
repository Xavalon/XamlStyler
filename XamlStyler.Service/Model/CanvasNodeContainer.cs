using System.Globalization;
using System.Xml.Linq;

namespace XamlStyler.Core.Model
{
    public class CanvasNodeContainer : NodeContainer
    {
        public string Left { get; set; }
        public string Top { get; set; }
        public string Right { get; set; }
        public string Bottom { get; set; }

        public double LeftNumeric { get; set; }
        public double TopNumeric { get; set; }
        public double RightNumeric { get; set; }
        public double BottomNumeric { get; set; }

        public CanvasNodeContainer(XNode node, double left, double top, double right, double bottom)
        {
            Node = node;
            LeftNumeric = left;
            TopNumeric = top;
            RightNumeric = right;
            BottomNumeric = bottom;

            Left = LeftNumeric.ToString();
            Top = TopNumeric.ToString();
            Right = RightNumeric.ToString();
            Bottom = BottomNumeric.ToString();

        }


        public CanvasNodeContainer(XNode node, string left, string top, string right, string bottom)
        {
            Node = node;

            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;

            ParseValues();
        }

        private void ParseValues()
        {
            double leftNumeric;
            double rightNumeric;
            double topNumeric;
            double bottomNumeric;
            double.TryParse(Left, NumberStyles.Number, CultureInfo.InvariantCulture, out leftNumeric);
            double.TryParse(Right, NumberStyles.Number, CultureInfo.InvariantCulture, out rightNumeric);
            double.TryParse(Bottom, NumberStyles.Number, CultureInfo.InvariantCulture, out bottomNumeric);
            double.TryParse(Top, NumberStyles.Number, CultureInfo.InvariantCulture, out topNumeric);

            LeftNumeric = leftNumeric;
            TopNumeric = topNumeric;
            RightNumeric = rightNumeric;
            BottomNumeric = bottomNumeric;
        }
    }
}
