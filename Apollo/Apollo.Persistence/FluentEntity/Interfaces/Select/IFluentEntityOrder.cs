namespace Apollo.Persistence.FluentEntity.Interfaces.Select
{
    public interface IFluentEntityOrder<T> : IFluentEntityLimit<T>
    {
        IFluentEntityOrderBy<T> Order();
    }
}
