using HtmlAgilityPack;
using System.Linq;

namespace Gyldendal.Jini.Services.Common.Utils
{
    public static class HtmlPackUtil
    {
        public static string RemoveHtmlExtras(string html)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(html);

            RemoveComments(doc.DocumentNode);
            RemoveStyleAttributes(doc);

            return doc.DocumentNode.OuterHtml;
        }

        /// <summary>
        /// Remove tags of style and script and also inline styles
        /// </summary>
        /// <param name="html"></param>
        private static void RemoveStyleAttributes(HtmlDocument html)
        {
            var elementsWithStyleAttribute = html.DocumentNode.SelectNodes("//@style");

            if (elementsWithStyleAttribute != null)
            {
                foreach (var element in elementsWithStyleAttribute)
                {
                    element.Attributes["style"].Remove();
                }
            }

            html.DocumentNode.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style")
                .ToList()
                .ForEach(n => n.Remove());
        }

        /// <summary>
        /// Remove comments from HTML
        /// </summary>
        /// <param name="node"></param>
        private static void RemoveComments(HtmlNode node)
        {
            if (!node.HasChildNodes)
            {
                return;
            }

            for (var i = 0; i < node.ChildNodes.Count; i++)
            {
                if (node.ChildNodes[i].NodeType == HtmlNodeType.Comment)
                {
                    node.ChildNodes.RemoveAt(i);
                    --i;
                }
            }

            foreach (HtmlNode subNode in node.ChildNodes)
            {
                RemoveComments(subNode);
            }
        }
    }
}