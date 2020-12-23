using System.Text.Json.Serialization;

namespace Examples
{
    public class UserMessage
    {
        [JsonPropertyName("name")]
        public string UserName { get; set; }
        [JsonPropertyName("message")]
        public string Message { get; set; }

        public UserMessage(string user, string message)
        {
            UserName = user;
            Message = message;
        }
    }
}
