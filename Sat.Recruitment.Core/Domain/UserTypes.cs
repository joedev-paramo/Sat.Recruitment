using System.Text.Json.Serialization;

namespace Sat.Recruitment.Core.Domain
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum UserTypes
    {
        Normal,
        SuperUser,
        Premium
    }
}
