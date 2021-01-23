using System;

namespace Apollo.Persistence.Attributes.Contracts
{
    public interface IIdentifiable<T> : IEquatable<T>
    {
        long Id { get; set; }
    }
}
