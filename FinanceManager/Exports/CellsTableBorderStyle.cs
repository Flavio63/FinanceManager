using NPOI.SS.UserModel;

namespace FinanceManager.Exports
{
    public static class CellsTableBorderStyle
    {
        public static ICellStyle LeftTable(ICellStyle style)
        {
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Dotted;
            style.BorderRight = BorderStyle.Dotted;
            return style;
        }

        public static ICellStyle CenterTable(ICellStyle style)
        {
            style.BorderLeft = BorderStyle.Dotted;
            style.BorderBottom = BorderStyle.Dotted;
            style.BorderRight = BorderStyle.Dotted;
            return style;
        }

        public static ICellStyle RightTable(ICellStyle style)
        {
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Dotted;
            style.BorderLeft = BorderStyle.Dotted;
            return style;
        }

        public static ICellStyle BottomSx(ICellStyle style)
        {
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Dotted;
            return style;

        }

        public static ICellStyle BottomCenter(ICellStyle style)
        {
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Dotted;
            style.BorderRight = BorderStyle.Dotted;
            return style;
        }

        public static ICellStyle BottomDx(ICellStyle style)
        {
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Dotted;
            return style;
        }

        public static ICellStyle TopSx(ICellStyle style)
        {
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Dotted;
            //style.FillForegroundColor = 64;
            //style.FillPattern = FillPattern.SolidForeground;
            return style;
        }

        public static ICellStyle TopCenter(ICellStyle style)
        {
            style.BorderTop = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Dotted;
            style.BorderLeft = BorderStyle.Dotted;
            //style.FillForegroundColor = 64;
            //style.FillPattern = FillPattern.SolidForeground;
            return style;
        }

        public static ICellStyle TopDx(ICellStyle style)
        {
            style.BorderTop = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Dotted;
            //style.FillForegroundColor = 64;
            //style.FillPattern = FillPattern.SolidForeground;
            return style;
        }

    }
}
