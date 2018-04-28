using FinanceManager.Events;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Models
{
    public class ManagerLiquidAsset : ViewModelBase
    {
        private DateTime _MovementDate;

        public int idLiquidAsset { get; set; }
        public int IdOwner { get; set; }
        public string OwnerName { get; set; }

        public int IdLocation { get; set; }
        public string DescLocation { get; set; }

        public int IdCurrency { get; set; }
        public string CodeCurrency { get; set; }

        public int IdCurrency2 { get; set; }
        public string CodeCurrency2 { get; set; }

        public int IdMovement { get; set; }
        public string DescMovement { get; set; }

        public int? IdShare { get; set; }
        public string Isin { get; set; }

        public DateTime MovementDate
        {
            get
            {
                if (_MovementDate.Date.ToShortDateString() == "01/01/0001")
                    _MovementDate = DateTime.Now.Date;
                return _MovementDate;
            }
            set
            {
                if (value.Date.ToShortDateString() == "01/01/0001")
                    _MovementDate = DateTime.Now.Date;
                _MovementDate = value;
            }
        }
        //[ExcludeChar("/[a-z][A-Z]!@#$£€", ErrorMessage = "Sono permessi solo numeri")]
        [VerifyNumber(ErrorMessage = "Inserire una cifra diversa da zero e che sia disponibile.")]
        public double Amount
        {
            get { return GetValue(() => Amount); }
            set { SetValue(() => Amount, value); }
        }
        [Range(0.2, 1.8, ErrorMessage = "Controllare la cifra immessa.")]
        public double ExchangeValue
        {
            get { return GetValue(() => ExchangeValue); }
            set { SetValue(() => ExchangeValue, value); }
        }
        public double AmountChangedValue { get; set; }
        public bool Available { get; set; }
        public string Note { get; set; }
    }
}
