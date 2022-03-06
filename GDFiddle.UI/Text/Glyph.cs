using System.Numerics;

namespace GDFiddle.UI.Text
{
    internal readonly struct Glyph
    {
        public readonly int Code;
        public readonly Vector2 UVMin;
        public readonly Vector2 UVMax;
        public readonly int Width;
        public readonly int Heigth;
        public readonly int XOffset;
        public readonly int YOffset;
        public readonly int XAdvance;

        public Glyph(int code, Vector2 uvMin, Vector2 uvMax, int width, int heigth, int xOffset, int yOffset, int xAdvance)
        {
            Code = code;
            UVMin = uvMin;
            UVMax = uvMax;
            Width = width;
            Heigth = heigth;
            XOffset = xOffset;
            YOffset = yOffset;
            XAdvance = xAdvance;
        }
    }
}
