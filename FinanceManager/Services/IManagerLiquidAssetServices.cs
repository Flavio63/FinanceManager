using FinanceManager.Models;
using System.Data;

namespace FinanceManager.Services
{
    public interface IManagerLiquidAssetServices
    {
        ManagerLiquidAssetList GetManagerLiquidAssetListByOwnerAndLocation(int IdOwner, int idLocation);
        ManagerLiquidAssetList GetManagerLiquidAssetListByOwnerLocationAndMovementType(int IdOwner, int IdLocation, int[] IdMovement);
        ManagerLiquidAssetList GetManagerSharesMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        ManagerLiquidAsset GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        double GetCurrencyAvailable(int IdOwner, int IdLocation, int IdCurrency);
        ManagerLiquidAssetList GetShareMovements(int IdOwner, int IdLocation, uint IdShare);
        double GetSharesQuantity(int IdOwner, int IdLocation, uint idShare);
        double GetProfitLossByCurrency(int IdOwner, int IdLocation, int IdCurrency);
        void AddManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void UpdateManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void DeleteManagerLiquidAsset(int id);
    }
}
