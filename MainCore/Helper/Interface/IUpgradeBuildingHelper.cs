﻿using FluentResults;
using MainCore.Enums;
using MainCore.Models.Runtime;
using System;

namespace MainCore.Helper.Interface
{
    public interface IUpgradeBuildingHelper
    {
        Result CheckResource(int accountId, int villageId, PlanTask task, bool isNewBuilding);

        Result<PlanTask> ChooseBuilding(int accountId, int villageId);

        Result Construct(int accountId, PlanTask task);

        Result Execute(int accountId, int villageId);

        PlanTask ExtractResField(int villageId, PlanTask task);

        PlanTask GetFirstBuildingTask(int villageId);

        PlanTask GetFirstResTask(int villageId);

        PlanTask GetFirstTask(int villageId);

        DateTime GetNextExecute(DateTime completeTime);

        Resources GetResourceNeed(int accountId, BuildingEnums building, bool multiple = false);

        Result<bool> GotoCorrectTab(int accountId, int villageId, PlanTask task);

        bool IsEnoughFreeCrop(int villageId, BuildingEnums building);

        bool IsInfrastructureTaskVaild(int villageId, PlanTask planTask);

        bool IsNeedAdsUpgrade(int accountId, int villageId, PlanTask task);

        void RemoveFinishedCB(int villageId);

        Result Upgrade(int accountId, PlanTask task);

        Result UpgradeAds(int accountId, PlanTask task);

        Result UpgradeAds_AccpetAds(int accountId);

        Result UpgradeAds_ClickPlayAds(int accountId, PlanTask task);

        void UpgradeAds_CloseOtherTab(int accountId);

        Result UpgradeAds_DontShowThis(int accountId);

        Result UpgradeAds_UpgradeClicking(int accountId, PlanTask task);
    }
}