using System.Numerics;
using GDFiddle.Ecs;
using GDFiddle.Ecs.Querying;
using GDFiddle.Framework;
using GDFiddle.Framework.Graphics;

namespace TestGame
{
    public class RenderSystem : IRenderSystem
    {
        private readonly IScene _scene;
        private readonly IRenderer _renderer;
        private readonly EntityQuery<PositionComponent, SpriteComponent> _renderQuery;

        public RenderSystem(IScene scene, IRenderer renderer)
        {
            _scene = scene;
            _renderer = renderer;
            _renderQuery = scene.Querying.DefineQuery<PositionComponent, SpriteComponent>();
        }

        public void Render()
        {
            _renderer.BeginFrame(GetViewTransform());
            _renderQuery.VisitAll((ids, positions, sprites) =>
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

        /// <summary>
        /// Finds the entity at a certain screenposition (by its sprite). Return null if no entity is there.
        /// </summary>
        public EntityId? GetEntityAt(Vector2 screenPosition)
        {
            EntityId? entityId = null;
            var worldPosition = Vector2.Transform(screenPosition, GetViewInvTransform());
            _renderQuery.VisitAll((ids, positions, sprites) =>
            {
                for (var i = 0; i < ids.Length; i++)
                {
                    var sprite = sprites[i].Sprite;
                    if (sprite == null)
                        continue;

                    var origin = positions[i].Position;
                    var aabb = sprite.Aabb.Translate(origin);
                    if (aabb.Contains(worldPosition))
                    {
                        entityId = ids[i];
                        return;
                    }
                }
            });
            return entityId;
        }

        public Aabb GetEntityScreenAabb(EntityId entityId)
        {
            var position = _scene.GetComponentRef<PositionComponent>(entityId).Position;
            var spriteAabb = _scene.GetComponentRef<SpriteComponent>(entityId).Sprite?.Aabb;
            var aabbWorld = spriteAabb?.Translate(position) ?? new Aabb(position, Vector2.Zero);
            var viewInv = GetViewTransform();
            return new Aabb(Vector2.Transform(aabbWorld.TopLeft, viewInv), Vector2.TransformNormal(aabbWorld.Size, viewInv));

        }

        private Matrix3x2 GetViewTransform()
        {
            return Matrix3x2.CreateScale(2f) * Matrix3x2.CreateTranslation(_renderer.LastFrameSize * 0.5f);
        }

        private Matrix3x2 GetViewInvTransform()
        {
            return Matrix3x2.CreateTranslation(_renderer.LastFrameSize * -0.5f) * Matrix3x2.CreateScale(0.5f);
        }
    }
}