using StereoKit;
using System.Collections.Generic;
using VRConchRepublic.Data;

namespace VRConchRepublic
{
    public static class Globals
    {

        private static bool dataLoaded;

        public static List<EventDataDetails> keylargoEventDetails;
        public static List<EventDataDetails> islamoradaEventDetails;
        public static List<EventDataDetails> marathonEventDetails;
        public static List<EventDataDetails> lowerkeysEventDetails;
        public static List<EventDataDetails> keywestEventDetails;

        //References to Steppers created by the App class, after calling SK.AddStepper...
        public static EventDetailDisplay refEventDetailsDisplay;
        public static EventsMenu refEventsMenu;
        public static BackgroundSound refBackgroundSound;

        public static Matrix shipMatrix;

        public static bool DataLoaded { get => dataLoaded; set => dataLoaded = value; }
    }
}
