using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.Dao;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.Exception;
using Apollo.Persistence.Util;
using Apollo.Repository.Interfaces;
using Apollo.Util.Logger;

namespace Apollo.Repository.Implementation
{
    public abstract class Repository : IRepository
    {
        private readonly IApolloLogger<Repository> Logger = LoggerFactory.CreateLogger<Repository>();

        protected readonly IDaoFactory DaoFactory;

        protected Repository(IConnectionFactory connectionFactory)
        {
            DaoFactory = new DaoFactory(connectionFactory);
        }

        public async Task ValidateIds<T>(IBaseDao<T> dao, IEnumerable<long> ids) where T : BaseEntity<T>
        {
            foreach (var id in ids)
            {
                await ValidateId(dao, id);
            }
        }

        public async Task ValidateId<T>(IBaseDao<T> dao, long id) where T : BaseEntity<T>
        {
            if (!await dao.ExistByIdAsync(id))
            {
                var invalidEntityIdException = new InvalidEntityIdException(id, "Entity with this id does not exist!");
                Logger.Error(invalidEntityIdException, "{type} with {id} does not exist!", typeof(T), id);
                throw invalidEntityIdException;
            }
        }

        public async Task ValidateUniqueId<T>(IBaseDao<T> dao, long id) where T : BaseEntity<T>
        {
            if (await dao.ExistByIdAsync(id))
            {
                var duplicateEntryException = new DuplicateEntryException(id, "Entity already exists!");
                Logger.Error(duplicateEntryException, "{type} with {id} already exists!", typeof(T), id);
                throw duplicateEntryException;
            }
        }

        public async Task ValidateNotDeletedById<T>(IBaseDao<T> dao, long id) where T : BaseEntity<T>
        {
            if (!await dao.ExistAndNotDeletedByIdAsync(id))
            {
                var invalidEntityIdException = new InvalidEntityIdException(id, "Entity with this id does not exist!");
                Logger.Error(invalidEntityIdException, "{type} with {id} does not exist!", typeof(T), id);
                throw invalidEntityIdException;
            }
        }

        public void ValidateCollection<T>(IEnumerable<T> collection)
        {
            if (!collection.Any())
            {
                var argumentException = new ArgumentException($"{nameof(ValidateCollection)} there must be at least one entity in the collection");
                Logger.Error(argumentException, "In {method} {type} was empty!", nameof(ValidateCollection), typeof(T));
                throw argumentException;
            }
        }

        public void ValidateDateRange(DateTime from, DateTime to)
        {
            if (from > to)
            {
                var argumentException = new ArgumentException($"{nameof(ValidateDateRange)} to can not be before from date");
                Logger.Error(argumentException, "In {method} {from} date was before {to} date!", nameof(ValidateDateRange), from, to);
                throw argumentException;
            }
        }

        public void ValidateNotNull<T>(T entity)
        {
            if (entity == null)
            {
                var argumentException = new ArgumentNullException(nameof(entity));
                Logger.Error(argumentException , "{Entity} of {type} was null!", nameof(entity), typeof(T));
                throw argumentException;
            }
        }
    }
}