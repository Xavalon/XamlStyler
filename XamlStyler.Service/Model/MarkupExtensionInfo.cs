using System.Collections.Generic;

namespace XamlStyler.Core.Model
{
    public class MarkupExtensionInfo
    {
        /// <summary>
        /// Value could be string or MarkupExtensionInfo
        /// </summary>
        public IList<KeyValuePair<string, object>> KeyValueProperties { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Value could be string or MarkupExtensionInfo
        /// </summary>
        public IList<object> ValueOnlyProperties { get; set; }

        public MarkupExtensionInfo()
        {
            ValueOnlyProperties = new List<object>();
            KeyValueProperties = new List<KeyValuePair<string, object>>();
        }
    }
}