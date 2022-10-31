using SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //Metotlar aynı olmasına rağmen dönüş tipleri farklı olduğu için ServiceGeneric ve GenericRepository oluşturuyoruz.
    public interface IServiceGeneric<TEntity,TDto> where TEntity:class where TDto : class
    {
        //Client tarafına dönüş yapacağımız için Response olarak direkt dönüyoruz. Bunu Apide yapmak yerine Service tarafında yaptık.
        Task<Response<TDto>> GetByIdAsync(int id);
        Task<Response<IEnumerable<TDto>>> GetAllAsync();
        Task <Response<IEnumerable<TDto>>> WhereAsync(Expression<Func<TEntity, bool>> predicate);
        Task<Response<TDto>> AddAsync(TDto dto);
        Task<Response<NoDataDto>> RemoveAsync(int id);
        Task<Response<NoDataDto>> UpdateAsync(TDto dto);


    }
}
