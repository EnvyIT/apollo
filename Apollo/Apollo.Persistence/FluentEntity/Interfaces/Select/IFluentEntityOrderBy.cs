using System;
using System.Linq.Expressions;

namespace Apollo.Persistence.FluentEntity.Interfaces.Select
{
    public interface IFluentEntityOrderBy<T> : IFluentEntityLimit<T>
    {
        IFluentEntityOrderBy<T> ByAscending<TKey>(Expression<Func<T, TKey>> key);

        IFluentEntityOrderBy<T> ByDescending<TKey>(Expression<Func<T, TKey>> key);
    }
}
