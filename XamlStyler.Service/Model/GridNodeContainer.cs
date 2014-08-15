using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace XamlStyler.Core.Model
{
    public class GridNodeContainer : NodeContainer
    {

        public int Row { get; set; }
        public int Column { get; set; }
        public GridNodeContainer(XNode node, int row, int column)
        {
            Node = node;
            Row = row;
            Column = column;
        }
    }
}
