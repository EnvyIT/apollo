using System;
using System.Linq.Expressions;
using Apollo.Persistence.FluentEntity.Interfaces.Select;

namespace Apollo.Persistence.FluentEntity.Interfaces
{
    public interface IFluentEntitySelectRequired<T>
    {
        IFluentEntitySelect<T> Column<TKey>(Expression<Func<T, TKey>> key);

        IFluentEntitySelect<T> ColumnRef<TRef, TKey>(Expression<Func<TRef, TKey>> key);
    }
}
