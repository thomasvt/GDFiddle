namespace GDFiddle.UI.Controls.Grids
{
    public struct ActualLayout
    {
        public float Offset { get; }
        public float Size { get; }

        public ActualLayout(float offset, float size)
        {
            Offset = offset;
            Size = size;
        }
    }
}
