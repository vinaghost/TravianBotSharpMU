using FluentResults;
using MainCore.Errors;
using MainCore.Helper.Implementations.Base;
using MainCore.Tasks.Base;
using Splat;
using System;
using System.Linq;
using System.Threading;

namespace MainCore.Tasks.FunctionTasks
{
    public class HeroReviveTask : AccountBotTask
    {
        private readonly HeroReviveHelper _heroReviveHelper;

        public HeroReviveTask(int accountId, CancellationToken cancellationToken = default) : base(accountId, cancellationToken)
        {
            _heroReviveHelper = Locator.Current.GetService<HeroReviveHelper>();
        }

        public override Result Execute()
        {
            if (CancellationToken.IsCancellationRequested) return Result.Fail(new Cancel());
            var result = _heroReviveHelper.Execute(AccountId);
            if (result.IsFailed)
            {
                if (result.HasError<NoResource>())
                {
                    ExecuteAt = DateTime.Now.AddMinutes(Random.Shared.Next(30, 40));
                    return Result.Fail(result.Errors.First());
                }
                return Result.Fail(result.Errors);
            }
            return Result.Ok();
        }
    }
}