using FinanceManager.Models;
using System;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Data;

namespace FinanceManager.Events
{
    public class ComboboxDisableConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return value;
            if (parameter is TextBlock TB)
            {
                switch (TB.Text)
                {
                    case "GestioneQuote":
                        if (value != null && value.GetType().Name == "RegistryMovementType")
                        {
                            RegistryMovementType RMT = value as RegistryMovementType;
                            if (RMT.Id_tipo_movimento > 2 && RMT.Id_tipo_movimento != 12)
                                return true;
                        }
                        break;
                    case "PortfolioMovement":
                        if (value != null && value.GetType().Name == "RegistryMovementType")
                        {
                            RegistryMovementType RMT = value as RegistryMovementType;
                            if (RMT.Id_tipo_movimento > 2 && RMT.Id_tipo_movimento != 4)
                                return true;
                        }
                        break;
                    case "PortfolioChange":
                        if (value != null && value.GetType().Name == "RegistryMovementType")
                        {
                            RegistryMovementType RMT = value as RegistryMovementType;
                            if (RMT.Id_tipo_movimento != 3)
                                return true;
                        }
                        if (value != null && value.GetType().Name == "RegistryCurrency")
                        {
                            RegistryCurrency RC = value as RegistryCurrency;
                            if (RC.IdCurrency > 4)
                                return true;
                        }
                        break;
                    case "PortfolioShares":
                        if (value != null && value.GetType().Name == "RegistryMovementType")
                        {
                            RegistryMovementType RMT = value as RegistryMovementType;
                            if (RMT.Id_tipo_movimento < 5 || RMT.Id_tipo_movimento > 6)
                                return true;
                        }
                        break;
                }
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
