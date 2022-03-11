namespace GDFiddle.Ecs
{
    // normal
    public delegate void EntityCallback1<TC1>(EntityId entityid, ref TC1 component) where TC1 : struct;
    public delegate void EntityCallback2<TC1, TC2>(EntityId entityId, ref TC1 component1, ref TC2 component2);
    public delegate void EntityCallback3<TC1, TC2, TC3>(EntityId entityId, ref TC1 component1, ref TC2 component2, ref TC3 component3);
    public delegate void EntityCallback4<TC1, TC2, TC3, TC4>(EntityId entityId, ref TC1 component1, ref TC2 component2, ref TC3 component3, ref TC4 component4);
    public delegate void EntityCallback5<TC1, TC2, TC3, TC4, TC5>(EntityId entityId, ref TC1 component1, ref TC2 component2, ref TC3 component3, ref TC4 component4, ref TC5 component5);

    // bulk
    public delegate void EntityCallbackBulk1<TC1>(ReadOnlySpan<EntityId> entityIds, Span<TC1> componentBlock1) where TC1 : struct;
    public delegate void EntityCallbackBulk2<TC1, TC2>(ReadOnlySpan<EntityId> entityId, Span<TC1> componentBlock1, Span<TC2> componentBlock2);
    public delegate void EntityCallbackBulk3<TC1, TC2, TC3>(ReadOnlySpan<EntityId> entityId, Span<TC1> componentBlock1, Span<TC2> componentBlock2, Span<TC3> componentBlock3);
    public delegate void EntityCallbackBulk4<TC1, TC2, TC3, TC4>(ReadOnlySpan<EntityId> entityId, Span<TC1> componentBlock1, Span<TC2> componentBlock2, Span<TC3> componentBlock3, Span<TC4> componentBlock4);
    public delegate void EntityCallbackBulk5<TC1, TC2, TC3, TC4, TC5>(ReadOnlySpan<EntityId> entityId, Span<TC1> componentBlock1, Span<TC2> componentBlock2, Span<TC3> componentBlock3, Span<TC4> componentBlock4, Span<TC5> componentBlock5);
}
