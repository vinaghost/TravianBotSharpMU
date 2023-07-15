using HtmlAgilityPack;
using MainCore.Parsers.Interface;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Parsers.Implementations.TravianOfficial
{
    public class QuestParser : IQuestParser
    {
        public HtmlNode GetQuestMasterButton(HtmlDocument doc)
        {
            return doc.GetElementbyId("questmasterButton");
        }

        public bool IsQuestMasterClaimable(HtmlNode node)
        {
            return node.HasClass("claimable");
        }

        public List<HtmlNode> GetCollectButtons(HtmlDocument doc)
        {
            return doc.DocumentNode.Descendants("button").Where(x => x.HasClass("collect") && !x.HasClass("disabled")).ToList();
        }
    }
}