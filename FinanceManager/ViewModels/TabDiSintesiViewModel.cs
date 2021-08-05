using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;

using System;

namespace FinanceManager.ViewModels
{
    public class TabDiSintesiViewModel : ViewModelBase
    {
        IManagerLiquidAssetServices _liquidAssetServices;

        public TabDiSintesiViewModel(IManagerLiquidAssetServices managerLiquidServices)
        {
            _liquidAssetServices = managerLiquidServices ?? throw new ArgumentNullException("Registry Services in Gestione Conto Corrente");
            init();
        }

        private void init()
        {
            SintesiSoldiR = _liquidAssetServices.GetCurrencyAvailable(1);
            SintesiSoldiDF = _liquidAssetServices.GetCurrencyAvailable(2);
            SintesiSoldiDFV = _liquidAssetServices.GetCurrencyAvailable(7);
            SintesiSoldiInt = _liquidAssetServices.GetCurrencyAvailable(8);
        }

        #region Getter&Setter
        /// <summary>
        /// il riepilogo dei soldi per la gestione Interactive
        /// </summary>
        public SintesiSoldiList SintesiSoldiInt
        {
            get { return GetValue(() => SintesiSoldiInt); }
            private set { SetValue(() => SintesiSoldiInt, value); }
        }
        /// <summary>
        /// il riepilogo dei soldi per la gestione Dany&Fla
        /// </summary>
        public SintesiSoldiList SintesiSoldiDF
        {
            get { return GetValue(() => SintesiSoldiDF); }
            private set { SetValue(() => SintesiSoldiDF, value); }
        }

        /// <summary>
        /// il riepilogo dei soldi per la gestione Rubiu
        /// </summary>
        public SintesiSoldiList SintesiSoldiR
        {
            get { return GetValue(() => SintesiSoldiR); }
            private set { SetValue(() => SintesiSoldiR, value); }
        }

        /// <summary>
        /// il riepilogo dei soldi per la gestione Dany&Fla_Volatili
        /// </summary>
        public SintesiSoldiList SintesiSoldiDFV
        {
            get { return GetValue(() => SintesiSoldiDFV); }
            private set { SetValue(() => SintesiSoldiDFV, value); }
        }
        #endregion

    }
}
