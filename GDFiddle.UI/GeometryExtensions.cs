using System.Drawing;
using System.Numerics;

namespace GDFiddle.UI
{
    public static class GeometryExtensions
    {
        public static Vector2 ToVector2(this Point point)
        {
            return new Vector2(point.X, point.Y);
        }
    }
}
