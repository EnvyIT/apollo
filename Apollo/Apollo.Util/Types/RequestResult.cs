namespace Apollo.Util.Types
{
    public class RequestResult<T>
    {
        public bool IsValid { get; set; }
        public T Content { get; set; }

        public RequestResult(bool isValid, T content)
        {
            IsValid = isValid;
            Content = content;
        }
    }
}