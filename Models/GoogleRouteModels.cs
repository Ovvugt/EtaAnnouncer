using System.Text.Json.Serialization;

namespace EtaAnnouncer.Models
{
    public record GoogleComputeRoutesResponse(Route[] Routes);

    public record Route(string Duration);

    public record LatLng(double Latitude, double Longitude);

    public record Location(LatLng LatLng);

    public record Origin(Location Location);

    public record Destination(Location Location);

    public enum RoutingPreference
    {
        ROUTING_PREFERENCE_UNSPECIFIED,
        TRAFFIC_UNAWARE,
        TRAFFIC_AWARE,
        TRAFFIC_AWARE_OPTIMAL
    }

    public enum RouteTravelMode
    {
        TRAVEL_MODE_UNSPECIFIED,
        DRIVE,
        BICYCLE,
        WALK,
        TWO_WHEELER,
        TRANSIT
    }

    public record GoogleComputeRoutesRequest(Origin Origin,
                                             Destination Destination,
                                             [property: JsonConverter(typeof(JsonStringEnumConverter))] RouteTravelMode TravelMode,
                                             [property: JsonConverter(typeof(JsonStringEnumConverter))] RoutingPreference RoutingPreference);
}
