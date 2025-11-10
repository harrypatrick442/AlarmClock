using AlarmClock.Electronics.DataMemberNames;
using JSON;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;
using TapoDevices.Credentials;
using System.IO;

namespace AlarmClock.Electronics.Configurations
{
    [DataContract]
    public class TapoConfiguration
    {
        [JsonPropertyName(TapoConfigurationDataMemberNames.Credentials)]
        [JsonInclude]
        [DataMember(Name = TapoConfigurationDataMemberNames.Credentials)]
        public TapoCredentials Credentials { get; protected set; }
        [JsonPropertyName(TapoConfigurationDataMemberNames.BulbNames)]
        [JsonInclude]
        [DataMember(Name = TapoConfigurationDataMemberNames.BulbNames)]
        public string[]? BulbNames { get; protected set; }
        [JsonPropertyName(TapoConfigurationDataMemberNames.PlugNames)]
        [JsonInclude]
        [DataMember(Name = TapoConfigurationDataMemberNames.PlugNames)]
        public string[]? PlugNames { get; protected set; }
        private TapoConfiguration(TapoCredentials? credentials, string[]? bulbNames, string[]? plugNames) { 
            Credentials = credentials;
            BulbNames = bulbNames;
            PlugNames = plugNames;
        }
        protected TapoConfiguration()
        {

        }
        public static TapoConfiguration Load(string path)
        {
            try
            {
                return Json.Deserialize<TapoConfiguration>(File.ReadAllText(path));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
                return null;
            }
        }
        public void Save(string path)
        {
            try
            {
                File.WriteAllText(path, Json.Serialize(this));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex);
            }
        }
        public static TapoConfiguration CreateTemplate() {
            return new TapoConfiguration(
                credentials:new TapoCredentials("UsernameHere", "PasswordHere"),
                bulbNames:new string[] { "bulbName1", "bulbName2Etc" },
                plugNames:new string[] { "plubName1", "plugName2Etc" });
        }
    }
}
