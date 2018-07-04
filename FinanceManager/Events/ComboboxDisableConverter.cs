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
            TextBlock TB = parameter as TextBlock;
            if (value == null)
                return value;
            if (TB != null)
            {
                switch (TB.Text)
                {
                    case "PortfolioMovement":
                        if (value != null && value.GetType().Name == "RegistryMovementType")
                        {
                            RegistryMovementType RMT = value as RegistryMovementType;
                            if (RMT.IdMovement > 2 && RMT.IdMovement != 4)
                                return true;
                        }
                        break;
                    case "PortfolioChange":
                        if (value != null && value.GetType().Name == "RegistryMovementType")
                        {
                            RegistryMovementType RMT = value as RegistryMovementType;
                            if (RMT.IdMovement != 3)
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
                            if (RMT.IdMovement < 5 || RMT.IdMovement > 6 )
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
