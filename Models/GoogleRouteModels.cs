using System.Text.Json.Serialization;

namespace ETA_announcer.Models
{
    public class GoogleComputeRoutesResponse
    {
        public Route[] Routes { get; set; }
    }

    public class Route
    {
        public string Duration { get; set; }
    }

    public class LatLng
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Location
    {
        public LatLng LatLng { get; set; }
    }

    public class Origin
    {
        public Location Location { get; set; }
    }

    public class Destination
    {
        public Location Location { get; set; }
    }

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

    public class GoogleComputeRoutesRequest
    {
        public Origin Origin { get; set; }
        public Destination Destination { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RouteTravelMode TravelMode { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RoutingPreference RoutingPreference { get; set; }
    }
}
