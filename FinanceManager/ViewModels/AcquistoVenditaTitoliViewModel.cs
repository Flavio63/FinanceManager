using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class AcquistoVenditaTitoliViewModel : ViewModelBase
    {
        private IRegistryServices _services;
        private IManagerLiquidAssetServices _liquidAssetServices;
        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        Predicate<object> _Filter;

        public AcquistoVenditaTitoliViewModel
            (IRegistryServices services, IManagerLiquidAssetServices liquidAssetServices, Collection<RegistryOwner> GestioniScelte,
            Collection<RegistryLocation> ContiScelti, RegistryMovementType TipoMovimentoScelto)
        {
            _services = services ?? throw new ArgumentNullException("Services in Manager Portfolio Movement View Model");
            _liquidAssetServices = liquidAssetServices ?? throw new ArgumentNullException("Liquid Asset Services in Manager Portfolio Movement View Model");
            GestioneMod = GestioniScelte;
            ContoMod = ContiScelti;
            CosaMod = TipoMovimentoScelto;
            CloseMeCommand = new CommandHandler(CloseMe);
            SetUpData();
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            ClearCommand = new CommandHandler(CleanCommand);
        }

        private void SetUpData()
        {
            try
            {
                TobinOk = "Collapsed";
                DisaggioOk = "Collapsed";
                RitenutaOk = "Collapsed";
                ContEuroVisib = "Collapsed";
                if (CosaMod.IdMovement == 6) RitenutaOk = "Visible";
                Cosa = CosaMod.DescMovement;
                Gestione = GestioneMod[0].OwnerName;
                Conto = ContoMod[0].DescLocation;
                CurrencyList = new ObservableCollection<RegistryCurrency>(_services.GetRegistryCurrencyList());
                SharesList = new ObservableCollection<RegistryShare>(_services.GetRegistryShareList());
                Disponibili = _liquidAssetServices.GetCurrencyAvailable(GestioneMod[0].IdOwner, ContoMod[0].IdLocation, 1).
                    ToString("#,##0.0#", CultureInfo.CreateSpecificCulture("it-IT")) + " EUR";
                _Filter = new Predicate<object>(Filter);
                RowLiquidAsset = new ManagerLiquidAsset();
                RowLiquidAsset.Data_Movimento = DateTime.Now;
                RowLiquidAsset.Id_tipo_movimento = CosaMod.IdMovement;
                RowLiquidAsset.Id_gestione = GestioneMod[0].IdOwner;
                RowLiquidAsset.Id_conto = ContoMod[0].IdLocation;
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(
                    _liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto));
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Set up Acquisto Vendita Titoli", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        #region events
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                RegistryCurrency RC = e.AddedItems[0] as RegistryCurrency;
                if (RC != null)
                {
                    Disponibili = _liquidAssetServices.GetCurrencyAvailable(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, RC.IdCurrency).
                       ToString("#,##0.0#", CultureInfo.CreateSpecificCulture("it-IT")) + " " + RC.CodeCurrency;
                    RowLiquidAsset.Id_valuta = RC.IdCurrency;
                    if (RC.IdCurrency == 1)
                    {
                        ContEuroVisib = "Collapsed";
                        RowLiquidAsset.Valore_di_cambio = 1;
                    }
                    else
                        ContEuroVisib = "Visible";
                    return;
                }
                RegistryShare RS = e.AddedItems[0] as RegistryShare;
                if (RS != null)
                {
                    RowLiquidAsset.Id_titolo = RS.IdShare;
                    RowLiquidAsset.Id_tipo_titolo = RS.IdShareType;
                    RowLiquidAsset.Isin = RS.Isin;
                    RowLiquidAsset.Id_azienda = RS.IdFirm;
                    SharesOwned = _liquidAssetServices.GetSharesQuantity(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, (uint)RowLiquidAsset.Id_titolo);
                }
                UpdateTotals();
            }
        }
        /// <summary>
        /// Imposto i campi sopra la griglia quando viene selezionata una riga
        /// </summary>
        /// <param name="sender">Grid dei dati</param>
        /// <param name="e">Cambio di selezione</param>
        public void GridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ManagerLiquidAsset MLA = e.AddedItems[0] as ManagerLiquidAsset;
            if (MLA != null)
            {
                if (MLA.Id_tipo_movimento != CosaMod.IdMovement) return;
                RowLiquidAsset = MLA;
                if (RowLiquidAsset.Id_valuta != 1)
                {
                    ContEuroVisib = "Visible";
                }
                UpdateTotals();
                CanUpdateDelete = true;
                CanInsert = false;
            }
        }

        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            if (e.Key == Key.Decimal)
            {
                textBox.AppendText(",");
                textBox.SelectionStart = textBox.Text.Length;
                e.Handled = true;
            }
        }


        public void LostFocus(object sender, EventArgs e)
        {
            TextBox TB = sender as TextBox;
            double converted;
            if (!string.IsNullOrEmpty(TB.Text) && double.TryParse(TB.Text, out converted))
            {
                switch (TB.Name)
                {
                    case "unityLocalAmount":
                        RowLiquidAsset.Costo_unitario_in_valuta = Convert.ToDouble(TB.Text);
                        break;
                    case "NShares":
                        if (RowLiquidAsset.Id_tipo_movimento == 5 && Convert.ToDouble(TB.Text) > 0)
                        {
                            RowLiquidAsset.N_titoli = Convert.ToDouble(TB.Text);
                        }
                        else if (RowLiquidAsset.Id_tipo_movimento == 6 && Convert.ToDouble(TB.Text) < 0)
                        {
                            RowLiquidAsset.N_titoli = Convert.ToDouble(TB.Text);
                        }
                        else
                        {
                            MessageBox.Show("Inserire un importo positivo se si compra o negativo se si vende.", "DAF-C registrazione movimenti titoli", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                        break;
                    case "CommissionValue":
                        RowLiquidAsset.Commissioni_totale = Convert.ToDouble(TB.Text);
                        break;
                    case "Valore_di_cambio":
                        RowLiquidAsset.Valore_di_cambio = Convert.ToDouble(TB.Text);
                        break;
                    case "TobinTaxValue":
                        RowLiquidAsset.TobinTax = Convert.ToDouble(TB.Text);
                        break;
                    case "RitenutaFiscale":
                        RowLiquidAsset.RitenutaFiscale = Convert.ToDouble(TB.Text);
                        break;
                    case "DisaggioValue":
                        RowLiquidAsset.Disaggio_anticipo_cedole = Convert.ToDouble(TB.Text);
                        break;
                    case "Note":
                        RowLiquidAsset.Note = TB.Text;
                        break;
                }
            }
            UpdateTotals();
        }

        private void UpdateTotals()
        {
            // verifico che ci sia un importo unitario e un numero di azioni
            if (RowLiquidAsset.Costo_unitario_in_valuta >= 0 && RowLiquidAsset.N_titoli != 0)
            {
                if (RowLiquidAsset.Id_portafoglio == 0)
                    CanInsert = true;
                // Totale 1 VALORE TRANSAZIONE (conteggio diverso fra obbligazioni e tutto il resto)
                RowLiquidAsset.Importo_totale = RowLiquidAsset.Id_tipo_titolo != 2 ? -1 * (RowLiquidAsset.Costo_unitario_in_valuta * RowLiquidAsset.N_titoli) :
                    -1 * (RowLiquidAsset.Costo_unitario_in_valuta * RowLiquidAsset.N_titoli) / 100;

                if (RowLiquidAsset.Id_tipo_movimento == 5) //acquisto
                {
                    // totale 2 valore transazione più commissioni (negativo in caso di acquisto)
                    TotalLocalValue = RowLiquidAsset.Importo_totale + RowLiquidAsset.Commissioni_totale * -1;
                    // totale contabile in valuta
                    TotaleContabile = RowLiquidAsset.Id_valuta == 1 ?
                        TotalLocalValue + (RowLiquidAsset.TobinTax + RowLiquidAsset.Disaggio_anticipo_cedole + RowLiquidAsset.RitenutaFiscale) * -1 :
                        TotalLocalValue + (RowLiquidAsset.Disaggio_anticipo_cedole + (RowLiquidAsset.RitenutaFiscale * RowLiquidAsset.Valore_di_cambio)) * -1;
                    if ((RowLiquidAsset.Id_tipo_titolo == 1 || RowLiquidAsset.Id_tipo_titolo == 4) && RowLiquidAsset.Id_valuta == 1)
                    {
                        TobinOk = "Visible";
                    }
                    else
                    {
                        TobinOk = "Collapsed";
                    }
                    if (RowLiquidAsset.Id_tipo_titolo == 2)
                    {
                        DisaggioOk = "Visible";
                    }
                    else
                    {
                        DisaggioOk = "Collapsed";
                    }
                }
                else if (RowLiquidAsset.Id_tipo_movimento == 6) //vendita
                {
                    // totale 2 valore transazione più commissioni (positivo in caso di vendita)
                    TotalLocalValue = RowLiquidAsset.Importo_totale - RowLiquidAsset.Commissioni_totale;
                    // totale contabile in valuta
                    TotaleContabile = RowLiquidAsset.Id_valuta == 1 ?
                        TotalLocalValue - (RowLiquidAsset.TobinTax + RowLiquidAsset.Disaggio_anticipo_cedole + RowLiquidAsset.RitenutaFiscale) :
                        TotalLocalValue - (RowLiquidAsset.Disaggio_anticipo_cedole + (RowLiquidAsset.RitenutaFiscale * RowLiquidAsset.Valore_di_cambio));
                    RitenutaOk = "Visible";
                }
            }
            AmountChangedValue = TotaleContabile;
            // totale contabile in euro calcolato solo se è inserito un valore di cambio
            if (RowLiquidAsset.Id_valuta != 1)
                AmountChangedValue = RowLiquidAsset.Valore_di_cambio == 0 ? 0 : (TotaleContabile / RowLiquidAsset.Valore_di_cambio);
        }


        public bool Filter(object obj)
        {
            if (obj != null)
            {
                if (obj.GetType() == typeof(RegistryShare))
                {
                    var data = obj as RegistryShare;
                    if (!string.IsNullOrEmpty(SrchShares))
                        return data.Isin.ToUpper().Contains(SrchShares.ToUpper());
                }
            }
            return true;
        }

        #endregion

        #region Getter&Setter
        /// <summary>
        /// E' l'indicazione della gestione
        /// </summary>
        public RegistryMovementType CosaMod
        {
            get { return GetValue(() => CosaMod); }
            set { SetValue(() => CosaMod, value); }
        }
        public string Cosa
        {
            get { return GetValue(() => Cosa); }
            set { SetValue(() => Cosa, "Hai scelto: " + value); }
        }

        public Collection<RegistryOwner> GestioneMod
        {
            get { return GetValue(() => GestioneMod); }
            set { SetValue(() => GestioneMod, value); }
        }
        public string Gestione
        {
            get { return GetValue(() => Gestione); }
            set { SetValue(() => Gestione, "per la gestione di: " + value); }
        }

        public Collection<RegistryLocation> ContoMod
        {
            get { return GetValue(() => ContoMod); }
            set { SetValue(() => ContoMod, value); }
        }
        public string Conto
        {
            get { return GetValue(() => Conto); }
            set { SetValue(() => Conto, "sul conto in: " + value); }
        }

        public string Disponibili
        {
            get { return GetValue(() => Disponibili); }
            set { SetValue(() => Disponibili, "Ci sono: " + value + " disponibili"); }
        }
        /// <summary>
        /// E' il totale comprensivo delle commissioni
        /// </summary>
        public double TotalLocalValue
        {
            get { return GetValue<double>("TotalLocalValue"); }
            set { SetValue("TotalLocalValue", value); }
        }

        public double TotaleContabile
        {
            get { return GetValue<double>(() => TotaleContabile); }
            set { SetValue<double>(() => TotaleContabile, value); }
        }

        /// <summary>
        /// Totale Contabile convertita in euro
        /// </summary>
        public double AmountChangedValue
        {
            get { return GetValue<double>(() => AmountChangedValue); }
            set { SetValue<double>(() => AmountChangedValue, value); }
        }

        public string SrchShares
        {
            get { return GetValue(() => SrchShares); }
            set
            {
                SetValue(() => SrchShares, value);
                SharesListView.Filter = _Filter;
                SharesListView.Refresh();

            }
        }

        public double SharesOwned
        {
            get { return GetValue(() => SharesOwned); }
            set { SetValue(() => SharesOwned, value); }
        }

        public ObservableCollection<ManagerLiquidAsset> LiquidAssetList
        {
            get { return GetValue(() => LiquidAssetList); }
            set { SetValue(() => LiquidAssetList, value); }
        }
        public ManagerLiquidAsset RowLiquidAsset
        {
            get { return GetValue(() => RowLiquidAsset); }
            set { SetValue(() => RowLiquidAsset, value); }
        }

        public ObservableCollection<RegistryCurrency> CurrencyList
        {
            get { return GetValue(() => CurrencyList); }
            set { SetValue(() => CurrencyList, value); }
        }

        public ObservableCollection<RegistryShare> SharesList
        {
            get { return GetValue(() => SharesList); }
            set
            {
                SetValue(() => SharesList, value);
                SharesListView = new ListCollectionView(value);
            }
        }

        public ListCollectionView SharesListView
        {
            get { return GetValue(() => SharesListView); }
            set { SetValue(() => SharesListView, value); }
        }
        /// <summary>
        /// Gestisce la visualizzazione del campo TobinTax
        /// </summary>
        public string TobinOk
        {
            get { return GetValue(() => TobinOk); }
            set { SetValue(() => TobinOk, value); }
        }
        /// <summary>
        /// Gestisce la visualizzazione del campo Disaggio
        /// </summary>
        public string DisaggioOk
        {
            get { return GetValue(() => DisaggioOk); }
            set { SetValue(() => DisaggioOk, value); }
        }

        public string RitenutaOk
        {
            get { return GetValue(() => RitenutaOk); }
            set { SetValue(() => RitenutaOk, value); }
        }
        public string ContEuroVisib
        {
            get { return GetValue(() => ContEuroVisib); }
            set { SetValue(() => ContEuroVisib, value); }
        }

        #endregion

        #region command
        public void SaveCommand(object param)
        {
            try
            {
                ManagerLiquidAsset MLA = new ManagerLiquidAsset();
                MLA.Id_gestione = RowLiquidAsset.Id_gestione;
                MLA.Id_conto = RowLiquidAsset.Id_conto;
                MLA.Id_valuta = RowLiquidAsset.Id_valuta;
                MLA.Id_tipo_movimento = RowLiquidAsset.Id_tipo_movimento;
                MLA.Id_titolo = RowLiquidAsset.Id_titolo;
                MLA.Data_Movimento = RowLiquidAsset.Data_Movimento;
                MLA.Importo_totale = RowLiquidAsset.Importo_totale;
                MLA.N_titoli = RowLiquidAsset.N_titoli;
                MLA.Costo_unitario_in_valuta = RowLiquidAsset.Costo_unitario_in_valuta;
                MLA.Commissioni_totale = RowLiquidAsset.Commissioni_totale;
                MLA.TobinTax = RowLiquidAsset.TobinTax;
                MLA.Disaggio_anticipo_cedole = RowLiquidAsset.Disaggio_anticipo_cedole;
                MLA.RitenutaFiscale = RowLiquidAsset.RitenutaFiscale;
                MLA.Valore_di_cambio = RowLiquidAsset.Valore_di_cambio;
                MLA.Note = RowLiquidAsset.Note;

                _liquidAssetServices.AddManagerLiquidAsset(MLA);    // ho inserito il movimento in portafoglio
                MLA = _liquidAssetServices.GetLastShareMovementByOwnerAndLocation(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto); // ricarico l'ultimo record
                ContoCorrente cc = new ContoCorrente()
                {
                    Id_Conto = MLA.Id_conto,
                    Id_Quote_Investimenti = 0,
                    Id_Valuta = MLA.Id_valuta,
                    Id_Portafoglio_Titoli = MLA.Id_portafoglio,
                    Id_Tipo_Movimento = MLA.Id_tipo_movimento,
                    Id_Gestione = MLA.Id_gestione,
                    Id_Titolo = (int)MLA.Id_titolo,
                    Data_Movimento = MLA.Data_Movimento,
                    Ammontare = TotaleContabile,
                    Valore_Cambio = MLA.Valore_di_cambio,
                    Causale = MLA.Note
                };
                _liquidAssetServices.InsertAccountMovement(cc);
                // reimposto la griglia con quanto inserito
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto));
                CanInsert = false;          // disabilito la possibilità di un inserimento accidentale
                CanUpdateDelete = true;     // abilito la possibilità di modificare / cancellare il record

                RowLiquidAsset = MLA;
                SharesOwned = _liquidAssetServices.GetSharesQuantity(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto, (uint)RowLiquidAsset.Id_titolo);
                SrchShares = "";
                MessageBox.Show("Record caricato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nel caricamento del record: " + Environment.NewLine +
                    err.Message, Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void UpdateCommand(object param)
        {
            try
            {
                _liquidAssetServices.UpdateManagerLiquidAsset(RowLiquidAsset);      //registro la modifica in portafoglio
                                                                                    //registro la modifica in conto corrente

                // reimposto la griglia con quanto inserito
                LiquidAssetList = new ObservableCollection<ManagerLiquidAsset>(_liquidAssetServices.GetManagerSharesMovementByOwnerAndLocation(RowLiquidAsset.Id_gestione, RowLiquidAsset.Id_conto));
                SrchShares = "";
                MessageBox.Show("Record modificato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nel modificare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void DeleteCommand(object param)
        {
            try
            {
                _liquidAssetServices.DeleteManagerLiquidAsset(RowLiquidAsset.Id_portafoglio);
                SetUpData();
                SrchShares = "";
                MessageBox.Show("Record eliminato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nell'eliminare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void CleanCommand(object param)
        {
            SetUpData();
        }
        public void CloseMe(object param)
        {
            AcquistoVenditaTitoliView MFMV = param as AcquistoVenditaTitoliView;
            Grid wp = MFMV.Parent as Grid;
            wp.Children.Remove(MFMV);
            wp.Visibility = Visibility.Collapsed;
        }

        public bool CanInsert
        {
            get { return GetValue(() => CanInsert); }
            set { SetValue(() => CanInsert, value); }
        }

        public bool CanSave(object param)
        {
            return CanInsert;
        }

        public bool CanUpdateDelete
        {
            get { return GetValue(() => CanUpdateDelete); }
            set { SetValue(() => CanUpdateDelete, value); }
        }

        public bool CanModify(object param)
        {
            if (CanUpdateDelete)
                return true;
            return false;
        }

        #endregion
    }
}
