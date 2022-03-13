using GDFiddle.Ecs.Bulk;
using GDFiddle.Ecs.Querying;
using GDFiddle.Ecs.Scheduling;

namespace GDFiddle.Ecs
{
    /// <summary>
    /// The entry point to all ECS functionality. Use subobjects Hierarchy, Querying and AtFrameEnd for more specific functionality.
    /// </summary>
    public interface IScene
    {
        EntityId CreateEntity(Archetype archetype);
        /// <summary>
        /// Adds the component to that entity, changing its archetype.
        /// </summary>
        void AddComponent<TComponent>(EntityId entityId) where TComponent : struct;
        /// <summary>
        /// Adds a component to the entity and sets its value. Throws if the entity already has a component of that type.
        /// </summary>
        void AddComponent<TComponent>(EntityId entityId, TComponent value) where TComponent : struct;
        /// <summary>
        /// Removes the component from the entity, changing its archetype. 
        /// </summary>
        void RemoveComponent<TComponent>(EntityId entityId) where TComponent : struct;
        void Remove(EntityId entityId);
        ref TComponent GetComponentRef<TComponent>(EntityId entityId) where TComponent : struct;
        bool HasComponent<TComponent>(EntityId entityId) where TComponent : struct;
        Archetype DefineArchetype();
        void ClearScene();
        int EntityCount { get; }
        int ComponentCount { get; }
        /// <summary>
        /// Provides data querying features on the entities in the scene.
        /// </summary>
        IEcsQueryManager Querying { get; }
        IEcsBulkManager Bulk { get; }
        IScheduler AfterFrame { get; }
        Archetype GetArchetype(EntityId entityId);
        bool Exists(EntityId entityId);
        void SetComponent<TComponent>(EntityId entityId, in TComponent component) where TComponent : struct;

        /// <summary>
        /// Registers a callback for when components of a specific type are removed. This gets triggered when an entity with a matching component is removed, or just the component is removed from an entity.
        /// </summary>
        void RegisterComponentRemoveCallback<TComponent>(EntityCallback1<TComponent> callback) where TComponent : struct;

        /// <summary>
        /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
        /// </summary>
        EntityId CreateEntity<TC1>(in Func<EntityId, TC1> c1) where TC1 : struct;

        /// <summary>
        /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
        /// </summary>
        EntityId CreateEntity<TC1, TC2>(in Func<EntityId, TC1> c1, in Func<EntityId, TC2> c2) where TC1 : struct where TC2 : struct;

        /// <summary>
        /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
        /// </summary>
        EntityId CreateEntity<TC1, TC2, TC3>(in Func<EntityId, TC1> c1, in Func<EntityId, TC2> c2, in Func<EntityId, TC3> c3) where TC1 : struct where TC2 : struct where TC3 : struct;

        /// <summary>
        /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
        /// </summary>
        EntityId CreateEntity<TC1, TC2, TC3, TC4>(in Func<EntityId, TC1> c1, in Func<EntityId, TC2> c2, in Func<EntityId, TC3> c3, in Func<EntityId, TC4> c4) where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct;

        /// <summary>
        /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
        /// </summary>
        EntityId CreateEntity<TC1, TC2, TC3, TC4, TC5>(in Func<EntityId, TC1> c1, in Func<EntityId, TC2> c2, in Func<EntityId, TC3> c3, in Func<EntityId, TC4> c4, in Func<EntityId, TC5> c5) where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct where TC5 : struct;
    }
}