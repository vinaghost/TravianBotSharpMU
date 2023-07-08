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
            if (optionsButton is null) return null;
            var optionsHerf = optionsButton.Descendants("a").FirstOrDefault();
            return optionsHerf;
        }

        public HtmlNode GetContextualHelpCheckBox(HtmlDocument doc)
        {
            var contextualHelpCheckbox = doc.GetElementbyId("hideContextualHelp");
            return contextualHelpCheckbox;
        }

        public HtmlNode GetSaveButton(HtmlDocument doc)
        {
            var submitButtonContainer = doc.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("submitButtonContainer"));
            if (submitButtonContainer is null) return null;
            var saveButton = submitButtonContainer.Descendants("button").FirstOrDefault(x => x.HasClass("green"));
            return saveButton;
        }
    }
}