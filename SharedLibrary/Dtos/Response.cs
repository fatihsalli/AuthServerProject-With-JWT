using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharedLibrary.Dtos
{
    public class Response<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }
        public ErrorDto Error { get; set; }

        //Static factory method tanımladık factory desing patternden gelir.(Nesne üretme yolu)
        public static Response<T> Success(T data,int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode };
        }

        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data = default, StatusCode = statusCode };
        }

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T> { Error = errorDto, StatusCode = statusCode };
        }


    }
}
