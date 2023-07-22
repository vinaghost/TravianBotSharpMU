using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public class HeroEquipHelper : IHeroEquipHelper
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected readonly IChromeManager _chromeManager;
        protected readonly IHeroSectionParser _heroSectionParser;
        protected readonly IGeneralHelper _generalHelper;
        protected readonly ILogHelper _logHelper;

        public HeroEquipHelper(IChromeManager chromeManager, IHeroSectionParser heroSectionParser, IGeneralHelper generalHelper, IDbContextFactory<AppDbContext> contextFactory, ILogHelper logHelper)
        {
            _chromeManager = chromeManager;
            _heroSectionParser = heroSectionParser;
            _generalHelper = generalHelper;
            _contextFactory = contextFactory;
            _logHelper = logHelper;
        }

        public Result Execute(int accountId)
        {
            var result = _generalHelper.ToHeroInventory(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            result = Equip(accountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }

        private Result Equip(int accountId)
        {
            using var context = _contextFactory.CreateDbContext();
            var heroItems = context.HeroesItems.Where(x => x.AccountId == accountId).Select(x => x.Item).ToList();

            var chromeBrowser = _chromeManager.Get(accountId);
            var html = chromeBrowser.GetHtml();

            var helmet = GetBetterHelmet(heroItems.AsReadOnly(), html);
            _logHelper.Information(accountId, $"Found new helmet: {helmet}");

            if (helmet != HeroItemEnums.None)
            {
                var result = ClickItem(accountId, helmet);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            var body = GetBetterBody(heroItems.AsReadOnly(), html);
            _logHelper.Information(accountId, $"Found new armor: {body}");

            if (body != HeroItemEnums.None)
            {
                var result = ClickItem(accountId, body);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            var shoes = GetBetterShoes(heroItems.AsReadOnly(), html);
            _logHelper.Information(accountId, $"Found new boots: {shoes}");

            if (shoes != HeroItemEnums.None)
            {
                var result = ClickItem(accountId, shoes);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            var horse = GetBetterHorse(heroItems.AsReadOnly(), html);
            _logHelper.Information(accountId, $"Found new horse: {horse}");

            if (horse != HeroItemEnums.None)
            {
                var result = ClickItem(accountId, horse);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }
            return Result.Ok();
        }

        public Result ClickItem(int accountId, HeroItemEnums item)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var node = _heroSectionParser.GetItemSlot(doc, (int)item);
            if (node is null)
            {
                return Result.Fail($"Cannot find item {item}");
            }

            var result = _generalHelper.Click(accountId, By.XPath(node.XPath), waitPageLoaded: false);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            result = _generalHelper.Wait(accountId, driver =>
            {
                var html = new HtmlDocument();
                html.LoadHtml(driver.PageSource);
                var inventoryPageWrapper = html.DocumentNode.Descendants("div").FirstOrDefault(x => x.HasClass("inventoryPageWrapper"));
                return !inventoryPageWrapper.HasClass("loading");
            });
            _generalHelper.DelayClick(accountId);

            return Result.Ok();
        }

        private static HeroItemEnums GetBetterGear(HeroItemEnums currentGear, ReadOnlyCollection<HeroItemEnums> items, List<HeroItemEnums> listGear)
        {
            var inventoryGear = items.Intersect(listGear).ToList();
            if (!inventoryGear.Any()) return 0;

            var indexCurrentGear = listGear.IndexOf(currentGear);
            if (indexCurrentGear != -1)
            {
                //remove all item has lower attribute
                listGear.RemoveRange(indexCurrentGear, listGear.Count - indexCurrentGear);
            }

            //remove all item isn't in inventory
            listGear.RemoveAll(x => !inventoryGear.Contains(x));
            if (!listGear.Any()) return 0;

            return listGear.First();
        }

        private HeroItemEnums GetBetterHelmet(ReadOnlyCollection<HeroItemEnums> items, HtmlDocument doc)
        {
            var currentGear = (HeroItemEnums)_heroSectionParser.GetHelmet(doc);
            var listGear = new List<HeroItemEnums>()
            {
                HeroItemEnums.HelmetExperience3,
                HeroItemEnums.HelmetRegeneration3,
                HeroItemEnums.HelmetExperience2,
                HeroItemEnums.HelmetRegeneration2,
                HeroItemEnums.HelmetExperience1,
                HeroItemEnums.HelmetRegeneration1,
            };

            return GetBetterGear(currentGear, items, listGear);
        }

        private HeroItemEnums GetBetterBody(ReadOnlyCollection<HeroItemEnums> items, HtmlDocument doc)
        {
            var currentGear = (HeroItemEnums)_heroSectionParser.GetBody(doc);
            var listGear = new List<HeroItemEnums>()
            {
                HeroItemEnums.ArmorBreastplate3,
                HeroItemEnums.ArmorSegmented3,
                HeroItemEnums.ArmorScale3,
                HeroItemEnums.ArmorRegeneration3,
                HeroItemEnums.ArmorBreastplate2,
                HeroItemEnums.ArmorSegmented2,
                HeroItemEnums.ArmorScale2,
                HeroItemEnums.ArmorRegeneration2,
                HeroItemEnums.ArmorBreastplate1,
                HeroItemEnums.ArmorSegmented1,
                HeroItemEnums.ArmorScale1,
                HeroItemEnums.ArmorRegeneration1,
            };

            return GetBetterGear(currentGear, items, listGear);
        }

        private HeroItemEnums GetBetterShoes(ReadOnlyCollection<HeroItemEnums> items, HtmlDocument doc)
        {
            var currentGear = (HeroItemEnums)_heroSectionParser.GetShoes(doc);
            var listGear = new List<HeroItemEnums>()
            {
                HeroItemEnums.BootsSpurs3,
                HeroItemEnums.BootsMercenery3,
                HeroItemEnums.BootsRegeneration3,
                HeroItemEnums.BootsSpurs2,
                HeroItemEnums.BootsMercenery2,
                HeroItemEnums.BootsRegeneration2,
                HeroItemEnums.BootsSpurs1,
                HeroItemEnums.BootsMercenery1,
                HeroItemEnums.BootsRegeneration1,
            };

            return GetBetterGear(currentGear, items, listGear);
        }

        private HeroItemEnums GetBetterHorse(ReadOnlyCollection<HeroItemEnums> items, HtmlDocument doc)
        {
            var currentGear = (HeroItemEnums)_heroSectionParser.GetHorse(doc);
            var listGear = new List<HeroItemEnums>()
            {
                HeroItemEnums.Horse3,
                HeroItemEnums.Horse2,
                HeroItemEnums.Horse1,
            };

            return GetBetterGear(currentGear, items, listGear);
        }
    }
}