﻿using FluentResults;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace MainCore.Helper.Implementations.TravianOfficial
{
    public class NPCHelper : Base.NPCHelper
    {
        private readonly ISystemPageParser _systemPageParser;

        public NPCHelper(IChromeManager chromeManager, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, ISystemPageParser systemPageParser) : base(chromeManager, generalHelper, contextFactory)
        {
            _systemPageParser = systemPageParser;
        }

        public override bool IsEnoughGold(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var info = context.AccountsInfo.Find(accountId);

            var goldNeed = 3;

            return info.Gold > goldNeed;
        }

        public override Result EnterNumber(int accountId, int villageId, Resources _ratio)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.Find(villageId);
            var ratio = new long[4];
            if (_ratio is null)
            {
                ratio[0] = setting.AutoNPCWood;
                ratio[1] = setting.AutoNPCClay;
                ratio[2] = setting.AutoNPCIron;
                ratio[3] = setting.AutoNPCCrop;
            }
            else
            {
                ratio[0] = _ratio.Wood;
                ratio[1] = _ratio.Clay;
                ratio[2] = _ratio.Iron;
                ratio[3] = _ratio.Crop;
            }
            var ratioSum = ratio.Sum();

            if (ratioSum == 0)
            {
                Array.ForEach(ratio, x => x = 1);
                ratioSum = 4;
            }
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var nodeSum = _systemPageParser.GetNpcSumNode(html);
            var sumCurrent = nodeSum.InnerText.ToNumeric();
            var current = new long[4];
            for (var i = 0; i < 4; i++)
            {
                current[i] = (long)(sumCurrent * ratio[i]) / ratioSum;
            }
            var sum = current.Sum();
            var diff = sumCurrent - sum;
            current[3] += diff;

            var chrome = chromeBrowser.GetChrome();
            var script = "";

            for (int i = 0; i < 4; i++)
            {
                script = $"document.getElementsByName('desired{i}')[0].value = {current[i]};";
                chrome.ExecuteScript(script);
            }

            return Result.Ok();
        }
    }
}