namespace FinanceManager.Services.SQL
{
    public class DAFconnection
    {
        private string dafConnection = "SERVER=192.168.1.253; port=3307; DATABASE=finanza_test; UID=Flavio; password=Fla63AuDa; pooling=false; convert zero datetime=true";

        public string DafConnection
        {
            get { return dafConnection; }
        }
    }
}
