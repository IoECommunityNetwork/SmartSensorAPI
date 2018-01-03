namespace SmartSensorAPIModule {

using System.Runtime.Serialization;
[DataContract(Name="input")]
    public class InputData
    {
       [DataMember(Name="deviceID")]
        public string DeviceID { get; set; }

        [DataMember(Name="token")]
        public string Token { get; set; }

        [DataMember(Name="dmin")]
        public int FromTime { get; set; } = 0;

        [DataMember(Name="dmax")]
        public int ToTime { get; set; } = 0;
    }
}