using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;

namespace GDFiddle
{
    /// <summary>
    /// Contains all user created game content.
    /// </summary>
    internal class GDFiddleGame
    {
        public IRenderSystem? RenderSystem { get; set; }
        public List<IInitialize> Initializers { get; set; }

    }
}
