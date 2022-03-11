namespace GDFiddle.Ecs.Querying
{
    public interface IEcsQueryManager
    {
        EntityQuery<TC> DefineQuery<TC>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC : struct;

        EntityQuery<TC1, TC2> DefineQuery<TC1, TC2>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC1 : struct
            where TC2 : struct;

        EntityQuery<TC1, TC2, TC3> DefineQuery<TC1, TC2, TC3>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC1 : struct
            where TC2 : struct
            where TC3 : struct;

        EntityQuery<TC1, TC2, TC3, TC4> DefineQuery<TC1, TC2, TC3, TC4>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC1 : struct
            where TC2 : struct
            where TC3 : struct
            where TC4 : struct;

        EntityQuery<TC1, TC2, TC3, TC4, TC5> DefineQuery<TC1, TC2, TC3, TC4, TC5>(Action<QueryCriteria>? criteriaBuilder = null)
            where TC1 : struct
            where TC2 : struct
            where TC3 : struct
            where TC4 : struct
            where TC5 : struct;
    }
}