using Newtonsoft.Json;
using StereoKit;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Resources;
using System.Threading.Tasks;
using VRConchRepublic.Data;

namespace VRConchRepublic
{
    public class ConchRepublicDataFetch
    {
        // HttpClient is intended to be instantiated once per application, rather than per-use. See Remarks.
        static readonly HttpClient client = new HttpClient();
        public string conchRepublicDataString;
        private bool dataAvailable = false;
        private readonly List<string> keyLocationsToFetch = new List<string> { "key-largo", "islamorada", "marathon", "lower-keys", "key-west" };

        private async Task<bool> GetData(string keysLocation)
        {
            ResourceManager rm = new ResourceManager(typeof(ConchRepublicStrings));
            if (rm == null || rm.GetString("ConchRepublicURL").Length == 0)
                return false;

            // Call asynchronous network methods in a try/catch block to handle exceptions.
            try
            {
                Log.Info("GetData: Attempting to Fetch Conch Republic API data for : " + keysLocation);
                dataAvailable = false;
                conchRepublicDataString = await client.GetStringAsync(rm.GetString("ConchRepublicURL") + keysLocation);
                conchRepublicDataString = conchRepublicDataString.Replace("\\\"", "\"");
                conchRepublicDataString = conchRepublicDataString.Replace("\"[", "[");
                conchRepublicDataString = conchRepublicDataString.Replace("]\"", "]");
                conchRepublicDataString = conchRepublicDataString.Replace("\\\\u0027", "'");
                conchRepublicDataString = conchRepublicDataString.Replace("\\\\n", "");
                conchRepublicDataString = conchRepublicDataString.Replace("\\\\\"", "'");

                //  Log.Info(conchRepublicDataString);
                dataAvailable = true;

            }
            catch (HttpRequestException e)
            {
                dataAvailable = false;
                Log.Err("Exception Caught! : " + e.Message);
            }
            catch (IOException e)
            {
                //This can happen if there is no internet/wifi connection in the headset
                dataAvailable = false;
                Log.Err("IOException Caught! : " + e.Message);
            }

            return dataAvailable;

        }

        public async Task<bool> FetchData(string keysLocation)
        {
            var result = await GetData(keysLocation);
            return result;
        }

        public async Task<bool> LoadKeysData()
        {
            foreach (var location in keyLocationsToFetch)
            {
                var haveData = await FetchData(location);
                if (haveData)
                {
                    EventData eventData = JsonConvert.DeserializeObject<EventData>(conchRepublicDataString);

                    if (eventData.body.Count > 0)
                    {
                        List<EventDataDetails> list = eventData.body.ToList<EventDataDetails>();
                        IEnumerable<EventDataDetails> sortedEnum = list.OrderBy(f => f.eventStartDate);

                        switch (location)
                        {
                            case "key-largo":
                                Globals.keylargoEventDetails = sortedEnum.ToList();
                                break;
                            case "islamorada":
                                Globals.islamoradaEventDetails = sortedEnum.ToList();
                                break;
                            case "marathon":
                                Globals.marathonEventDetails = sortedEnum.ToList();
                                break;
                            case "lower-keys":
                                Globals.lowerkeysEventDetails = sortedEnum.ToList();
                                break;
                            case "key-west":
                                Globals.keywestEventDetails = sortedEnum.ToList();
                                break;
                        }
                        Log.Info("Event Data found for location: " + location);

                        //Log.Info($"EventID: {list[0].eventID}");
                        //Log.Info($"EventName: {list[0].eventName}");
                        //Log.Info($"EventLocation: {list[0].eventLocation}");
                        //Log.Info($"EventStartDate: {list[0].eventStartDate}");
                        //Log.Info($"EventEndDate: {list[0].eventEndDate}");
                        //Log.Info($"EventDescription: {list[0].eventDescription}");
                    }
                    else
                    {
                        Log.Info("NO Event Data found for location: " + location);
                    }

                }

                else
                    Log.Info("\nDID NOT successfully return data");
            }

            return true;
        }
    }
}

