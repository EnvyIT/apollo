using Apollo.Persistence.FluentEntity.Interfaces;
using Apollo.Persistence.FluentEntity.Types;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Apollo.Persistence.Attributes.Base;
using Apollo.Persistence.FluentEntity.Interfaces.Select;
using System.Text;
using Apollo.Persistence.Attributes.Attributes;
using Apollo.Persistence.Dao.Interfaces;

namespace Apollo.Persistence.FluentEntity.Ado
{
    public class FluentEntityAdoSelect<T> :
        IFluentEntitySelectRequired<T>,
        IFluentEntityWhereSelect<T>,
        IFluentEntityWhereConditionSelect<T>,
        IFluentEntityWhereConjunctionSelect<T>,
        IFluentEntityJoin<T>,
        IFluentEntityOrder<T>,
        IFluentEntityOrderBy<T>,
        IFluentEntityLimit<T>,
        IFluentEntitySelect<T>,
        IFluentEntityCall<T> where T : BaseEntity<T>, new()
    {
        private readonly string _tableName;
        private readonly HashSet<string> _tableNames;
        private readonly Dictionary<string, List<FluentEntityProperty<EntityColumnAttribute>>> _selectedColumns;
        private readonly Dictionary<string, object> _joinedTableObjects;

        private readonly StringBuilder _joinCommandBuilder;
        private readonly StringBuilder _orderCommandBuilder;
        private readonly FluentEntityAdoWhereHelper<T> _whereHelper;
        private readonly IDaoHelper _daoHelper;

        private readonly bool _allColumns;
        private readonly bool _autoReset;

        private T _referenceModel;
        private bool _isFirstOrder;

        public FluentEntityAdoSelect(IDaoHelper daoHelper, bool allColumns, bool autoReset)
        {
            _daoHelper = daoHelper ?? throw new ArgumentNullException(nameof(daoHelper));
            _whereHelper = new FluentEntityAdoWhereHelper<T>(autoReset);
            _joinCommandBuilder = new StringBuilder();
            _orderCommandBuilder = new StringBuilder();

            _tableName = FluentEntityAdoHelper.GetTableName(typeof(T));
            _tableNames = new HashSet<string>
            {
                _tableName
            };
            _selectedColumns = new Dictionary<string, List<FluentEntityProperty<EntityColumnAttribute>>>
            {
                { _tableName, new List<FluentEntityProperty<EntityColumnAttribute>>() }
            };
            _joinedTableObjects = new Dictionary<string, object>();

            _referenceModel = new T();
            _allColumns = allColumns;
            _autoReset = autoReset;

            if (allColumns)
            {
                AddAllColumns<T>(_tableName);
            }
        }

        protected  void TableJoined<TJoined>(string tableSource, string tableDestination,
            FluentEntityProperty<EntityColumnRefAttribute> joinedProperty,
            object emptyObject)
        {
            // Create linking.
            var isBase = tableSource.Equals(_tableName);
            var propertyModel = isBase ? _referenceModel : _joinedTableObjects[tableSource];
            joinedProperty?.Property.SetValue(propertyModel, emptyObject);

            // Check if all properties
            if (_allColumns)
            {
                AddAllColumns<TJoined>(tableDestination);
            }

            // Add data
            _tableNames.Add(tableDestination);
            _joinedTableObjects.Add(tableDestination, emptyObject);
        }

        public IFluentEntitySelect<T> Column<TKey>(Expression<Func<T, TKey>> key)
        {
            return AddColumn(_tableName, key);
        }

        public IFluentEntitySelect<T> ColumnRef<TRef, TKey>(Expression<Func<TRef, TKey>> key)
        {
            var tableName = FluentEntityAdoHelper.GetTableName(typeof(TRef));            
            return AddColumn(tableName, key);
        }

        public IFluentEntityWhereConditionSelect<T> Where<TKey>(Expression<Func<T, TKey>> key)
        {
            _whereHelper.Where(key);
            return this;
        }

        public IFluentEntityWhereConjunctionSelect<T> WhereActive()
        {
            _whereHelper.Where(entity => entity.Deleted);
            _whereHelper.Equal(false);
            return this;
        }

        public IFluentEntityWhereConjunctionSelect<T> Equal<TKey>(TKey key)
        {
            _whereHelper.Equal(key);
            return this;
        }

        public IFluentEntityWhereConjunctionSelect<T> GreaterThan<TKey>(TKey key)
        {
            _whereHelper.GreaterThan(key);
            return this;
        }

        public IFluentEntityWhereConjunctionSelect<T> LowerThan<TKey>(TKey key)
        {
            _whereHelper.LowerThan(key);
            return this;
        }

        public IFluentEntityWhereConjunctionSelect<T> GreaterThanEquals<TKey>(TKey key)
        {
            _whereHelper.GreaterThanEquals(key);
            return this;
        }

        public IFluentEntityWhereConjunctionSelect<T> LowerThanEquals<TKey>(TKey key)
        {
            _whereHelper.LowerThanEquals(key);
            return this;
        }

        public IFluentEntityWhereConjunctionSelect<T> StartsWith<TKey>(TKey key)
        {
            _whereHelper.StartsWith(key);
            return this;
        }

        public IFluentEntityWhereConjunctionSelect<T> In<TKey>(IEnumerable<TKey> keys)
        {
            _whereHelper.In(keys);
            return this;
        }

        public IFluentEntityWhereConjunctionSelect<T> NotIn<TKey>(IEnumerable<TKey> keys)
        {
            _whereHelper.NotIn(keys);
            return this;
        }

        public IFluentEntityWhereConditionSelect<T> And<TKey>(Expression<Func<T, TKey>> key)
        {
            _whereHelper.And(key);
            return this;
        }

        public IFluentEntityWhereConditionSelect<T> Or<TKey>(Expression<Func<T, TKey>> key)
        {
            _whereHelper.Or(key);
            return this;
        }

        public async Task<IEnumerable<T>> QueryAsync()
        {
            var columns = FluentEntityAdoHelper.JoinEnumerable(
                _selectedColumns.SelectMany(keyValue =>
                    keyValue.Value.Select(value =>
                        GetColumnNameWithAs(keyValue.Key, value.Attribute.Name))),
                ", ");
            var sqlCommand = $"SELECT {columns} FROM {_tableName} {_joinCommandBuilder} {_whereHelper.CommandWhere} {_orderCommandBuilder}";
            var result =  await _daoHelper.SelectAsync<T>(sqlCommand, MapObject);
            
            if (_autoReset)
            {
                Reset();
            }

            return result;
        }

        public  async Task<T> QuerySingleAsync()
        {
            return (await QueryAsync()).SingleOrDefault();
        }

        private void Reset()
        {
            _joinCommandBuilder.Clear();
            _orderCommandBuilder.Clear();

            _tableNames.Clear();
            _tableNames.Add(_tableName);

            _selectedColumns.Clear();
            _joinedTableObjects.Clear();
            
            _referenceModel = new T();
        }
        
        private T MapObject(IDataRecord record)
        {
            foreach (var tableName in _tableNames)
            {
                var currentModel = _joinedTableObjects.ContainsKey(tableName) ?
                    _joinedTableObjects[tableName] :
                    _referenceModel;

                if (_selectedColumns.ContainsKey(tableName))
                {
                    foreach (var column in _selectedColumns[tableName])
                    {
                        var field = FluentEntityAdoHelper.ConcatTableColumnAs(tableName, column.Attribute.Name);
                            var value = record[field];
                            column.Property.SetValue(currentModel, ConvertDbValue(value, column));
                    }
                }
            }

            return (T)_referenceModel.Clone();
        }

        private static object ConvertDbValue(object value, FluentEntityProperty<EntityColumnAttribute> column)
        {
            if (value == DBNull.Value)
            {
                value = null;
            } else if (FluentEntityAdoHelper.IsBoolean(column))
            {
                value = Convert.ToBoolean(value); //we need to ensure that uint64 is properly converted to bool before we apply it to the model 
            }
            return value;
        }


        private IFluentEntitySelect<T> AddColumn<TRef, TKey>(string tableName, Expression<Func<TRef, TKey>> key)
        {
            AddColumnData(tableName, FluentEntityAdoHelper.GetColumnProperty(key));
            return this;
        }

        private void AddAllColumns<TColumn>(string tableName)
        {
            foreach (var entry in FluentEntityAdoHelper.GetAllColumnProperties(typeof(TColumn)))
            {
                AddColumnData(tableName, entry);
            }
        }

        private void AddColumnData(string tableName, FluentEntityProperty<EntityColumnAttribute> data)
        {
            if (!_selectedColumns.ContainsKey(tableName))
            {
                _selectedColumns.Add(tableName, new List<FluentEntityProperty<EntityColumnAttribute>>());
            }
            _selectedColumns[tableName].Add(data);
        }

        private string GetColumnNameWithAs(string source, string key)
        {
            var name = FluentEntityAdoHelper.ConcatTableColumn(source, key);
            var nameAs = FluentEntityAdoHelper.ConcatTableColumnAs(source, key);
            return $"{name} AS {nameAs}";
        }

        public IFluentEntityCall<T> Limit(long limit, long offset = 0)
        {
            _orderCommandBuilder.Append($" LIMIT {offset}, {limit}");
            return this;
        }

        public IFluentEntityLimit<T> Offset(int pageSize)
        {
            _orderCommandBuilder.Append($" OFFSET {pageSize}");
            return this;
        }

        public IFluentEntityJoin<T> InnerJoin<TJoin1, TJoin2, TKey1, TKey2>(
            Expression<Func<T, TJoin2>> property,
            Expression<Func<TJoin1, TKey1>> onColumn1,
            Expression<Func<TJoin2, TKey2>> onColumn2) where TJoin2 : new()
        {
            return GenericJoin(property, onColumn1, onColumn2, "INNER JOIN");
        }

        public IFluentEntityOrderBy<T> Order()
        {
            _isFirstOrder = true;
            _orderCommandBuilder.Append(" ORDER BY");
            return this;
        }

        public IFluentEntityOrderBy<T> ByAscending<TKey>(Expression<Func<T, TKey>> key)
        {
            return AddOrderBy(key, "ASC");
        }

        public IFluentEntityOrderBy<T> ByDescending<TKey>(Expression<Func<T, TKey>> key)
        {
            return AddOrderBy(key, "DESC");
        }

        private IFluentEntityJoin<T> GenericJoin<TJoin1, TJoin2, TKey1, TKey2>(
            Expression<Func<T, TJoin2>> property,
            Expression<Func<TJoin1, TKey1>> onColumn1,
            Expression<Func<TJoin2, TKey2>> onColumn2,
            string joinCommand) where TJoin2 : new()
        {
            FluentEntityProperty<EntityColumnRefAttribute> joinedProperty = null;
            if (property != null)
            {
                joinedProperty = FluentEntityAdoHelper.GetColumnRefProperty(property);
            }
            var tableSource = FluentEntityAdoHelper.GetTableName(typeof(TJoin1));
            var tableDestination = FluentEntityAdoHelper.GetTableName(typeof(TJoin2));
            var propertySource = FluentEntityAdoHelper.ConcatTableColumn(tableSource, FluentEntityAdoHelper.GetColumnPropertyName(onColumn1));
            var propertyDestination = FluentEntityAdoHelper.ConcatTableColumn(tableDestination, FluentEntityAdoHelper.GetColumnPropertyName(onColumn2));

            _joinCommandBuilder.Append($" {joinCommand} {tableDestination} ON {propertySource} = {propertyDestination}");
            TableJoined<TJoin2>(tableSource, tableDestination, joinedProperty, new TJoin2());

            return this;
        }

        private IFluentEntityOrderBy<T> AddOrderBy<TKey>(Expression<Func<T, TKey>> key, string order)
        {
            if (!_isFirstOrder)
            {
                _orderCommandBuilder.Append(" ,");
            }
            _isFirstOrder = false;

            _orderCommandBuilder.Append($" {FluentEntityAdoHelper.GetColumnPropertyNameQualified(key)} {order}");
            return this;
        }

    }
}
