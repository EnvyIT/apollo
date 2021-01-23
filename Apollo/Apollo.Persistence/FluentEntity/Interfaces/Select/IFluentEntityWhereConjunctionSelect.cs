using System;
using System.Linq.Expressions;

namespace Apollo.Persistence.FluentEntity.Interfaces.Select
{
    public interface IFluentEntityWhereConjunctionSelect<T> : IFluentEntityOrder<T>
    {
        IFluentEntityWhereConditionSelect<T> And<TKey>(Expression<Func<T, TKey>> key);

        IFluentEntityWhereConditionSelect<T> Or<TKey>(Expression<Func<T, TKey>> key);
    }
}
