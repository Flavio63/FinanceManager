﻿using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace FinanceManager.Views
{
    /// <summary>
    /// Logica di interazione per ReportGuadagniView.xaml
    /// </summary>
    public partial class ReportGuadagniView : UserControl
    {
        private ReportProfitLoss DataReport = new ReportProfitLoss();
        public ReportGuadagniView(ReportProfitLoss reportProfitLoss)
        {
            InitializeComponent();
            DataReport = reportProfitLoss ?? throw new ArgumentNullException("ReportProfitLoss can't be null");
            SetDefaultData();
        }

        #region DependencyProperty
        public static readonly DependencyProperty DescValutaProperty = DependencyProperty.Register("DescValuta", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Obb_CedProperty = DependencyProperty.Register("Obb_Ced", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Obb_VenProperty = DependencyProperty.Register("Obb_Ven", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Obb_TotProperty = DependencyProperty.Register("Obb_Tot", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Azi_CedProperty = DependencyProperty.Register("Azi_Ced", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Azi_VenProperty = DependencyProperty.Register("Azi_Ven", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Azi_TotProperty = DependencyProperty.Register("Azi_Tot", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Fon_CedProperty = DependencyProperty.Register("Fon_Ced", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Fon_VenProperty = DependencyProperty.Register("Fon_Ven", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Fon_TotProperty = DependencyProperty.Register("Fon_Tot", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Etf_CedProperty = DependencyProperty.Register("Etf_Ced", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Etf_VenProperty = DependencyProperty.Register("Etf_Ven", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Etf_TotProperty = DependencyProperty.Register("Etf_Tot", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Vol_CedProperty = DependencyProperty.Register("Vol_Ced", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Vol_VenProperty = DependencyProperty.Register("Vol_Ven", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Vol_TotProperty = DependencyProperty.Register("Vol_Tot", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Tot_CedProperty = DependencyProperty.Register("Tot_Ced", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Tot_VenProperty = DependencyProperty.Register("Tot_Ven", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));

        public static readonly DependencyProperty Tot_TotProperty = DependencyProperty.Register("Tot_Tot", typeof(string), typeof(ReportGuadagniView),
            new UIPropertyMetadata(string.Empty));
        #endregion

        #region GetSetValue
        public string DescValuta
        {
            get { return (string)GetValue(DescValutaProperty); }
            set { SetValue(DescValutaProperty, value); }
        }

        public string Obb_Ced
        {
            get { return (string)GetValue(Obb_CedProperty); }
            set { SetValue(Obb_CedProperty, value); }
        }

        public string Obb_Ven
        {
            get { return (string)GetValue(Obb_VenProperty); }
            set { SetValue(Obb_VenProperty, value); }
        }

        public string Obb_Tot
        {
            get { return (string)GetValue(Obb_TotProperty); }
            set { SetValue(Obb_TotProperty, value); }
        }

        public string Azi_Ced
        {
            get { return (string)GetValue(Azi_CedProperty); }
            set { SetValue(Azi_CedProperty, value); }
        }

        public string Azi_Ven
        {
            get { return (string)GetValue(Azi_VenProperty); }
            set { SetValue(Azi_VenProperty, value); }
        }

        public string Azi_Tot
        {
            get { return (string)GetValue(Azi_TotProperty); }
            set { SetValue(Azi_TotProperty, value); }
        }

        public string Fon_Ced
        {
            get { return (string)GetValue(Fon_CedProperty); }
            set { SetValue(Fon_CedProperty, value); }
        }

        public string Fon_Ven
        {
            get { return (string)GetValue(Fon_VenProperty); }
            set { SetValue(Fon_VenProperty, value); }
        }

        public string Fon_Tot
        {
            get { return (string)GetValue(Fon_TotProperty); }
            set { SetValue(Fon_TotProperty, value); }
        }

        public string Etf_Ced
        {
            get { return (string)GetValue(Etf_CedProperty); }
            set { SetValue(Etf_CedProperty, value); }
        }

        public string Etf_Ven
        {
            get { return (string)GetValue(Etf_VenProperty); }
            set { SetValue(Etf_VenProperty, value); }
        }

        public string Etf_Tot
        {
            get { return (string)GetValue(Etf_TotProperty); }
            set { SetValue(Etf_TotProperty, value); }
        }

        public string Vol_Ced
        {
            get { return (string)GetValue(Vol_CedProperty); }
            set { SetValue(Vol_CedProperty, value); }
        }

        public string Vol_Ven
        {
            get { return (string)GetValue(Vol_VenProperty); }
            set { SetValue(Vol_VenProperty, value); }
        }

        public string Vol_Tot
        {
            get { return (string)GetValue(Vol_TotProperty); }
            set { SetValue(Vol_TotProperty, value); }
        }

        public string Tot_Ced
        {
            get { return (string)GetValue(Tot_CedProperty); }
            set { SetValue(Tot_CedProperty, value); }
        }

        public string Tot_Ven
        {
            get { return (string)GetValue(Tot_VenProperty); }
            set { SetValue(Tot_VenProperty, value); }
        }

        public string Tot_Tot
        {
            get { return (string)GetValue(Tot_TotProperty); }
            set { SetValue(Tot_TotProperty, value); }
        }
        #endregion

        private void SetDefaultData()
        {
            DescValuta = DataReport.CodiceValuta;
            Obb_Ced = DataReport.Obb_Ced.ToString("0.##");
            Obb_Ven = DataReport.Obb_Ven.ToString("0.##");
            Obb_Tot = DataReport.Obb_Tot.ToString("0.##");
            Azi_Ced = DataReport.Azi_Ced.ToString("0.##");
            Azi_Ven = DataReport.Azi_Ven.ToString("0.##");
            Azi_Tot = DataReport.Azi_Tot.ToString("0.##");
            Fon_Ced = DataReport.Fon_Ced.ToString("0.##");
            Fon_Ven = DataReport.Fon_Ven.ToString("0.##");
            Fon_Tot = DataReport.Fon_Tot.ToString("0.##");
            Etf_Ced = DataReport.Etf_Ced.ToString("0.##");
            Etf_Ven = DataReport.Etf_Ven.ToString("0.##");
            Etf_Tot = DataReport.Etf_Tot.ToString("0.##");
            Vol_Ced = DataReport.Vol_Ced.ToString("0.##");
            Vol_Ven = DataReport.Vol_Ven.ToString("0.##");
            Vol_Tot = DataReport.Vol_Tot.ToString("0.##");
            Tot_Ced = DataReport.Tot_Ced.ToString("0.##");
            Tot_Ven = DataReport.Tot_Ven.ToString("0.##");
            Tot_Tot = DataReport.Tot_Tot.ToString("0.##");
        }
    }
}
