using System.Text.Json;
using Microsoft.Extensions.Logging;
using TodoList.Application.Common.Interfaces;

namespace TodoList.Infrastructure.Services;

public class LocationService : ILocationService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<LocationService> _logger;

    public LocationService(IHttpClientFactory httpClientFactory, ILogger<LocationService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<LocationInfo?> GetLocationInfoAsync(string ipAddress)
    {
        try
        {
            // Create client using factory
            var client = _httpClientFactory.CreateClient();
            
            // Call the IP-API service as required
            var response = await client.GetAsync($"http://ip-api.com/json/{ipAddress}");

            if (!response.IsSuccessStatusCode)
                return null;

            var json = await response.Content.ReadAsStringAsync();
            
            // Simple JSON parsing
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;
            
            if (root.GetProperty("status").GetString() != "success")
                return null;

            return new LocationInfo
            {
                Country = root.GetProperty("country").GetString() ?? "",
                City = root.GetProperty("city").GetString() ?? "",
                Region = root.GetProperty("regionName").GetString() ?? "",
                Latitude = root.GetProperty("lat").GetDouble(),
                Longitude = root.GetProperty("lon").GetDouble()
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting location for IP: {IpAddress}", ipAddress);
            return null;
        }
    }
}