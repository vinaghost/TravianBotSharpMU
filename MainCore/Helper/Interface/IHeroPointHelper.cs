using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface IHeroPointHelper
    {
        Result Execute(int accountId);
        bool IsLevelUp(int accountId);
    }
}