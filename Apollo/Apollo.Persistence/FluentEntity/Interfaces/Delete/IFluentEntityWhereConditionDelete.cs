using System.Collections.Generic;

namespace Apollo.Persistence.FluentEntity.Interfaces.Delete
{
    public interface IFluentEntityWhereConditionDelete<T>
    {
        IFluentEntityWhereConjunctionDelete<T> Equal<TKey>(TKey key);

        IFluentEntityWhereConjunctionDelete<T> GreaterThan<TKey>(TKey key);

        IFluentEntityWhereConjunctionDelete<T> LowerThan<TKey>(TKey key);

        IFluentEntityWhereConjunctionDelete<T> GreaterThanEquals<TKey>(TKey key);

        IFluentEntityWhereConjunctionDelete<T> LowerThanEquals<TKey>(TKey key);
        IFluentEntityWhereConjunctionDelete<T> StartsWith<TKey>(TKey key);
        IFluentEntityWhereConjunctionDelete<T> In<TKey>(IEnumerable<TKey> keys);
        IFluentEntityWhereConjunctionDelete<T> NotIn<TKey>(IEnumerable<TKey> keys);
    }
}
