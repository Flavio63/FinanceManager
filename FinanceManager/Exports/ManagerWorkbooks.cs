using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using FinanceManager.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;

namespace FinanceManager.Exports
{
    public class ManagerWorkbooks
    {
        /// <summary>
        /// Esporta i dati di una tabella in un nuovo file di excel
        /// </summary>
        /// <param name="param">i dati della tabella</param>
        public static void ExportDataInXlsx(object param)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Excel files (*.xlsx)|*.xlsx";
            saveFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            IWorkbook workbook = new XSSFWorkbook();
            if (saveFileDialog.ShowDialog() == true)
            {
                int riga = 1;           // la riga in excel parte da uno perchè in 0 c'è l'intestazione
                int colonna = 0;        // la colonna in excel
                int ultimaColonna = 0;    // l'ultima colonna dei dati
                string valoreCampo;     // il valore del campo da scrivere in excel
                ISheet sheet;           // il foglio di excel
                IRow rigaXLS;           // la riga di excel
                ICell cellaXLS;         // la cella di excel

                #region style
                ICellStyle rowStyle = workbook.CreateCellStyle();
                ICellStyle R0C0 = CellsTableBorderStyle.TopSx(workbook.CreateCellStyle());
                ICellStyle R0CX = CellsTableBorderStyle.TopCenter(workbook.CreateCellStyle());
                ICellStyle R0CF = CellsTableBorderStyle.TopDx(workbook.CreateCellStyle());
                ICellStyle RXC0 = CellsTableBorderStyle.LeftTable(workbook.CreateCellStyle());
                ICellStyle RXCX = CellsTableBorderStyle.CenterTable(workbook.CreateCellStyle());
                ICellStyle RXCF = CellsTableBorderStyle.RightTable(workbook.CreateCellStyle());
                ICellStyle RFC0 = CellsTableBorderStyle.BottomSx(workbook.CreateCellStyle());
                ICellStyle RFCX = CellsTableBorderStyle.BottomCenter(workbook.CreateCellStyle());
                ICellStyle RFCF = CellsTableBorderStyle.BottomDx(workbook.CreateCellStyle());
                ICellStyle RXCData = CellsTableBorderStyle.CenterTable(workbook.CreateCellStyle());
                ICellStyle RXCPerc = CellsTableBorderStyle.CenterTable(workbook.CreateCellStyle());
                ICellStyle RXCValuta = CellsTableBorderStyle.CenterTable(workbook.CreateCellStyle());
                ICellStyle RFCData = CellsTableBorderStyle.BottomCenter(workbook.CreateCellStyle());
                ICellStyle RFCPerc = CellsTableBorderStyle.BottomCenter(workbook.CreateCellStyle());
                ICellStyle RFCValuta = CellsTableBorderStyle.BottomCenter(workbook.CreateCellStyle());
                ICellStyle RXCFPerc = CellsTableBorderStyle.RightTable(workbook.CreateCellStyle());
                ICellStyle RFCFPerc = CellsTableBorderStyle.BottomDx(workbook.CreateCellStyle());
                IFont myBoldFont = workbook.CreateFont();
                myBoldFont.Boldweight = (short)FontBoldWeight.Bold;
                myBoldFont.FontHeightInPoints = 14;
                IFont myPlainFont = workbook.CreateFont();
                myPlainFont.Boldweight = (short)FontBoldWeight.Normal;
                myPlainFont.FontHeightInPoints = 12;
                #endregion

                if (param is ReportProfitLossList report1)
                {
                    sheet = workbook.CreateSheet("ProfitLoss");
                    ultimaColonna = SearchEndColumn(report1[0]);
                    RXCF.DataFormat = 8;
                    RFCF.DataFormat = 8;
                    RXCValuta.DataFormat = 8;
                    RFCValuta.DataFormat = 8;
                    foreach (ReportProfitLoss RPL in report1)
                    {
                        colonna = 0;
                        bool total = false;
                        foreach (var prop in RPL.GetType().GetProperties())
                        {
                            valoreCampo = prop.GetValue(RPL) == null ? "" : prop.GetValue(RPL).ToString();
                            bool isDbl = double.TryParse(valoreCampo, out double dbl);
                            if (riga - 1 == 0)  // la riga di intestazione in excel
                            {
                                rigaXLS = colonna == 0 ? sheet.CreateRow(0) : sheet.GetRow(0);
                                rowStyle.SetFont(myBoldFont);
                                rigaXLS.RowStyle = rowStyle;
                                cellaXLS = rigaXLS.CreateCell(colonna);
                                cellaXLS.SetCellValue(prop.Name);
                                if (colonna == 0) cellaXLS.CellStyle = R0C0;
                                else if (colonna > 0 && colonna < ultimaColonna) cellaXLS.CellStyle = R0CX;
                                else if (colonna == ultimaColonna) cellaXLS.CellStyle = R0CF;
                            } // fine intestazione
                            rigaXLS = colonna == 0 ? sheet.CreateRow(riga) : sheet.GetRow(riga);
                            rowStyle.SetFont(myPlainFont);
                            rigaXLS.RowStyle = rowStyle;
                            cellaXLS = rigaXLS.CreateCell(colonna);
                            if (riga < report1.Count)   // il corpo centrale del report
                            {
                                if (colonna == 0) cellaXLS.CellStyle = RXC0;
                                else if (colonna < 4) cellaXLS.CellStyle = RXCX;
                                else if (colonna >= 4 && colonna < ultimaColonna) cellaXLS.CellStyle = RXCValuta;
                                else if (colonna == ultimaColonna) cellaXLS.CellStyle = RXCF;
                            }
                            if (riga == report1.Count)  // la riga di chiusura
                            {
                                if (colonna == 0) cellaXLS.CellStyle = RFC0;
                                else if (colonna < 4) cellaXLS.CellStyle = RFCX;
                                else if (colonna >= 4 && colonna < ultimaColonna) cellaXLS.CellStyle = RFCValuta;
                                else if (colonna == ultimaColonna) cellaXLS.CellStyle = RFCF;
                            }
                            if (isDbl) cellaXLS.SetCellValue(dbl);
                            else if (!isDbl)
                            {
                                cellaXLS.SetCellValue(valoreCampo);
                                if (valoreCampo.Contains("TOTALE"))
                                    total = true;
                            }
                            colonna++;
                        }
                        if (total)
                        {
                            rigaXLS = sheet.GetRow(riga);
                            rowStyle.SetFont(myBoldFont);
                            rigaXLS.RowStyle = rowStyle;
                        }
                        //else
                        //{
                        //    rigaXLS = sheet.GetRow(riga);
                        //    foreach (ICell cell in rigaXLS)
                        //        cell.CellStyle.SetFont(myPlainFont);
                        //}
                        riga++;
                    }
                }
                else if (param is ReportMovementDetailedList report2)
                {
                    sheet = workbook.CreateSheet("DettaglioTitolo");
                    IDataFormat format = workbook.CreateDataFormat();
                    int EndCol = SearchEndColumn(report2[0]);
                    MakeTopTableRow(report2[0], workbook, sheet, EndCol, report2.Count);
                    int ExcelRow = 1;
                    foreach (ReportMovementDetailed RMD in report2)
                    {
                        IRow row;
                        int iCol = 0;
                        row = sheet.CreateRow(ExcelRow);
                        // dati
                        foreach (var prop in RMD.GetType().GetProperties())
                        {
                            string fieldValue = prop.GetValue(RMD) == null ? "" : prop.GetValue(RMD).ToString();
                            bool isDbl = double.TryParse(fieldValue, out double dbl);
                            bool isDate = DateTime.TryParse(fieldValue, out DateTime dt);
                            ICell cell = row.CreateCell(iCol);
                            SetStyle(workbook, cell, iCol, ExcelRow, EndCol, report2.Count);
                            if (isDbl)
                            {
                                cell.SetCellValue(dbl);
                                cell.CellStyle.DataFormat = 8;
                            }
                            else if (isDate)
                            {
                                cell.SetCellValue(dt);
                                cell.CellStyle.DataFormat = 14;
                            }
                            else
                            {
                                cell.SetCellValue(fieldValue);
                            }
                            iCol++;
                        }
                        ExcelRow++;
                    }
                }
                else if (param is ReportTitoliAttiviList report3)
                {
                    sheet = workbook.CreateSheet("TitoliAttivi");
                    IDataFormat format = workbook.CreateDataFormat();
                    int EndCol = SearchEndColumn(report3[0]);
                    MakeTopTableRow(report3[0], workbook, sheet, EndCol, report3.Count);
                    int ExcelRow = 1;
                    foreach (ReportTitoliAttivi RTA in report3)
                    {
                        IRow row;
                        int iCol = 0;
                        row = sheet.CreateRow(ExcelRow);
                        // dati
                        foreach (var prop in RTA.GetType().GetProperties())
                        {
                            string fieldValue = prop.GetValue(RTA) == null ? "" : prop.GetValue(RTA).ToString();
                            bool isDbl = double.TryParse(fieldValue, out double dbl);
                            ICell cell = row.CreateCell(iCol);
                            SetStyle(workbook, cell, iCol, ExcelRow, EndCol, report3.Count);
                            if (isDbl && prop.Name != "N_Titoli")
                            {
                                cell.SetCellValue(dbl);
                                cell.CellStyle.DataFormat = 8;
                            }
                            else if (isDbl && prop.Name == "N_Titoli")
                            {
                                cell.SetCellValue(dbl);
                                cell.CellStyle.DataFormat = 4;
                            }
                            else
                            {
                                cell.SetCellValue(fieldValue);
                            }
                            iCol++;
                        }
                        ExcelRow++;
                    }
                }
                else if (param is ObservableCollection<AnalisiPortafoglio> report4)
                {
                    sheet = workbook.CreateSheet("AnalisiPortafoglio");
                    IDataFormat format = workbook.CreateDataFormat();
                    int TotalRow = SearchEndColumn(report4[0]) - 6;
                    int iColText = 0;
                    int iCol = 1;
                    IRow row;
                    ICell cell;
                    foreach (AnalisiPortafoglio analisiPortafoglio in report4)
                    {
                        int iRow = 0;
                        foreach (var prop in analisiPortafoglio.GetType().GetProperties())
                        {
                            if (prop.Name != "id_titolo" && prop.Name != "desc_titolo" && prop.Name != "Isin"
                                && prop.Name != "id_tipo_titolo" && prop.Name != "id_azienda" && prop.Name != "data_modifica")
                            {
                                string fieldValue = prop.GetValue(analisiPortafoglio) == null ? "" : prop.GetValue(analisiPortafoglio).ToString();
                                bool isDbl = double.TryParse(fieldValue, out double dbl);

                                if (iCol == 1)
                                {
                                    row = sheet.CreateRow(iRow);
                                    cell = row.CreateCell(iColText);
                                    cell.SetCellValue(prop.Name);
                                    SetStyle(workbook, cell, iColText, iRow, iCol, TotalRow);
                                    if (iRow < 2) MakeCellBold(workbook, cell);
                                }
                                else
                                    row = sheet.GetRow(iRow);

                                cell = row.CreateCell(iCol);
                                SetStyle(workbook, cell, iCol, iRow, iCol, TotalRow);
                                if (isDbl)
                                {
                                    cell.SetCellValue(dbl);
                                    if (iRow < 2)
                                        cell.CellStyle.DataFormat = 8;
                                    else
                                        cell.CellStyle.DataFormat = format.GetFormat("0.00%");
                                }
                                else
                                    cell.SetCellValue(fieldValue);
                                if (iRow < 2)
                                {
                                    MakeCellBold(workbook, cell);
                                }
                                iRow++;
                            }
                        }
                        iCol++;
                    }
                }
                else if (param is GuadagnoPerQuoteList report5)
                {
                    sheet = workbook.CreateSheet("GuadagnoPerQuote");
                    #region syling
                    R0C0.SetFont(myBoldFont);
                    R0CF.SetFont(myBoldFont);
                    R0CX.SetFont(myBoldFont);
                    RXC0.SetFont(myPlainFont);
                    RXCX.SetFont(myPlainFont);
                    RXCF.SetFont(myPlainFont);
                    RFC0.SetFont(myPlainFont);
                    RFCX.SetFont(myPlainFont);
                    RFCF.SetFont(myPlainFont);
                    RXCData.SetFont(myPlainFont);
                    RXCPerc.SetFont(myPlainFont);
                    RXCValuta.SetFont(myPlainFont);
                    RFCData.SetFont(myPlainFont);
                    RFCPerc.SetFont(myPlainFont);
                    RFCValuta.SetFont(myPlainFont);
                    RXCData.DataFormat = 14;
                    RXCPerc.DataFormat = 10;
                    RXCValuta.DataFormat = 8;
                    RFCData.DataFormat = 14;
                    RFCPerc.DataFormat = 10;
                    RFCValuta.DataFormat = 8;
                    #endregion
                    int EndCol = SearchEndColumn(report5[0]);
                    int Riga = 1;
                    int Colonna;
                    IRow row;
                    foreach (GuadagnoPerQuote GPQ in report5)                   // tutti i record
                    {
                        Colonna = 0;
                        foreach (var prop in GPQ.GetType().GetProperties())
                        {
                            if (prop.Name != "IdTipoMovimento")
                            {
                                ICell cell;
                                string fieldValue = prop.GetValue(GPQ) == null ? "" : prop.GetValue(GPQ).ToString();
                                bool isDbl = double.TryParse(fieldValue, out double dbl);
                                bool isDate = DateTime.TryParse(fieldValue, out DateTime dt);
                                if (Riga - 1 == 0)
                                {
                                    row = Colonna == 0 ? sheet.CreateRow(0) : sheet.GetRow(0);
                                    cell = row.CreateCell(Colonna);
                                    cell.SetCellValue(prop.Name);
                                    if (Colonna == 0) cell.CellStyle = R0C0;
                                    else if (Colonna > 0 && Colonna < EndCol) cell.CellStyle = R0CX;
                                    else if (Colonna == EndCol) cell.CellStyle = R0CF;
                                }
                                row = Colonna == 0 ? sheet.CreateRow(Riga) : sheet.GetRow(Riga);
                                cell = row.CreateCell(Colonna);
                                if (Riga < report5.Count)
                                {
                                    if (Colonna == 0) cell.CellStyle = RXC0;
                                    else if (Colonna < 7) cell.CellStyle = RXCX;
                                    else if (Colonna == 7) cell.CellStyle = RXCData;
                                    else if (Colonna == 8) cell.CellStyle = RXCPerc;
                                    else if (Colonna > 8 && Colonna < 13) cell.CellStyle = RXCValuta;
                                    else if (Colonna == 13) cell.CellStyle = RXCF;
                                }
                                else if (Riga == report5.Count)
                                {
                                    if (Colonna == 0) cell.CellStyle = RFC0;
                                    else if (Colonna < 7) cell.CellStyle = RFCX;
                                    else if (Colonna == 7) cell.CellStyle = RFCData;
                                    else if (Colonna == 8) cell.CellStyle = RFCPerc;
                                    else if (Colonna > 8 && Colonna < 13) cell.CellStyle = RFCValuta;
                                    else if (Colonna == 13) cell.CellStyle = RFCF;
                                }
                                if (isDbl)
                                {
                                    cell.SetCellValue(dbl);
                                }
                                else if (isDate)
                                {
                                    cell.SetCellValue(dt);
                                }
                                else if (!isDbl && !isDate)
                                {
                                    cell.SetCellValue(fieldValue);
                                }
                                Colonna++;
                            }
                        }
                        Riga++;
                    }
                }
                else if (param is GuadagnoPerPeriodoList report6)
                {
                    sheet = workbook.CreateSheet("DeltaPerPeriodo");
                    int EndCol = SearchEndColumn(report6[0]);
                    int Riga = 1;
                    int Colonna;
                    IRow row;
                    #region syling
                    R0C0.SetFont(myBoldFont);
                    R0CF.SetFont(myBoldFont);
                    R0CX.SetFont(myBoldFont);
                    RXC0.SetFont(myPlainFont);
                    RXCX.SetFont(myPlainFont);
                    RXCValuta.SetFont(myPlainFont);
                    RFCValuta.SetFont(myPlainFont);
                    RXCFPerc.SetFont(myPlainFont);
                    RFCFPerc.SetFont(myPlainFont);
                    RFCFPerc.DataFormat = 10;
                    RXCFPerc.DataFormat = 10;
                    RXCValuta.DataFormat = 8;
                    RFCValuta.DataFormat = 8;
                    #endregion
                    foreach (GuadagnoPerPeriodo GPP in report6)
                    {
                        Colonna = 0;
                        foreach (var prop in GPP.GetType().GetProperties())
                        {
                            if (prop.Name != "IdGestione")
                            {
                                ICell cell;
                                string fieldValue = prop.GetValue(GPP) == null ? "" : prop.GetValue(GPP).ToString();
                                bool isDbl = double.TryParse(fieldValue, out double dbl);
                                bool isDate = DateTime.TryParse(fieldValue, out DateTime dt);
                                if (Riga - 1 == 0)
                                {
                                    row = Colonna == 0 ? sheet.CreateRow(0) : sheet.GetRow(0);
                                    cell = row.CreateCell(Colonna);
                                    cell.SetCellValue(prop.Name);
                                    if (Colonna == 0) cell.CellStyle = R0C0;
                                    else if (Colonna > 0 && Colonna < EndCol) cell.CellStyle = R0CX;
                                    else if (Colonna == EndCol) cell.CellStyle = R0CF;
                                }
                                row = Colonna == 0 ? sheet.CreateRow(Riga) : sheet.GetRow(Riga);
                                cell = row.CreateCell(Colonna);
                                if (Riga < report6.Count)
                                {
                                    if (Colonna == 0) cell.CellStyle = RXC0;
                                    else if (Colonna == 1) cell.CellStyle = RXCX;
                                    else if (Colonna < 5) cell.CellStyle = RXCValuta;
                                    else if (Colonna == 5) cell.CellStyle = RXCFPerc;
                                }
                                else if (Riga == report6.Count)
                                {
                                    if (Colonna == 0) cell.CellStyle = RFC0;
                                    else if (Colonna == 1) cell.CellStyle = RFCX;
                                    else if (Colonna < 5) cell.CellStyle = RFCValuta;
                                    else if (Colonna == 5) cell.CellStyle = RFCFPerc;
                                }
                                if (isDbl)
                                {
                                    cell.SetCellValue(dbl);
                                }
                                else if (!isDbl && !isDate)
                                {
                                    cell.SetCellValue(fieldValue);
                                }
                                Colonna++;
                            }
                        }
                        Riga++;
                    }
                }
                FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create);
                workbook.Write(file);
                file.Close();
            }
        }

        private static void SetStyle(IWorkbook workbook, ICell cell, int Column, int Row, int EndColumn, int EndRow)
        {
            ICellStyle style = workbook.CreateCellStyle();
            if (Column == 0)
            {
                if (Row == 0)
                    cell.CellStyle = CellsTableBorderStyle.TopSx(style);
                else if (Row > 0 && Row < EndRow)
                    cell.CellStyle = CellsTableBorderStyle.LeftTable(style);
                else if (Row == EndRow)
                    cell.CellStyle = CellsTableBorderStyle.BottomSx(style);
            }
            else if (Column > 0 && Column < EndColumn)
            {
                if (Row == 0)
                    cell.CellStyle = CellsTableBorderStyle.TopCenter(style);
                else if (Row > 0 && Row < EndRow)
                    cell.CellStyle = CellsTableBorderStyle.CenterTable(style);
                else if (Row == EndRow)
                    cell.CellStyle = CellsTableBorderStyle.BottomCenter(style);
            }
            else if (Column == EndColumn)
            {
                if (Row == 0)
                    cell.CellStyle = CellsTableBorderStyle.TopDx(style);
                else if (Row > 0 && Row < EndRow)
                    cell.CellStyle = CellsTableBorderStyle.RightTable(style);
                else if (Row == EndRow)
                    cell.CellStyle = CellsTableBorderStyle.BottomDx(style);
            }
        }

        private static void MakeCellBold(IWorkbook workbook, ICell cell)
        {
            ICellStyle style = workbook.CreateCellStyle();
            IFont font = workbook.CreateFont();
            font.IsBold = true;
            font.FontHeightInPoints = 12;
            cell.CellStyle = cell.CellStyle == null ? style : cell.CellStyle;
            cell.CellStyle.SetFont(font);
        }

        private static void MakeRowBold(IWorkbook workbook, IRow row)
        {
            ICellStyle style = workbook.CreateCellStyle();
            IFont font = workbook.CreateFont();
            font.IsBold = true;
            font.FontHeightInPoints = 12;

            foreach (ICell cell in row)
            {
                cell.CellStyle = cell.CellStyle == null ? style : cell.CellStyle;
                cell.CellStyle.SetFont(font);
            }
        }

        private static int SearchEndColumn(object param)
        {
            int EndCol = 0;
            foreach (var prop in param.GetType().GetProperties())
            {
                EndCol++;
            }
            EndCol--;
            return EndCol;
        }

        /// <summary>
        /// Scrive le intestazioni di colonna in excel, la riga e la
        /// colonna finale servono per gestire i grassetti e gli stili
        /// </summary>
        /// <param name="param">I dati</param>
        /// <param name="workbook">Il file</param>
        /// <param name="sheet">Il foglio</param>
        /// <param name="EndCol">La colonna finale</param>
        /// <param name="EndRow">La riga finale</param>
        private static void MakeTopTableRow(object param, IWorkbook workbook, ISheet sheet, int EndCol, int EndRow)
        {
            IRow row = sheet.CreateRow(0);
            int iCol = 0;
            foreach (var prop in param.GetType().GetProperties())
            {
                ICell cell = row.CreateCell(iCol);
                SetStyle(workbook, cell, iCol, 0, EndCol, EndRow);
                cell.SetCellValue(prop.Name);
                iCol++;
            }
            MakeRowBold(workbook, row);
        }

    }
}
