using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;

namespace TravelCheap.Api.Models.Api.Requests;

public record GetCheapestRouteRequest(string Origin, string Destination);
