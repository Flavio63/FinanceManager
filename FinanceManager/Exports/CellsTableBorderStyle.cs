using NPOI.SS.UserModel;

namespace FinanceManager.Exports
{
    public static class CellsTableBorderStyle
    {
        /// <summary>
        /// Bordo a sinistra sottile
        /// Bordo alto e basso a puntini
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ICellStyle LeftTable(ICellStyle style)
        {
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Dotted;
            style.BorderRight = BorderStyle.Dotted;
            return style;
        }
        /// <summary>
        /// Bordo sinistra, alto e basso a puntini
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ICellStyle CenterTable(ICellStyle style)
        {
            style.BorderLeft = BorderStyle.Dotted;
            style.BorderBottom = BorderStyle.Dotted;
            style.BorderRight = BorderStyle.Dotted;
            return style;
        }
        /// <summary>
        /// Bordo a destra sottile
        /// Sopra e sotto a puntini
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ICellStyle RightTable(ICellStyle style)
        {
            style.BorderRight = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Dotted;
            style.BorderLeft = BorderStyle.Dotted;
            return style;
        }
        /// <summary>
        /// Bordo sinistra e sotto sottile
        /// destra a puntini
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ICellStyle BottomSx(ICellStyle style)
        {
            style.BorderLeft = BorderStyle.Thin;
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Dotted;
            return style;
        }
        /// <summary>
        /// Bordo inferiore sottile
        /// Sinistra e destra a puntini
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ICellStyle BottomCenter(ICellStyle style)
        {
            style.BorderBottom = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Dotted;
            style.BorderRight = BorderStyle.Dotted;
            return style;
        }
        /// <summary>
        /// Bordo inferiore e destro sottili
        /// sinistra a puntini
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ICellStyle BottomDx(ICellStyle style)
        {
            style.BorderBottom = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Dotted;
            return style;
        }
        /// <summary>
        /// Bordo superiore e sinistro sottili
        /// destro a puntini
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ICellStyle TopSx(ICellStyle style)
        {
            style.BorderTop = BorderStyle.Thin;
            style.BorderLeft = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Dotted;
            //style.FillForegroundColor = 64;
            //style.FillPattern = FillPattern.SolidForeground;
            return style;
        }
        /// <summary>
        /// Bordo superiore sottile
        /// sinistro e destro a puntini
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
        public static ICellStyle TopCenter(ICellStyle style)
        {
            style.BorderTop = BorderStyle.Thin;
            style.BorderRight = BorderStyle.Dotted;
            style.BorderLeft = BorderStyle.Dotted;
            //style.FillForegroundColor = 64;
            //style.FillPattern = FillPattern.SolidForeground;
            return style;
        }
        /// <summary>
        /// Bordo superiore e destro sottile
        /// sinistro a puntini
        /// </summary>
        /// <param name="style"></param>
        /// <returns></returns>
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
