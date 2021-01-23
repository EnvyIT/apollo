using System.Data;
using System.Data.Common;

namespace Apollo.Persistence.Test.Util
{
    public class MockMsSqlCommand : DbCommand
    {
        public override void Cancel()
        {
            throw new System.NotImplementedException($"Mock has no {nameof(Prepare)} implementation");
        }

        public override int ExecuteNonQuery() => 0;

        public override object ExecuteScalar() => 0L;

        public override void Prepare()
        {
            throw new System.NotImplementedException($"Mock has no {nameof(Prepare)} implementation");
        }

        public override string CommandText { get; set; }
        public override int CommandTimeout { get; set; }
        public override CommandType CommandType { get; set; }
        public override UpdateRowSource UpdatedRowSource { get; set; }
        protected override DbConnection DbConnection { get; set; }
        protected override DbParameterCollection DbParameterCollection { get; }
        protected override DbTransaction DbTransaction { get; set; }
        public override bool DesignTimeVisible { get; set; }

        protected override DbParameter CreateDbParameter()
        {
            throw new System.NotImplementedException($"Mock has no {nameof(CreateDbParameter)} implementation");
        }

        protected override DbDataReader ExecuteDbDataReader(CommandBehavior behavior)
        {
            throw new System.NotImplementedException($"Mock has no {nameof(ExecuteDbDataReader)} implementation");
        }
    }
}
