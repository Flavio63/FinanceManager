using FinanceManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinanceManager.Services
{
    public interface IContoTitoliServices
    {
        /// <summary>
        /// Aggiunge un movimento al conto Titoli
        /// </summary>
        /// <param name="RecordTitolo">Il movimento da aggiungere</param>
        void AddMovimentoTitoli(PortafoglioTitoli RecordTitolo);
        /// <summary>
        /// Ottiente la lista di tutti i titoli posseduti nel tempo
        /// (sia attivi che non )
        /// </summary>
        /// <param name="IdOwner">Chi li possiede</param>
        /// <param name="idLocation">Dove sono</param>
        /// <returns>Lista di titoli</returns>
        PortafoglioTitoliList GetListTitoliByOwnerAndLocation(int IdOwner = 0, int idLocation = 0);
        /// <summary>
        /// Estrae tutti i record legati a un titolo
        /// </summary>
        /// <param name="link_movimenti">il link di tutti i movimenti di un titolo</param>
        /// <returns>Lista di record</returns>
        PortafoglioTitoliList GetListaTitoliByLinkMovimenti(DateTime link_movimenti);
        /// <summary>
        /// Ritorna l'ultimo movimento titoli in base ai
        /// parametri inseriti
        /// </summary>
        /// <param name="IdOwner">La gestione del titolo</param>
        /// <param name="IdLocation">Il conto a cui è legato</param>
        /// <returns>Il record in questione</returns>
        PortafoglioTitoli GetLastShareMovementByOwnerAndLocation(int IdOwner, int IdLocation);
        /// <summary>
        /// Preleva il record di portafoglio titoli
        /// identifica dal suo id di riga
        /// </summary>
        /// <param name="IdPortafoglioTitoli">Identificativo di riga</param>
        /// <returns>Il record con tutti i campi</returns>
        PortafoglioTitoli GetPortafoglioTitoliById(int IdPortafoglioTitoli);
        /// <summary>
        /// Prelevo le info per i costi medi dei titoli attivi
        /// </summary>
        /// <returns>Lista di records</returns>
        PortafoglioTitoliList GetCostiMediPerTitolo();
        /// <summary>
        /// Data una gestione, un conto e il codice di un titolo
        /// restituisce quanti titoli si hanno in portafoglio
        /// </summary>
        /// <param name="IdOwner">La gestione</param>
        /// <param name="IdLocation">Il conto</param>
        /// <param name="id_titolo">Il titolo</param>
        /// <returns>ritorna il numero di titoli</returns>
        double GetSharesQuantity(int IdOwner, int IdLocation, uint id_titolo);
        /// <summary>
        /// Aggiorna i campi di un movimento titoli
        /// </summary>
        /// <param name="managerLiquidAsset">Il record da modificare</param>
        void UpdateMovimentoTitoli(PortafoglioTitoli managerLiquidAsset);
        /// <summary>
        /// Restituisce i titoli attivi in portafoglio sulla base dei parametri dati
        /// </summary>
        /// <param name="id_gestione">La gestione</param>
        /// <param name="id_conto">Il conto</param>
        /// <param name="id_titolo">Il titolo</param>
        /// <returns>Lista di titoli attivi</returns>
        Ptf_CCList GetListaTitoliAttiviByContoGestioneTitolo(int id_gestione, int id_conto, int id_titolo);
        /// <summary>
        /// Elimina un movimento nel conto titoli
        /// </summary>
        /// <param name="id">Identificativo di riga</param>
        void DeleteManagerLiquidAsset(int id);
    }
}
