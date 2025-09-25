namespace TodoList.Application.Common.Interfaces;

public interface ILocationService
{
    Task<LocationInfo?> GetLocationInfoAsync(string ipAddress);
}

public class LocationInfo
{
    public string Country { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}