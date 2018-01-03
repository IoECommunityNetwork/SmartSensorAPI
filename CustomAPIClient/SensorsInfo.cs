using System.Runtime.Serialization;
using System.Globalization;
using System;
using System.Collections.Generic;
namespace CustomAPIClient 
{
    [DataContract(Name="sensorsinfo")]
    public class SensorsInfo{
        [DataMember(Name="status")]
        public string Status { get; set; }
        
        [DataMember(Name="message")]
        public SensorsInfoMessage Message { get; set; }

        
    }

    [DataContract(Name="sensorsinfomessage")]
    public class SensorsInfoMessage{
        [DataMember(Name="current_parameter")]
        public string CurrentParameter { get; set; }
        
        [DataMember(Name="next_parameter")]
        public string NextParameter { get; set; }

        [DataMember(Name="previous_parameter")]
        public string PreviousParameter { get; set; }

        [DataMember(Name="start")]
        public int Start { get; set; }

        [DataMember(Name="total")]
        public int Total { get; set; }

        [DataMember(Name="lists")]
        public List<SensorInfo> SensorInfoList { get; set; }

        
    }
    [DataContract(Name="sensorInfo")]
    public class SensorInfo{
        [DataMember(Name="sensorsID")]
        public int SensorsID { get; set; }
        
        [DataMember(Name="sensorstokenID")]
        public int SensorstokenID { get; set; }

        [DataMember(Name="sensorCompany")]
        public string SensorCompany { get; set; }

        [DataMember(Name="sensorDeviceID")]
        public string SensorDeviceID { get; set; }

        [DataMember(Name="firmwareVersion")]
        public string FirmwareVersion { get; set; }

        [DataMember(Name="sensorName")]
        public string SensorName { get; set; }

        [DataMember(Name="isAllocated")]
        public string IsAllocated { get; set; }

       [IgnoreDataMember]
       public Boolean Allocated
       {
       get
       {
        return (IsAllocated == "Y") ? true: false;
       }
       }

        [DataMember(Name="description")]
        public string Description { get; set; }

        [DataMember(Name="companyID")]
        public int CompanyID { get; set; }

        [DataMember(Name="createdByUserID")]
        public int createdByUserID { get; set; }

        [DataMember(Name="updatedByUserID")]
        public int UpdatedByUserID { get; set; }

        [DataMember(Name="latest_sensorsdata")]
        public SensorData LatestSensorsData { get; set; }

        [DataMember(Name="createdDate")]
       private string CreatedDate { get; set; }

       [IgnoreDataMember]
       public string Created
       {
       get
       {
        if(CreatedDate != null && CreatedDate.Length > 0) {
            //DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(CreatedDate));
            //string ausTimezoneId = "Australian Eastern Standard Time";
            //TimeZoneInfo ausTimeZone = TimeZoneInfo.Local;
            return (new DateTime(1970, 1, 1).AddSeconds(Convert.ToInt64(CreatedDate))).ToLocalTime().ToString();
        }else   
        return "";
       }
       }

       [DataMember(Name="updatedDate")]
       private string UpdatedDate { get; set; }

       [IgnoreDataMember]
       public string Updated
       {
       get
       {
          if(UpdatedDate != null && UpdatedDate.Length > 0) {
            //DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(Convert.ToInt64(UpdatedDate));
            //string ausTimezoneId = "Australian Eastern Standard Time";
            //TimeZoneInfo ausTimeZone = ;
            return (new DateTime(1970, 1, 1).AddSeconds(Convert.ToInt64(UpdatedDate))).ToLocalTime().ToString();
        }else   
        return "";
        
       }
       }
    }
}