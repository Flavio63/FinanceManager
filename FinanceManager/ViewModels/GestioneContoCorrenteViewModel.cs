using FinanceManager.Events;
using FinanceManager.Models;
using FinanceManager.Models.Enumeratori;
using FinanceManager.Services;
using FinanceManager.Views;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace FinanceManager.ViewModels
{
    public class GestioneContoCorrenteViewModel : ViewModelBase
    {
        IRegistryServices _registryServices;
        IContoCorrenteServices _contoCorrenteServices;
        IQuoteGuadagniServices _quoteServices;
        public ICommand CloseMeCommand { get; set; }
        public ICommand InsertCommand { get; set; }
        public ICommand ModifyCommand { get; set; }
        public ICommand EraseCommand { get; set; }
        public ICommand ClearCommand { get; set; }
        private int[] typeOfShares = { 147 };
        Predicate<object> _Filter;

        private TabControl _TabControl = new TabControl();

        public GestioneContoCorrenteViewModel
            (IRegistryServices registryServices,
            IContoCorrenteServices contoCorrenteServices, IQuoteGuadagniServices quoteServices)
        {
            _registryServices = registryServices ?? throw new ArgumentNullException("Registry Services in Gestione Conto Corrente");
            //_liquidAssetServices = managerLiquidServices ?? throw new ArgumentNullException("Liquid Asset Services in Gestione Conto Corrente");
            _contoCorrenteServices = contoCorrenteServices ?? throw new ArgumentNullException("Conto corrente services assente");
            _quoteServices = quoteServices ?? throw new ArgumentNullException("Quote guadagni services assente");
            CloseMeCommand = new CommandHandler(CloseMe);
            InsertCommand = new CommandHandler(SaveCommand, CanSave);
            ModifyCommand = new CommandHandler(UpdateCommand, CanModify);
            EraseCommand = new CommandHandler(DeleteCommand, CanModify);
            ClearCommand = new CommandHandler(CleanCommand);
            try
            {
                SetUpData();
                Init();
            }
            catch (Exception err)
            {
                MessageBox.Show("Errore nella richiesta dei dati." + Environment.NewLine + err.Message, "DAF-C Quote Investitori");
            }
            _quoteServices = quoteServices;
        }

        private void SetUpData()
        {
            try
            {
                ListMovimenti = new RegistryMovementTypeList();
                ListGestioni = new RegistryOwnersList();
                ListConti = new RegistryLocationList();
                ListValute = new RegistryCurrencyList();
                TipoSoldis = new TipoSoldiList();
                RegistryMovementTypeList listaOriginale = new RegistryMovementTypeList();
                listaOriginale = _registryServices.GetRegistryMovementTypesList();
                var RMTL = from movimento in listaOriginale
                           where (movimento.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto) ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.Costi ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.CambioValuta ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.AcquistoTitoli ||
                           movimento.Id_tipo_movimento == (int)TipologiaMovimento.VenditaTitoli
                           select movimento;
                foreach (RegistryMovementType registry in RMTL)
                    ListMovimenti.Add(registry);
                ListValute = _registryServices.GetRegistryCurrencyList();
                RegistryOwnersList ListaInvestitoreOriginale = new RegistryOwnersList();
                ListaInvestitoreOriginale = _registryServices.GetGestioneList();
                var ROL = from gestione in ListaInvestitoreOriginale
                          where (gestione.Tipologia == "Gestore")
                          select gestione;
                _TabControl.TabStripPlacement = Dock.Left;
                foreach (RegistryOwner registryOwner in ROL)
                {
                    // per ogni gestione acquisisco is dati per la sintesi soldi
                    TabItem tabItem = new TabItem();
                    tabItem.Header = registryOwner.Nome_Gestione;
                    _TabControl.Items.Add(tabItem);
                    ListGestioni.Add(registryOwner);
                }
                ListConti = _registryServices.GetRegistryLocationList();
                TipoSoldis = _registryServices.GetTipoSoldiList();
                SharesList = new ObservableCollection<RegistryShare>(_registryServices.GetRegistryShareList());
                _Filter = new Predicate<object>(Filter);

                BirdsListView = _registryServices.GetSharesByFirms(typeOfShares);
            }
            catch (Exception err)
            {
                throw new Exception("Errore nel setup." + Environment.NewLine + err.Message);
            }
        }


        private void Init()
        {
            try
            {
                //=====================================================================================
                // popolo la griglia con la disponibilità di tutti i conti (codice 0)
                TotaleDisponibili = new ContoCorrenteList();
                TotaleDisponibili = _contoCorrenteServices.GetTotalAmountByAccount(0);
                //=====================================================================================
                RecordContoCorrente = new ContoCorrente();
                SrchShares = "";
                ListContoCorrente = _contoCorrenteServices.GetContoCorrenteList();
                CommonFieldsEnabled = true;
                OperazioneEnabled = true;
                CedoleEnabled = false;
                VolatiliEnabled = false;
                CanUpdateDelete = false;
                CanInsert = false;
                FiltroConto = "";
                FiltroGestione = "";
                FiltroTipoSoldi = "";
                FiltroTipoMovimento = "";
            }
            catch (Exception err)
            {
                throw new Exception("Errore in init." + Environment.NewLine + err.Message);
            }
        }

        #region Getter&Setter
        /// <summary>
        /// Combo box con is movimenti
        /// </summary>
        public RegistryMovementTypeList ListMovimenti
        {
            get { return GetValue(() => ListMovimenti); }
            set { SetValue(() => ListMovimenti, value); }
        }

        /// <summary>
        /// combo box con la lista delle valute
        /// </summary>
        public RegistryCurrencyList ListValute
        {
            get { return GetValue(() => ListValute); }
            set { SetValue(() => ListValute, value); }
        }

        /// <summary>
        /// combo box con la lista dei C/C
        /// </summary>
        public RegistryLocationList ListConti
        {
            get { return GetValue(() => ListConti); }
            set { SetValue(() => ListConti, value); }
        }

        /// <summary>
        /// combo box con la lista dei C/C
        /// </summary>
        public RegistryOwnersList ListGestioni
        {
            get { return GetValue(() => ListGestioni); }
            set { SetValue(() => ListGestioni, value); }
        }

        /// <summary>
        /// Elenco con tutti is movimenti del conto corrente
        /// </summary>
        public ContoCorrenteList ListContoCorrente
        {
            get { return GetValue(() => ListContoCorrente); }
            set { SetValue(() => ListContoCorrente, value); AccountCollectionView = CollectionViewSource.GetDefaultView(value); }
        }

        /// <summary>
        /// E' la lista di tutti is record Conto corrente usata per is filtri della griglia
        /// </summary>
        public System.ComponentModel.ICollectionView AccountCollectionView
        {
            get { return GetValue(() => AccountCollectionView); }
            set { SetValue(() => AccountCollectionView, value); }
        }
        /// <summary>
        /// Elenco con la somma delle disponibilità
        /// </summary>
        public ContoCorrenteList TotaleDisponibili
        {
            get { return GetValue(() => TotaleDisponibili); }
            set { SetValue(() => TotaleDisponibili, value); TotaleDisponibiliView = CollectionViewSource.GetDefaultView(value); }
        }
        public System.ComponentModel.ICollectionView TotaleDisponibiliView
        {
            get { return GetValue(() => TotaleDisponibiliView); }
            set { SetValue(() => TotaleDisponibiliView, value); }
        }

        /// <summary>
        /// Elenco con is tipo soldi
        /// </summary>
        public TipoSoldiList TipoSoldis
        {
            get { return GetValue(() => TipoSoldis); }
            set { SetValue(() => TipoSoldis, value); }
        }

        /// <summary>
        /// La ricerca degli isin dei titoli
        /// </summary>
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

        /// <summary>
        /// Elenco con is titoli disponibili
        /// </summary>
        public ObservableCollection<RegistryShare> SharesList
        {
            get { return GetValue(() => SharesList); }
            set
            {
                SetValue(() => SharesList, value);
                SharesListView = new ListCollectionView(value);
            }
        }

        /// <summary>
        /// Elenco con is titoli disponibili da verificare se serve
        /// </summary>
        public ListCollectionView SharesListView
        {
            get { return GetValue(() => SharesListView); }
            set { SetValue(() => SharesListView, value); }
        }

        /// <summary>
        /// Elenco con is titoli disponibili filtrato per la tipologia 
        /// certificati e futures
        /// </summary>
        public RegistryShareList BirdsListView
        {
            get { return GetValue(() => BirdsListView); }
            set { SetValue(() => BirdsListView, value); }
        }

        /// <summary>
        /// Singolo record del portafoglio
        /// </summary>
        public ContoCorrente RecordContoCorrente
        {
            get { return GetValue(() => RecordContoCorrente); }
            set { SetValue(() => RecordContoCorrente, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione del blocco dedicato alla registrazione delle cedole
        /// </summary>
        public bool CedoleEnabled
        {
            get { return GetValue(() => CedoleEnabled); }
            private set { SetValue(() => CedoleEnabled, value); }
        }

        ///<summary>
        /// Abilita / disabilita la possibilità di modificare is campi nei parametri comuni
        ///</summary>
        public bool CommonFieldsEnabled
        {
            get { return GetValue(() => CommonFieldsEnabled); }
            set { SetValue(() => CommonFieldsEnabled, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione a inserire is profit/loss delle azioni volatili
        /// </summary>
        public bool VolatiliEnabled
        {
            get { return GetValue(() => VolatiliEnabled); }
            private set { SetValue(() => VolatiliEnabled, value); }
        }

        /// <summary>
        /// Gestisce l'abilitazione del tipo di operazione
        /// </summary>
        public bool OperazioneEnabled
        {
            get { return GetValue(() => OperazioneEnabled); }
            set { SetValue(() => OperazioneEnabled, value); }
        }
        #endregion

        #region Filtri per DataGrid
        /// <summary>
        /// Il filtro per is titoli
        /// </summary>
        /// <param name="obj"> Il tipo di voce nell'elenco da filtrare </param>
        /// <returns> La voce filtrata </returns>
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
                else if (obj is ContoCorrente CConto)
                {
                    if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && 
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // tutti e 5 i filtri
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.NomeGestione == FiltroGestione && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi
                            && CConto.Desc_tipo_movimento == FiltroTipoMovimento && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && 
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 4 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.NomeGestione == FiltroGestione && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi
                            && CConto.Desc_tipo_movimento == FiltroTipoMovimento;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) && 
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 4 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.NomeGestione == FiltroGestione && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi
                            && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 4 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.NomeGestione == FiltroGestione && CConto.Desc_tipo_movimento == FiltroTipoMovimento
                            && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 4 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi && CConto.Desc_tipo_movimento == FiltroTipoMovimento
                            && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 4 su 5 attivi
                    {
                        return CConto.NomeGestione == FiltroGestione && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi && CConto.Desc_tipo_movimento == FiltroTipoMovimento
                            && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 3 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.NomeGestione == FiltroGestione && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 3 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.NomeGestione == FiltroGestione && CConto.Desc_tipo_movimento == FiltroTipoMovimento;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 3 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.NomeGestione == FiltroGestione && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 3 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 3 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.Desc_tipo_movimento == FiltroTipoMovimento && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 3 su 5 attivi
                    {
                        return CConto.Desc_Tipo_Soldi == FiltroTipoSoldi && CConto.Desc_tipo_movimento == FiltroTipoMovimento && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 3 su 5 attivi
                    {
                        return CConto.NomeGestione == FiltroGestione && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi && CConto.Desc_tipo_movimento == FiltroTipoMovimento;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.NomeGestione == FiltroGestione;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.Desc_tipo_movimento == FiltroTipoMovimento;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.NomeGestione == FiltroGestione && CConto.Desc_Tipo_Soldi == FiltroTipoSoldi;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.NomeGestione == FiltroGestione && CConto.Desc_tipo_movimento == FiltroTipoMovimento;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.NomeGestione == FiltroGestione && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.Desc_Tipo_Soldi == FiltroTipoSoldi && CConto.Desc_tipo_movimento == FiltroTipoMovimento;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.Desc_Tipo_Soldi == FiltroTipoSoldi && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 2 su 5 attivi
                    {
                        return CConto.Desc_tipo_movimento == FiltroTipoMovimento && CConto.Cod_Valuta == FiltroValuta;
                    }
                    else if (!string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 1 su 5 attivi
                    {
                        return CConto.Desc_Conto == FiltroConto;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && !string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 1 su 5 attivi
                    {
                        return CConto.NomeGestione == FiltroGestione;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && !string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 1 su 5 attivi
                    {
                        return CConto.Desc_Tipo_Soldi == FiltroTipoSoldi;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        !string.IsNullOrWhiteSpace(FiltroTipoMovimento) && string.IsNullOrWhiteSpace(FiltroValuta)) // 1 su 5 attivi
                    {
                        return CConto.Desc_tipo_movimento == FiltroTipoMovimento;
                    }
                    else if (string.IsNullOrWhiteSpace(FiltroConto) && string.IsNullOrWhiteSpace(FiltroGestione) && string.IsNullOrWhiteSpace(FiltroTipoSoldi) &&
                        string.IsNullOrWhiteSpace(FiltroTipoMovimento) && !string.IsNullOrWhiteSpace(FiltroValuta)) // 1 su 5 attivi
                    {
                        return CConto.Cod_Valuta == FiltroValuta;
                    }

                }

            }
            return true;
        }

        private string _FiltroConto;
        private string FiltroConto
        {
            get { return _FiltroConto; }
            set
            {
                _FiltroConto = value;
                AccountCollectionView.Filter = _Filter; AccountCollectionView.Refresh();
                TotaleDisponibiliView.Filter = _Filter; TotaleDisponibiliView.Refresh();
            }
        }
        private string _FiltroGestione;
        private string FiltroGestione
        {
            get { return _FiltroGestione; }
            set
            {
                _FiltroGestione = value;
                AccountCollectionView.Filter = _Filter; AccountCollectionView.Refresh();
                TotaleDisponibiliView.Filter = _Filter; TotaleDisponibiliView.Refresh();
            }
        }
        private string _FiltroTipoSoldi;
        private string FiltroTipoSoldi
        {
            get { return _FiltroTipoSoldi; }
            set
            {
                _FiltroTipoSoldi = value;
                AccountCollectionView.Filter = _Filter; AccountCollectionView.Refresh();
                TotaleDisponibiliView.Filter = _Filter; TotaleDisponibiliView.Refresh();
            }
        }
        private string _FiltroTipoMovimento;
        private string FiltroTipoMovimento
        {
            get { return _FiltroTipoMovimento; }
            set { _FiltroTipoMovimento = value; AccountCollectionView.Filter = _Filter; AccountCollectionView.Refresh(); }
        }
        private string _FiltroValuta;
        private string FiltroValuta
        {
            get { return _FiltroValuta; }
            set { _FiltroValuta = value; TotaleDisponibiliView.Filter = _Filter; AccountCollectionView.Refresh(); }
        }
        #endregion


        #region Events

        /// <summary>
        /// Gli eventi al cambio di selezione del record nella griglia dell'investitore
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void CbSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count > 0)
            {
                // tranne che nel cambio valuta in tutti gli altri if imposto il record 2 identico al record 1
                // è in altri punti del codice che verrà effettuata la differenziazione dei 2
                if (e.AddedItems[0] is DateTime DT)
                {
                    RecordContoCorrente.DataMovimento = DT.Date;
                }
                else if (e.AddedItems[0] is RegistryLocation RL)
                {
                    RecordContoCorrente.Id_Conto = RL.Id_Conto;
                    RecordContoCorrente.Desc_Conto = RL.Desc_Conto;
                    FiltroConto = RL.Desc_Conto;
                }
                else if (e.AddedItems[0] is RegistryOwner RO)
                {
                    RecordContoCorrente.Id_Gestione = RO.Id_gestione;
                    RecordContoCorrente.NomeGestione = RO.Nome_Gestione;
                    FiltroGestione = RO.Nome_Gestione;
                }
                else if (e.AddedItems[0] is RegistryCurrency RC)
                {
                    RecordContoCorrente.Id_Valuta = RC.IdCurrency;
                    RecordContoCorrente.Cod_Valuta = RC.CodeCurrency;
                    FiltroValuta = RC.CodeCurrency;
                }
                else if (e.AddedItems[0] is TipoSoldi TS)
                {
                    RecordContoCorrente.Id_Tipo_Soldi = TS.Id_Tipo_Soldi;
                    RecordContoCorrente.Desc_Tipo_Soldi = TS.Desc_Tipo_Soldi;
                    FiltroTipoSoldi = TS.Desc_Tipo_Soldi;
                }
                else if (e.AddedItems[0] is RegistryMovementType RMT)
                {
                    RecordContoCorrente.Id_tipo_movimento = RMT.Id_tipo_movimento;
                    RecordContoCorrente.Desc_tipo_movimento = RMT.Desc_tipo_movimento;
                    // abilito il blocco di input dati sulla base di questa scelta
                    CedoleEnabled = RMT.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ? true : false;
                    VolatiliEnabled = RMT.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ? true : false;
                    CanInsert = RMT.Id_tipo_movimento == (int)TipologiaMovimento.Costi ? true : false;
                    FiltroTipoMovimento = RMT.Desc_tipo_movimento;
                }
                else if (e.AddedItems[0] is RegistryShare RS)
                {
                    RecordContoCorrente.Id_Titolo = (int)RS.id_titolo;
                    RecordContoCorrente.Desc_Titolo = RS.desc_titolo;
                    CanInsert = RecordContoCorrente.Id_RowConto > 0 ? false : true;
                }
            }
        }

        /// <summary>
        /// Imposto is campi sopra la griglia quando viene selezionata una riga
        /// nell'elenco sottostante
        /// </summary>
        /// <param name="sender">Grid dei dati</param>
        /// <param name="e">Cambio di selezione</param>
        public void GridSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.AddedItems.Count == 0)
            {
                e.Handled = true;
            }
            else if (e.AddedItems[0] is ContoCorrente CC)
            {
                RecordContoCorrente = CC;
                if (CC.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ||
                    CC.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ||
                    CC.Id_tipo_movimento == (int)TipologiaMovimento.Costi)
                {
                    CommonFieldsEnabled = CC.Id_tipo_movimento == (int)TipologiaMovimento.Giroconto ? true : false;
                    OperazioneEnabled = false;
                    CedoleEnabled = CC.Id_tipo_movimento == (int)TipologiaMovimento.Cedola ? true : false;
                    VolatiliEnabled = CC.Id_tipo_movimento == (int)TipologiaMovimento.InsertVolatili ? true : false;
                    CanUpdateDelete = true;
                }
                else
                {
                    RecordContoCorrente = new ContoCorrente();
                    CedoleEnabled = false;
                    VolatiliEnabled = false;
                    CommonFieldsEnabled = false;
                    CanUpdateDelete = false;
                    OperazioneEnabled = false;
                }
                CanInsert = false;
                e.Handled = true;
            }
        }

        /// <summary>
        /// Dopo aver inserito l'importo verifico che sia congruo 
        /// rispetto alla selezione
        /// </summary>
        /// <param name="sender">TextBox</param>
        /// <param name="e">LostFocus</param>
        public void LostFocus(object sender, EventArgs e)
        {
            if (sender is TextBox TB)
            {
                switch (TB.Name)
                {
                    case ("Ammontare"):
                        RecordContoCorrente.Valore_Cambio = 1;
                        break;
                    case ("Causale"):
                        break;
                }
            }
        }

        /// <summary>
        /// Controlla che il punto del tastierino numerico venga trasformato in virgola
        /// </summary>
        /// <param name="sender">Tastiera</param>
        /// <param name="e">Pressione del tasto</param>
        public void PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (sender is TextBox textBox && (textBox.Name == "Ammontare" || textBox.Name == "Valore_Cambio"))
                if (e.Key == Key.Decimal || e.Key == Key.OemPeriod)
                {
                    var pos = textBox.SelectionStart;
                    textBox.Text = textBox.Text.Insert(pos, ",");
                    textBox.SelectionStart = pos + 1;
                    e.Handled = true;
                }
        }

        #endregion

        #region command
        public void SaveCommand(object param)
        {
            try
            {
                // Inserisco il codice del periodo quote_guadagno
                RecordContoCorrente.Id_Quote_Periodi = _quoteServices.GetIdPeriodoQuote(RecordContoCorrente.DataMovimento, RecordContoCorrente.Id_Tipo_Soldi);
                // registro il record
                _contoCorrenteServices.InsertAccountMovement(RecordContoCorrente);
                // Inserisco il guadagno ripartito per is soci
                _quoteServices.AddSingoloGuadagno(RecordContoCorrente);

                MessageBox.Show(string.Format("Ho effettuato l'operazione {0} correttamente.", RecordContoCorrente.Desc_tipo_movimento),
                    Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                Init();
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
                // prelevo il codice periodi
                RecordContoCorrente.Id_Quote_Periodi = _quoteServices.GetIdPeriodoQuote(RecordContoCorrente.DataMovimento, RecordContoCorrente.Id_Tipo_Soldi);
                //registro la modifica in conto corrente
                _contoCorrenteServices.UpdateRecordContoCorrente(RecordContoCorrente, TipologiaIDContoCorrente.IdContoCorrente);
                // modifico di conseguenza is record del guadagno totale anno
                _quoteServices.ModifySingoloGuadagno(RecordContoCorrente);
                MessageBox.Show("Record modificato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                Init();
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
                // con il codice del record elimino anche le righe inserite nella tabella guadagno
                _quoteServices.DeleteRecordGuadagno_Totale_anno(RecordContoCorrente.Id_RowConto);
                // registro l'eliminazione in conto corrente
                _contoCorrenteServices.DeleteRecordContoCorrente(RecordContoCorrente.Id_RowConto);
                MessageBox.Show("Record eliminato!", Application.Current.FindResource("DAF_Caption").ToString(), MessageBoxButton.OK, MessageBoxImage.Information);
                Init();
            }
            catch (Exception err)
            {
                MessageBox.Show("Problemi nell'eliminare il record" + Environment.NewLine + err.Message, Application.Current.FindResource("DAF_Caption").ToString(),
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        public void CleanCommand(object param)
        {
            Init();
        }

        public void CloseMe(object param)
        {
            GestioneContoCorrenteView MFMV = param as GestioneContoCorrenteView;
            DockPanel wp = MFMV.Parent as DockPanel;
            wp.Children.Remove(MFMV);
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
