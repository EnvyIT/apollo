using System;
using System.Collections.Generic;
using System.Linq;
using Apollo.Core.Exception;
using Apollo.Util.Logger;

namespace Apollo.Util
{
    public static class ValidationHelper
    {
        public static void ValidateNull<TClassLogger, T>(IApolloLogger<TClassLogger> logger, T entity)
        {
            if (entity == null)
            {
                var argumentNullException = new ArgumentNullException(nameof(ValidateNull));
                logger.Error(argumentNullException, "{Entity} was null", typeof(T).Name);
                throw argumentNullException;
            }
        }

        public static void ValidateEnumerableIsNotNullOrEmpty<TClassLogger, T>(IApolloLogger<TClassLogger> logger, IEnumerable<T> enumerable, string memberName)
        {
            if (enumerable == null || !enumerable.Any())
            {
                var exception = new ArgumentException("Null or empty", memberName);
                logger.Error(exception, "{Enumerable} was null or empty", memberName);
                throw exception;
            }
        }

        public static void ValidateUpdate<TClassLogger, T>(IApolloLogger<TClassLogger> logger, int affected, int expected = 1)
        {
            if (affected != expected)
            {
                var persistenceException = new PersistenceException("Update failed!");
                logger.Error(persistenceException, "Update {entity} failed", typeof(T).Name);
                throw persistenceException;
            }
        }

        public static void ValidateId<TClassLogger, T>(IApolloLogger<TClassLogger> logger, long id)
        {
            if (id <= 0)
            {
                var persistenceException = new PersistenceException("Insert failed!");
                logger.Error(persistenceException, "Insert {Entity} failed", typeof(T).Name);
                throw persistenceException;
            }
        }

        public static void ValidateLessOrEquals<TClassLogger, T>(IApolloLogger<TClassLogger> logger,T actual, T upperBound) where T: IComparable<T>
        {
            if(actual.CompareTo(upperBound) == 1)
            {
                var persistenceException = new ArgumentOutOfRangeException(nameof(ValidateLessOrEquals), "Actual value is out of range");
                logger.Error(persistenceException, "Value not in range {actual} differs {expected}", typeof(T).Name);
                throw persistenceException;
            }
        }

    }
}