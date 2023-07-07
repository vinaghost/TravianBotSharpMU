using HtmlAgilityPack;

namespace MainCore.Parsers.Interface
{
    public interface IOptionParser
    {
        HtmlNode GetContextualHelpCheckBox(HtmlDocument doc);
        HtmlNode GetOptionsButton(HtmlDocument doc);
        HtmlNode GetSaveButton(HtmlDocument doc);
    }
}