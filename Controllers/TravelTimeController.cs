using EtaAnnouncer.Models;
using EtaAnnouncer.Services;
using Microsoft.AspNetCore.Mvc;

namespace EtaAnnouncer.Controllers
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

            var requestBody = new GoogleComputeRoutesRequest(request.Origin,
                                                             new Destination(new Location(new LatLng(52.06159976913799, 5.106039907536568))),
                                                             request.TravelMode,
                                                             RoutingPreference.TRAFFIC_AWARE_OPTIMAL);

            var routes = await mapsService.GetRouteAsync(requestBody, apiKey);
            if (routes == null)
            {
                return BadRequest();
            }

            var routeSeconds = double.Parse(routes.Routes[0].Duration.Trim('s'));
            var durationMinutes = (int)Math.Round(routeSeconds / 60.0);
            logger.LogInformation("Got route information, duration is {DurationMinutes}", durationMinutes);

            return new TravelTimeResponse(DateTime.UtcNow.AddSeconds(routeSeconds).ToLocalTime(), durationMinutes);
        }
    }
}
