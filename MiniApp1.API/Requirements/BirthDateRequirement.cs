using Microsoft.AspNetCore.Authorization;
using System;
using System.Threading.Tasks;

namespace MiniApp1.API.Requirements
{
    //Policy tabanlı bir yetkilendirme yapmak için bu classı tanımladık."IAuthorizationRequirement"'den miras aldırdık. Örnek olması açısından yaş üzerinden bir policy tabanlı yetkilendirme oluşturacağız.
    public class BirthDateRequirement:IAuthorizationRequirement
    {
        public int Age { get; set; }
        public BirthDateRequirement(int age)
        {
            Age = age;
        }

        //Bir class daha oluşturarak "BirthDateRequirement" "AuthorizationHandler" classına Requirement olarak ekledik. Artık business codu yazdığımız kısım burasıdır.
        public class BirthDateRequirementHandler : AuthorizationHandler<BirthDateRequirement>
        {
            protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, BirthDateRequirement requirement)
            {
                var birthDate = context.User.FindFirst("birth-date");

                if (birthDate==null)
                {
                    //birthdate null olması durumunda fail olarak geriye dönecektir.
                    context.Fail();
                    //void değil task olduğu için bu şekilde döndük.
                    return Task.CompletedTask;
                }

                var today = DateTime.Now;
                var age = today.Year - Convert.ToDateTime(birthDate.Value).Year;

                //18>=18 istek yapabilir - 16>=18 istek yapamaz
                if (age>=requirement.Age)
                {
                    context.Succeed(requirement);
                }
                else
                {
                    context.Fail();
                }
                return Task.CompletedTask;
            }
        }



    }
}
