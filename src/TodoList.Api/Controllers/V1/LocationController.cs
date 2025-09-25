using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TodoList.Application.Common.Interfaces;

namespace TodoList.Api.Controllers.v1;

[ApiController]
[Route("api/v1/[controller]")]
public class LocationController : ControllerBase
{
    private readonly ILocationService _locationService;

    public LocationController(ILocationService locationService)
    {
        _locationService = locationService;
    }

    [HttpGet("test")]
    [AllowAnonymous]
    public async Task<IActionResult> TestGoogleDns()
    {
        var location = await _locationService.GetLocationInfoAsync("8.8.8.8");
        if (location == null)
            return NotFound("Could not get location");

        return Ok(location);
    }
    [HttpGet("my-location")]
    [Authorize]
    public async Task<IActionResult> GetMyLocation()
    {
        var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "8.8.8.8";
        
        if (ip == "::1" || ip == "127.0.0.1")
            ip = "8.8.8.8";

        var location = await _locationService.GetLocationInfoAsync(ip);
        return Ok(location);
    }
}