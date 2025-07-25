namespace TravelCheap.Api.Infrastructure.Exceptions;

public class NotFoundException : BusinessLogicException
{
    public NotFoundException(Type type, string argument, int id) : base($"{type.Name} not found with {argument}: {id}")
    {
    }

    public NotFoundException(Type type, string argument, string value) : base(
        $"{type.Name} not found with {argument}: {value}")
    {
    }
}
