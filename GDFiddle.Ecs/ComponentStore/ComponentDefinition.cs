namespace GDFiddle.Ecs.ComponentStore
{
    public abstract class ComponentDefinition
    {
        protected ComponentDefinition(byte componentId, Type componentType)
        {
            ComponentId = componentId;
            ComponentType = componentType;
        }

        public byte ComponentId { get; }
        public Type ComponentType { get; }

        internal abstract ComponentArray CreateComponentArray(int size);
    }

    internal class ComponentDefinition<TComponent>
        : ComponentDefinition
        where TComponent : struct
    {
        internal ComponentDefinition(byte componentId) : base(componentId, typeof(TComponent))
        {
        }

        internal override ComponentArray CreateComponentArray(int size)
        {
            return new ComponentArray<TComponent>(size);
        }
    }
}
