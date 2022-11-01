using AutoMapper;
using System;

namespace AuthServer.Service.Mapping
{
    //Lazy loading sadece ihtiyaç halinde yüklenmesi için. Diğer türlü uygulama ayağa kalktığında ram üzerine nesne örneğini çıkartır.
    public static class ObjectMapper
    {
        private static readonly Lazy<IMapper> lazy = new Lazy<IMapper>(() =>
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MapProfile>();
            });

            return config.CreateMapper();
        });

        //Lazy loading yaptık ObjectMapper.Mapper kodu çağrıldığında yukarıdaki kod çalışarak ram üzerine çıkartılacak. Program ayağa kalktığında çıkartılmayacak.
        //public static IMapper Mapper { get { return lazy.Value; } } bunu yazmak yerine aşağıdaki gibi yazdık aynı ifade.
        public static IMapper Mapper => lazy.Value;


    }
}
