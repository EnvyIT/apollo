using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.Dao;
using Apollo.Persistence.Dao.Interfaces;
using Apollo.Persistence.FluentEntity.Interfaces.Insert;
using Apollo.Persistence.FluentEntity.Interfaces.Shared;
using Apollo.Persistence.FluentEntity.Types;
using Microsoft.VisualBasic.CompilerServices;

namespace Apollo.Persistence.FluentEntity.Ado
{
    public class FluentEntityAdoInsert<T> :
        IFluentEntityInsert
        where T : BaseEntity<T>, new()
    {
        private readonly IDaoHelper _daoHelper;
        private readonly StringBuilder _commandBuilder;
        private readonly IDictionary<string, IList<IList<QueryParameter>>> _inserts;
        private readonly string _baseCommand;
        private readonly IList<FluentEntityProperty<EntityColumnAttribute>> _properties;

        public FluentEntityAdoInsert(IDaoHelper daoHelper, T value) : this(daoHelper, new List<T> { value })
        {
        }

        public FluentEntityAdoInsert(IDaoHelper daoHelper, IEnumerable<T> entities)
        {
            _daoHelper = daoHelper ?? throw new ArgumentNullException(nameof(daoHelper));
             _properties = FluentEntityAdoHelper.GetAllColumnProperties(typeof(T)).ToList();
            _commandBuilder = new StringBuilder();
            _inserts = new Dictionary<string, IList<IList<QueryParameter>>>();
            _baseCommand = $"INSERT INTO {FluentEntityAdoHelper.GetTableName(typeof(T))} (";
            ParseCommands(entities);
        }

        private void ParseCommands(IEnumerable<T> entities)
        {
            foreach (var entity in entities)
            {
                var commandKey = GetCommand(entity);
                if (_inserts.ContainsKey(commandKey))
                {
                    _inserts.TryGetValue(commandKey, out var commands);
                    commands?.Add(GetQueryParams(entity));
                }
                else
                {
                    _inserts.Add(commandKey, new List<IList<QueryParameter>>{GetQueryParams(entity)});
                }
            }
        }

        private string GetCommand(object entity)
        {
            _commandBuilder.Clear();
            _commandBuilder.Append(_baseCommand);
            _properties.Perform(AppendPropertyName, PostAppend, CanAppend, entity);
            _commandBuilder.Append("VALUES(");
            _properties.Perform(AppendPropertyNamePlaceholder, PostAppend, CanAppend, entity);
            return _commandBuilder.ToString();
        }

        public void AppendPropertyName(FluentEntityProperty<EntityColumnAttribute> property)
        {
            _commandBuilder.Append($"{property.Attribute.Name},");
        }

        public void AppendPropertyNamePlaceholder(FluentEntityProperty<EntityColumnAttribute> property)
        {
            _commandBuilder.Append($"@{property.Attribute.Name},");
        }

        public void PostAppend()
        {
            _commandBuilder.Remove(_commandBuilder.Length - 1, 1);
            _commandBuilder.Append(")");
        }

        private static bool CanAppend(FluentEntityProperty<EntityColumnAttribute> property, object entity)
        {
            var id = 0L;
            if (property.Attribute.IsKey)
            {
                id = LongType.FromObject(property.Property.GetValue(entity) ?? 0L);
            }
            return !property.Attribute.IsKey || id != 0L;
        }

        private IList<QueryParameter> GetQueryParams(object entity)
        {
            
            var queryParams = new List<QueryParameter>(_properties.Count);
            _properties.Perform(property =>
            {
                var value = FluentEntityAdoHelper.GetValue(property, entity);
                queryParams.Add(new QueryParameter(property.Attribute.Name, value));
            }, PostAppend, CanAppend, entity);
            return queryParams;
        }



        public async Task<IEnumerable<long>> ExecuteAsync()
        {
            return await _daoHelper.ExecuteInsertAsync<T>(_inserts);
        }

        Task ICallExecute.ExecuteAsync()
        {
            return ExecuteAsync();
        }
    }
}
