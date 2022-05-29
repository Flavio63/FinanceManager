using FinanceManager.ViewModels;
using System.Configuration;

namespace FinanceManager.Services.SQL
{
    public class DAFconnection : IDAFconnection
    {
        private string ConnectionType = "default";
        private readonly string DefaultLocalConnection = "SERVER=127.0.0.1; port=3307; DATABASE=finanza; UID=root; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        private readonly string DefaultServerConnection = "SERVER=192.168.1.253; port=3307; DATABASE=finanza; UID=root; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        private readonly string DefaultTestConnection = "SERVER=192.168.1.253; port=3307; DATABASE=finanza_test; UID=flavio; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        private readonly string OutdoorTestConnection = "SERVER=famvilla.synology.me; port=3307; DATABASE=finanza_test; UID=flavio; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        private readonly string OutdoorServerConnection = "SERVER=famvilla.synology.me; port=3307; DATABASE=finanza; UID=flavio; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        private readonly string SqLiteServer = @"URI=file:H:\Investimenti\sqlite\piggybank.db";
        private readonly string SqLiteLocal = @"URI=file:C:\Users\flavi\SynologyDrive\Investimenti\sqlite\piggybank.db;";

        public string GetConnectionType ()
        {
            switch(ConnectionType)
            {
                case ("SqLiteServer"):
                    return SqLiteServer;
                case ("SqLiteLocal"):
                    return SqLiteLocal;
                case ("outdoor"):
                    return OutdoorServerConnection;
                case ("testServer"):
                    return DefaultTestConnection;
                case ("outdoorTestServer"):
                    return OutdoorTestConnection;
                case ("localhost"):
                    return DefaultLocalConnection;
                default:
                    return DefaultServerConnection;
            }
        }

        public void SetConnectionType(string connectionType = "default")
        {
            ConnectionType = connectionType;
        }

    }

    public interface IDAFconnection
    {
        void SetConnectionType(string connectionType);
        string GetConnectionType();
    }
}
