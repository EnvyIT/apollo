using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.Dao;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.FluentEntity.Interfaces.Update;
using Apollo.Persistence.FluentEntity.Types;

namespace Apollo.Persistence.FluentEntity.Ado
{
    public class FluentEntityAdoUpdate<T> : IFluentEntityUpdate<T> where T : BaseEntity<T>, new()
    {
        private const string SetCommand = "SET";
        private const string UpdateCommand = "UPDATE";
        private const string WhereCommand = "WHERE";
        private const string LastUpdate = "lastUpdate";
        private const string Id = "id";

        private const int Offset = 2;

        private readonly IDaoHelper _daoHelper;
        private readonly IDictionary<string, IList<IList<QueryParameter>>> _commands;
        private readonly List<FluentEntityProperty<EntityColumnAttribute>> _properties;

        public FluentEntityAdoUpdate(IDaoHelper daoHelper, T value) : this(daoHelper, new List<T> { value })
        {
        }

        public FluentEntityAdoUpdate(IDaoHelper daoHelper, IEnumerable<T> entities)
        {
            _daoHelper = daoHelper ?? throw new ArgumentNullException(nameof(daoHelper));
            _properties = FluentEntityAdoHelper.GetAllColumnProperties(typeof(T)).ToList();
            _commands = new Dictionary<string, IList<IList<QueryParameter>>>
            {
                {PrepareBaseCommand(), PrepareQueryParameters(entities)}
            };
        }

        private string PrepareBaseCommand()
        {
            var commandBuilder = new StringBuilder();
            var tableName = FluentEntityAdoHelper.GetTableName(typeof(T));
            commandBuilder.Append($"{UpdateCommand} {tableName} {SetCommand} ");
            _properties.Perform(property => commandBuilder.Append($"{property.Attribute.Name} = @{property.Attribute.Name}, "), IsNotKey);
            commandBuilder.Remove(commandBuilder.Length - Offset, Offset);
            commandBuilder.Append($" {WhereCommand} {tableName}.id = @{Id} AND {tableName}.row_version = @{LastUpdate}");
            return commandBuilder.ToString();
        }

        private static bool IsNotKey(FluentEntityProperty<EntityColumnAttribute> property)
        {
            return !property.Attribute.IsKey;
        }

        
        private IList<IList<QueryParameter>> PrepareQueryParameters(IEnumerable<T> entities)
        {
            return entities.Select(MapQueryParams).ToList();
        }

        private IList<QueryParameter> MapQueryParams(T entity)
        {
            var queryParams = new List<QueryParameter>(_properties.Capacity);
            _properties.Perform(property =>
            {
                var value = FluentEntityAdoHelper.GetValue(property, entity);
                queryParams.Add(new QueryParameter(property.Attribute.Name, value));
            });
            queryParams.Add(new QueryParameter(LastUpdate, entity.RowVersion));
            return queryParams;
        }

        public async Task<int> ExecuteAsync()
        {
            return await _daoHelper.ExecuteAsync<T>(_commands);
        }

        Task ICallExecute.ExecuteAsync()
        {
            return ExecuteAsync();
        }
    }
}
