using Healthy_sourse_norm.Repositories.Interfaces;
using System.Data;

namespace Healthy_sourse_norm.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        public IWorkOutRepository _workoutRepository { get; }

        public IWorkOutTypeRepository _workoutTypeRepository { get; }

        public ICategoryWorkOutRepository _categoryWorkOutRepository { get; }

        readonly IDbTransaction _dbTransaction;

        public UnitOfWork(IWorkOutRepository workoutRepository, 
            IWorkOutTypeRepository workouttypeRepository, 
            ICategoryWorkOutRepository categoryworkoutRepository, 
            IDbTransaction dbTransaction)
        {
            _workoutRepository = workoutRepository;
            _workoutTypeRepository = workouttypeRepository;
            _categoryWorkOutRepository = categoryworkoutRepository;
            _dbTransaction = dbTransaction;
        }

        public void Commit()
        {
            try
            {
                _dbTransaction.Commit();
            }
            catch (Exception ex)
            {
                _dbTransaction.Rollback();
            }
        }

        public void Dispose()
        {
            _dbTransaction.Connection?.Close();
            _dbTransaction.Connection?.Dispose();
            _dbTransaction.Dispose();
        }
    }
}
