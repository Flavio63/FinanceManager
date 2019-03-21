namespace FinanceManager.Services.SQL
{
    public class DAFconnection
    {
        public string DafConnection { get; } = "SERVER=192.168.1.253; port=3307; DATABASE=finanza; UID=flavio; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        //public string DafConnection { get; } = "SERVER=127.0.0.1; port=3306; DATABASE=finanza; UID=root; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        //public string DafConnection { get; } = "SERVER=192.168.1.253; port=3307; DATABASE=finanza_test; UID=flavio; password=Fla63AuDa; pooling=false; convert zero datetime=true";
    }
}
