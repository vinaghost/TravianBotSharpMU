using FluentResults;
using HtmlAgilityPack;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Models.Runtime;
using MainCore.Parsers.Interface;
using MainCore.Services.Interface;
using Microsoft.EntityFrameworkCore;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MainCore.Helper.Implementations.Base
{
    public sealed class TrainTroopHelper : ITrainTroopHelper
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        private readonly IGeneralHelper _generalHelper;
        private readonly ILogHelper _logHelper;

        private readonly IChromeManager _chromeManager;

        private readonly ITrainTroopParser _trainTroopParser;
        private readonly INPCHelper _npcHelper;

        public TrainTroopHelper(IDbContextFactory<AppDbContext> contextFactory, IGeneralHelper generalHelper, ILogHelper logHelper, IChromeManager chromeManager, ITrainTroopParser trainTroopParser, INPCHelper npcHelper)
        {
            _contextFactory = contextFactory;
            _generalHelper = generalHelper;
            _logHelper = logHelper;
            _chromeManager = chromeManager;
            _trainTroopParser = trainTroopParser;
            _npcHelper = npcHelper;
        }

        public Result ExecuteTimer(int accountId, int villageId, BuildingEnums trainBuilding)
        {
            var result = _generalHelper.ToDorf2(accountId, villageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var buildingLoc = GetBuilding(villageId, trainBuilding);
            if (buildingLoc == -1)
            {
                DisableSetting(villageId, trainBuilding);
                _logHelper.Information(accountId, $"There is no {trainBuilding} in village");
                return Result.Ok();
            }

            result = _generalHelper.ToBuilding(accountId, villageId, buildingLoc);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var troop = GetTroopTraining(villageId, trainBuilding);

            var timeTrain = GetTroopTime(accountId, villageId, troop);
            var amountTroop = GetAmountTroop(accountId, villageId, trainBuilding, timeTrain);
            var maxTroop = GetMaxTroop(accountId, troop);
            if (maxTroop <= 0)
            {
                return Result.Fail(NoResource.Train(trainBuilding));
            }

            if (amountTroop > maxTroop)
            {
                using var context = _contextFactory.CreateDbContext();
                var setting = context.VillagesSettings.Find(villageId);
                if (setting.IsMaxTrain)
                {
                    amountTroop = maxTroop;
                }
                else
                {
                    return Result.Fail(NoResource.Train(trainBuilding));
                }
            }

            _logHelper.Information(accountId, $"Training {amountTroop} {(TroopEnums)troop}(s)");
            result = InputAmountTroop(accountId, troop, amountTroop);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }

        public Result ExecuteResource(int accountId, int villageId)
        {
            var dataList = GetData(villageId);
            var totalResource = new Resources();
            foreach (var data in dataList)
            {
                var (building, _, percent, troop, amount) = data;
                _logHelper.Information(accountId, $"Training {amount} {troop} in {building} (cost {percent:P2} current resource)");

                totalResource += troop.GetTrainCost() * amount;
            }

            totalResource.Crop += GetCurrentCrop(villageId);
            _logHelper.Information(accountId, $"NPC resource before train ({totalResource})");

            var result = _npcHelper.Execute(accountId, villageId, totalResource);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            foreach (var data in dataList)
            {
                var (building, buildingLoc, _, troop, amount) = data;
                _logHelper.Information(accountId, $"Training {amount} {troop}(s)");
                result = _generalHelper.ToDorf2(accountId, villageId);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                result = _generalHelper.ToBuilding(accountId, villageId, buildingLoc);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

                result = InputAmountTroop(accountId, (int)troop, amount);
                if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            }

            return Result.Ok();
        }

        private List<(BuildingEnums, int, double, TroopEnums, int)> GetData(int villageId)
        {
            var buildings = new List<BuildingEnums>()
            {
                BuildingEnums.Barracks,
                BuildingEnums.GreatBarracks,
                BuildingEnums.Stable,
                BuildingEnums.GreatStable,
                BuildingEnums.Workshop
            };
            var settings = GetResourceSetting(villageId);

            var data = new List<(BuildingEnums, int, double, TroopEnums, int)>();

            // correct percent (building not exist)
            for (var i = 0; i < 5; i++)
            {
                var building = buildings[i];
                var setting = settings[i];
                if (setting == 0)
                {
                    continue;
                }

                var locBuilding = GetBuilding(villageId, building);
                if (locBuilding == -1)
                {
                    settings[i] = 0;
                    continue;
                }
            }
            var resource = GetSumResource(villageId);

            for (var i = 0; i < 5; i++)
            {
                var building = buildings[i];
                var setting = settings[i];
                if (setting == 0)
                {
                    //data.Add((building, -1, 0, TroopEnums.None));
                    continue;
                }

                var sum = settings.Sum();
                var percent = Math.Round((double)setting / sum);
                var locBuilding = GetBuilding(villageId, building);
                var troop = (TroopEnums)GetTroopTraining(villageId, building);

                var resourceAllow = (long)(resource * percent);
                var resourceTroop = troop.GetTrainCost();
                var amount = resourceAllow / resourceTroop.Sum();

                data.Add((building, locBuilding, percent, troop, (int)amount));
            }

            return data;
        }

        private long GetSumResource(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var resource = context.VillagesResources.Find(villageId);

            var sum = resource.Wood + resource.Clay + resource.Iron;
            return sum;
        }

        private long GetCurrentCrop(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var resource = context.VillagesResources.Find(villageId);
            return resource.Crop;
        }

        private int[] GetResourceSetting(int villageId)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == villageId);
            return new int[5] { setting.PercentResForBarrack, setting.PercentResForGreatBarrack, setting.PercentResForStable, setting.PercentResForGreatStable, setting.PercentResForWorkshop };
        }

        public int GetBuilding(int villageId, BuildingEnums trainBuilding)
        {
            using var context = _contextFactory.CreateDbContext();
            var building = context.VillagesBuildings.Where(x => x.VillageId == villageId)
                                                    .FirstOrDefault(x => x.Type == trainBuilding && x.Level > 0);
            if (building is null) return -1;
            return building.Id;
        }

        public void DisableSetting(int villageId, BuildingEnums trainBuilding)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == villageId);
            switch (trainBuilding)
            {
                case BuildingEnums.Barracks:
                    setting.IsBarrack = false;
                    break;

                case BuildingEnums.GreatBarracks:
                    setting.IsGreatBarrack = false;
                    break;

                case BuildingEnums.Stable:
                    setting.IsStable = false;
                    break;

                case BuildingEnums.GreatStable:
                    setting.IsGreatStable = false;
                    break;

                case BuildingEnums.Workshop:
                    setting.IsWorkshop = false;
                    break;

                default:
                    break;
            }
            context.Update(setting);
            context.SaveChanges();
        }

        public int GetTroopTraining(int villageId, BuildingEnums trainBuilding)
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == villageId);
            return trainBuilding switch
            {
                BuildingEnums.Barracks or BuildingEnums.GreatBarracks => setting.BarrackTroop,
                BuildingEnums.Stable or BuildingEnums.GreatStable => setting.StableTroop,
                BuildingEnums.Workshop => setting.WorkshopTroop,
                _ => 0,
            };
        }

        public TimeSpan GetTroopTime(int accountId, int villageId, int troop)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var nodes = _trainTroopParser.GetTroopNodes(doc);
            HtmlNode troopNode = null;
            foreach (var node in nodes)
            {
                if (_trainTroopParser.GetTroopType(node) == troop)
                {
                    troopNode = node;
                    break;
                }
            }

            return _trainTroopParser.GetTrainTime(troopNode);
        }

        public int GetAmountTroop(int accountId, int villageId, BuildingEnums trainBuilding, TimeSpan trainTime)
        {
            //var chromeBrowser = _chromeManager.Get(accountId);
            //var doc = chromeBrowser.GetHtml();
            //var timeRemaining = _trainTroopParser.GetQueueTrainTime(doc);

            using var context = _contextFactory.CreateDbContext();
            var setting = context.VillagesSettings.FirstOrDefault(x => x.VillageId == villageId);
            var timeTrainMinutes = 0;
            switch (trainBuilding)
            {
                case BuildingEnums.Barracks:
                    timeTrainMinutes = Random.Shared.Next(setting.BarrackTroopTimeMin, setting.BarrackTroopTimeMax);
                    break;

                case BuildingEnums.GreatBarracks:
                    timeTrainMinutes = Random.Shared.Next(setting.GreatBarrackTroopTimeMin, setting.GreatBarrackTroopTimeMax);
                    break;

                case BuildingEnums.Stable:
                    timeTrainMinutes = Random.Shared.Next(setting.StableTroopTimeMin, setting.StableTroopTimeMax);
                    break;

                case BuildingEnums.GreatStable:
                    timeTrainMinutes = Random.Shared.Next(setting.GreatStableTroopTimeMin, setting.GreatStableTroopTimeMax);
                    break;

                case BuildingEnums.Workshop:
                    timeTrainMinutes = Random.Shared.Next(setting.WorkshopTroopTimeMin, setting.WorkshopTroopTimeMax);
                    break;

                default:
                    break;
            }

            var timeTrain = TimeSpan.FromMinutes(timeTrainMinutes);

            //if (timeRemaining > timeTrain) return 0;

            //var timeLeft = timeTrain - timeRemaining;

            // quite confused but this comment will explain
            // we caclulate how many troop we will train
            // timeTrain aka time we want added into train queue
            // trainTime aka time to train 1 troop

            var result = (int)(timeTrain.TotalMilliseconds / trainTime.TotalMilliseconds);
            return result > 0 ? result + 1 : result;
        }

        public int GetMaxTroop(int accountId, int troop)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var nodes = _trainTroopParser.GetTroopNodes(doc);
            HtmlNode troopNode = null;
            foreach (var node in nodes)
            {
                if (_trainTroopParser.GetTroopType(node) == troop)
                {
                    troopNode = node;
                    break;
                }
            }

            return _trainTroopParser.GetMaxAmount(troopNode);
        }

        public Result InputAmountTroop(int accountId, int troop, int amountTroop)
        {
            var chromeBrowser = _chromeManager.Get(accountId);
            var doc = chromeBrowser.GetHtml();
            var nodes = _trainTroopParser.GetTroopNodes(doc);
            HtmlNode troopNode = null;
            foreach (var node in nodes)
            {
                if (_trainTroopParser.GetTroopType(node) == troop)
                {
                    troopNode = node;
                    break;
                }
            }

            var input = _trainTroopParser.GetInputBox(troopNode);
            var result = _generalHelper.Input(accountId, By.XPath(input.XPath), $"{amountTroop}");
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            var button = _trainTroopParser.GetTrainButton(doc);
            result = _generalHelper.Click(accountId, By.XPath(button.XPath));
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}