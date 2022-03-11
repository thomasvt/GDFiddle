using System.Drawing;
using GDFiddle.UI;
using GDFiddle.UI.Controls;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle
{
    internal class GameView : Control
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly FiddleGame _game;
        private RenderTarget2D? _renderTarget;

        public GameView(GraphicsDevice graphicsDevice, FiddleGame _game)
        {
            _graphicsDevice = graphicsDevice;
            this._game = _game;
        }

        public override void Render(Renderer renderer, Size size)
        {
            if (_renderTarget == null || _renderTarget.Width != size.Width || _renderTarget.Height != size.Height)
            {
                _renderTarget?.Dispose();
                _renderTarget = new RenderTarget2D(_graphicsDevice, size.Width, size.Height);
            }

            base.Render(renderer, size);
            _graphicsDevice.SetRenderTarget(_renderTarget);
            _game.Draw();
            _graphicsDevice.SetRenderTarget(null);
            renderer.Draw(_renderTarget);
        }
    }
}
