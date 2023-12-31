﻿using HtmlAgilityPack;
using System.Collections.Generic;

namespace MainCore.Parsers.Interface
{
    public interface IHeroSectionParser
    {
        public int GetHealth(HtmlDocument doc);

        public int GetStatus(HtmlDocument doc);

        public int GetAdventureNum(HtmlDocument doc);

        public HtmlNode GetHeroAvatar(HtmlDocument doc);

        public HtmlNode GetHeroTab(HtmlDocument doc, int index);

        public bool IsCurrentTab(HtmlNode tabNode);

        public HtmlNode GetAdventuresButton(HtmlDocument doc);

        public List<HtmlNode> GetAdventures(HtmlDocument doc);

        public int GetAdventureDifficult(HtmlNode node);

        public (int, int) GetAdventureCoordinates(HtmlNode node);

        public List<(int, int)> GetItems(HtmlDocument doc);

        public HtmlNode GetItemSlot(HtmlDocument doc, int type);

        public HtmlNode GetAmountBox(HtmlDocument doc);

        public HtmlNode GetConfirmButton(HtmlDocument doc);

        public HtmlNode GetStartAdventureButton(HtmlDocument doc, int x, int y);

        bool IsLevelUp(HtmlDocument doc);

        HtmlNode GetResourceProductionInputBox(HtmlDocument doc);

        HtmlNode GetDefBonusInputBox(HtmlDocument doc);

        HtmlNode GetOffBonusInputBox(HtmlDocument doc);

        HtmlNode GetFightingStrengthInputBox(HtmlDocument doc);

        int GetAvailablePoint(HtmlDocument doc);

        HtmlNode GetSaveButton(HtmlDocument doc);

        long[] GetRevivedResource(HtmlDocument doc);
        HtmlNode GetReviveButton(HtmlDocument doc);
        int GetHelmet(HtmlDocument doc);
        int GetShoes(HtmlDocument doc);
        int GetLeftHand(HtmlDocument doc);
        int GetHorse(HtmlDocument doc);
        int GetBody(HtmlDocument doc);
        int GetRightHand(HtmlDocument doc);
    }
}