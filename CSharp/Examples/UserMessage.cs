using System;

namespace Examples
{
    [Serializable]
    public class UserMessage
    {
        public string UserName;
        public string Message;

        public UserMessage(string user, string message)
        {
            UserName = user;
            Message = message;
        }
    }
}
