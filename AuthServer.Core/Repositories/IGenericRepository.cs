using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        Task<T> GetByIdAsync(int id);
        //Eğer data üzerinde where sorgusu ya da ilave sorgular yapsaydık IQueryable dönmek daha mantıklı. ToList diyene kadar databaseden gelen IQueryable tipteki listede istediğimiz sorguyu yazabiliriz. Ne zaman ki ToList dedik o zaman gidip databaseden o veriyi çekecektir.
        IQueryable<T> GetAll();
        //Where(x=> x.id>5) => buradaki x=> "T" tipine x.id>5 "bool" olarak aşağıda belirtilir.
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Remove(T entity);
        //_context.Entry(entity).State=EntityState.Modified => Remove veya update işlemi esnasında biz bu metotları çağırdığımızda sadece state'ini Modified veya Deleted olarak değiştirir. Database'e kaydetme işlemi ayrıca SaveChange ile yapıldığı için bu işlemler aşırı performans istemediğinden ötürü asenkronu yoktur.
        T Update(T entity);


    }
}
