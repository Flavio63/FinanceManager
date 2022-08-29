using FinanceManager.ViewModels;
using System.Configuration;

namespace FinanceManager.Services.SQL
{
    public class DAFconnection : IDAFconnection
    {
        private string ConnectionType = "default";
        private readonly string ConnessioneTest = @"URI=file:C:\Temp\sqlite\finanza.db";
        private readonly string NuovoDbTest = @"URI=file:C:\Temp\newPiggybank\PiggyBank.db";
        private readonly string SqLiteServer = @"URI=file:H:\Investimenti\sqlite\piggybank.db";
        private readonly string outdoor = @"URI=file:C:\Users\flavi\SynologyDrive\Investimenti\sqlite\piggybank.db";

        public string GetConnectionType ()
        {
            switch(ConnectionType)
            {
                case ("ConnessioneTest"):
                    return ConnessioneTest;
                case ("outdoor"):
                    return outdoor;
                case ("NewDatabase"):
                    return NuovoDbTest;
                default:
                    return SqLiteServer;
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
