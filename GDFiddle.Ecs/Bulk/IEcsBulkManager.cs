namespace GDFiddle.Ecs.Bulk
{
    public interface IEcsBulkManager
    {
        /// <summary>
        /// Creates multiple entities of the archetype in one go. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        EntityId CreateEntities(Archetype archetype, int count);

        /// <summary>
        /// Creates multiple entities of the archetype and calls a component initializer callback for each instance in one operation. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        EntityId CreateEntities<TC1>(Archetype archetype, int count, EntityCallback1<TC1> initializer)
            where TC1 : struct;

        /// <summary>
        /// Creates multiple entities of the archetype and calls a component initializer callback for each instance in one operation. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        EntityId CreateEntities<TC1, TC2>(Archetype archetype, int count, EntityCallback2<TC1, TC2> initializer)
            where TC1 : struct where TC2 : struct;

        /// <summary>
        /// Creates multiple entities of the archetype and calls a component initializer callback for each instance in one operation. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        EntityId CreateEntities<TC1, TC2, TC3>(Archetype archetype, int count, EntityCallback3<TC1, TC2, TC3> initializer)
            where TC1 : struct where TC2 : struct where TC3 : struct;

        /// <summary>
        /// Creates multiple entities of the archetype and calls a component initializer callback for each instance in one operation. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        EntityId CreateEntities<TC1, TC2, TC3, TC4>(Archetype archetype, int count, EntityCallback4<TC1, TC2, TC3, TC4> initializer)
            where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct;

        /// <summary>
        /// Creates multiple entities of the archetype and calls a component initializer callback for each instance in one operation. Returns the first EntityId of an uninterrupted sequence of ids of the created entities.
        /// </summary>
        EntityId CreateEntities<TC1, TC2, TC3, TC4, TC5>(Archetype archetype, int count, EntityCallback5<TC1, TC2, TC3, TC4, TC5> initializer)
            where TC1 : struct where TC2 : struct where TC3 : struct where TC4 : struct where TC5 : struct;
    }
}