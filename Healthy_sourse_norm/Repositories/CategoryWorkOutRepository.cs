using Dapper;
using Healthy_sourse_norm.Entities;
using Healthy_sourse_norm.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Healthy_sourse_norm.DAL.Repositories
{
    public class CategoryWorkOutRepository : GenericRepository<Category_WorkOut>, ICategoryWorkOutRepository
    {
        public CategoryWorkOutRepository(SqlConnection sqlConnection, 
            IDbTransaction dbTransaction) : base(sqlConnection, dbTransaction, "Category_WorkOut")
        {
        }

        public async Task<IEnumerable<Category_WorkOut>> GetCategoryByNameAsync(string name)
        {
            string sql = @"SELECT * FROM [Category_WorkOut] WHERE CategoryName = @CategoryName";

            var categories = await _sqlConnection.QueryAsync<Category_WorkOut>(sql,
                param: new { CategoryName = name },
                transaction: _dbTransaction);
            return categories;
        }
    }
}
