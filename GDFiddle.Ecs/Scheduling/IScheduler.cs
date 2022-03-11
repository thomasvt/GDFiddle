namespace GDFiddle.Ecs.Scheduling
{
    public interface IScheduler
    {
        /// <summary>
        /// Schedules to remove an entity at the end of the current frame.
        /// </summary>
        void Remove(EntityId entityId);

        /// <summary>
        /// Schedules to Add a component to an entity at the end of the current frame.
        /// </summary>
        void AddComponent<TComponent>(EntityId entityId) where TComponent : struct;
        /// <summary>
        /// Schedules to remove a component from an entity at the end of the current frame.
        /// </summary>
        void RemoveComponent<TComponent>(EntityId entityId) where TComponent : struct;
    }
}
