using StereoKit;
using StereoKit.Framework;
using System;
using System.Collections.Generic;
using VRConchRepublic.Data;

namespace VRConchRepublic
{
    public class EventsMenu : IStepper
    {
        string windowTitle;

        Pose originalWindowPose = new Pose(0f, 0.4f, -0.5f, Quat.FromAngles(15f, 180f, 0));
        Pose windowPose;

        Vec2 fixedWindowSize = V.XY(0.35f, 0.25f);
        bool displayUI = false;
        List<EventDataDetails> locationEvents;
        int page = 0;
        readonly int maxItemsPerPage = 5;

        string[] eventButtonText;

        public void EnableDisplay(bool enable, string keyLocation)
        {
            //Set the Menu window pose to an original value so that its repositioned when the user
            //requests events for a new location..

            windowPose = originalWindowPose;
            displayUI = enable;
            page = 0;

            if (displayUI)
            {
                switch (keyLocation)
                {
                    case "KeyLargo":
                        windowTitle = "Events happening in Key Largo";
                        locationEvents = Globals.keylargoEventDetails;
                        break;
                    case "Islamorada":
                        windowTitle = "Events happening in Islamorada";
                        locationEvents = Globals.islamoradaEventDetails;
                        break;
                    case "Marathon":
                        windowTitle = "Events happening in Marathon";
                        locationEvents = Globals.marathonEventDetails;
                        break;
                    case "LowerKeys":
                        windowTitle = "Events happening in The Lower Keys";
                        locationEvents = Globals.lowerkeysEventDetails;
                        break;
                    case "KeyWest":
                        windowTitle = "Events happening in Key West";
                        locationEvents = Globals.keywestEventDetails;
                        break;
                }

                if (locationEvents.Count > 0)
                {
                    eventButtonText = new string[locationEvents.Count];
                    int i = 0;
                    foreach (var item in locationEvents)
                    {
                        eventButtonText[i] = item.eventName;
                        i++;
                    }
                }
            }
        }


        public bool Enabled => true;

        public bool Initialize()
        {
            return true;
        }

        public void Shutdown()
        {
        }

        public void Step()
        {
            if (displayUI)
            {
                UI.WindowBegin(windowTitle, ref windowPose, fixedWindowSize, UIWin.Normal);
                var index = page * maxItemsPerPage;
                try
                {
                    for (int i = 0; i < maxItemsPerPage; i++)
                    {
                        if (index + i > locationEvents.Count - 1)
                        {
                            UI.Label("");
                        }
                        else
                        {
                            if (UI.Button(eventButtonText[index + i]))
                            {
                                Globals.refEventDetailsDisplay.SetTextDisplay(locationEvents[index + i]);
                            }
                        }
                    }

                }
                catch (IndexOutOfRangeException e)
                {
                    //If we end up here, it means we dont have enough events on this page to fill the full maxItemsPerPage Buttons..
                    //In that case, rather than error, just pad the space the button would have been with an empty label.
                    UI.Label("");
                    Log.Err("Error in EventsMenu: " + e.Message);
                }

                UI.HSeparator();
                if (UI.Button("Close window"))
                {
                    displayUI = false;
                    Globals.refEventDetailsDisplay.ClearTextDisplay();
                };

                if (page > 0)
                {
                    UI.SameLine();
                    if (UI.Button("Previous"))
                    {
                        page -= 1;
                    };
                }
                if (locationEvents.Count > index + maxItemsPerPage)
                {
                    UI.SameLine();
                    if (UI.Button("Next"))
                    {
                        page += 1;
                    };
                }


                UI.WindowEnd();

            }

        }


    }
}
