using AuthServer.Core.DTOs;
using SharedLibrary.Dtos;
using System.Threading.Tasks;

namespace AuthServer.Core.Services
{
    //Repository oluşturmadık çünkü Identity kütüphanesi ile beraber hazır metotlar geldiği için repository katmanına gerek yoktur. 3 tane önemli class gelir bu kütüphaneyle birlikte 1-Usermanager (Kullanıcı hakkındaki işlemler için) 2-Rolemanager (Kullanıcı rolleri üzerinde değişiklikler için) 3-SignInManager (Login-logout işlemleri için)
    public interface IUserService
    {
        Task<Response<UserAppDto>> CreateUserAsync(CreateUserDto createUserDto);
        Task<Response<UserAppDto>> GetUserByNameAsync(string userName);
        //Kullanıcıya rol tanımlamak için aşağıdaki metotu yazdık.
        Task<Response<NoDataDto>> CreateUserRolesAsync(string userName);

    }
}
