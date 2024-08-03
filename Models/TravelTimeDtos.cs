using System.Text.Json.Serialization;

namespace EtaAnnouncer.Models
{
    public record TravelTimeResponse(DateTime TimeOfArrival, int DurationMinutes);

    public record TravelTimeRequest(Origin Origin,
                                    [property: JsonConverter(typeof(JsonStringEnumConverter))] RouteTravelMode TravelMode,
                                    Guid DestinationUserId);
}
