using System;
using System.Linq.Expressions;

namespace Apollo.Persistence.FluentEntity.Interfaces.Select
{
    public interface IFluentEntityWhereSelect<T> : IFluentEntityOrder<T>
    {
        IFluentEntityWhereConditionSelect<T> Where<TKey>(Expression<Func<T, TKey>> key);

        IFluentEntityWhereConjunctionSelect<T> WhereActive();
    }
}
