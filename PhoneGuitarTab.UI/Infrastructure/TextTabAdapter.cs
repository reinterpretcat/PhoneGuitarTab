using System;
using System.IO;

namespace PhoneGuitarTab.UI.Infrastructure
{
    /// <summary>
    ///     Adapts text file to browser view. In general, it means avoiding of incorrect line breaks
    /// </summary>
    public class TextTabAdapter
    {
        public string Adapt(string content)
        {
            if (!IsNeedToAdapt(content)) return content;

            var writer = new StringWriter();
            var lines = content.Split(new[] {Environment.NewLine}, StringSplitOptions.None);
            writer.WriteLine("<pre>");

            foreach (var line in lines)
                writer.WriteLine(line + "<span class=\"line_end\"></span>");

            writer.WriteLine("</pre>");

            return writer.ToString();
        }

        private bool IsNeedToAdapt(string content)
        {
            return !content.StartsWith("<pre>");
        }
    }
}