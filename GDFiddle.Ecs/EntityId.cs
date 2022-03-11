namespace GDFiddle.Ecs
{
    public readonly struct EntityId
        : IEquatable<EntityId>
    {
        public EntityId(int id)
        {
            Id = id;
        }

        public readonly int Id;
        public bool IsNone => Id == 0;

        public override string ToString()
        {
            return Id.ToString();
        }

        public bool Equals(EntityId other)
        {
            return Id == other.Id;
        }

        public override bool Equals(object? obj)
        {
            return obj is EntityId other && Equals(other);
        }

        public override int GetHashCode()
        {
            return Id;
        }

        public static bool operator ==(EntityId a, EntityId b)
        {
            return a.Id == b.Id;
        }

        public static bool operator !=(EntityId a, EntityId b)
        {
            return a.Id != b.Id;
        }

        public static implicit operator int(EntityId entityId) => entityId.Id;
        public static explicit operator EntityId(int entityId) => new(entityId);

        public static EntityId None = new(0);
    }
}
