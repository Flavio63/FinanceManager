namespace FinanceManager.Models
{
    public class RegistryCurrency
    {
        public int IdCurrency { get; set; }
        public string DescCurrency { get; set; }
        public string CodeCurrency { get; set; }
        public string CurrencyLongName
        {
            get { return DescCurrency + " (" + CodeCurrency + ")"; }
        }
    }
}
