namespace GDFiddle.UI.Controls.Grids
{
    public class GridProperties
    {
        public GridProperties()
        {
            Column = 0;
            Row = 0;
        }

        public int Column { get; set; }
        public int Row { get; set; }

        public static GridProperties Default() => new();
    }
}
