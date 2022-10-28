using AuthServer.Core.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        //Sonrasında ProductRepository vs. oluşturduğumda context'e ulaşabilmek için protected tanımladık.
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

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        public async Task<T> GetByIdAsync(int id)
        {
            //Find primary key bölümünü arar. Metotu çağırdığımızda "params object [] keyValues" olarak ifade yer alır bunun nedeni Sql tarafında bir tabloda 2 alan primary key olabilir.
            var entity= await _dbSet.FindAsync(id);
            if (entity!=null)
            {
                //Memory'de tutulmaması için track edilmesini kapattık.
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
