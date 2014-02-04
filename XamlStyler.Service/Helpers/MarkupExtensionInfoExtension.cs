using System;
using System.Collections.Generic;
using System.Text;
using XamlStyler.Core.Model;

namespace XamlStyler.Core.Helpers
{
    public static class MarkupExtensionInfoExtension
    {
        #region Methods

        public static string ToMultiLineString(this MarkupExtensionInfo info, string baseIndentationString)
        {
            string currentIndentationString = baseIndentationString + String.Empty.PadLeft(info.Name.Length + 2, ' ');

            var buffer = new StringBuilder();
            buffer.Append('{');

            buffer.Append(info.Name);

            if (info.ValueOnlyProperties.Count > 0)
            {
                buffer.Append(' ');

                for (int i = 0; i < info.ValueOnlyProperties.Count; i++)
                {
                    object valueObject = info.ValueOnlyProperties[i];

                    if (valueObject is MarkupExtensionInfo)
                    {
                        var nestedInfo = valueObject as MarkupExtensionInfo;
                        string value = nestedInfo.ToMultiLineString(currentIndentationString);
                        buffer.Append(value);
                    }
                    else
                    {
                        var value = valueObject as String;
                        value = value.ToXmlEncodedString();
                        buffer.Append(value);
                    }

                    if (i == info.ValueOnlyProperties.Count - 1)
                    {
                        if (info.KeyValueProperties.Count > 0)
                        {
                            buffer.Append(',');
                            buffer.Append(Environment.NewLine);
                            buffer.Append(currentIndentationString);
                        }
                        else
                        {
                            // Do nothing
                        }
                    }
                    else
                    {
                        buffer.Append(',');
                        buffer.Append(Environment.NewLine);
                        buffer.Append(currentIndentationString);
                    }
                }
            }

            if (info.KeyValueProperties.Count > 0)
            {
                // Append a space after Markup Extension Name
                if (0 == info.ValueOnlyProperties.Count)
                {
                    buffer.Append(' ');
                }

                for (int i = 0; i < info.KeyValueProperties.Count; i++)
                {
                    KeyValuePair<string, object> keyValue = info.KeyValueProperties[i];

                    if (keyValue.Value is MarkupExtensionInfo)
                    {
                        var nestedInfo = keyValue.Value as MarkupExtensionInfo;
                        string nextLevelIndentationString = currentIndentationString +
                                                            String.Empty.PadLeft(keyValue.Key.Length + 1, ' ');
                        string value = nestedInfo.ToMultiLineString(nextLevelIndentationString);

                        buffer.AppendFormat("{0}={1}", keyValue.Key, value);
                    }
                    else
                    {
                        var value = keyValue.Value as String;
                        value = value.ToXmlEncodedString();
                        buffer.AppendFormat("{0}={1}", keyValue.Key, value);
                    }

                    if (i != info.KeyValueProperties.Count - 1)
                    {
                        buffer.Append(',');
                        buffer.Append(Environment.NewLine);
                        buffer.Append(currentIndentationString);
                    }
                }
            }

            buffer.Append('}');

            return buffer.ToString();
        }

        public static string ToSingleLineString(this MarkupExtensionInfo info)
        {
            var buffer = new StringBuilder();
            buffer.Append('{');

            buffer.Append(info.Name);

            if (info.ValueOnlyProperties.Count > 0)
            {
                for (int i = 0; i < info.ValueOnlyProperties.Count; i++)
                {
                    object valueObject = info.ValueOnlyProperties[i];

                    buffer.Append(' ');

                    if (valueObject is MarkupExtensionInfo)
                    {
                        var nestedInfo = valueObject as MarkupExtensionInfo;
                        string value = nestedInfo.ToSingleLineString();

                        buffer.Append(value);
                    }
                    else
                    {
                        var value = valueObject as String;
                        value = value.ToXmlEncodedString();
                        buffer.Append(value);
                    }

                    if (i == info.ValueOnlyProperties.Count - 1)
                    {
                        if (info.KeyValueProperties.Count > 0)
                        {
                            buffer.Append(',');
                        }
                        else
                        {
                            // Do nothing
                        }
                    }
                    else
                    {
                        buffer.Append(',');
                    }
                }
            }

            if (info.KeyValueProperties.Count > 0)
            {
                for (int i = 0; i < info.KeyValueProperties.Count; i++)
                {
                    buffer.Append(' ');

                    KeyValuePair<string, object> keyValue = info.KeyValueProperties[i];

                    if (keyValue.Value is MarkupExtensionInfo)
                    {
                        var nestedInfo = keyValue.Value as MarkupExtensionInfo;
                        string value = nestedInfo.ToSingleLineString();

                        buffer.AppendFormat("{0}={1}", keyValue.Key, value);
                    }
                    else
                    {
                        var value = keyValue.Value as String;
                        value = value.ToXmlEncodedString();
                        buffer.AppendFormat("{0}={1}", keyValue.Key, value);
                    }

                    if (i != info.KeyValueProperties.Count - 1)
                    {
                        buffer.Append(',');
                    }
                }
            }

            buffer.Append('}');

            return buffer.ToString();
        }

        #endregion Methods
    }
}