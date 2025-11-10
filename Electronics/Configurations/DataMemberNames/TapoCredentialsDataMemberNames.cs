using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TapoDevices;
using TapoDevices.Credentials;

namespace AlarmClock.Electronics.DataMemberNames
{
    [DataContract]
    public class TapoCredentialsDataMemberNames
    {
        public const string Username = "username";
        public const string Password = "password";
    }
}
