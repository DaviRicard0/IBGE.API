using System.Net;

namespace IBGE.API.Domain.DTOs.Common;

public class ResponseDto
{
    public HttpStatusCode? Status { get; set; }
    public string? Message { get; set; }
    public object? Data { get; set; }
}
