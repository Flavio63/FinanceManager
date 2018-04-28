using FinanceManager.Models;
using System.Data;

namespace FinanceManager.Services
{
    public interface IManagerLiquidAssetServices
    {
        ManagerLiquidAssetList GetManagerLiquidAssetListByOwner(int IdOwner);
        ManagerLiquidAssetList GetManagerLiquidAssetListByOwner_MovementType(int IdOwner, int[] IdMovement);
        DataTable GetCurrencyAvailable(int IdOwner, int IdLocation, int IdCurrency);
        void AddManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void UpdateManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void DeleteManagerLiquidAsset(int id);
    }
}
