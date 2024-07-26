using System.Text.Json.Serialization;

namespace ETA_announcer.Models
{
    public class TravelTimeResponse
    {
        public DateTime TimeOfArrival { get; set; }
        public int DurationMinutes { get; set; }
    }

    public class TravelTimeRequest
    {
        public Origin Origin { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public RouteTravelMode TravelMode { get; set; }
        public Guid DestinationUserId { get; set; }
    }
}
