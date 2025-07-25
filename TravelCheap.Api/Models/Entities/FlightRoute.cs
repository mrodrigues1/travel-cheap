namespace TravelCheap.Api.Models.Entities;

public class FlightRoute
{
    public int Id { get; private set; }
    public string Origin { get; private set; }
    public string Destination { get; private set; }
    public int Cost { get; private set; }

    public FlightRoute(int id, string origin, string destination, int cost)
    {
        Id = id;
        Origin = origin;
        Destination = destination;
        Cost = cost;
    }
}
