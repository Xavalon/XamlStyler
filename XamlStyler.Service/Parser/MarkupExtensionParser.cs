using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using XamlStyler.Core.Helpers;
using XamlStyler.Core.Model;

namespace XamlStyler.Core.Parser
{
    internal static class MarkupExtensionParser
    {
        // Fields
        private static readonly Regex MarkupExtensionPattern = new Regex("^{(?!}).*}$");

        public static MarkupExtensionInfo Parse(string input)
        {
            #region Parameter Checks

            if (!MarkupExtensionPattern.IsMatch(input))
            {
                string msg = $"{input} is not a MarkupExtension.";
                throw new InvalidOperationException(msg);
            }

            #endregion Parameter Checks

            var resultInfo = new MarkupExtensionInfo();

            using (var reader = new StringReader(input))
            {
                var parsingMode = MarkupExtensionParsingModeEnum.START;

                try
                {
                    //Debug.Print("Parsing '{0}'", input);
                    //Debug.Indent();

                    while (MarkupExtensionParsingModeEnum.END != parsingMode
                           && MarkupExtensionParsingModeEnum.UNEXPECTED != parsingMode)
                    {
                        //Debug.Print(context.ToString());
                        //Debug.Indent();

                        switch (parsingMode)
                        {
                            case MarkupExtensionParsingModeEnum.START:
                                parsingMode = reader.ReadMarkupExtensionStart();
                                break;

                            case MarkupExtensionParsingModeEnum.MARKUP_NAME:
                                parsingMode = reader.ReadMarkupName(resultInfo);
                                break;

                            case MarkupExtensionParsingModeEnum.NAME_VALUE_PAIR:
                                parsingMode = reader.ReadNameValuePair(resultInfo);
                                break;
                        }

                        //Debug.Unindent();
                    }
                }
                catch (Exception exp)
                {
                    throw new InvalidDataException(
                        String.Format("Cannot parse markup extension string:\r\n \"{0}\"", input), exp);
                }
            }

            return resultInfo;
        }

        // TODO: move following extension methods to helper namespace
        private static bool IsEnd(this StringReader reader)
        {
            return (reader.Peek() < 0);
        }

        private static char PeekChar(this StringReader reader)
        {
            var result = (char) reader.Peek();

            //Debug.Print("?Peek '{0}'", result);

            return result;
        }

        private static char ReadChar(this StringReader reader)
        {
            var result = (char) reader.Read();

            //Debug.Print("!Read '{0}'", result);

            return result;
        }

        private static MarkupExtensionParsingModeEnum ReadMarkupExtensionStart(this StringReader reader)
        {
            reader.SeekTill(x => '{' != x && !Char.IsWhiteSpace(x));

            return MarkupExtensionParsingModeEnum.MARKUP_NAME;
        }

        private static MarkupExtensionParsingModeEnum ReadMarkupName(this StringReader reader, MarkupExtensionInfo info)
        {
            char[] stopChars = {' ', '}'};
            var resultParsingMode = MarkupExtensionParsingModeEnum.UNEXPECTED;
            var buffer = new StringBuilder();

            while (!reader.IsEnd())
            {
                char c = reader.ReadChar();

                if (stopChars.Contains(c))
                {
                    switch (c)
                    {
                        case ' ':
                            resultParsingMode = MarkupExtensionParsingModeEnum.NAME_VALUE_PAIR;
                            break;

                        case '}':
                            resultParsingMode = MarkupExtensionParsingModeEnum.END;
                            break;

                        default:
                            throw new InvalidDataException($"[{nameof(ReadMarkupName)}] Should not encounter '{c}'.");
                    }

                    info.Name = buffer.ToString().Trim();
                    buffer.Clear();

                    // break out the while
                    break;
                }
                
                buffer.Append(c);
            }

            if (MarkupExtensionParsingModeEnum.UNEXPECTED == resultParsingMode)
            {
                throw new InvalidDataException($"[{nameof(ReadMarkupName)}] Invalid result context: {resultParsingMode}");
            }

            return resultParsingMode;
        }

        private static MarkupExtensionParsingModeEnum ReadNameValuePair(this StringReader reader,
                                                                        MarkupExtensionInfo info)
        {
            char[] stopChars = {',', '=', '}'};

            MarkupExtensionParsingModeEnum resultParsingMode;
            string key = null;
            object value = null;

            reader.SeekTill(x => !Char.IsWhiteSpace(x));

            // When '{' is the starting char, the following must be a value instead of a key.
            //
            // E.g.,
            //    <Setter x:Uid="Setter_75"
            //            Property="Foreground"
            //            Value="{DynamicResource {x:Static SystemColors.ControlTextBrushKey}}" />
            //
            // In other words, "key" shall not start with '{', as it won't be a valid property name.
            if ('{' != reader.PeekChar())
            {
                string temp = reader.ReadTill(stopChars.Contains).Trim();
                char keyValueIndicatorChar = reader.PeekChar();

                switch (keyValueIndicatorChar)
                {
                    case ',':
                    case '}':
                        value = temp;
                        break;

                    case '=':
                        key = temp;

                        // Consume the '='
                        reader.Read();
                        break;

                    default:
                        throw new InvalidDataException($"[{nameof(ReadNameValuePair)}] Should not encounter '{keyValueIndicatorChar}'.");
                }
            }

            if (value == null)
            {
                reader.SeekTill(x => !(Char.IsWhiteSpace(x)));

                string input = reader.ReadValueString();

                if (MarkupExtensionPattern.IsMatch(input))
                {
                    value = Parse(input);
                }
                else
                {
                    value = input;
                }
            }

            if (String.IsNullOrEmpty(key))
            {
                info.ValueOnlyProperties.Add(value);
            }
            else
            {
                info.KeyValueProperties.Add(new KeyValuePair<string, object>(key, value));
            }

            reader.SeekTill(x => !Char.IsWhiteSpace(x));

            char stopChar = reader.ReadChar();

            switch (stopChar)
            {
                case ',':
                    resultParsingMode = MarkupExtensionParsingModeEnum.NAME_VALUE_PAIR;
                    break;

                case '}':
                    resultParsingMode = MarkupExtensionParsingModeEnum.END;
                    break;

                default:
                    throw new InvalidDataException($"[{nameof(ReadNameValuePair)}] Should not encounter '{stopChar}'.");
            }

            if (MarkupExtensionParsingModeEnum.UNEXPECTED == resultParsingMode)
            {
                throw new InvalidDataException($"[{nameof(ReadNameValuePair)}] Invalid result context: {resultParsingMode}");
            }

            return resultParsingMode;
        }

        private static string ReadTill(this StringReader reader, Func<char, bool> stopAt)
        {
            var buffer = new StringBuilder();

            while (!reader.IsEnd())
            {
                if (stopAt((char) reader.Peek()))
                {
                    break;
                }
                buffer.Append(reader.ReadChar());
            }

            if (reader.IsEnd())
            {
                throw new InvalidDataException($"[{nameof(ReadTill)}] Cannot meet the stop condition.");
            }

            return buffer.ToString();
        }

        private static string ReadValueString(this StringReader reader)
        {
            var buffer = new StringBuilder();
            int curlyBracePairCounter = 0;
            MarkupExtensionParsingModeEnum parsingMode;

            // ignore leading spaces
            reader.SeekTill(x => !Char.IsWhiteSpace(x));

            // Determine parsing mode
            char c = reader.ReadChar();
            buffer.Append(c);

            if ('{' == c)
            {
                char peek = reader.PeekChar();
                parsingMode = '}' != peek ? MarkupExtensionParsingModeEnum.MARKUP_EXTENSION_VALUE : MarkupExtensionParsingModeEnum.LITERAL_VALUE;
                curlyBracePairCounter++;
            }
            else if ('\'' == c)
            {
                parsingMode = MarkupExtensionParsingModeEnum.QUOTED_LITERAL_VALUE;
            }
            else
            {
                parsingMode = MarkupExtensionParsingModeEnum.LITERAL_VALUE;
            }

            switch (parsingMode)
            {
                case MarkupExtensionParsingModeEnum.MARKUP_EXTENSION_VALUE:
                    while (curlyBracePairCounter > 0 && (!reader.IsEnd()))
                    {
                        c = reader.ReadChar();
                        buffer.Append(c);

                        switch (c)
                        {
                            case '{':
                                curlyBracePairCounter++;
                                break;

                            case '}':
                                curlyBracePairCounter--;
                                break;
                        }
                    }
                    break;

                case MarkupExtensionParsingModeEnum.QUOTED_LITERAL_VALUE:

                    // Following case is handled:
                    //      StringFormat='{}{0}\'s email'
                    do
                    {
                        buffer.Append(reader.ReadTill(x => '\'' == x));
                        buffer.Append(reader.ReadChar());
                    } while (buffer.Length > 2 && '\'' == buffer[buffer.Length - 1] && '\\' == buffer[buffer.Length - 2]);

                    break;

                case MarkupExtensionParsingModeEnum.LITERAL_VALUE:
                    bool shouldStop = false;

                    while (!reader.IsEnd())
                    {
                        switch (reader.PeekChar())
                        {
                            case '{':
                                curlyBracePairCounter++;
                                break;

                            case '}':
                                if (curlyBracePairCounter > 0)
                                {
                                    curlyBracePairCounter--;
                                }
                                else
                                {
                                    shouldStop = true;
                                }
                                break;

                            // Escape character
                            case '\\':
                                buffer.Append(reader.ReadChar());
                                break;

                            case ',':
                                shouldStop = (curlyBracePairCounter == 0);
                                break;
                        }

                        if (!shouldStop)
                        {
                            buffer.Append(reader.ReadChar());
                        }
                        else
                        {
                            break;
                        }
                    }
                    break;

                default:
                    throw new InvalidDataException($"[{nameof(ReadValueString)}] Should not encouter parsingMode {parsingMode}");
            }

            return buffer.TrimUnescaped(' ').ToString();
        }

        private static void SeekTill(this StringReader reader, Func<char, bool> stopAt)
        {
            while (!reader.IsEnd())
            {
                if (stopAt((char) reader.Peek()))
                {
                    break;
                }
                reader.ReadChar();
            }

            if (reader.IsEnd())
            {
                throw new InvalidDataException($"[{nameof(SeekTill)}] Cannot meet the stop condition.");
            }
        }
    }
}