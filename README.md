# TravelCheap API
This API provides services for calculating the cheapest travel routes.
## How to Run the App
To run the application, navigate to the `TravelCheap.Api` directory and use the following command:
``` bash
dotnet run
```

Or run in your preferred IDE.

## How to Access Swagger
Once the application is running, you can access the Swagger UI for API documentation and testing at the following URL:
[http://localhost:5141/swagger](http://localhost:5141/swagger)
## Tools Used
The application is built using the .NET framework and C#. It utilizes the following key components:
- **ASP.NET Core**: For building the web API.
- **Swagger (Swashbuckle)**: For API documentation.
- **FluentValidation**: For request validation.
- **Dijkstra's algorithm**: a classic algorithm for finding the shortest paths between nodes in a graph.
- **Persistence in .txt file**: located at `TravelCheap\TravelCheap.Api\Infrastructure\Data\Flight_Routes.txt`

## Cheapest Route Development
The cheapest route is calculated using an implementation of Dijkstra's algorithm. This algorithm finds the shortest path between two nodes in a graph, where the "distance" is the cost of travel. The implementation can be found in the `RouteCalculator.cs` file within the `TravelCheap.Api/Services` directory.
