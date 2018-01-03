using System.Runtime.Serialization;
namespace CustomAPIClient 
{
    [DataContract(Name="authorize")]
    public class Authorize{
        [DataMember(Name="status")]
        public string Status { get; set; }
        
        [DataMember(Name="message")]
        public Message Message { get; set; }

        
    }

    [DataContract(Name="message")]
    public class Message{
        [DataMember(Name="username")]
        public string UserName { get; set; }
        
        [DataMember(Name="password")]
        public string Password { get; set; }

        [DataMember(Name="notificationCenterType")]
        public string NotificationCenterType { get; set; }

        [DataMember(Name="deviceType")]
        public string DeviceType { get; set; }

        [DataMember(Name="appVersion")]
        public string AppVersion { get; set; }

        [DataMember(Name="OSVersion")]
        public string OSVersion { get; set; }

        [DataMember(Name="deviceID")]
        public string DeviceID { get; set; }

        [DataMember(Name="notifToken")]
        public string NotifToken { get; set; }

        [DataMember(Name="timezoneOffset")]
        public string TimezoneOffset { get; set; }

        [DataMember(Name="timezoneName")]
        public string TimezoneName { get; set; }

        [DataMember(Name="token")]
        public string Token { get; set; }




    }
}