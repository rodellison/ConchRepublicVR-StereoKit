using Newtonsoft.Json;
using StereoKit;
using System;
using System.Collections.Generic;
using System.Linq;
using VRConchRepublic;
using VRConchRepublic.Data;

namespace ConsoleTestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            ConchRepublicDataFetch c = new ConchRepublicDataFetch();
            SK.PreLoadLibrary();

            var haveData = c.FetchData("marathon");
            haveData.Wait();

            if (haveData.Result)
            {
                Console.WriteLine("\nSuccessully returned data");

                try
                {
                    EventData eventData = JsonConvert.DeserializeObject<EventData>(c.conchRepublicDataString);

                    if (eventData.body.Count > 0)
                    {
                        List<EventDataDetails> list = eventData.body.ToList<EventDataDetails>();
                        IEnumerable<EventDataDetails> sortedEnum = list.OrderBy(f => f.eventStartDate);
                        list = sortedEnum.ToList();

                        Console.WriteLine($"EventID: {list[0].eventID}");
                        Console.WriteLine($"EventName: {list[0].eventName}");
                        Console.WriteLine($"EventLocation: {list[0].eventLocation}");
                        Console.WriteLine($"EventStartDate: {list[0].eventStartDate}");
                        Console.WriteLine($"EventEndDate: {list[0].eventEndDate}");
                        Console.WriteLine($"EventDescription: {list[0].eventDescription}");
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("JSON Deserialize exception: " + e.Message);
                }


            }

            else
                Console.WriteLine("\nDID NOT successfully return data");
        }
    }
}
