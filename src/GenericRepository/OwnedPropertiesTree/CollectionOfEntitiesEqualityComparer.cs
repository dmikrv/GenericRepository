namespace GenericRepository.OwnedPropertiesTree;

public class CollectionOfEntitiesEqualityComparer : IEqualityComparer<object?[]>
{
    public bool Equals(object?[]? x, object?[]? y)
    {
        if (x == null || y == null) return x == y;

        return x.SequenceEqual(y);
    }

    public int GetHashCode(object?[] obj)
    {
        unchecked
        {
            return obj.Aggregate(17, (current, item) => current * 31 + (item?.GetHashCode() ?? 0));
        }
    }
}