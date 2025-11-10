using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TapoDevices;
using TapoDevices.Credentials;

namespace AlarmClock.Electronics.DataMemberNames
{
    [DataContract]
    public class TapoConfigurationDataMemberNames
    {
        public const string Credentials = "credentials";
        public const string BulbNames = "bulb_names";
        public const string PlugNames = "plug_names";
    }
}
