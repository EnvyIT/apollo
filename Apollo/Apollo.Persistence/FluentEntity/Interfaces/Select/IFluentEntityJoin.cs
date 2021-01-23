using System;
using System.Linq.Expressions;

namespace Apollo.Persistence.FluentEntity.Interfaces.Select
{
    public interface IFluentEntityJoin<T1> : IFluentEntityWhereSelect<T1> 
    {
        IFluentEntityJoin<T1> InnerJoin<TJoin1, TJoin2, TKey1, TKey2>(
            Expression<Func<T1, TJoin2>> property,
            Expression<Func<TJoin1, TKey1>> onColumn1,
            Expression<Func<TJoin2, TKey2>> onColumn2
            ) where TJoin2 : new();

    }
}
