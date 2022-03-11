namespace GDFiddle.Ecs.ComponentStore
{
    internal class EntityMovedEventArgs
    : EventArgs
    {
        public EntityMovedEventArgs(EntityId entityId, int idx)
        {
            EntityId = entityId;
            NewIdx = idx;
        }

        public readonly EntityId EntityId;
        public readonly int NewIdx;
    }
}
