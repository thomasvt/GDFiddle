using GDFiddle.Ecs;
using GDFiddle.Ecs.Querying;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace GDFiddle
{
    internal class FiddleGame
    {
        private readonly GraphicsDevice _graphicsDevice;
        private readonly IEcsScene _scene;
        private readonly Texture2D _texture;
        private readonly SpriteBatch _sb;
        private readonly EntityQuery<PositionComponent> _renderQuery;

        public FiddleGame(GraphicsDevice graphicsDevice)
        {
            _graphicsDevice = graphicsDevice;
            _scene = new EcsScene(new EcsConfig { InitialEntityCapacity = 100, MaxArchetypeCount = 100 });
            _texture = Texture2D.FromFile(_graphicsDevice, "hero.png");
            _sb = new SpriteBatch(_graphicsDevice);

            _scene.CreateEntity(id => new PositionComponent { Position = new Vector2(100, 100) });
            _scene.CreateEntity(id => new PositionComponent { Position = new Vector2(150, 100) });
            _scene.CreateEntity(id => new PositionComponent { Position = new Vector2(100, 150) });
            _scene.CreateEntity(id => new PositionComponent { Position = new Vector2(100, 50) });
            _scene.CreateEntity(id => new PositionComponent { Position = new Vector2(50, 100) });
            _scene.CreateEntity(id => new PositionComponent { Position = new Vector2(0,0) });
            _renderQuery = _scene.Querying.DefineQuery<PositionComponent>();
        }

        public void Update(Time time)
        {
            _scene.Tick(time);
        }

        public void Draw()
        {
            _sb.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Matrix.CreateTranslation(_graphicsDevice.Viewport.Width * 0.5f, _graphicsDevice.Viewport.Height * 0.5f, 0));
            _renderQuery.VisitAllBulk((ids, positions) =>
            {
                for (var i = 0; i < ids.Length; i++)
                    _sb.Draw(_texture, positions[i].Position, Color.White);
            });
            _sb.End();
        }
    }
}
