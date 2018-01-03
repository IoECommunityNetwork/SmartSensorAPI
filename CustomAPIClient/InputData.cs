using System.Runtime.Serialization;

namespace CustomAPIClient{
[DataContract(Name="input")]
    public class InputData
    {
       [DataMember(Name="deviceID")]
        public string DeviceID { get; set; }

        [DataMember(Name="token")]
        public string Token { get; set; }
    }
}