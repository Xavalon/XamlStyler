using EnvDTE;
using System.Collections.Generic;
using System.Linq;

namespace Xavalon.XamlStyler.Package
{
    public static class DocumentExtensions
    {
        public static IEnumerable<Document> OpenedDocumentsOnly(this IEnumerable<Document> source)
        {
            return source.Where(document => document.ActiveWindow != null);
        }
    }
}