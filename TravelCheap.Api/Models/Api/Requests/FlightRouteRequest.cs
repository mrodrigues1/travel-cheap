namespace TravelCheap.Api.Models.Api.Requests;

public record FlightRouteRequest(int Id, string Origin, string Destination, int Cost);
