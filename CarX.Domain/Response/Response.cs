using System.Net;

namespace CarX.Domain.Response;

public class Response<T>
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
    public bool Succeeded { get; set; }

    // Конструктор для УСПЕХА (передаем данные)
    public Response(T data)
    {
        StatusCode = 200;
        Succeeded = true;
        Data = data;
        Message = "Success";
    }

    // Конструктор для ОШИБКИ (передаем код и сообщение)
    public Response(HttpStatusCode statusCode, string message)
    {
        StatusCode = (int)statusCode;
        Succeeded = false;
        Message = message;
    }
    
    // Конструктор для списка ошибок (если валидация не прошла)
    public Response(HttpStatusCode statusCode, List<string> errors)
    {
        StatusCode = (int)statusCode;
        Succeeded = false;
        Message = string.Join(", ", errors);
    }
}