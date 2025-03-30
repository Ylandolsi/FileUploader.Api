namespace FileUploader.Api.Exceptions;

public class ApiException : Exception
{
    public int StatusCode { get; }

    public ApiException(string message, int statusCode = 500) 
        : base(message)
    {
        StatusCode = statusCode;
    }
}

public class NotFoundException : ApiException
{
    public NotFoundException(string message) 
        : base(message, 404)
    {
    }
}

public class BadRequestException : ApiException
{
    public BadRequestException(string message) 
        : base(message, 400)
    {
    }
}

public class UnauthorizedException : ApiException
{
    public UnauthorizedException(string message) 
        : base(message, 401)
    {
    }
}

public class InvalidOperationException : ApiException
{
    public InvalidOperationException(string message) 
        : base(message, 409)
    {
    }
}