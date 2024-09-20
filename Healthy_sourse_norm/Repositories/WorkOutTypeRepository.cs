using Dapper;
using Healthy_sourse_norm.Entities;
using Healthy_sourse_norm.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Healthy_sourse_norm.DAL.Repositories
{
    public class WorkOutTypeRepository : GenericRepository<WorkOutType>, IWorkOutTypeRepository
    {
        public WorkOutTypeRepository(SqlConnection sqlConnection,
            IDbTransaction dbTransaction) : base(sqlConnection, dbTransaction, "WorkOutType")
        {
        }

        public async Task<WorkOutType> GetTypeByNameAsync(string name)
        {
            string sql = @"SELECT * FROM [WorkOutType] WHERE TypeName = @TypeName";

            var types = await _sqlConnection.QuerySingleOrDefaultAsync<WorkOutType>(sql,
                param: new { TypeName = name },
                transaction: _dbTransaction);
            return types;
        }
    }
}
