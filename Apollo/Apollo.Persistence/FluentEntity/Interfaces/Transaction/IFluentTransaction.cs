using System;
using System.Threading.Tasks;
using Apollo.Persistence.FluentEntity.Interfaces.Insert;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;

namespace Apollo.Persistence.FluentEntity.Interfaces.Transaction
{
    public interface IFluentTransaction: IFluentTransactionCommit
    {
        IFluentTransaction Perform(IFluentEntityCallExecute action);
        IFluentTransaction Perform(IFluentEntityCallInsertExecute action);

        IFluentTransactionCommit PerformBlock(Func<Task> block);
    }
}
