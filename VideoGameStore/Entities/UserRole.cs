using System.Text.Json.Serialization;

namespace VideoGameStore.Entities
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserRole
    {
        CUSTOMER, SELLER
    }
}
