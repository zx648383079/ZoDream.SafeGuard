using System.Text;

namespace ZoDream.Shared.TextCalibrate.Formatters
{
    public class JsFormatter : ITextFormatter
    {
        public string Format(string value)
        {
            var sb = new StringBuilder();
            var indentCount = 0;
            var oldIndentCount = 0;
            var oldIndentStep = 0;
            var reader = new CharIterator(value);
            var lastIsNewLine = false;
            while (reader.MoveNext())
            {
                var code = reader.Current;
                if (code == '}')
                {
                    if (!lastIsNewLine)
                    {
                        sb.Append('\n');
                        if (indentCount > 0)
                        {
                            indentCount--;
                            oldIndentCount -= oldIndentStep;
                        }
                        sb.Append(new string(' ', indentCount * 4));
                    }
                    sb.Append(code);
                    if (reader.NextIs(';'))
                    {
                        reader.Position++;
                        sb.Append(';');
                    }
                    JumpWhitespace(reader);
                    if (!reader.NextIs('\r', '\n', '}'))
                    {
                        sb.Append('\n');
                        sb.Append(new string(' ', indentCount * 4));
                        lastIsNewLine = true;
                    }
                    continue;
                }
                lastIsNewLine = false;
                if (code is ';' or '{' or ',')
                {
                    sb.Append(code);
                    JumpWhitespace(reader);
                    if (code == '{')
                    {
                        indentCount++;
                        oldIndentCount += oldIndentStep;
                    }
                    if (!reader.NextIs('\r', '\n'))
                    {
                        if (!reader.NextIs('}'))
                        {
                            sb.Append('\n');
                            sb.Append(new string(' ', indentCount * 4));
                            lastIsNewLine = true;
                        }
                    }
                    continue;
                }
                if (code is '\r' or '\n')
                {
                    if (code == '\r' && reader.NextIs('\n'))
                    {
                        reader.Position ++;
                    }
                    var i = WhitespaceCount(reader);
                    if (!reader.NextIs('}'))
                    {
                        sb.Append('\n');
                    }
                    if (!reader.NextIs('\r', '\n', '}'))
                    {
                        if (i > oldIndentCount)
                        {
                            indentCount++;
                        } else if (i < oldIndentCount)
                        {
                            indentCount--;
                        }
                        if (oldIndentCount == 0)
                        {
                            oldIndentStep = i;
                        }
                        oldIndentCount = i;
                        sb.Append(new string(' ', indentCount * 4));
                        lastIsNewLine = true;
                    }
                    continue;
                }
                if (code == '\'' || code == '"')
                {
                    var i = reader.IndexOf(code);
                    sb.Append(reader.Read(i - reader.Position + 1));
                    reader.Position = i;
                    continue;
                }
                if (code == '(')
                {
                    var i = reader.IndexOf(')');
                    var j = reader.IndexOf('{');
                    if ((j > 0 && i > j) || i < 0)
                    {
                        sb.Append(code);
                    } else if (i > 0)
                    {
                        sb.Append(reader.Read(i - reader.Position + 1));
                        reader.Position = i;
                    }
                    continue;
                }
                sb.Append(code);
            }
            return sb.ToString();
        }

        private static int WhitespaceCount(CharIterator reader)
        {
            int count = 0;
            while (reader.MoveNext())
            {
                if (reader.Current is '　' or ' ')
                {
                    count++;
                    continue;
                }
                if (reader.Current == '\t')
                {
                    count+=2;
                    continue;
                }
                reader.Position--;
                break;
            }
            return count;
        }

        private static void JumpWhitespace(CharIterator reader)
        {
            while (reader.MoveNext())
            {
                if (reader.Current is '　' or ' ' or '\t')
                {
                    continue;
                }
                reader.Position--;
                break;
            }
        }

    }
}
