using System.Text.Json.Serialization;

namespace Examples
{
    public class UserInput
    {
        [JsonPropertyName("input")]
        public string Input { get; set; }

        public UserInput()
        {
        }

        public UserInput(string input)
        {
            Input = input;
        }
    }
}
