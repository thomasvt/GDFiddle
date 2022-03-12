using GDFiddle.Ecs;
using GDFiddle.Ecs.Querying;
using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;

namespace TestGame
{
    public class RenderSystem : IRenderSystem, IInitialize
    {
        private readonly IRenderer _renderer;
        private readonly IScene _scene;
        private EntityQuery<PositionComponent, SpriteComponent>? _renderQuery;

        public RenderSystem(IRenderer renderer, IScene scene)
        {
            _renderer = renderer;
            _scene = scene;
        }

        public void Initialize()
        {
            _renderQuery = _scene.Querying.DefineQuery<PositionComponent, SpriteComponent>();
        }

        public void Render()
        {
            _renderer.BeginFrame();
            _renderQuery!.VisitAllBulk((ids, positions, sprites) =>
            {
                for (var i = 0; i < ids.Length; i++)
                {
                    var sprite = sprites[i].Sprite;
                    if (sprite != null)
                        _renderer.Draw(sprite, positions[i].Position);
                }
            });
            _renderer.EndFrame();
        }
    }
}