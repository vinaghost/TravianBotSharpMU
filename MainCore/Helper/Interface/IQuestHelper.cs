using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface IQuestHelper
    {
        Result ClaimRewards(int accountId, int villageId);
    }
}