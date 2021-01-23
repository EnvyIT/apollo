using System.Collections.Generic;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.FluentEntity.Interfaces.Delete;
using Apollo.Persistence.FluentEntity.Interfaces.Insert;
using Apollo.Persistence.FluentEntity.Interfaces.Select;
using Apollo.Persistence.FluentEntity.Interfaces.Update;

namespace Apollo.Persistence.FluentEntity.Interfaces.Shared
{
    public interface IFluentEntityFrom
    {
        IFluentEntitySelectRequired<T> Select<T>(bool autoReset = true) where T : BaseEntity<T>, new();

        IFluentEntityJoin<T> SelectAll<T>(bool autoReset = true) where T : BaseEntity<T>, new();

        IFluentEntityInsert InsertInto<T>(T value) where T : BaseEntity<T>, new();

        IFluentEntityInsert InsertInto<T>(IEnumerable<T> values)where T : BaseEntity<T>, new();
        IFluentEntityDelete<T> Delete<T>() where T : BaseEntity<T>, new();
        IFluentEntityUpdate<T> Update<T>(T value) where T : BaseEntity<T>, new();
        IFluentEntityUpdate<T> Update<T>(IEnumerable<T> values) where T : BaseEntity<T>, new();
    }
}
