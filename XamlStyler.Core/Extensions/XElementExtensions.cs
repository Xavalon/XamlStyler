// © Xavalon. All rights reserved.

using System.Linq;
using System.Xml.Linq;

namespace Xavalon.XamlStyler.Core.Extensions
{
    public static class XElementExtensions
    {
        public static void RemoveWithTrailingWhitespace(this XElement element)
        {
            var textNodes = element.NodesAfterSelf().TakeWhile(_ => _ is XText).Cast<XText>();

            if (element.ElementsAfterSelf().Any())
            {
                // Remove trailing text nodes.
                textNodes.ToList().ForEach(_ => _.Remove());
            }
            else
            {
                // Remove trailing whitespace.
                textNodes.TakeWhile(_ => !_.Value.Contains("\n")).ToList().ForEach(_ => _.Remove());

                // Retrieve text node containing newline, if any.
                var newLineTextNode = element.NodesAfterSelf().OfType<XText>().FirstOrDefault();

                if (newLineTextNode != null)
                {
                    string value = newLineTextNode.Value;
                    if (value.Length > 1)
                    {
                        // Composite text node, trim until newline (inclusive).
                        newLineTextNode.AddAfterSelf(new XText(value.Substring(value.IndexOf('\n') + 1)));
                    }

                    // Remove original node.
                    newLineTextNode.Remove();
                }
            }

            element.Remove();
        }
    }
}