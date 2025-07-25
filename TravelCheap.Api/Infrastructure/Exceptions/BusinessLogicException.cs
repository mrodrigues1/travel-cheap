﻿namespace TravelCheap.Api.Infrastructure.Exceptions;

public class BusinessLogicException : Exception
{
    public BusinessLogicException(string message) : base(message)
    {
    }

    public BusinessLogicException(string message, Exception innerException) : base(message, innerException)
    {
    }
}
