using System;
using System.Linq.Expressions;

namespace Apollo.Persistence.FluentEntity.Interfaces.Select
{
    public interface IFluentEntitySelect<T> : IFluentEntityJoin<T>
    {
        IFluentEntitySelect<T> Column<TKey>(Expression<Func<T, TKey>> key);

        IFluentEntitySelect<T> ColumnRef<TRef, TKey>(Expression<Func<TRef, TKey>> key);
    }
}
