using System;
using System.Linq.Expressions;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;

namespace Apollo.Persistence.FluentEntity.Interfaces.Delete
{
    public interface IFluentEntityWhereDelete<T>: IFluentEntityCallExecute
    {
        IFluentEntityWhereConditionDelete<T> Where<TKey>(Expression<Func<T, TKey>> key);
    }
}
