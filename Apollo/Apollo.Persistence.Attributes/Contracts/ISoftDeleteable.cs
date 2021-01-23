namespace Apollo.Persistence.Attributes.Contracts
{
    public interface ISoftDeleteable
    {
         bool Deleted { get; set; }
    }
}
