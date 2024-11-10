namespace SchoolHub.Web.Infrastructure
{
    using System.Text;

    public class HtmlEncodeHelper
    {
        public static string EscapeHtmlSpecialCharacters(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return string.Empty;
            }

            var builder = new StringBuilder();
            foreach (var character in input)
            {
                switch (character)
                {
                    case '<':
                        builder.Append("&lt;");
                        break;
                    case '>':
                        builder.Append("&gt;");
                        break;
                    case '&':
                        builder.Append("and");
                        break;
                    case '"':
                        builder.Append("&quot;");
                        break;
                    case '\'':
                        builder.Append("&#39;");
                        break;
                    default:
                        builder.Append(character);
                        break;
                }
            }

            return builder.ToString();
        }
    }
}
