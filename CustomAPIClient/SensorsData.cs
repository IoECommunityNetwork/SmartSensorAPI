using System.Runtime.Serialization;
using System.Globalization;
using System;
using System.Collections.Generic;
namespace CustomAPIClient 
{
    [DataContract(Name="sensorsdata")]
    public class SensorsData{
        [DataMember(Name="status")]
        public string Status { get; set; }
        
        [DataMember(Name="message")]
        public SensorsDataMessage Message { get; set; }

        
    }

    [DataContract(Name="sensorsdatamessage")]
    public class SensorsDataMessage{
        [DataMember(Name="current_parameter")]
        public string CurrentParameter { get; set; }
        
        [DataMember(Name="next_parameter")]
        public string NextParameter { get; set; }

        [DataMember(Name="previous_parameter")]
        public string PreviousParameter { get; set; }

        [DataMember(Name="start")]
        public int Start { get; set; }

        [DataMember(Name="depthWhenEmpty_cm")]
        public int DepthWhenEmptycm { get; set; }

        [DataMember(Name="distanceSensorToFillLine_cm")]
        public int DistanceSensorToFillLinecm { get; set; }

        [DataMember(Name="lists")]
        public List<SensorData> SensorDataList { get; set; }

        
    }
    [DataContract(Name="sensorData")]
    public class SensorData{
        [DataMember(Name="sensorsdataID")]
        public int SensorsdataID { get; set; }
        
        [DataMember(Name="sensorstokenID")]
        public int SensorstokenID { get; set; }

        [DataMember(Name="sensorallocatedID")]
        public int SensorallocatedID { get; set; }

        [DataMember(Name="sensorEventID")]
        public string SensorEventID { get; set; }

        [DataMember(Name="sensorDeviceID")]
        public string SensorDeviceID { get; set; }

        [DataMember(Name="firmwareVersion")]
        public string FirmwareVersion { get; set; }

        [DataMember(Name="headerMethod")]
        public string HeaderMethod { get; set; }

        [DataMember(Name="reason")]
        public string Reason { get; set; }

        [DataMember(Name="temperatureExist")]
        public string TempExist { get; set; }
        
        [IgnoreDataMember]
       public Boolean TemperatureExist
       {
       get
       {
        return (TempExist == "Y") ? true: false;
       }
       }

        [DataMember(Name="temperatureValue")]
        public double TemperatureValue { get; set; }

        [DataMember(Name="temperatureOkay")]
        public string TempOkay { get; set; }
        
        [IgnoreDataMember]
       public Boolean TemperatureOkay
       {
       get
       {
        return (TempOkay == "Y") ? true: false;
       }
       }

        [DataMember(Name="accelerometer_x")]
        public double AccelerometerX { get; set; }

        [DataMember(Name="accelerometer_y")]
        public double AccelerometerY { get; set; }

        [DataMember(Name="accelerometer_z")]
        public double AccelerometerZ { get; set; }

        [DataMember(Name="ultrasoundExist")]
        public string UltraSExist { get; set; }
        
        [IgnoreDataMember]
       public Boolean UltrasoundExist
       {
       get
       {
        return (UltraSExist == "Y") ? true: false;
       }
       }

        [DataMember(Name="ultrasound")]
        public double Ultrasound { get; set; }

        [DataMember(Name="batteryVoltage_mV")]
        public double BatteryVoltagemV { get; set; }

        [DataMember(Name="signalStrengthExist")]
        public string SignalSExist { get; set; }

         [IgnoreDataMember]
       public Boolean SignalStrengthExist
       {
       get
       {
        return (SignalSExist == "Y") ? true: false;
       }
       }

        [DataMember(Name="signalStrength_rssi_dbm")]
        public double SignalStrengthRSSIdbm { get; set; }

        [DataMember(Name="signalStrength_bitErrorRate")]
        public double SignalStrengthBitErrorRate { get; set; }

        [DataMember(Name="timestampdata")]
       private string Timestampdata { get; set; }

       [IgnoreDataMember]
       public string Timestamp
       {
       get
       {
        return (new DateTime(1970, 1, 1).AddSeconds(Convert.ToInt64(Timestampdata))).ToLocalTime().ToString();
       }
       }

       
    }
}