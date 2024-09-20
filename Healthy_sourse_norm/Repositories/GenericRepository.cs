using Healthy_sourse_norm.Repositories.Interfaces;
using System.Data.SqlClient;
using System.ComponentModel;
using System.Text;
using System.Data;
using Dapper;
using System.Reflection;

namespace Healthy_sourse_norm.DAL.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        
        protected SqlConnection _sqlConnection;
        protected IDbTransaction _dbTransaction;
        private readonly string _tableName;

        protected GenericRepository(SqlConnection sqlConnection, 
            IDbTransaction dbTransaction, string tableName)
        {
            _sqlConnection = sqlConnection;
            _dbTransaction = dbTransaction;
            _tableName = tableName;
        }

        public async Task<IEnumerable<T>> GetAllAsync() //+
        {
            return await _sqlConnection.QueryAsync<T>($"SELECT * FROM [{_tableName}]",
                transaction: _dbTransaction);
        }

        public async Task<T> GetByIdAsync(int id) //+
        {
            var result = await _sqlConnection.QuerySingleOrDefaultAsync<T>($"SELECT * FROM {_tableName} WHERE Id=@Id",
                param: new { Id = id },
                transaction: _dbTransaction);
            if (result == null)
                throw new KeyNotFoundException($"{_tableName} with id [{id}] could not be found.");
            return result;
        }

        public async Task DeleteAsync(int id) //+
        {
            await _sqlConnection.ExecuteAsync($"DELETE FROM [{_tableName}] WHERE Id=@Id",
                param: new { Id = id },
                transaction: _dbTransaction);
        }

        public async Task<int> AddAsync(T t) //+
        {
            var insertQuery = GenerateInsertQuery();
            var newId = await _sqlConnection.ExecuteScalarAsync<int>(insertQuery,
                param: t,
                transaction: _dbTransaction);
            return newId;
        }

        public async Task UpdateAsync(T t) //+
        {
            var updateQuery = GenerateUpdateQuery();
            await _sqlConnection.ExecuteAsync(updateQuery,
                param: t,
                transaction: _dbTransaction);
        }

        public async Task ReplaceAsync(T t) //+
        {
            var updateQuery = GenerateUpdateQuery();
            await _sqlConnection.ExecuteAsync(updateQuery,
                param: t,
                transaction: _dbTransaction);
        }

        //   //    //   //    //   //    //

        private IEnumerable<PropertyInfo> GetProperties => typeof(T).GetProperties();
        private static List<string> GenerateListOfProperties(IEnumerable<PropertyInfo> listOfProperties)
        {
            return (from prop in listOfProperties
                    let attributes = prop.GetCustomAttributes(typeof(DescriptionAttribute), false)
                    where attributes.Length <= 0 || (attributes[0] as DescriptionAttribute)?.Description != "ignore"
                    select prop.Name).ToList();
        } //два попередні - отримали список властивостей


        private string GenerateInsertQuery()
        {
            var insertQuery = new StringBuilder($"INSERT INTO {_tableName} ");
            insertQuery.Append("(");
            var properties = GenerateListOfProperties(GetProperties);
            properties.Remove("Id");
            //
            properties.ForEach(prop => { insertQuery.Append($"[{prop}],"); });
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(") VALUES (");

            properties.ForEach(prop => { insertQuery.Append($"@{prop},"); });
            insertQuery
                .Remove(insertQuery.Length - 1, 1)
                .Append(")");
            insertQuery.Append("; SELECT SCOPE_IDENTITY()");
            return insertQuery.ToString();
        }  //INSERT INTO [Persons] ([Name],[Age]) VALUES (@Name,@Age); SELECT SCOPE_IDENTITY()++поверне значення первинного ключа для нового запису++  - приклад отриманого запиту


        private string GenerateUpdateQuery()
        {
            var updateQuery = new StringBuilder($"UPDATE {_tableName} SET ");
            var properties = GenerateListOfProperties(GetProperties);
            properties.ForEach(property =>
            {
                if (!property.Equals("Id"))
                {
                    updateQuery.Append($"{property}=@{property},");
                }
            });
            updateQuery.Remove(updateQuery.Length - 1, 1); 
            updateQuery.Append(" WHERE Id=@Id");
            return updateQuery.ToString();
        } // UPDATE [Persons] SET [Name]=@Name, [Age]=@Age WHERE Id=@Id

    }
}