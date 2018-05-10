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
        DataTable GetCurrencyAvailable(int IdOwner, int IdLocation, int IdCurrency);
        double GetSharesQuantity(int IdOwner, int IdLocation, uint idShare);
        void AddManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void UpdateManagerLiquidAsset(ManagerLiquidAsset managerLiquidAsset);
        void DeleteManagerLiquidAsset(int id);
    }
}
