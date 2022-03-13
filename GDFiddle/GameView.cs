using System.Drawing;
using GDFiddle.UI;
using GDFiddle.UI.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle
{
    internal class GameView : Control
    {
        private readonly GraphicsDevice _graphicsDevice;
        private RenderTarget2D? _renderTarget;

        public GameView(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
        }

        public override void Render(GuiRenderer guiRenderer, Size size)
        {
            base.Render(guiRenderer, size);
            var gameFrame = RenderGame(size);
            guiRenderer.Draw(gameFrame);
        }
        
        /// <summary>
        /// Renders the current game frame to a texture.
        /// </summary>
        private Texture2D RenderGame(Size size)
        {
            var renderTarget = PrepareRenderTarget(size);
            _graphicsDevice.SetRenderTarget(renderTarget);
            Game?.Render();
            _graphicsDevice.SetRenderTarget(null);
            return renderTarget;
        }

        /// <summary>
        /// Reuses the existing RenderTarget in videoram, or prepares a new one if the <see cref="GameView"/>'s size has changed.
        /// </summary>
        private RenderTarget2D PrepareRenderTarget(Size size)
        {
            if (_renderTarget == null || _renderTarget.Width != size.Width || _renderTarget.Height != size.Height)
            {
                _renderTarget?.Dispose();
                _renderTarget = new RenderTarget2D(_graphicsDevice, size.Width, size.Height);
            }
            return _renderTarget;
        }

        public GDFiddleGame? Game { get; set; }
    }
}
