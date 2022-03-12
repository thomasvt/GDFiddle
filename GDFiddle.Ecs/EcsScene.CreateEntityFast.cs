using GDFiddle.Ecs.ComponentStore;

namespace GDFiddle.Ecs;

public partial class Scene
{
    /// <summary>
    /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
    /// </summary>
    public EntityId CreateEntity<TC1>(in Func<EntityId, TC1> c1) where TC1 : struct
    {
        ThrowIfNotMainThread();
        var archetype = DefineArchetype().With<TC1>();
        var poolIdx = GetOrCreatePool(archetype);
        var pool = Pools[poolIdx];
        var entityId = EntityIndex.Alloc();
        var idxInPool = pool.Alloc(entityId, false); // skip init, because we're going to set each component's value anyway.
        EntityIndex.SetEntityInfo(entityId, poolIdx, idxInPool);

        SetComponentUnsafe(pool, idxInPool, c1(entityId));
        return entityId;
    }

    /// <summary>
    /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
    /// </summary>
    public EntityId CreateEntity<TC1, TC2>(in Func<EntityId, TC1> c1, in Func<EntityId, TC2> c2) where TC1 : struct where TC2 : struct
    {
        ThrowIfNotMainThread();
        var archetype = DefineArchetype().With<TC1, TC2>();
        var poolIdx = GetOrCreatePool(archetype);
        var pool = Pools[poolIdx];
        var entityId = EntityIndex.Alloc();
        var idxInPool = pool.Alloc(entityId, false); // skip init, because we're going to set each component's value anyway.
        EntityIndex.SetEntityInfo(entityId, poolIdx, idxInPool);

        SetComponentUnsafe(pool, idxInPool, c1(entityId));
        SetComponentUnsafe(pool, idxInPool, c2(entityId));
        return entityId;
    }

    /// <summary>
    /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
    /// </summary>
    public EntityId CreateEntity<TC1, TC2, TC3>(in Func<EntityId, TC1> c1, in Func<EntityId, TC2> c2, in Func<EntityId, TC3> c3) where TC1 : struct where TC2 : struct where TC3 : struct
    {
        ThrowIfNotMainThread();
        var archetype = DefineArchetype().With<TC1, TC2, TC3>();
        var poolIdx = GetOrCreatePool(archetype);
        var pool = Pools[poolIdx];
        var entityId = EntityIndex.Alloc();
        var idxInPool = pool.Alloc(entityId, false); // skip init, because we're going to set each component's value anyway.
        EntityIndex.SetEntityInfo(entityId, poolIdx, idxInPool);

        SetComponentUnsafe(pool, idxInPool, c1(entityId));
        SetComponentUnsafe(pool, idxInPool, c2(entityId));
        SetComponentUnsafe(pool, idxInPool, c3(entityId));
        return entityId;
    }

    /// <summary>
    /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
    /// </summary>
    public EntityId CreateEntity<TC1, TC2, TC3, TC4>(in Func<EntityId, TC1> c1, in Func<EntityId, TC2> c2, in Func<EntityId, TC3> c3, in Func<EntityId, TC4> c4) where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct
    {
        ThrowIfNotMainThread();
        var archetype = DefineArchetype().With<TC1, TC2, TC3, TC4>();
        var poolIdx = GetOrCreatePool(archetype);
        var pool = Pools[poolIdx];
        var entityId = EntityIndex.Alloc();
        var idxInPool = pool.Alloc(entityId, false); // skip init, because we're going to set each component's value anyway.
        EntityIndex.SetEntityInfo(entityId, poolIdx, idxInPool);

        SetComponentUnsafe(pool, idxInPool, c1(entityId));
        SetComponentUnsafe(pool, idxInPool, c2(entityId));
        SetComponentUnsafe(pool, idxInPool, c3(entityId));
        SetComponentUnsafe(pool, idxInPool, c4(entityId));
        return entityId;
    }

    /// <summary>
    /// Combines creating an archetype, entity and settings its component value in one. It's also faster than using separate steps.
    /// </summary>
    public EntityId CreateEntity<TC1, TC2, TC3, TC4, TC5>(in Func<EntityId, TC1> c1, in Func<EntityId, TC2> c2, in Func<EntityId, TC3> c3, in Func<EntityId, TC4> c4, in Func<EntityId, TC5> c5) where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct where TC5 : struct
    {
        ThrowIfNotMainThread();
        var archetype = DefineArchetype().With<TC1, TC2, TC3, TC4, TC5>();
        var poolIdx = GetOrCreatePool(archetype);
        var pool = Pools[poolIdx];
        var entityId = EntityIndex.Alloc();
        var idxInPool = pool.Alloc(entityId, false); // skip init, because we're going to set each component's value anyway.
        EntityIndex.SetEntityInfo(entityId, poolIdx, idxInPool);

        SetComponentUnsafe(pool, idxInPool, c1(entityId));
        SetComponentUnsafe(pool, idxInPool, c2(entityId));
        SetComponentUnsafe(pool, idxInPool, c3(entityId));
        SetComponentUnsafe(pool, idxInPool, c4(entityId));
        SetComponentUnsafe(pool, idxInPool, c5(entityId));
        return entityId;
    }

    private void SetComponentUnsafe<TComponent>(ComponentSetPool pool, int idxInPool, in TComponent component) where TComponent : struct
    {
        var componentId = ComponentRegistry.ComponentIdsByType[typeof(TComponent)];
        var componentArray = (ComponentArray<TComponent>)pool.ComponentArraysPerComponentId[componentId];
        componentArray.Records[idxInPool] = component;
    }
}