using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public sealed class StartFarmList : AccountBotTask
    {
        private readonly IRallypointHelper _rallypointHelper;
        private readonly ICheckHelper _checkHelper;

        public StartFarmList(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _rallypointHelper = Locator.Current.GetService<IRallypointHelper>();
            _checkHelper = Locator.Current.GetService<ICheckHelper>(); ;
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            var resultVillage = _checkHelper.GetCurrentVillageId(AccountId);
            if (resultVillage.IsFailed) return Result.Fail(resultVillage.Errors).WithError(new Trace(Trace.TraceMessage()));

            var result = _rallypointHelper.StartFarmList(AccountId, resultVillage.Value);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            SetNextExecute();
            return Result.Ok();
        }

        private void SetNextExecute()
        {
            using var context = _contextFactory.CreateDbContext();
            var setting = context.AccountsSettings.Find(AccountId);
            var time = Random.Shared.Next(setting.FarmIntervalMin, setting.FarmIntervalMax);
            ExecuteAt = DateTime.Now.AddSeconds(time);
        }
    }
}