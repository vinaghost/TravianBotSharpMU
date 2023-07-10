using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public class HeroReviveHelper : IHeroReviveHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IHeroSectionParser _heroSectionParser;
        private readonly IGeneralHelper _generalHelper;
        private readonly IChromeManager _chromeManager;
        private readonly IHeroResourcesHelper _heroResourcesHelper;

        public HeroReviveHelper(IHeroSectionParser heroSectionParser, IGeneralHelper generalHelper, IChromeManager chromeManager, IHeroResourcesHelper heroResourcesHelper, IDbContextFactory<AppDbContext> contextFactory)
        {
            _heroSectionParser = heroSectionParser;
            _generalHelper = generalHelper;
            _chromeManager = chromeManager;
            _heroResourcesHelper = heroResourcesHelper;
            _contextFactory = contextFactory;
        }

        public Result Execute(int accountId)
        {
            var villageId = GetVillageRevive(accountId);
            if (villageId == -1) return Result.Fail(new Skip("No village to revive. Please check setting page"));

            var result = _generalHelper.SwitchVillage(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.ToHeroAttributes(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = CheckResource(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public int GetVillageRevive(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var accountSetting = context.AccountsSettings.Find(accountId);
            var villageId = accountSetting.HeroReviveVillageId;
            var villages = context.Villages.Where(x => x.AccountId == accountId);
            if (villages.Any(x => x.Id == villageId)) return villageId;
            return -1;
        }

        private Resources GetResourceNeed(int accountId)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();
            var resourceArr = _heroSectionParser.GetRevivedResource(html);
            return new Resources(resourceArr);
        }

        private Result CheckResource(int accountId, int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var resCurrent = context.VillagesResources.Find(villageId);
            var resNeed = GetResourceNeed(accountId);
            if (resNeed < resCurrent) return Result.Ok();

            var setting = context.AccountsSettings.Find(accountId);
            if (!setting.IsUseHeroResToRevive) return Result.Fail(NoResource.ReviveHero(resNeed));

            var result = _generalHelper.ToHeroInventory(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _heroResourcesHelper.FillResource(accountId, villageId, resNeed - resCurrent);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = _generalHelper.ToHeroAttributes(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}