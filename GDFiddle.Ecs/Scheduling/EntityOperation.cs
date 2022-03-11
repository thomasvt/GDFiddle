namespace GDFiddle.Ecs.Scheduling
{
    internal abstract class EntityOperation
    {
        public abstract void Execute(EcsScene sceneManager);
    }

    internal class EntityOperation<TComponent> : EntityOperation where TComponent : struct
    {
        public EntityId EntityId;
        public EntityOperationType Type;
        public override void Execute(EcsScene sceneManager)
        {
            switch (Type)
            {
                case EntityOperationType.RemoveEntity:
                    sceneManager.RemoveInternal(EntityId);
                    break;
                case EntityOperationType.AddComponent:
                    sceneManager.AddComponentInternal<TComponent>(EntityId);
                    break;
                case EntityOperationType.RemoveComponent:
                    sceneManager.RemoveComponentInternal<TComponent>(EntityId);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}
