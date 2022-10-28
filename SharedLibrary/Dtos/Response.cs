using System.Text.Json.Serialization;

namespace SharedLibrary.Dtos
{
    //Response modelimizi burada oluşturduk.
    public class Response<T> where T : class
    {
        public T Data { get; private set; }
        public int StatusCode { get; private set; }
        //Client tarafında açmayacağız. Kendi içimizde Apiler arasında olayın başarılı olup olmadığını kısa yoldan görmek için tanımladık. Json dataya serialize edildiğinde ignore edilecek. Client tarafına göstermemek kendi içimizde kullanmak için.
        [JsonIgnore]
        public bool IsSuccessful { get; private set; }
        public ErrorDto Error { get; private set; }

        //Static factory method tanımladık factory desing patternden gelir.(Nesne üretme yolu)
        public static Response<T> Success(T data,int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode, IsSuccessful = true };
        }

        public static Response<T> Success(int statusCode)
        {
            return new Response<T> { Data=default, StatusCode = statusCode, IsSuccessful = true };
        }

        public static Response<T> Fail(ErrorDto errorDto, int statusCode)
        {
            return new Response<T> { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }

        public static Response<T> Fail(string errorMessage, int statusCode,bool isShow)
        {
            var errorDto = new ErrorDto(errorMessage,isShow);
            return new Response<T> { Error = errorDto, StatusCode = statusCode, IsSuccessful = false };
        }





    }
}
