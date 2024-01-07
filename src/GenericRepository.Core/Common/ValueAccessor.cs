namespace GenericRepository.Core.Common;

public class ValueAccessor<T>(Func<T> accessor)
{
    private readonly Func<T> _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));

    public T Value => _accessor();
}