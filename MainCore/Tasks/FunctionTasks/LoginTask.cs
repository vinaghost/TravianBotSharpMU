﻿using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Services.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class LoginTask : AccountBotTask
    {
        private readonly IPlanManager _planManager;
        private readonly ITaskManager _taskManager;

        private readonly ILoginHelper _loginHelper;
        private readonly IUpdateHelper _updateHelper;

        public LoginTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _planManager = Locator.Current.GetService<IPlanManager>();

            _loginHelper = Locator.Current.GetService<ILoginHelper>();
            _updateHelper = Locator.Current.GetService<IUpdateHelper>();
            _taskManager = Locator.Current.GetService<ITaskManager>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            var result = _loginHelper.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            _updateHelper.Update(AccountId);
            AddTask();

            return Result.Ok();
        }

        private void AddTask()
        {
            using var context = _contextFactory.CreateDbContext();
            var villages = context.Villages.Where(x => x.AccountId == AccountId);
            var listTask = _taskManager.GetList(AccountId);

            var upgradeBuildingList = listTask.OfType<UpgradeBuilding>();
            var updateList = listTask.OfType<RefreshVillage>();
            var trainTroopList = listTask.OfType<TrainTroopsTask>();

            foreach (var village in villages)
            {
                var queue = _planManager.GetList(village.Id);
                if (queue.Any())
                {
                    var task = upgradeBuildingList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (task is null)
                    {
                        _taskManager.Add<UpgradeBuilding>(AccountId, village.Id);
                    }
                }
                var setting = context.VillagesSettings.Find(village.Id);
                if (setting.IsAutoRefresh)
                {
                    var task = updateList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (task is null)
                    {
                        _taskManager.Add<RefreshVillage>(AccountId, village.Id);
                    }
                }

                if (setting.BarrackTroop != 0 || setting.StableTroop != 0 || setting.WorkshopTroop != 0)
                {
                    var task = trainTroopList.FirstOrDefault(x => x.VillageId == village.Id);
                    if (task is null)
                    {
                        _taskManager.Add<TrainTroopsTask>(AccountId, village.Id);
                    }
                }
            }
        }
    }
}