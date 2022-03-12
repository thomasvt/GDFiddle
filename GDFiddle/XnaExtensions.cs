using Microsoft.Xna.Framework;

namespace GDFiddle
{
    internal static class XnaExtensions
    {
        public static Rectangle ToXna(this System.Drawing.RectangleF r)
        {
            return new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }

        public static Vector2 ToXna(this System.Numerics.Vector2 v)
        {
            return new Vector2(v.X, v.Y);
        }
    }
}
