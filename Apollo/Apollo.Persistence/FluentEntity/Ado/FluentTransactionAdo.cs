using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Insert;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.FluentEntity.Interfaces.Transaction;

namespace Apollo.Persistence.FluentEntity.Ado
{
    public class FluentTransactionAdo :
        IFluentTransaction,
        IFluentTransactionCommit
    {
        private readonly IDaoHelper _daoHelper;
        private readonly List<ICallExecute> _executions;
        private Func<Task> _executionBlock;

        public FluentTransactionAdo(IDaoHelper daoHelper)
        {
            _daoHelper = daoHelper ?? throw new ArgumentNullException(nameof(daoHelper));
            _executions = new List<ICallExecute>();
        }

        public IFluentTransaction Perform(IFluentEntityCallExecute action)
        {
            _executions.Add(action);
            return this;
        }

        public IFluentTransaction Perform(IFluentEntityCallInsertExecute action)
        {
            _executions.Add(action);
            return this;
        }

        public IFluentTransactionCommit PerformBlock(Func<Task> block)
        {
            _executionBlock = block;
            return this;
        }

        public async Task Commit()
        {
            try
            {
                if (_executionBlock != null)
                {
                    await _daoHelper.PerformTransaction(_executionBlock);
                }
                else
                {
                    await _daoHelper.PerformTransaction(async () =>
                    {
                        foreach (var execution in _executions)
                        {
                            await execution.ExecuteAsync();
                        }
                    });
                }
            }
            finally
            {
                Reset();
            }
        }

        private void Reset()
        {
            _executions.Clear();
            _executionBlock = null;
        }
    }
}
