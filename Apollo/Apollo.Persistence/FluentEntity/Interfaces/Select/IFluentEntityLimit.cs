namespace Apollo.Persistence.FluentEntity.Interfaces.Select
{
    public interface IFluentEntityLimit<T> : IFluentEntityCall<T>
    {
        IFluentEntityCall<T> Limit(long limit, long offset = 0);
    }
}
