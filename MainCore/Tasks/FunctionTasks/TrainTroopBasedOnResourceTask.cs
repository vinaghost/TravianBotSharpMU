using FluentResults;
using MainCore.Enums;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Collections.Generic;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public class TrainTroopBasedOnResourceTask : VillageBotTask
    {
        private readonly List<BuildingEnums> _buildings = new();

        private readonly ITrainTroopHelper _trainTroopHelper;
        private readonly IGeneralHelper _generalHelper;

        private readonly ILogHelper _logHelper;

        public TrainTroopBasedOnResourceTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _trainTroopHelper = Locator.Current.GetService<ITrainTroopHelper>();
            _generalHelper = Locator.Current.GetService<IGeneralHelper>();
            _logHelper = Locator.Current.GetService<ILogHelper>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            var result = _generalHelper.ToDorf2(AccountId, VillageId, switchVillage: true);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}