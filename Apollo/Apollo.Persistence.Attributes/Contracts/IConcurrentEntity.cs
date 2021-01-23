using System;

namespace Apollo.Persistence.Attributes.Contracts
{
    public interface IConcurrentEntity
    {
        DateTime RowVersion { get; set; }
    }
}
