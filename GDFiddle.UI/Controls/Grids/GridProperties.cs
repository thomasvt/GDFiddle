namespace GDFiddle.UI.Controls.Grids
{
    public record GridProperties(int Column, int Row)
    {
        public static GridProperties Default() => new(0,0);
    }
}
