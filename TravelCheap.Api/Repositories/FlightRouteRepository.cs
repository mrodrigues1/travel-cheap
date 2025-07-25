using System.Text.Json;
using TravelCheap.Api.Infrastructure.Exceptions;
using TravelCheap.Api.Models.Api.Requests;
using TravelCheap.Api.Models.Entities;

namespace TravelCheap.Api.Repositories;

public interface IFlightRouteRepository
{
    Task<FlightRoute> GetAsync(int id, CancellationToken cancellationToken = default);
    Task<List<FlightRoute>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<List<FlightRoute>> CreateAsync(
        FlightRouteRequest request,
        CancellationToken cancellationToken = default);

    Task<FlightRoute> UpdateAsync(FlightRouteRequest request, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

public class FlightRouteRepository : IFlightRouteRepository
{
    private string _filePath;

    public FlightRouteRepository()
    {
        _filePath = Path.Combine(AppContext.BaseDirectory, "Infrastructure\\Data\\Flight_Routes.txt");
    }

    public async Task<FlightRoute> GetAsync(int id, CancellationToken cancellationToken = default)
    {
        var routes = await GetAllAsync(cancellationToken);

        var route = routes.FirstOrDefault(x => x.Id == id);

        if (route is null)
        {
            throw new NotFoundException(typeof(FlightRoute), nameof(id), id);
        }

        return route;
    }


    public async Task<List<FlightRoute>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var flightRoutes = await LoadFlightRoutesFromFileAsync(cancellationToken);

            return flightRoutes ?? [];
        }
        catch (FileNotFoundException)
        {
            return [];
        }
        catch (JsonException)
        {
            return [];
        }
    }

    private async Task<List<FlightRoute>?> LoadFlightRoutesFromFileAsync(CancellationToken cancellationToken)
    {
        var jsonContent = await File.ReadAllTextAsync(_filePath, cancellationToken);

        var flightRoutes = JsonSerializer.Deserialize<List<FlightRoute>>(
            jsonContent,
            new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

        return flightRoutes;
    }

    public async Task<List<FlightRoute>> CreateAsync(
        FlightRouteRequest request,
        CancellationToken cancellationToken = default)
    {
        var routes = await GetAllAsync(cancellationToken);

        if (routes.Any(x => x.Id == request.Id))
        {
            throw new BusinessLogicException($"Route with id {request.Id} already exists.");
        }

        var newRoute = new FlightRoute(
            request.Id,
            request.Origin,
            request.Destination,
            request.Cost);

        routes.Add(newRoute);

        await SaveFlightRoutesToFile(routes, cancellationToken);

        return routes;
    }

    public async Task<FlightRoute> UpdateAsync(
        FlightRouteRequest request,
        CancellationToken cancellationToken = default)
    {
        var routes = await GetAllAsync(cancellationToken);

        var existingRoute = routes.FirstOrDefault(x => x.Id == request.Id);

        if (existingRoute is null)
        {
            throw new NotFoundException(typeof(FlightRouteRequest), nameof(request.Id), request.Id);
        }
        
        routes.Remove(existingRoute);

        var updatedRoute = new FlightRoute(
            request.Id,
            request.Origin,
            request.Destination,
            request.Cost);

        routes.Add(updatedRoute);

        await SaveFlightRoutesToFile(routes, cancellationToken);

        return updatedRoute;
    }


    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var routes = await GetAllAsync(cancellationToken);

        var existingRoute = routes.FirstOrDefault(x => x.Id == id);

        if (existingRoute == null)
        {
            throw new NotFoundException(typeof(FlightRouteRequest), nameof(id), id);
        }
        
        routes.Remove(existingRoute);

        await SaveFlightRoutesToFile(routes, cancellationToken);
    }

    private async Task SaveFlightRoutesToFile(List<FlightRoute> routes, CancellationToken cancellationToken)
    {
        var jsonContent = JsonSerializer.Serialize(
            routes,
            new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

        await File.WriteAllTextAsync(_filePath, jsonContent, cancellationToken);
    }
}
