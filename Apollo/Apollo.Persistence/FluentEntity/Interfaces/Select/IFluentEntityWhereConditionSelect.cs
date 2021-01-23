using System.Collections.Generic;

namespace Apollo.Persistence.FluentEntity.Interfaces.Select
{
    public interface IFluentEntityWhereConditionSelect<T>
    {
        IFluentEntityWhereConjunctionSelect<T> Equal<TKey>(TKey key);

        IFluentEntityWhereConjunctionSelect<T> GreaterThan<TKey>(TKey key);

        IFluentEntityWhereConjunctionSelect<T> LowerThan<TKey>(TKey key);

        IFluentEntityWhereConjunctionSelect<T> GreaterThanEquals<TKey>(TKey key);

        IFluentEntityWhereConjunctionSelect<T> LowerThanEquals<TKey>(TKey key);

        IFluentEntityWhereConjunctionSelect<T> StartsWith<TKey>(TKey key);

        IFluentEntityWhereConjunctionSelect<T> In<TKey>(IEnumerable<TKey> keys);
        IFluentEntityWhereConjunctionSelect<T> NotIn<TKey>(IEnumerable<TKey> keys);
    }
}
