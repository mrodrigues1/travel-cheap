using TravelCheap.Api.Infrastructure.Exceptions;
using TravelCheap.Api.Models.Api.Responses;
using TravelCheap.Api.Models.Entities;
using TravelCheap.Api.Services.Models;

namespace TravelCheap.Api.Services;

public interface IRouteCalculator
{
    GetCheapestRouteResponse GetShortestRoute(List<FlightRoute> routes, string origin, string destination);
}

public class RouteCalculator : IRouteCalculator
{
    private Dictionary<string, List<FlightRoute>> _graph;

    public RouteCalculator()
    {
        _graph = new Dictionary<string, List<FlightRoute>>();
    }

    private void AddRoutes(List<FlightRoute> routes)
    {
        foreach (var route in routes)
        {
            if (!_graph.ContainsKey(route.Origin))
            {
                _graph[route.Origin] = [];
            }

            _graph[route.Origin].Add(route);
        }
    }

    public GetCheapestRouteResponse GetShortestRoute(
        List<FlightRoute> routes,
        string origin,
        string destination)
    {
        AddRoutes(routes);

        var originalAirportExists = _graph.ContainsKey(origin);

        if (originalAirportExists is false)
        {
            throw new NotFoundException(typeof(FlightRoute), nameof(FlightRoute.Origin), origin);
        }

        var nodes = new Dictionary<string, PathNode>();

        CreateNodesForAllOriginAirports(nodes);

        AddNodesForOnlyDestinationAirports(nodes);

        var destinationAirportExists = nodes.ContainsKey(destination);

        if (destinationAirportExists is false)
        {
            throw new NotFoundException(typeof(FlightRoute), nameof(FlightRoute.Destination), destination);
        }

        // Set starting point: cost to reach origin is 0
        nodes[origin].TotalCost = 0;

        while (true)
        {
            var currentNode = FindCheapestUnvisitedNode(nodes);

            if (currentNode?.TotalCost == Constants.TotalCostInvalidValue)
            {
                break;
            }

            currentNode.Visited = true;

            var destinationReached = currentNode.Airport == destination;

            if (destinationReached)
            {
                break;
            }

            UpdateNeighborAirportCosts(currentNode, nodes);
        }

        var destinationIsUnreachable = nodes[destination].TotalCost == Constants.TotalCostInvalidValue;

        if (destinationIsUnreachable)
        {
            throw new BusinessLogicException($"No route found from {origin} to {destination}");
        }

        var path = CreateTravelPath(destination, nodes);

        string routeDescription = string.Join(" - ", path);
        int totalCost = nodes[destination].TotalCost;

        return new GetCheapestRouteResponse(routeDescription, totalCost);
    }

    private void CreateNodesForAllOriginAirports(Dictionary<string, PathNode> nodes)
    {
        foreach (var airport in _graph.Keys)
        {
            nodes[airport] = new PathNode(airport);
        }
    }

    private void AddNodesForOnlyDestinationAirports(Dictionary<string, PathNode> nodes)
    {
        foreach (var routes in _graph.Values)
        {
            foreach (var route in routes)
            {
                if (!nodes.ContainsKey(route.Destination))
                {
                    nodes[route.Destination] = new PathNode(route.Destination);
                }
            }
        }
    }

    private static PathNode? FindCheapestUnvisitedNode(Dictionary<string, PathNode> nodes)
    {
        PathNode currentNode = null;

        foreach (var node in nodes.Values)
        {
            if (node.Visited is false && (currentNode is null || node.TotalCost < currentNode.TotalCost))
            {
                currentNode = node;
            }
        }

        return currentNode;
    }

    private void UpdateNeighborAirportCosts(PathNode currentNode, Dictionary<string, PathNode> nodes)
    {
        if (_graph.TryGetValue(currentNode.Airport, out var airport))
        {
            foreach (var route in airport)
            {
                var neighbor = nodes[route.Destination];

                var cost = currentNode.TotalCost + route.Cost;

                if (cost < neighbor.TotalCost)
                {
                    neighbor.TotalCost = cost;
                    neighbor.PreviousAirport = currentNode.Airport;
                }
            }
        }
    }

    private static List<string> CreateTravelPath(string destination, Dictionary<string, PathNode> nodes)
    {
        var path = new List<string>();
        string current = destination;

        while (current != null)
        {
            path.Add(current);
            current = nodes[current].PreviousAirport;
        }

        path.Reverse();

        return path;
    }
}
