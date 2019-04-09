using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using NPOI.SS.UserModel;
using System.IO;
using FinanceManager.Models;
using Microsoft.Win32;
using System;

namespace FinanceManager.Exports
{
    public class ManagerWorkbooks
    {
        private static IWorkbook _workbook;
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
            _workbook = workbook;
            if (saveFileDialog.ShowDialog() == true)
            {
                if (param is ReportProfitLossList report1)
                {
                    ISheet sheet = workbook.CreateSheet("ProfitLoss");
                    IDataFormat format = workbook.CreateDataFormat();
                    int EndCol = SearchEndColumn(report1[0]);
                    MakeTopTableRow(report1[0], workbook, sheet, EndCol, report1.Count);
                    for (int xRow = 0; xRow < report1.Count; xRow++)
                    {
                        IRow row;
                        int iCol = 0;
                        row = sheet.CreateRow(xRow + 1);
                        // dati
                        bool total = false;
                        foreach (var prop in report1[xRow].GetType().GetProperties())
                        {
                            bool isDbl = double.TryParse(prop.GetValue(report1[xRow]).ToString(), out double dbl);
                            ICell cell = row.CreateCell(iCol);
                            SetStyle(workbook, cell, iCol, xRow + 1, EndCol, report1.Count);
                            if (isDbl && iCol > 0)
                            {
                                cell.SetCellValue(dbl);
                                cell.CellStyle.DataFormat = 8;
                                if (iCol == EndCol)
                                    MakeCellBold(workbook, cell);
                            }
                            else if (iCol == 0)
                            {
                                cell.SetCellValue(dbl);
                                cell.CellStyle.DataFormat = format.GetFormat("0");
                            }
                            else
                            {
                                if (prop.GetValue(report1[xRow]).ToString() == "Totale")
                                    total = true;
                                cell.SetCellValue(prop.GetValue(report1[xRow]).ToString());
                            }
                            iCol++;
                        }
                        if (total)
                            MakeRowBold(workbook, row);
                    }
                }
                else if (param is ReportMovementDetailedList report2)
                {
                    ISheet sheet = workbook.CreateSheet("DettaglioTitolo");
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
                    ISheet sheet = workbook.CreateSheet("TitoliAttivi");
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
                FileStream file = new FileStream(saveFileDialog.FileName, FileMode.Create);
                workbook.Write(file);
                file.Close();
            }
        }

        /// <summary>
        /// Esporta i dati di una tabella in un nuovo foglio di 
        /// un file di excel esistente
        /// </summary>
        /// <param name="param">i dati della tabella</param>
        public static void ExportDataToXlsx(object param)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "Excel files (*.xlsx)|*xlsx";
            openFileDialog.Multiselect = false;
            openFileDialog.CheckFileExists = true;
            openFileDialog.CheckPathExists = true;
            openFileDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (openFileDialog.ShowDialog() == true)
            {
                IWorkbook workbook;
                FileStream file = new FileStream(openFileDialog.FileName, FileMode.Open, FileAccess.ReadWrite);
                workbook = new XSSFWorkbook(file);
                foreach (ISheet sheet in workbook)
                {
                    foreach(IRow row in sheet)
                    {
                        foreach(ICell cell in row)
                        {

                        }
                    }
                }
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

        private static IFont Bold(IFont font)
        {
            font.IsBold = true;
            font.FontHeightInPoints = 12;
            return font;
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
