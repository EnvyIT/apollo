using System;
using System.Linq.Expressions;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;

namespace Apollo.Persistence.FluentEntity.Interfaces.Delete
{
    public interface IFluentEntityWhereConjunctionDelete<T>: IFluentEntityCallExecute
    {
        IFluentEntityWhereConditionDelete<T> And<TKey>(Expression<Func<T, TKey>> key);

        IFluentEntityWhereConditionDelete<T> Or<TKey>(Expression<Func<T, TKey>> key);
    }
}
