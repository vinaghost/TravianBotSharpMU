using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Interface;
using MainCore.Tasks.Base;
using Splat;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public class HeroPointTask : AccountBotTask
    {
        private readonly IHeroPointHelper _heroPointHelper;

        public HeroPointTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _heroPointHelper = Locator.Current.GetService<IHeroPointHelper>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());

            var result = _heroPointHelper.Execute(AccountId);
            if (result.IsFailed) return result.WithError(new Trace(Trace.TraceMessage()));

            return Result.Ok();
        }
    }
}