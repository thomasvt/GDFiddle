using System.Collections.Concurrent;

namespace GDFiddle.Ecs.Scheduling
{
    internal class Scheduler : IScheduler
    {
        private readonly Scene _sceneManager;
        private readonly ConcurrentQueue<EntityOperation> _scheduledOperations;

        public Scheduler(Scene sceneManager)
        {
            _sceneManager = sceneManager;
            _scheduledOperations = new ConcurrentQueue<EntityOperation>();
        }

        public void Remove(EntityId entityId)
        {
            _scheduledOperations.Enqueue(new EntityOperation<bool> { EntityId = entityId, Type = EntityOperationType.RemoveEntity });
        }

        public void AddComponent<TComponent>(EntityId entityId) where TComponent : struct
        {
            _scheduledOperations.Enqueue(new EntityOperation<TComponent>
            {
                EntityId = entityId,
                Type = EntityOperationType.AddComponent,
            });
        }

        public void RemoveComponent<TComponent>(EntityId entityId) where TComponent : struct
        {
            _scheduledOperations.Enqueue(new EntityOperation<TComponent>
            {
                EntityId = entityId,
                Type = EntityOperationType.RemoveComponent
            });
        }

        internal void ExecuteScheduledActions()
        {
            while (_scheduledOperations.TryDequeue(out var operation))
            {
                operation.Execute(_sceneManager);
            }
        }

        public void Clear()
        {
            while (_scheduledOperations.TryDequeue(out _)) { }
        }
    }
}
