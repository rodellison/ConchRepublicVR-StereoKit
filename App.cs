
using StereoKit;
using StereoKit.Framework;
using System.Threading;


namespace VRConchRepublic
{
    public class App : IStepper
    {
        public SKSettings Settings => new SKSettings
        {
            appName = "VR Conch Republic",
            assetsFolder = "Assets",
            displayPreference = DisplayMode.MixedReality
        };

        private float timeAtStart;
        private bool appScenesPlaying = false;

        private RadialMenuUI theRadialMenuUI;
        private ConchRepublicDataFetch conchRepublicDataFetch;

        public bool Enabled => true;
        public bool Initialize()
        {
            // Adjust the camera a bit, so the user isn't standing in the
            // middle of the button
            Renderer.CameraRoot = SK.ActiveDisplayMode == DisplayMode.MixedReality
                ? Matrix.T(0, 0.5f, 0.0f)
                //				? Matrix.T(0, 1.4f, 0.3f)
                : Matrix.T(0, 0, 0.2f);

            conchRepublicDataFetch = new ConchRepublicDataFetch();
            //Make a call to the Data fetcher.. don't await on any result at the moment, we're just calling to get the APIGateway/Lambda function
            //loaded and cached at AWS so later requests are very quick
            Thread dataFetchThread = new Thread(HandleDataFetch);
            dataFetchThread.Start();

            timeAtStart = 0f;

            //Initially, just load the welcome screen (logo/title screen) and step it for a few seconds before 
            //loading the main app scenes
            SK.AddStepper(new SceneWelcome());

            return true;

        }

        public async void HandleDataFetch()
        {
            Globals.DataLoaded = false;
            var fetchComplete = await conchRepublicDataFetch.LoadKeysData();
            if (fetchComplete)
            {
                Log.Info("Threaded Data Fetch Complete");
                Globals.DataLoaded = true;
                conchRepublicDataFetch = null;
            }
        }

        public void Step()
        {
            if (!appScenesPlaying)
            {
                //After x seconds, load the main app's scenes and remove the welcome screen
                if (Time.Totalf - timeAtStart > 4f)
                {
                    appScenesPlaying = true;
                    SK.AddStepper(new AvatarSkeleton());
                    SK.AddStepper(new Scenery());
                    SK.AddStepper(new InteractableBrochures());
                    Globals.refBackgroundSound = SK.AddStepper(new BackgroundSound());
                    Globals.refEventDetailsDisplay = SK.AddStepper(new EventDetailDisplay());
                    Globals.refEventsMenu = SK.AddStepper(new EventsMenu());
                    SK.AddStepper(new Fireworks());

                    theRadialMenuUI = SK.AddStepper(new RadialMenuUI());

                    SK.AddStepper(new HandMenuRadial(
                      new HandRadialLayer("Root", 0,
                      new HandMenuItem("Quit", null, () =>
                      {
                          Hand hand = Input.Hand(Handed.Right);
                          if (hand.IsTracked)
                          {
                              theRadialMenuUI.ShowPanel(Input.Hand(Handed.Right).Get(FingerId.Index, JointId.Tip).Pose, RadialMenuUI.RadialMenuSelectionType.Quit);
                          }

                      }),
                      new HandMenuItem("About", null, () =>
                      {
                          Hand hand = Input.Hand(Handed.Right);
                          if (hand.IsTracked)
                          {
                              theRadialMenuUI.ShowPanel(Input.Hand(Handed.Right).Get(FingerId.Index, JointId.Tip).Pose, RadialMenuUI.RadialMenuSelectionType.About);
                          }
                      })
                      )));

                    SK.RemoveStepper<SceneWelcome>();

                }
            }

        }

        public void Shutdown()
        {
            SK.RemoveStepper<AvatarSkeleton>();
            SK.RemoveStepper<BackgroundSound>();
            SK.RemoveStepper<Scenery>();
            SK.RemoveStepper<Fireworks>();
            SK.RemoveStepper<InteractableBrochures>();
            SK.RemoveStepper<RadialMenuUI>();
            SK.RemoveStepper<EventDetailDisplay>();
            SK.RemoveStepper<EventsMenu>();

        }


    }
}