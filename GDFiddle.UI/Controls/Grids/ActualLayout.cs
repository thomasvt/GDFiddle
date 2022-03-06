using System.Runtime.CompilerServices;

namespace GDFiddle.UI.Controls.Grids
{
    public readonly struct ActualLayout
    {
        public float Offset { get; }
        public float Size { get; }

        public ActualLayout(float offset, float size)
        {
            Offset = offset;
            Size = size;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Contains(float position)
        {
            return Offset <= position && Offset + Size > position;
        }
    }
}
