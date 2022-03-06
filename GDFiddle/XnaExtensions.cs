using Microsoft.Xna.Framework;

namespace GDFiddle
{
    internal static class XnaExtensions
    {
        public static Rectangle ToXna(this System.Drawing.RectangleF r)
        {
            return new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
        }
    }
}
