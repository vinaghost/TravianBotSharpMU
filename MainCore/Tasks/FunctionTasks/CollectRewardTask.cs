using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public class CollectRewardTask : VillageBotTask
    {
        private readonly IQuestHelper _questHelper;

        public CollectRewardTask(int villageId, int accountId, CancellationToken cancellationToken = default) : base(villageId, accountId, cancellationToken)
        {
            _questHelper = Locator.Current.GetService<IQuestHelper>();
        }

        public override Result Execute()
        {
            var result = _questHelper.ClaimRewards(AccountId, VillageId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));
            return Result.Ok();
        }
    }
}