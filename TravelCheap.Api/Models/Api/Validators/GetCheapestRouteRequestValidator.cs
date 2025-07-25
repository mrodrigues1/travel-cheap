using FluentValidation;
using TravelCheap.Api.Models.Api.Requests;

namespace TravelCheap.Api.Models.Api.Validators;

public class GetCheapestRouteRequestValidator : AbstractValidator<GetCheapestRouteRequest>
{
    public GetCheapestRouteRequestValidator()
    {
        RuleFor(x => x.Origin)
            .NotEmpty()
            .WithMessage("Origin is required")
            .Length(3)
            .WithMessage("Origin must be 3 characters long");

        RuleFor(x => x.Destination)
            .NotEmpty()
            .WithMessage("Destination is required")
            .Length(3)
            .WithMessage("Destination must be 3 characters long");

        RuleFor(x => x)
            .Must(x => !string.Equals(x.Origin, x.Destination, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Origin and Destination cannot be the same");
    }
}
