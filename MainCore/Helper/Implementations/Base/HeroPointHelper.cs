using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;

namespace MainCore.Helper.Implementations.Base
{
    public class HeroPointHelper : IHeroPointHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IHeroSectionParser _heroSectionParser;
        private readonly IChromeManager _chromeManager;

        private readonly IGeneralHelper _generalHelper;

        public HeroPointHelper(IHeroSectionParser heroSectionParser, IDbContextFactory<AppDbContext> contextFactory, IChromeManager chromeManager, IGeneralHelper generalHelper)
        {
            _heroSectionParser = heroSectionParser;
            _contextFactory = contextFactory;
            _chromeManager = chromeManager;
            _generalHelper = generalHelper;
        }

        public Result Execute(int accountId)
        {
            var result = _generalHelper.ToHeroAttributes(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = Handle(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public bool IsLevelUp(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var isLevelUp = _heroSectionParser.IsLevelUp(html);
            return isLevelUp;
        }

        private Result Handle(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var pointAvailable = _heroSectionParser.GetAvailablePoint(html);
            if (pointAvailable == 0)
            {
                return Result.Fail(new Skip("No point available"));
            }

            var fightingInput = _heroSectionParser.GetFightingStrengthInputBox(html);
            if (fightingInput is null)
            {
                return Result.Fail(new Retry("Cannot find fighting strength input box"));
            }
            var offInput = _heroSectionParser.GetOffBonusInputBox(html);
            if (offInput is null)
            {
                return Result.Fail(new Retry("Cannot find off bonus input box"));
            }
            var defInput = _heroSectionParser.GetDefBonusInputBox(html);
            if (defInput is null)
            {
                return Result.Fail(new Retry("Cannot find def bonus input box"));
            }
            var resourceInput = _heroSectionParser.GetResourceProductionInputBox(html);
            if (resourceInput is null)
            {
                return Result.Fail(new Retry("Cannot find resource production input box"));
            }

            var currentFightingPoint = fightingInput.GetAttributeValue("value", 0);
            var currentOffPoint = offInput.GetAttributeValue("value", 0);
            var currentDefPoint = defInput.GetAttributeValue("value", 0);
            var currentResourcePoint = resourceInput.GetAttributeValue("value", 0);

            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(accountId);

            currentFightingPoint += setting.HeroFightingPoint;
            currentOffPoint += setting.HeroOffPoint;
            currentDefPoint += setting.HeroDefPoint;
            currentResourcePoint += setting.HeroResourcePoint;

            var result = _generalHelper.Input(accountId, By.XPath(fightingInput.XPath), $"{currentFightingPoint}");
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Input(accountId, By.XPath(offInput.XPath), $"{currentOffPoint}");
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Input(accountId, By.XPath(defInput.XPath), $"{currentDefPoint}");
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.Input(accountId, By.XPath(resourceInput.XPath), $"{currentResourcePoint}");
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var saveButton = _heroSectionParser.GetSaveButton(html);
            if (saveButton is null)
            {
                return Result.Fail(new Retry("Cannot find save button"));
            }

            result = _generalHelper.Click(accountId, By.XPath(saveButton.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}