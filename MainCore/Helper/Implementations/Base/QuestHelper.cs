using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using OpenQA.Selenium;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public class QuestHelper : IQuestHelper
    {
        private readonly IGeneralHelper _generalHelper;

        private readonly IQuestParser _questParser;
        private readonly IChromeManager _chromeManager;

        public QuestHelper(IGeneralHelper generalHelper, IQuestParser questParser, IChromeManager chromeManager)
        {
            _generalHelper = generalHelper;
            _questParser = questParser;
            _chromeManager = chromeManager;
        }

        public Result ClaimRewards(int accountId, int villageId)
        {
            var result = _generalHelper.SwitchVillage(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = OpenQuestWindow(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = CollectReward(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Result OpenQuestWindow(int accountId)
        {
            var chromeManager = _chromeManager.Get(accountId);
            var html = chromeManager.GetHtml();
            var questMaster = _questParser.GetQuestMasterButton(html);

            var result = _generalHelper.Click(accountId, By.XPath(questMaster.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        private Result CollectReward(int accountId)
        {
            var chromeManager = _chromeManager.Get(accountId);
            var count = 3;
            var currentTab = 0;
            Result result;
            do
            {
                var html = chromeManager.GetHtml();
                var questMaster = _questParser.GetQuestMasterButton(html);
                if (!_questParser.IsQuestMasterClaimable(questMaster)) break;
                var collectButtons = _questParser.GetCollectButtons(html);
                if (!collectButtons.Any())
                {
                    currentTab = currentTab == 0 ? 1 : 0;
                    result = _generalHelper.SwitchTab(accountId, currentTab);
                    if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
                    count--;
                    continue;
                }

                result = _generalHelper.Click(accountId, By.XPath(collectButtons.First().XPath));
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            while (count > 0);
            return Result.Ok();
        }
    }
}