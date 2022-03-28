using System.Numerics;
using GDFiddle.Ecs;
using GDFiddle.Framework.Messaging;
using GDFiddle.UI;
using GDFiddle.UI.Controls;
using Microsoft.Xna.Framework.Graphics;
using Color = Microsoft.Xna.Framework.Color;

namespace GDFiddle.Editor
{
    internal class GameView : Control, IDisposable
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IMessageBus _messageBus;
        private RenderTarget2D? _renderTarget;
        private EntityId? _selectedEntityId;

        public GameView(GraphicsDevice graphicsDevice, IMessageBus messageBus)
        {
            _graphicsDevice = graphicsDevice;
            _messageBus = messageBus;
            SelectionColor = new Color(250, 206, 50);
            IsMouseInteractive = true;

            SubscribeMessageHandlers();
        }

        private void SubscribeMessageHandlers()
        {
            _messageBus.Subscribe<GameOpened>(opened => Game = opened.Game);
        }

        protected override void Render(GuiRenderer guiRenderer)
        {
            base.Render(guiRenderer);
            var gameFrame = RenderGame(ArrangedSize);
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
        private Texture2D RenderGame(Vector2 size)
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
        private RenderTarget2D PrepareRenderTarget(Vector2 size)
        {
            if (_renderTarget == null || _renderTarget.Width != (int)size.X || _renderTarget.Height != (int)size.Y)
            {
                _renderTarget?.Dispose();
                _renderTarget = new RenderTarget2D(_graphicsDevice, (int)size.X, (int)size.Y);
            }
            return _renderTarget;
        }

        public override void OnMouseDown(Vector2 mousePosition)
        {
            Select(Game?.GetEntityAt(mousePosition));
        }

        private void Select(EntityId? entityId)
        {
            _selectedEntityId = entityId;
            _messageBus.Publish(new EntitySelected(_selectedEntityId));
        }

        public void Dispose()
        {
            _renderTarget?.Dispose();
        }

        public GDFiddleGame? Game { get; set; }

        public Color SelectionColor { get; set; }
    }
}
