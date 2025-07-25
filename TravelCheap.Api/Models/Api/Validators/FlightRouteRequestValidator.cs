using FluentValidation;
using TravelCheap.Api.Models.Api.Requests;

namespace TravelCheap.Api.Models.Api.Validators;

public class FlightRouteRequestValidator : AbstractValidator<FlightRouteRequest>
{
    public FlightRouteRequestValidator()
    {
        RuleFor(x => x.Cost)
            .GreaterThan(0)
            .WithMessage("Cost must be greater than 0");
        
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

        RuleFor(x => x.Cost)
            .GreaterThan(0)
            .WithMessage("Cost must be greater than 0")
            .LessThanOrEqualTo(100)
            .WithMessage("Cost cannot exceed 100");

        RuleFor(x => x)
            .Must(x => !string.Equals(x.Origin, x.Destination, StringComparison.OrdinalIgnoreCase))
            .WithMessage("Origin and Destination cannot be the same");
    }
}
