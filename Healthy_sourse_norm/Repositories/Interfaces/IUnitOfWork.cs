namespace Healthy_sourse_norm.Repositories.Interfaces
{
    public interface IUnitOfWork
    {
        IWorkOutRepository _workoutRepository {  get; }
        IWorkOutTypeRepository _workoutTypeRepository { get; }
        ICategoryWorkOutRepository _categoryWorkOutRepository { get; }
        void Commit();
        void Dispose();
    }
}
