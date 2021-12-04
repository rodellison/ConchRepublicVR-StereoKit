using System.Collections.Generic;

namespace VRConchRepublic.Data
{
    public class EventData
    {
        public IList<EventDataDetails> body { get; set; }
        public int statusCode { get; set; }
    }

    public class EventDataDetails
    {
        public string eventID { get; set; }
        public string eventStartDate { get; set; }
        public string eventEndDate { get; set; }
        public string eventName { get; set; }
        public string eventContact { get; set; }
        public string eventLocation { get; set; }
        public string eventImgURL { get; set; }
        public string eventURL { get; set; }
        public string eventDescription { get; set; }







    }
}
