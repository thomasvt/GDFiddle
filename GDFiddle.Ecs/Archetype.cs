using System.Numerics;
using GDFiddle.Ecs.ComponentStore;

namespace GDFiddle.Ecs
{
    public readonly struct Archetype
    {
        private readonly ComponentRegistry _componentRegistry;
        public readonly ulong Mask;

        internal Archetype(ComponentRegistry componentRegistry)
        {
            _componentRegistry = componentRegistry;
            Mask = 0ul;
        }

        internal Archetype(in Archetype cloneSource, ulong mask)
        {
            _componentRegistry = cloneSource._componentRegistry;
            Mask = mask;
        }

        public Archetype With<TComponent>() where TComponent : struct
        {
            var componentId = _componentRegistry.GetOrRegisterComponentId<TComponent>();
            if ((Mask & (1ul << componentId)) != 0)
                throw new InvalidArchetypeException($"An archetype should not contain the same Component more than once. ({typeof(TComponent).Name})");

            return new Archetype(in this, Mask | (1ul << componentId));
        }

        public Archetype With<TC1, TC2>() where TC1 : struct where TC2 : struct
        {
            return With<TC1>().With<TC2>();
        }

        public Archetype With<TC1, TC2, TC3>() where TC1 : struct where TC2 : struct where TC3 : struct
        {
            return With<TC1>().With<TC2>().With<TC3>();
        }

        public Archetype With<TC1, TC2, TC3, TC4>() where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct
        {
            return With<TC1>().With<TC2>().With<TC3>().With<TC4>();
        }

        public Archetype With<TC1, TC2, TC3, TC4, TC5>() where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct where TC5 : struct
        {
            return With<TC1>().With<TC2>().With<TC3>().With<TC4>().With<TC5>();
        }

        public Archetype With<TC1, TC2, TC3, TC4, TC5, TC6>() where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct where TC5 : struct where TC6 : struct
        {
            return With<TC1>().With<TC2>().With<TC3>().With<TC4>().With<TC5>().With<TC6>();
        }

        internal Archetype Without<TComponent>() where TComponent : struct
        {
            var componentId = _componentRegistry.GetOrRegisterComponentId<TComponent>();
            if ((Mask & (1ul << componentId)) == 0)
                throw new InvalidOperationException("Cannot delete Component because this Archetype does not contain that Component.");

            return new Archetype(in this, Mask & ~(1ul << componentId));
        }

        public int ComponentCount => BitOperations.PopCount(Mask);

        public IEnumerable<byte> GetComponentIds()
        {
            var mask = Mask;
            for (byte i = 0; i < 64; i++)
            {
                if ((mask & 1ul) != 0)
                    yield return i;
                mask >>= 1;
            }
        }

        public IEnumerable<ComponentDefinition> GetComponentDefinitions()
        {
            var cr = _componentRegistry;
            return GetComponentIds().Select(cid => cr.GetComponentDefinition(cid));
        }

        public override string ToString()
        {
            var listing = string.Join(" + ", GetComponentDefinitions().OrderBy(cd => cd.ComponentType.Name).Select(cd =>
            {
                var name = cd.ComponentType.Name;
                return name.EndsWith("Component") ? name[..^"Component".Length] : name;
            }));
            return $"[{listing}]";
        }

        internal bool Contains(byte componentId)
        {
            return (Mask & (1ul << componentId)) != 0;
        }

        public bool Contains<TComponent>()
        {
            var componentId = _componentRegistry.ComponentIdsByType[typeof(TComponent)];
            return (Mask & (1ul << componentId)) != 0;
        }

        internal int GetHighestComponentId()
        {
            return 63 - BitOperations.LeadingZeroCount(Mask);
        }
    }
}
