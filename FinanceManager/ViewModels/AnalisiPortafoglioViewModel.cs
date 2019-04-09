using FinanceManager.Events;
using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.ViewModels
{
    public class AnalisiPortafoglioViewModel : ViewModelBase
    {
        public AnalisiPortafoglioViewModel(AnalisiPortafoglio analisiPortafoglio)
        {
            ActualRecord = analisiPortafoglio;
            SetTitle();
        }

        #region Getter&Setter
        public string Titolo
        {
            get { return GetValue(() => Titolo); }
            private set { SetValue(() => Titolo, value); }
        }

        public AnalisiPortafoglio ActualRecord
        {
            get { return GetValue(() => ActualRecord); }
            private set { SetValue(() => ActualRecord, value); }
        }
        #endregion

        #region private method
        private void SetTitle()
        {
            Titolo = "Analisi Portafoglio di " + ActualRecord.Nome;
        }
        #endregion
    }
}
