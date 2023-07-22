using HtmlAgilityPack;
using MainCore.Parsers.Interface;
using System;
using System.Collections.Generic;

namespace MainCore.Parsers.Implementations.TTWars
{
    public class QuestParser : IQuestParser
    {
        public List<HtmlNode> GetCollectButtons(HtmlDocument doc)
        {
            throw new NotImplementedException();
        }

        public HtmlNode GetQuestMasterButton(HtmlDocument doc)
        {
            throw new NotImplementedException();
        }

        public bool IsQuestMasterClaimable(HtmlNode node)
        {
            throw new NotImplementedException();
        }
    }
}