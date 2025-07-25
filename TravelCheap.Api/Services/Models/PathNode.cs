namespace TravelCheap.Api.Services.Models;

public class PathNode
{
    public string Airport { get; private set; }
    public int TotalCost { get; set; }
    public string? PreviousAirport { get; set; }
    public bool Visited { get; set; }

    public PathNode(string airport)
    {
        Airport = airport;
        TotalCost = Constants.TotalCostInvalidValue;
        PreviousAirport = null;
        Visited = false;
    }
}
