namespace FinanceManager.Services.SQL
{
    public class DAFconnection
    {
        //private string dafConnection = "SERVER=192.168.1.253; port=3307; DATABASE=finanza_test; UID=flavio; password=Fla63AuDa; pooling=false; convert zero datetime=true";

        private string dafConnection = "SERVER=127.0.0.1; port=3306; DATABASE=finanza; UID=root; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        public string DafConnection
        {
            get { return dafConnection; }
        }
    }
}
