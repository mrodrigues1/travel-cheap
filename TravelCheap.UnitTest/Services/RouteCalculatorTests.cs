using TravelCheap.Api.Infrastructure.Exceptions;
using TravelCheap.Api.Models.Entities;
using TravelCheap.Api.Services;

namespace TravelCheap.UnitTest.Services;

public class RouteCalculatorTests
{
    private RouteCalculator _sut;

    private List<FlightRoute> _routes =
    [
        new(
            1,
            "GRU",
            "BRC",
            10),
        new(
            2,
            "BRC",
            "SCL",
            5),
        new(
            3,
            "GRU",
            "CDG",
            75),
        new(
            4,
            "GRU",
            "SCL",
            20),
        new(
            5,
            "GRU",
            "ORL",
            56),
        new(
            6,
            "ORL",
            "CDG",
            5),
        new(
            7,
            "SCL",
            "ORL",
            20)
    ];

    public RouteCalculatorTests()
    {
        _sut = new();
    }

    [Theory]
    [InlineData("GRU", "CDG", 40)]
    [InlineData("BRC", "SCL", 5)]
    public void GetShortestRoute_ExistingRoutes_ReturnCheapestRoute(string origin, string destination, int cost)
    {
        // Act
        var result = _sut.GetShortestRoute(_routes, origin, destination);

        // Assert
        Assert.Equal(cost, result.TotalCost);
    }


    [Fact]
    public void GetShortestRoute_OriginNotExists_ThrowsBusinessLogicException()
    {
        // Arrange
        var origin = "originError";
        var destination = "GRU";

        // Act & Assert
        var exception = Assert.Throws<NotFoundException>(() =>
            _sut.GetShortestRoute(_routes, origin, destination));
        Assert.Equal($"FlightRoute not found with Origin: {origin}", exception.Message);
    }

    [Fact]
    public void GetShortestRoute_DestinationNotExists_ThrowsBusinessLogicException()
    {
        // Arrange
        var origin = "GRU";
        var destination = "destinationError";

        // Act & Assert
        var exception = Assert.Throws<NotFoundException>(() =>
            _sut.GetShortestRoute(_routes, origin, destination));
        Assert.Equal($"FlightRoute not found with Destination: {destination}", exception.Message);
    }


    [Fact]
    public void GetShortestRoute_DestinationNotReachable_ThrowsBusinessLogicException()
    {
        // Arrange
        var origin = "BRC";
        var destination = "GRU";

        // Act & Assert
        var exception = Assert.Throws<BusinessLogicException>(() =>
            _sut.GetShortestRoute(_routes, origin, destination));
        Assert.Equal($"No route found from {origin} to {destination}", exception.Message);
    }
}
