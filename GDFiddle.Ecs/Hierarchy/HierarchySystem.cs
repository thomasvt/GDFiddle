namespace GDFiddle.Ecs.Hierarchy
{
    /// <summary>
    /// Baseclass for a hierarchical ownership system from parents to children (1-to-many). Allows to recursively notify the children of messages from their parent, and remove children from the ECSScene when their parent is removed.
    /// Typically used for propagating transforms to children.
    /// </summary>
    public abstract class HierarchySystem<TMessage> : IDisposable where TMessage : struct
    {
        protected readonly IScene Scene;
        private readonly Dictionary<EntityId, EntityId> _parentPerChild;
        private readonly Dictionary<EntityId, ChildList> _childListPerParent;

        protected HierarchySystem(IScene scene)
        {
            Scene = scene;
            Scene.RegisterComponentRemoveCallback((EntityId entityid, ref ParentComponent parentComponent) => RemoveChildrenFromScene(entityid));
            _parentPerChild = new Dictionary<EntityId, EntityId>();
            _childListPerParent = new Dictionary<EntityId, ChildList>();
        }

        private void RemoveChildrenFromScene(EntityId parentId)
        {
            if (!_childListPerParent.ContainsKey(parentId))
                return;

            var childList = _childListPerParent[parentId];
            for (var i = 0; i < childList.Count; i++)
            {
                var childId = childList.ChildIds[i];
                Scene.Remove(childId); // will cause recursion if some children also have ParentComponent.
                _parentPerChild.Remove(childId);
            }
            // clear this parent's data:
            childList.Dispose();
            _childListPerParent.Remove(parentId);
        }

        /// <summary>
        /// Creates the hierarchic link and triggers a message cascade so the children can adjust to the new situation.
        /// </summary>
        public void Attach(in EntityId childId, in EntityId parentId, in TMessage initializeMessage)
        {
#if DEBUG
            if (_parentPerChild.ContainsKey(childId))
                throw new Exception($"Cannot attach {childId} to parent {parentId} because it already has parent {_parentPerChild[childId]}.");
#endif
            if (!_childListPerParent.TryGetValue(parentId, out var childList))
            {
                _childListPerParent.Add(parentId, new ChildList(childId));
            }
            else
            {
                childList.Add(childId);
            }

            if (!Scene.HasComponent<ParentComponent>(parentId))
                Scene.AddComponent<ParentComponent>(parentId);
            _parentPerChild.Add(childId, parentId);

            NotifyChildInternal(childId, initializeMessage);
        }

        /// <summary>
        /// Removes the hierarchic link and triggers a message cascade so the children can adjust to the new situation.
        /// </summary>
        public void Detach(in EntityId parentId, in EntityId childId, in TMessage uninitializeMessage)
        {
#if DEBUG
            if (!_parentPerChild.ContainsKey(childId))
                throw new Exception($"Cannot detach {childId} from parent {parentId} because it has no parent at all.");
#endif
            if (!_childListPerParent.TryGetValue(parentId, out var childList))
            {
                throw new Exception($"Cannot detach {childId} from parent {parentId} because it is not its parent.");
            }

            childList.Remove(childId);
            _parentPerChild.Remove(childId);
            if (childList.Count == 0) 
            {
                // no longer a parent.
                childList.Dispose();
                _childListPerParent.Remove(parentId);
                Scene.RemoveComponent<ParentComponent>(parentId);
            }

            NotifyChildInternal(childId, uninitializeMessage);
        }

        public void NotifyChildren(in EntityId parentId, in TMessage message)
        {
            if (!_childListPerParent.ContainsKey(parentId)) 
                return;

            var childList = _childListPerParent[parentId];
            for (var i = 0; i < childList.Count; i++)
            {
                var childId = childList.ChildIds[i];
                NotifyChildInternal(childId, message);
            }
        }

        private void NotifyChildInternal(in EntityId childId, in TMessage message)
        {
            ref var childMessage = ref NotifyChild(in childId, in message);
            NotifyChildren(in childId, in childMessage);
        }

        public void Dispose()
        {
            foreach (var childList in _childListPerParent.Values) 
                childList.Dispose();
        }

        /// <summary>
        /// Should update the child according to the message, and return the message for its own children.
        /// </summary>
        protected abstract ref TMessage NotifyChild(in EntityId childId, in TMessage message);
    }
}
