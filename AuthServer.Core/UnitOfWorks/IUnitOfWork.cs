using System.Threading.Tasks;

namespace AuthServer.Core.UnitOfWorks
{
    public interface IUnitOfWork
    {
        void Commit();
        Task CommitAsync();

    }
}
