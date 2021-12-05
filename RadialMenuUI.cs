using StereoKit;
using StereoKit.Framework;

namespace VRConchRepublic
{
    public class RadialMenuUI : IStepper
    {

        Pose windowPose;
        Sprite myFace;

        public enum RadialMenuSelectionType
        {
            Quit,
            About,
            Attributions
        }
        private RadialMenuSelectionType thisRadialMenuSelection;

        private bool isActive = false;

        public bool Enabled => true;


        public bool Initialize()
        {
            myFace = Sprite.FromFile("rodface.jpg");

            return true;
        }

        public void SetActive(bool active)
        {
            isActive = active;
        }

        public void ShowPanel(Pose position, RadialMenuSelectionType menuSelectionType)
        {
            thisRadialMenuSelection = menuSelectionType;
            windowPose = position;
            Pose headPose = Input.Head;

            //Fix up where the display shows the UI, and have it point towards the users head
            windowPose.position.y = 1.5f;

            windowPose.orientation = Quat.LookDir(headPose.Forward * -1);

            //  Log.Info("HandRadialMenu Pose: " + windowPose);

            SetActive(true);
        }

        public void Step()
        {
            if (isActive)
            {
                Hierarchy.Push(World.BoundsPose.ToMatrix());

                switch (thisRadialMenuSelection)
                {
                    case RadialMenuSelectionType.Quit:
                        UI.WindowBegin("Leaving so soon?", ref windowPose);
                        if (UI.Button("Yes, It's 5 o'clock somewhere"))
                        {
                            SK.Quit();
                        }
                        if (UI.Button("No, I'll stay a bit longer"))
                        {
                            SetActive(false);
                        }
                        UI.WindowEnd();

                        break;
                    case RadialMenuSelectionType.About:
                        UI.WindowBegin("About: Conch Republic VR", ref windowPose);
                        UI.Label("Created by: Rod Ellison");
                        UI.Label("www.rodellison.net");
                        UI.Image(myFace, new Vec2(0.1f, 0.1f));
                        UI.HSeparator();
                        if (UI.Button("OK"))
                            SetActive(false);
                        UI.SameLine();
                        if (UI.Button("Attributions"))
                            thisRadialMenuSelection = RadialMenuSelectionType.Attributions;

                        UI.WindowEnd();

                        break;
                    case RadialMenuSelectionType.Attributions:
                        UI.WindowBegin("Attributions: Conch Republic VR", ref windowPose);
                        UI.Label("All photos from Unsplash");
                        UI.Label("Fireworks sprites from OpenGameArt");
                        UI.Label("Fireworks sound from www.fesliyanstudios.com");
                        UI.Label("Steel drum music from freesound.org");
                        UI.Label("Ship, Palms, Chairs free models from CGTrader");
                        UI.HSeparator();
                        if (UI.Button("OK"))
                            thisRadialMenuSelection = RadialMenuSelectionType.About;

                        UI.WindowEnd();

                        break;

                }

                Hierarchy.Pop();

            }



        }

        public void Shutdown()
        { }
    }
}