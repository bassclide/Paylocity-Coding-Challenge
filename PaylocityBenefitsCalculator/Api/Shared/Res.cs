using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Api.Errors;
using Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Api.Shared;

public class Res
{
    protected Res()
    {
    }

    private Res(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    [MemberNotNullWhen(false, nameof(Error))]
    public bool IsSuccess { get; protected init; }

    public Error? Error { get; protected init; }

    public static implicit operator bool(Res res)
    {
        return res.IsSuccess;
    }

    public static implicit operator Res(Error error)
    {
        return new Res(error);
    }

    public static Res Success()
    {
        return new Res
        {
            IsSuccess = true
        };
    }

    public static Res Failure(Error error)
    {
        return new Res
        {
            IsSuccess = false,
            Error = error,
        };
    }
}

public sealed class Res<T> : Res
{
    private Res()
    {
    }

    public T Entity { get; private init; } = default!;

    public static implicit operator bool(Res<T> res)
    {
        return res.IsSuccess;
    }

    public static implicit operator T(Res<T> res)
    {
        Debug.Assert(res.IsSuccess);
        Debug.Assert(res.Entity is not null);

        return res.Entity;
    }

    public static implicit operator Res<T>(T obj)
    {
        return new Res<T>
        {
            IsSuccess = true,
            Entity = obj
        };
    }

    public static implicit operator Res<T>(Error error)
    {
        return new Res<T>
        {
            IsSuccess = false,
            Error = error
        };
    }
}

public static class Extensions
{
    public static ActionResult ToErrorResponse(this Res res)
    {
        if (!res.IsSuccess)
        {
            return res.Error.Reason switch
            {
                ErrorReason.Conflict => new ConflictResult(),
                ErrorReason.NotFound => new NotFoundResult(),
                _ => throw new InvalidOperationException()
            };
        }

        throw new InvalidOperationException();
    }

    public static ActionResult<ApiResponse<TResponse>> ToErrorResponse<TResponse>(this Res res)
    {
        if (!res.IsSuccess)
        {
            return res.Error.Reason switch
            {
                ErrorReason.Conflict => CreateObjectResult(StatusCodes.Status409Conflict),
                ErrorReason.NotFound => CreateObjectResult(StatusCodes.Status404NotFound),
                _ => throw new InvalidOperationException()
            };
        }

        throw new InvalidOperationException();

        ObjectResult CreateObjectResult(int statusCode)
        {
            var errorResult = new ObjectResult(new ApiResponse<TResponse>
            {
                Error = res.Error.ToString()!,
                Success = false,
                Message = res.Error.Message
            })
            {
                StatusCode = statusCode
            };
            return errorResult;
        }
    }
}