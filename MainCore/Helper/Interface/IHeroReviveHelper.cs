using FluentResults;

namespace MainCore.Helper.Interface
{
    public interface IHeroReviveHelper
    {
        Result Execute(int accountId);
        int GetVillageRevive(int accountId);
    }
}