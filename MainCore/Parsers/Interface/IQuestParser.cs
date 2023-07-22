using HtmlAgilityPack;
using System.Collections.Generic;

namespace MainCore.Parsers.Interface
{
    public interface IQuestParser
    {
        List<HtmlNode> GetCollectButtons(HtmlDocument doc);
        HtmlNode GetQuestMasterButton(HtmlDocument doc);
        bool IsQuestMasterClaimable(HtmlNode node);
    }
}