using GDFiddle.Ecs;

namespace GDFiddle
{
    internal class EntitySelected
    {
        public EntityId? EntityId { get; }

        public EntitySelected(EntityId? entityId)
        {
            EntityId = entityId;
        }
    }
}
