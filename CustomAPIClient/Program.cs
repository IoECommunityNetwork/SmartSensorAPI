using System;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;
using System.Text;



namespace CustomAPIClient
{
    class Program
    {
        static void Main(string[] args)
        {
            //var repositories = ProcessRepositories().Result;

            /* foreach (var repo in repositories)
            {
            Console.WriteLine(repo.Name);
            Console.WriteLine(repo.Description);
            Console.WriteLine(repo.GitHubHomeUrl);
            Console.WriteLine(repo.Homepage);
            Console.WriteLine(repo.Watchers);
            Console.WriteLine(repo.LastPush);
            Console.WriteLine();
            } */
            // Retrive Token by invoking Authorize API
            Authorize authorizeResponse = AuthorizeUser().Result;
            // If Success
            if(authorizeResponse != null){
            Console.WriteLine(authorizeResponse.Message.DeviceID);
            Console.WriteLine(authorizeResponse.Message.Token);
            // Prepare Input object for subsequent API requests
            InputData input = new InputData();
            input.DeviceID = authorizeResponse.Message.DeviceID;
            input.Token = authorizeResponse.Message.Token;
            // Invoke Get All Sensors API
            SensorsInfo sensorsInfoResponse = SensorsInfo(input).Result;
            // If Success
            if(sensorsInfoResponse != null){
            // Print All sensors info    
            Console.WriteLine(sensorsInfoResponse.Status);
            SensorsInfoMessage sInfoMessage = sensorsInfoResponse.Message;
            Console.WriteLine("CurrentParameter : " + sInfoMessage.CurrentParameter);
            Console.WriteLine("NextParameter : " + sInfoMessage.NextParameter);
            Console.WriteLine("PreviousParameter : " + sInfoMessage.PreviousParameter);
            Console.WriteLine("Start : " + sInfoMessage.Start);
            Console.WriteLine("Total : " + sInfoMessage.Total);
            foreach (var sInfo in sInfoMessage.SensorInfoList)
            { 
            Console.WriteLine("SensorsID : " + sInfo.SensorsID);
            Console.WriteLine("SensorstokenID : " + sInfo.SensorstokenID);
            Console.WriteLine("SensorCompany : " + sInfo.SensorCompany);
            Console.WriteLine("SensorDeviceID : " + sInfo.SensorDeviceID);
            Console.WriteLine("FirmwareVersion : " + sInfo.FirmwareVersion);
            Console.WriteLine("SensorName : " + sInfo.SensorName);
            Console.WriteLine("Allocated : " + sInfo.Allocated);
            Console.WriteLine("Description : " + sInfo.Description);
            Console.WriteLine("CompanyID : " + sInfo.CompanyID);
            Console.WriteLine("createdByUserID : " + sInfo.createdByUserID);
            Console.WriteLine("UpdatedByUserID : " + sInfo.UpdatedByUserID);
            Console.WriteLine("LatestSensorsData : " + sInfo.LatestSensorsData);
            Console.WriteLine("Created : " + sInfo.Created);
            Console.WriteLine("Updated : " + sInfo.Updated);
            
            Console.WriteLine();
            

            // Invoke Get Sensor Data API for Active sensors
            if(sInfo.Allocated){
            SensorsData sensorsDataResponse = SensorData(input,sInfo.SensorsID).Result;
            // If Success
            if(sensorsDataResponse != null){
            // Print Sensor Data    
            Console.WriteLine(sensorsDataResponse.Status);
            SensorsDataMessage sDataMessage = sensorsDataResponse.Message;
            Console.WriteLine("CurrentParameter : " + sDataMessage.CurrentParameter);
            Console.WriteLine("NextParameter : " + sDataMessage.NextParameter);
            Console.WriteLine("PreviousParameter : " + sDataMessage.PreviousParameter);
            Console.WriteLine("Start : " + sDataMessage.Start);
            Console.WriteLine("DepthWhenEmptycm : " + sDataMessage.DepthWhenEmptycm);
            Console.WriteLine("DistanceSensorToFillLinecm : " + sDataMessage.DistanceSensorToFillLinecm);
             foreach (var sData in sDataMessage.SensorDataList)
            { 
            Console.WriteLine("SensorsdataID : " + sData.SensorsdataID);
            Console.WriteLine("SensorstokenID : " + sData.SensorstokenID);
            Console.WriteLine("SensorallocatedID : " + sData.SensorallocatedID);
            Console.WriteLine("SensorEventID : " + sData.SensorEventID);
            Console.WriteLine("SensorDeviceID : " + sData.SensorDeviceID);
            Console.WriteLine("FirmwareVersion : " + sData.FirmwareVersion);
            Console.WriteLine("HeaderMethod : " + sData.HeaderMethod);
            Console.WriteLine("Reason : " + sData.Reason);
            Console.WriteLine("TemperatureExist : " + sData.TemperatureExist);
            Console.WriteLine("TemperatureValue : " + sData.TemperatureValue);
            Console.WriteLine("TemperatureOkay : " + sData.TemperatureOkay);
            Console.WriteLine("AccelerometerX : " + sData.AccelerometerX);
            Console.WriteLine("AccelerometerY : " + sData.AccelerometerY);
            Console.WriteLine("AccelerometerZ : " + sData.AccelerometerZ);
            Console.WriteLine("UltrasoundExist : " + sData.UltrasoundExist);
            Console.WriteLine("Ultrasound : " + sData.Ultrasound);
            Console.WriteLine("BatteryVoltagemV : " + sData.BatteryVoltagemV);
            Console.WriteLine("SignalStrengthExist : " + sData.SignalStrengthExist);
            Console.WriteLine("SignalStrengthRSSIdbm : " + sData.SignalStrengthRSSIdbm);
            Console.WriteLine("SignalStrengthBitErrorRate : " + sData.SignalStrengthBitErrorRate);
            Console.WriteLine("Timestamp : " + sData.Timestamp);
            
            Console.WriteLine();
            } 
            }
            }
            } 
            }
            }
            

        }
        // POST API - SmartSensor API Authorize User
        private static async Task<Authorize> AuthorizeUser()
         {
            var client = new HttpClient();
            try{
            var serializer = new DataContractJsonSerializer(typeof(Authorize));
            Message user = new Message();
            user.UserName = "venkat.penuganti@phamwoods.com";
            user.Password = "Password@1";
            user.NotificationCenterType = "apple";
            user.DeviceType = "iPad";
            user.AppVersion = "SmartSensor 1.0.0";
            user.OSVersion = "iOS 9.2.0";
            user.DeviceID = "ABCD-EFGH-IJKL-MNOP";
            user.NotifToken = "abcdefgh13234123423";
            user.TimezoneOffset = "420";
            user.TimezoneName = "Australia/Sydney";
            var streamTask = client.PostAsync("https://dashboard.smartsensor.com.au/api/users/authorize", new StringContent(SerializeJSon<Message>(user), Encoding.UTF8, "application/json"));
            HttpResponseMessage response = await streamTask;
            response.EnsureSuccessStatusCode();
            Stream content = await response.Content.ReadAsStreamAsync();
            var authorizeResponse = serializer.ReadObject(content) as Authorize;
            return authorizeResponse;
            } catch(Exception){
            return null;
            }

            
        }

        // POST API - SmartSensor API get All Sensors Info
        private static async Task<SensorsInfo> SensorsInfo(InputData input)
         {
            var client = new HttpClient();
            try{
            var serializer = new DataContractJsonSerializer(typeof(SensorsInfo));
            var streamTask = client.PostAsync("https://dashboard.smartsensor.com.au/api/sensors/list", new StringContent(SerializeJSon<InputData>(input), Encoding.UTF8, "application/json"));
            HttpResponseMessage response = await streamTask;
            response.EnsureSuccessStatusCode();
            Stream content = await response.Content.ReadAsStreamAsync();
            var sensorsInfoResponse = serializer.ReadObject(content) as SensorsInfo;
            
            return sensorsInfoResponse;
            } catch(Exception){
            return null;
            }
        }

        // POST API - SmartSensor API get sensor data for specific sernsor id
        private static async Task<SensorsData> SensorData(InputData input, int sensorId)
         {
            var client = new HttpClient();
            try{
            var serializer = new DataContractJsonSerializer(typeof(SensorsData));
            var streamTask = client.PostAsync("https://dashboard.smartsensor.com.au/api/sensors/rawsensordata/"+sensorId.ToString(), new StringContent(SerializeJSon<InputData>(input), Encoding.UTF8, "application/json"));
            HttpResponseMessage response = await streamTask;
            response.EnsureSuccessStatusCode();
            Stream content = await response.Content.ReadAsStreamAsync();
            var sensorsDataResponse = serializer.ReadObject(content) as SensorsData;
            
            return sensorsDataResponse;
            } catch(Exception){
            return null;
            }
        }
       
        private static string SerializeJSon<T>(T t)
        {   try{
            MemoryStream  stream = new MemoryStream();
            DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
            DataContractJsonSerializerSettings s = new DataContractJsonSerializerSettings();
            ds.WriteObject(stream, t);
            string jsonString = Encoding.UTF8.GetString(stream.ToArray());
            stream.Close();
            return jsonString;  
            } catch(Exception){
            return null;
            }                                            
        }

        //GET API - Reference Implementation get repository information from GitHub API
        private static async Task<List<Repository>> ProcessRepositories()
         {
            var client = new HttpClient();
            try{
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
            client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
            var serializer = new DataContractJsonSerializer(typeof(List<Repository>));
            var streamTask = client.GetStreamAsync("https://api.github.com/orgs/dotnet/repos");
            var repositories = serializer.ReadObject(await streamTask) as List<Repository>;
            

            return repositories;
            } catch(Exception){
            return null;
            }
        }
    }
    
    

    
}
