namespace Apollo.Persistence.Dao
{
    public class QueryParameter
    {
        public string Name { get; set; }
        public object Value { get; set; }

        public QueryParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }
    }
}
