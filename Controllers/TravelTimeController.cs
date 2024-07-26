using ETA_announcer.Models;
using ETA_announcer.Services;
using Microsoft.AspNetCore.Mvc;

namespace ETA_announcer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TravelTimeController(IConfiguration configuration, ILogger<TravelTimeController> logger, IMapsService mapsService) : ControllerBase
    {

        [HttpPost]
        public async Task<ActionResult<TravelTimeResponse>> GetTravelTime(TravelTimeRequest request)
        {
            var apiKey = configuration["ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                logger.LogError("ApiKey is not configured.");
                return Problem();
            }

            var requestBody = new GoogleComputeRoutesRequest()
            {
               TravelMode = request.TravelMode,
               RoutingPreference = RoutingPreference.TRAFFIC_AWARE_OPTIMAL,
               Origin = request.Origin,
               Destination = new Destination { Location = new Location { LatLng = new LatLng { Latitude = 52.06159976913799, Longitude = 5.106039907536568 } } }
            };

            var routes = await mapsService.GetRouteAsync(requestBody, apiKey);
            if (routes == null)
            {
                return BadRequest();
            }

            var routeSeconds = double.Parse(routes.Routes[0].Duration.Trim('s'));
            var durationMinutes = (int)Math.Round(routeSeconds / 60.0);
            logger.LogInformation("Got route information, duration is {DurationMinutes}", durationMinutes);

            return new TravelTimeResponse
            {
                TimeOfArrival = DateTime.UtcNow.AddSeconds(routeSeconds).ToLocalTime(),
                DurationMinutes = durationMinutes
            };
        }
    }
}
