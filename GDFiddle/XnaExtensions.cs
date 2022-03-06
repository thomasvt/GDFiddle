using Microsoft.Xna.Framework;

namespace GDFiddle
{
    internal static class XnaExtensions
    {
        public static Rectangle ToXna(this System.Drawing.Rectangle r)
        {
            return new Rectangle(r.X, r.Y, r.Width, r.Height);
        }
    }
}
