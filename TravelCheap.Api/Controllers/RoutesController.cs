using System.Net;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using TravelCheap.Api.Middlewares;
using TravelCheap.Api.Models.Api.Requests;
using TravelCheap.Api.Models.Api.Responses;
using TravelCheap.Api.Models.Entities;
using TravelCheap.Api.Repositories;
using TravelCheap.Api.Services;

namespace TravelCheap.Api.Controllers;

[Route("api/v1/routes")]
public class RoutesController : ControllerBase
{
    private readonly IRouteCalculator _routeCalculator;
    private readonly IFlightRouteRepository _flightRouteRepository;
    private readonly IValidator<GetCheapestRouteRequest> _getCheapestRouteValidator;
    private readonly IValidator<FlightRouteRequest> _flightRouteValidator;

    public RoutesController(
        IRouteCalculator routeCalculator,
        IFlightRouteRepository flightRouteRepository,
        IValidator<GetCheapestRouteRequest> getCheapestRouteValidator,
        IValidator<FlightRouteRequest> flightRouteValidator)
    {
        _routeCalculator = routeCalculator;
        _flightRouteRepository = flightRouteRepository;
        _getCheapestRouteValidator = getCheapestRouteValidator;
        _flightRouteValidator = flightRouteValidator;
    }
    
    /// <summary>
    /// Gets the cheapest route between specified origin and destination locations.
    /// </summary>
    /// <param name="request">Request containing origin and destination locations.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>List of flight routes representing the cheapest path between origin and destination.</returns>
    /// <response code="200">Returns the list of flight routes for the cheapest path.</response>
    /// <response code="400">If the request parameters are invalid.</response>
    /// <response code="404">If no route is found between origin and destination.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("cheapest/{Origin}/{Destination}")]
    [ProducesResponseType(typeof(GetCheapestRouteResponse), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetCheapestRouteAsync(
        [FromRoute] GetCheapestRouteRequest request,
        CancellationToken cancellationToken = default)
    {
        await _getCheapestRouteValidator.ValidateAndThrowAsync(request, cancellationToken);

        var routes = await _flightRouteRepository.GetAllAsync(cancellationToken);

        return Ok(_routeCalculator.GetShortestRoute(routes, request.Origin, request.Destination));
    }

    /// <summary>
    /// Gets a flight route by its ID.
    /// </summary>
    /// <param name="id">The ID of the flight route to retrieve.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The flight route with the specified ID.</returns>
    /// <response code="200">Returns the flight route.</response>
    /// <response code="404">If the flight route is not found.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(FlightRoute), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> GetAsync(
        int id,
        CancellationToken cancellationToken = default)
    {
        return Ok(await _flightRouteRepository.GetAsync(id, cancellationToken));
    }

    /// <summary>
    /// Creates a new flight route.
    /// </summary>
    /// <param name="request">The flight route details.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The list of all flight routes including the newly created one.</returns>
    /// <response code="200">Returns the updated list of flight routes.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If related data is not found.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPost("")]
    [ProducesResponseType(typeof(List<FlightRoute>), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> CreateFlightRoute(
        [FromBody] FlightRouteRequest request,
        CancellationToken cancellationToken = default)
    {
        await _flightRouteValidator.ValidateAndThrowAsync(request, cancellationToken);

        return Ok(await _flightRouteRepository.CreateAsync(request, cancellationToken));
    }

    /// <summary>
    /// Updates an existing flight route.
    /// </summary>
    /// <param name="request">The updated flight route details.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>The updated flight route.</returns>
    /// <response code="200">Returns the updated flight route.</response>
    /// <response code="400">If the request is invalid.</response>
    /// <response code="404">If the flight route is not found.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpPut("")]
    [ProducesResponseType(typeof(FlightRoute), (int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.BadRequest)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> UpdateFlightRoute(
        [FromBody] FlightRouteRequest request,
        CancellationToken cancellationToken = default)
    {
        await _flightRouteValidator.ValidateAndThrowAsync(request, cancellationToken);

        return Ok(await _flightRouteRepository.UpdateAsync(request, cancellationToken));
    }

    /// <summary>
    /// Deletes a flight route.
    /// </summary>
    /// <param name="id">The ID of the flight route to delete.</param>
    /// <param name="cancellationToken">Optional cancellation token.</param>
    /// <returns>No content.</returns>
    /// <response code="200">If the flight route was successfully deleted.</response>
    /// <response code="404">If the flight route is not found.</response>
    /// <response code="500">If an unexpected error occurs.</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType((int) HttpStatusCode.OK)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.NotFound)]
    [ProducesResponseType(typeof(ErrorResponse), (int) HttpStatusCode.InternalServerError)]
    public async Task<IActionResult> DeleteFlightRoute(
        int id,
        CancellationToken cancellationToken = default)
    {
        await _flightRouteRepository.DeleteAsync(id, cancellationToken);

        return Ok();
    }
}
