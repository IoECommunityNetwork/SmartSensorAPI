namespace SmartSensorAPIModule
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Loader;
    using System.Security.Cryptography.X509Certificates;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Azure.Devices.Client;
    using Microsoft.Azure.Devices.Client.Transport.Mqtt;
    using Microsoft.Azure.Devices.Shared;
    using Newtonsoft.Json;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Runtime.Serialization.Formatters.Binary;


    class Program
    {
        static int counter;
        static int fromTime { get; set; } = 0;
        static int toTime { get; set; } = 0;
        static void Main(string[] args)
        {
            // The Edge runtime gives us the connection string we need -- it is injected as an environment variable
            string connectionString = Environment.GetEnvironmentVariable("EdgeHubConnectionString");
            connectionString = "HostName=IOTAA-Course-IOTHub.azure-devices.net;GatewayHostName=siwin6.lrha0ktazqkuzg3ydukinat5fe.qx.internal.cloudapp.net;DeviceId=SimulatedDevice;ModuleId=smartsensorapiModule;SharedAccessKey=pn8GxSyVqNwRzFd2l1bm5wU3D+WAKKZb2/0Ul5qY8YM=";
            Console.WriteLine("Connection String -->: " + connectionString);
            // Cert verification is not yet fully functional when using Windows OS for the container
            bool bypassCertVerification = RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
            if (!bypassCertVerification) InstallCert();
            Init(connectionString, bypassCertVerification).Wait();

            // Wait until the app unloads or is cancelled
            var cts = new CancellationTokenSource();
            AssemblyLoadContext.Default.Unloading += (ctx) => cts.Cancel();
            Console.CancelKeyPress += (sender, cpe) => cts.Cancel();
            WhenCancelled(cts.Token).Wait();
        }

        /// <summary>
        /// Handles cleanup operations when app is cancelled or unloads
        /// </summary>
        public static Task WhenCancelled(CancellationToken cancellationToken)
        {
            var tcs = new TaskCompletionSource<bool>();
            cancellationToken.Register(s => ((TaskCompletionSource<bool>)s).SetResult(true), tcs);
            return tcs.Task;
        }

        /// <summary>
        /// Add certificate in local cert store for use by client for secure connection to IoT Edge runtime
        /// </summary>
        static void InstallCert()
        {
            string certPath = Environment.GetEnvironmentVariable("EdgeModuleCACertificateFile");
            if (string.IsNullOrWhiteSpace(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing path to certificate file.");
            }
            else if (!File.Exists(certPath))
            {
                // We cannot proceed further without a proper cert file
                Console.WriteLine($"Missing path to certificate collection file: {certPath}");
                throw new InvalidOperationException("Missing certificate file.");
            }
            X509Store store = new X509Store(StoreName.Root, StoreLocation.CurrentUser);
            store.Open(OpenFlags.ReadWrite);
            store.Add(new X509Certificate2(X509Certificate2.CreateFromCertFile(certPath)));
            Console.WriteLine("Added Cert: " + certPath);
            store.Close();
        }


        /// <summary>
        /// Initializes the DeviceClient and sets up the callback to receive
        /// messages containing temperature information
        /// </summary>
        static async Task Init(string connectionString, bool bypassCertVerification = false)
        {
            Console.WriteLine("Connection String {0}", connectionString);

            MqttTransportSettings mqttSetting = new MqttTransportSettings(TransportType.Mqtt_Tcp_Only);
            // During dev you might want to bypass the cert verification. It is highly recommended to verify certs systematically in production
            if (bypassCertVerification)
            {
                mqttSetting.RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            }
            ITransportSettings[] settings = { mqttSetting };

            // Open a connection to the Edge runtime
            DeviceClient ioTHubModuleClient = DeviceClient.CreateFromConnectionString(connectionString, settings);
            await ioTHubModuleClient.OpenAsync();
            Console.WriteLine("IoT Hub module client initialized.");
            
             // Attach callback for Twin desired properties updates
            await ioTHubModuleClient.SetDesiredPropertyUpdateCallbackAsync(onDesiredPropertiesUpdate, null);

            // Register callback to be called when a message is received by the module
            await ioTHubModuleClient.SetInputMessageHandlerAsync("input1", SmartSensorMessages, ioTHubModuleClient);
        }

static Task onDesiredPropertiesUpdate(TwinCollection desiredProperties, object userContext)
{
    try
    {
        Console.WriteLine("Desired property change:");
        Console.WriteLine(JsonConvert.SerializeObject(desiredProperties));

        if (desiredProperties["FromTime"]!=null)
            fromTime = desiredProperties["FromTime"];
        if (desiredProperties["ToTime"]!=null)
            toTime = desiredProperties["ToTime"];    

    }
    catch (AggregateException ex)
    {
        foreach (Exception exception in ex.InnerExceptions)
        {
            Console.WriteLine();
            Console.WriteLine("Error when receiving desired property: {0}", exception);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine();
        Console.WriteLine("Error when receiving desired property: {0}", ex.Message);
    }
    return Task.CompletedTask;
}

        /// <summary>
        /// This method is called whenever the module is sent a message from the EdgeHub. 
        /// It just pipe the messages without any change.
        /// It prints all the incoming messages.
        /// </summary>
        static async Task<MessageResponse> SmartSensorMessages(Microsoft.Azure.Devices.Client.Message message, object userContext)
        {
            int counterValue = Interlocked.Increment(ref counter);

            try
            {
                DeviceClient deviceClient = (DeviceClient)userContext;

                TimeSpan t = DateTime.UtcNow - new DateTime(1970, 1, 1);
                int secondsSinceEpoch = (int)t.TotalSeconds;
                if (fromTime == 0)
                    toTime = secondsSinceEpoch;
                else
                    toTime = fromTime;
                Console.WriteLine($"{counterValue} Smart Sensor Data API Invoked fromTime (Epoch) : [{fromTime}], toTime (Epoch) : [{toTime}]");
                fromTime = secondsSinceEpoch;
                //Get Token Object For API Calls
                InputData tokenInfo = GetTokenObjectForAPI("venkat.penuganti@phamwoods.com","Password@1");

               //Get All Sensors Information and push it as IOT Hub Message
                byte[] allSensorsMessageBytes = ObjectToByteArray(GetAllSensorsInfo(tokenInfo));
                var  allSensorsFilteredMessage = new Microsoft.Azure.Devices.Client.Message(allSensorsMessageBytes);
                allSensorsFilteredMessage.Properties.Add("MessageType", "Alert");
                await deviceClient.SendEventAsync("output1", allSensorsFilteredMessage);
               
                //Get All Active Sesnsors
               List<SensorInfo> activeSensors = GetActiveSensorsInfo(tokenInfo);
               if(activeSensors != null)
               {
                   foreach (var sInfo in activeSensors)
                   {
                       //Get Active Sensors Payload and push it as IOT Hub Message
                       byte[] activeSensorDataBytes = ObjectToByteArray(GetSensorPayload(tokenInfo,sInfo.SensorsID,fromTime,toTime));
                       var activeSensorsPayLoadMessage = new Microsoft.Azure.Devices.Client.Message(allSensorsMessageBytes);
                       activeSensorsPayLoadMessage.Properties.Add("MessageType", "Alert");
                       await deviceClient.SendEventAsync("output1", activeSensorsPayLoadMessage);
                   }
               } 
                              
                // Indicate that the message treatment is completed
                return MessageResponse.Completed;
            }
            catch (AggregateException ex)
            {
                foreach (Exception exception in ex.InnerExceptions)
                {
                    Console.WriteLine();
                    Console.WriteLine("Error in sample: {0}", exception);
                }
                // Indicate that the message treatment is not completed
                DeviceClient deviceClient = (DeviceClient)userContext;
                return MessageResponse.Abandoned;
            }
            catch (Exception ex)
            {
                Console.WriteLine();
                Console.WriteLine("Error in sample: {0}", ex.Message);
                // Indicate that the message treatment is not completed
                DeviceClient deviceClient = (DeviceClient)userContext;
                return MessageResponse.Abandoned;
            }
        }

        // POST API - SmartSensor API Authorize User
        private static async Task<Authorize> AuthorizeUser(string userN, string pwd)
        {
            var client = new HttpClient();
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(Authorize));
                Message user = new Message();
                user.UserName = userN;
                user.Password = pwd; //"Password@1"
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
            }
            catch (Exception)
            {
                return null;
            }


        }

        // POST API - SmartSensor API get All Sensors Info
        private static async Task<SensorsInfo> SensorsInfo(InputData input)
        {
            var client = new HttpClient();
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(SensorsInfo));
                var streamTask = client.PostAsync("https://dashboard.smartsensor.com.au/api/sensors/list", new StringContent(SerializeJSon<InputData>(input), Encoding.UTF8, "application/json"));
                HttpResponseMessage response = await streamTask;
                response.EnsureSuccessStatusCode();
                Stream content = await response.Content.ReadAsStreamAsync();
                var sensorsInfoResponse = serializer.ReadObject(content) as SensorsInfo;

                return sensorsInfoResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        // POST API - SmartSensor API get sensor data for specific sernsor id
        private static async Task<SensorsData> SensorData(InputData input, int sensorId)
        {
            var client = new HttpClient();
            try
            {
                var serializer = new DataContractJsonSerializer(typeof(SensorsData));
                var streamTask = client.PostAsync("https://dashboard.smartsensor.com.au/api/sensors/rawsensordata/" + sensorId.ToString(), new StringContent(SerializeJSon<InputData>(input), Encoding.UTF8, "application/json"));
                HttpResponseMessage response = await streamTask;
                response.EnsureSuccessStatusCode();
                Stream content = await response.Content.ReadAsStreamAsync();
                var sensorsDataResponse = serializer.ReadObject(content) as SensorsData;

                return sensorsDataResponse;
            }
            catch (Exception)
            {
                return null;
            }
        }

        static byte[] ObjectToByteArray(object obj)
{
    if(obj == null)
        return null;
    BinaryFormatter bf = new BinaryFormatter();
    using (MemoryStream ms = new MemoryStream())
    {
        bf.Serialize(ms, obj);
        return ms.ToArray();
    }
}

        private static string SerializeJSon<T>(T t)
        {
            try
            {
                MemoryStream stream = new MemoryStream();
                DataContractJsonSerializer ds = new DataContractJsonSerializer(typeof(T));
                DataContractJsonSerializerSettings s = new DataContractJsonSerializerSettings();
                ds.WriteObject(stream, t);
                string jsonString = Encoding.UTF8.GetString(stream.ToArray());
                stream.Close();
                return jsonString;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static InputData GetTokenObjectForAPI(string userName, string password) {
          if(userName == null || password == null)
          return null;
          else
          {
              InputData input = null;
           // Retrive Token by invoking Authorize API
            Authorize authorizeResponse = AuthorizeUser(userName, password).Result;
            // If Success
            if(authorizeResponse != null){
            // Prepare Input object for subsequent API requests
            input = new InputData();
            input.DeviceID = authorizeResponse.Message.DeviceID;
            input.Token = authorizeResponse.Message.Token;
            
          }
          return input;
        }
    }

    private static SensorsInfo GetAllSensorsInfo(InputData tokenInfo) {
        if(tokenInfo == null)
        return null;
        else
        {
            SensorsInfo sensorsInfoResponse = null;
            // Invoke Get All Sensors API
            sensorsInfoResponse = SensorsInfo(tokenInfo).Result;

           return  sensorsInfoResponse;
        }
    }

    private static List<SensorInfo> GetActiveSensorsInfo(InputData tokenInfo) {
        if(tokenInfo == null)
        return null;
        else
        {
            SensorsInfo sensorsInfoResponse = null;
            List<SensorInfo> activeList = new List<SensorInfo>();
            // Invoke Get All Sensors API
            sensorsInfoResponse = SensorsInfo(tokenInfo).Result;
            if(sensorsInfoResponse != null){
                SensorsInfoMessage sInfoMessage = sensorsInfoResponse.Message;
                
                foreach (var sInfo in sInfoMessage.SensorInfoList)
            { 
                // Filter Sensor Info data for Active sensors
            if(sInfo.Allocated){
                activeList.Add(sInfo);
            }

            }
            
            }

           return  activeList;
        } 
    }

    private static SensorsData GetSensorPayload(InputData tokenInfo, int sensorId, int fromTime, int toTime)
    {
        if(tokenInfo == null || sensorId <= 0)
        return null;
        else
        {
            tokenInfo.FromTime = fromTime;
            tokenInfo.ToTime = toTime;
            SensorsData sensorsDataResponse = null;
            sensorsDataResponse = SensorData(tokenInfo,sensorId).Result;

            return sensorsDataResponse;
        }
    }





}
}
