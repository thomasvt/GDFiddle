using System.Drawing;
using System.Numerics;
using GDFiddle.Ecs;
using GDFiddle.UI;
using GDFiddle.UI.Controls;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle
{
    internal class GameView : Control
    {
        private readonly GraphicsDevice _graphicsDevice;
        private RenderTarget2D? _renderTarget;
        private EntityId? _selectedEntityId;

        public GameView(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            SelectionColor = new Color(250, 206, 50);
        }

        public override void Render(GuiRenderer guiRenderer, Size size)
        {
            base.Render(guiRenderer, size);
            var gameFrame = RenderGame(size);
            guiRenderer.Draw(gameFrame);
            RenderGizmos(guiRenderer);
        }

        private void RenderGizmos(GuiRenderer guiRenderer)
        {
            if (_selectedEntityId.HasValue && Game != null)
            {
                var aabb = Game.GetEntityScreenAabb(_selectedEntityId.Value);
                guiRenderer.DrawRectangle(aabb.TopLeft, aabb.Size, null, SelectionColor);
            }
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

        public override void NotifyMouseDown(Vector2 mousePosition)
        {
            _selectedEntityId = Game?.GetEntityAt(mousePosition);
        }

        public GDFiddleGame? Game { get; set; }
        public Color SelectionColor { get; set; }
    }
}
