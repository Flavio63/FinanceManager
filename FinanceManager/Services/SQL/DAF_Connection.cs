using FinanceManager.ViewModels;
using System.Configuration;

namespace FinanceManager.Services.SQL
{
    public class DAFconnection : IDAFconnection
    {
        private string ConnectionType = "default";
        private readonly string DefaultLocalConnection = "SERVER=127.0.0.1; port=3306; DATABASE=finanza; UID=root; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        private readonly string DefaultServerConnection = "SERVER=192.168.1.253; port=3307; DATABASE=finanza; UID=flavio; password=Fla63AuDa; pooling=false; convert zero datetime=true";
        private readonly string OutdoorServerConnection = "SERVER=famvilla.synology.me; port=3307; DATABASE=finanza; UID=flavio; password=Fla63AuDa; pooling=false; convert zero datetime=true";

        public string GetConnectionType ()
        {
            switch(ConnectionType)
            {
                case ("default"):
                    return DefaultServerConnection;
                case ("outdoor"):
                    return OutdoorServerConnection;
                default:
                    return DefaultLocalConnection;
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
