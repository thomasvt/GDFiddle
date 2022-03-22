namespace GDFiddle.UI.Controls.Grids
{
    public class GridProperties
    {
        public GridProperties(int column, int row)
        {
            Column = column;
            Row = row;
        }

        public int Column { get; set; }
        public int Row { get; set; }

        public static GridProperties Default() => new(0,0);
    }
}
