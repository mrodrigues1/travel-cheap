using System.Reflection;
using FluentValidation;
using Microsoft.OpenApi.Models;
using TravelCheap.Api.Middlewares;
using TravelCheap.Api.Models.Api.Validators;
using TravelCheap.Api.Repositories;
using TravelCheap.Api.Services;

namespace TravelCheap.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddControllers();

        builder.Services.AddTransient<IRouteCalculator, RouteCalculator>();
        builder.Services.AddTransient<IFlightRouteRepository, FlightRouteRepository>();

        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        builder.Services.AddOpenApi();

        builder.Services.AddSwaggerGen(opt =>
        {
            opt.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "TravelCheap",
                    Version = "v1",
                    Description = "TravelCheap"
                });

            var xmlPath = Path.Combine(
                AppContext.BaseDirectory,
                $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");

            opt.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddValidatorsFromAssemblyContaining<GetCheapestRouteRequestValidator>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwagger();

            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "TravelCheap v1");
                options.RoutePrefix = "swagger";
                options.DisplayRequestDuration();
            });
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
