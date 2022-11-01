using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        //Sonrasında ProductRepository vs. oluşturduğumda context'e ulaşabilmek için protected tanımladım.
        protected readonly AppDbContext _context;
        private readonly DbSet<T> _dbSet;
        public GenericRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsNoTracking().AsQueryable();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            //Find primary key bölümünü arar. Metotu çağırdığımızda "params object [] keyValues" olarak ifade yer alır bunun nedeni Sql tarafında bir tabloda 2 alan primary key olabilir.
            var entity = await _dbSet.FindAsync(id);
            if (entity != null)
            {
                //Memory'de tutulmaması için track edilmesini kapattık. Burada kapadık Update methodunda zaten biz veriyoruz track edilirken aynı id olan modeller karışmaması için yaptık.
                _context.Entry(entity).State = EntityState.Detached;
            }
            return entity;
        }

        public void Remove(T entity)
        {
            //_context.Entry(entity).State = EntityState.Deleted; (Alternatif)
            _dbSet.Remove(entity);
        }

        public T Update(T entity)
        {
            //_context.Entry(entity).State = EntityState.Modified; (Alternatif)
            _dbSet.Update(entity);
            return entity;
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }
    }
}
