using EnvDTE;
using System.Collections.Generic;
using System.Linq;

namespace Xavalon.XamlStyler.Package
{
    public static class DocumentExtensions
    {
        public static IEnumerable<Document> OpenedOnly(this IEnumerable<Document> source, bool openedOnly)
        {
            if (openedOnly)
            {
                source = source.Where(d => d.ActiveWindow != null);
            }

            return source;
        }
    }
}