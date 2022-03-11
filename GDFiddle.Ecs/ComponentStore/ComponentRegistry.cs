namespace GDFiddle.Ecs.ComponentStore
{
    internal class ComponentRegistry
    {
        internal readonly Dictionary<Type, byte> ComponentIdsByType;
        private readonly Dictionary<byte, ComponentDefinition> _componentDefinitionsById;

        public ComponentRegistry(int maxComponentTypeCount)
        {
            _componentDefinitionsById = new Dictionary<byte, ComponentDefinition>(maxComponentTypeCount);
            MaxComponentTypeCount = maxComponentTypeCount;
            ComponentIdsByType = new Dictionary<Type, byte>(maxComponentTypeCount);
        }

        internal byte GetComponentId<TC>() where TC : struct
        {
            var componentType = typeof(TC);
            return ComponentIdsByType[componentType];
        }

        internal byte GetOrRegisterComponentId<TC>() where TC : struct
        {
            var componentType = typeof(TC);
            if (ComponentIdsByType.TryGetValue(componentType, out var id))
                return id;

            id = (byte)ComponentIdsByType.Count;
            if (id == MaxComponentTypeCount)
                throw new Exception($"Max of {MaxComponentTypeCount} types of Component exceeded.");

            ComponentIdsByType.Add(componentType, id);
            _componentDefinitionsById.Add(id, new ComponentDefinition<TC>(id));

            return id;
        }

        /// <summary>
        /// Registers a new TC if it wasnt registered yet.
        /// </summary>
        internal void RegisterComponent<TC>() where TC : struct
        {
            var componentType = typeof(TC);
            if (ComponentIdsByType.TryGetValue(componentType, out _))
            {
                return;
            }

            var id = (byte)ComponentIdsByType.Count;
            if (id == MaxComponentTypeCount)
                throw new Exception($"Max of {MaxComponentTypeCount} types of Component exceeded.");

            ComponentIdsByType.Add(componentType, id);
            _componentDefinitionsById.Add(id, new ComponentDefinition<TC>(id));
        }

        public ComponentDefinition GetComponentDefinition(byte componentId)
        {
            return _componentDefinitionsById[componentId];
        }

        public void Clear()
        {
            ComponentIdsByType.Clear();
            _componentDefinitionsById.Clear();
        }

        public int ComponentCount => ComponentIdsByType.Count;

        public int MaxComponentTypeCount { get; }
    }
}
