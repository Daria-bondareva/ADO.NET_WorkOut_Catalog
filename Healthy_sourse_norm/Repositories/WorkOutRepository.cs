using Dapper;
using Healthy_sourse_norm.Entities;
using Healthy_sourse_norm.Repositories.Interfaces;
using System.Data;
using System.Data.SqlClient;

namespace Healthy_sourse_norm.DAL.Repositories
{
    public class WorkOutRepository : GenericRepository<WorkOut>, IWorkOutRepository
    {
        public WorkOutRepository(SqlConnection sqlConnection, 
            IDbTransaction dbTransaction) : base(sqlConnection, dbTransaction, "WorkOut")
        {
        }

        public async Task<IEnumerable<WorkOut>> GetTopThreeWorkOutAsync()
        {
            string sql = @"SELECT TOP 3 * FROM WorkOut";
            var results = await _sqlConnection.QueryAsync<WorkOut>(sql,
                transaction: _dbTransaction);
            return results;
        }
    }
}
