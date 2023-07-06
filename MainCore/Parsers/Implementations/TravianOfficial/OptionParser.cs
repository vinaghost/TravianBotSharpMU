using HtmlAgilityPack;
using MainCore.Parsers.Interface;
using System.Linq;

namespace MainCore.Parsers.Implementations.TravianOfficial
{
    public class OptionParser : IOptionParser
    {
        public HtmlNode GetOptionsButton(HtmlDocument doc)
        {
            var outOfGame = doc.GetElementbyId("outOfGame");
            if (outOfGame is null) return null;
            var optionsButton = outOfGame.Descendants("li").FirstOrDefault(x => x.HasClass("options"));
            return optionsButton;
        }

        public HtmlNode GetContextualHelpCheckBox(HtmlDocument doc)
        {
            var contextualHelpCheckbox = doc.GetElementbyId("hideContextualHelp");
            return contextualHelpCheckbox;
        }
    }
}