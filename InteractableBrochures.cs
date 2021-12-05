using StereoKit;
using StereoKit.Framework;
using System.Resources;

namespace VRConchRepublic
{
    public class InteractableBrochures : IStepper
    {
        public enum KeyLocation
        {
            KeyLargo,
            Islamorada,
            Marathon,
            LowerKeys,
            KeyWest,
            None
        }

        private readonly float timeToLerpReturnBrochure = 2f;
        private readonly float waitTimeAfterRelease = 5f;

        private readonly Bounds brochureBounds = new Bounds(Vec3.Zero, new Vec3(0.25f, 0.32f, 0.05f));
        private readonly Pose originalkeylargoBrochurePose = new Pose(V.XYZ(-0.75f, 1.08f, 0.08f), new Quat(0.50f, -0.5f, 0.50f, 0.50f));
        private readonly Pose originalislamoradaBrochurePose = new Pose(V.XYZ(-0.65f, 1.08f, -0.25f), new Quat(0.36f, -0.60f, 0.60f, 0.36f));
        private readonly Pose originalmarathonBrochurePose = new Pose(V.XYZ(-0.02f, 0.89f, -0.6f), new Quat(0.36f, -0.60f, 0.60f, 0.36f));
        private readonly Pose originallowerkeysBrochurePose = new Pose(V.XYZ(0.74f, 1.13f, -0.55f), new Quat(-0.31f, -0.64f, 0.64f, -0.31f));
        private readonly Pose originalkeywestBrochurePose = new Pose(V.XYZ(0.90f, 1.13f, -0.2f), new Quat(-0.31f, -0.64f, 0.64f, -0.31f));

        private Pose keylargoBrochurePose;
        private Pose islamoradaBrochurePose;
        private Pose marathonBrochurePose;
        private Pose lowerkeysBrochurePose;
        private Pose keywestBrochurePose;

        private Pose keylargoBrochureButtonPose;
        private Pose islamoradaBrochureButtonPose;
        private Pose marathonBrochureButtonPose;
        private Pose lowerkeysBrochureButtonPose;
        private Pose keywestBrochureButtonPose;

        private Model brochureModel;
        private Model keyLargoBrochureModel;
        private Model islamoradaBrochureModel;
        private Model marathonBrochureModel;
        private Model lowerkeysBrochureModel;
        private Model keywestBrochureModel;

        private Material keyLargoBrochureMaterial;
        private Material islamoradaBrochureMaterial;
        private Material marathonBrochureMaterial;
        private Material lowerkeysBrochureMaterial;
        private Material keywestBrochureMaterial;

        private bool keylargoBrochureGrabbed;
        private bool islamoradaBrochureGrabbed;
        private bool marathonBrochureGrabbed;
        private bool lowerkeysBrochureGrabbed;
        private bool keywestBrochureGrabbed;

        public bool Enabled => true;

        private float timeGrabbedKeyLargo;
        private float timeGrabbedIslamorada;
        private float timeGrabbedMarathon;
        private float timeGrabbedLowerKeys;
        private float timeGrabbedKeyWest;

        private float timeReturnKeyLargo = 0f;
        private float timeReturnIslamorada = 0f;
        private float timeReturnMarathon = 0f;
        private float timeReturnLowerKeys = 0f;
        private float timeReturnKeyWest = 0f;

        public bool Initialize()
        {
            ResourceManager rm = new ResourceManager(typeof(ConchRepublicStrings));
            if (rm == null || rm.GetString("ConchRepublicURL").Length == 0)
                return false;

            brochureModel = Model.FromFile("Brochure.glb");

            keyLargoBrochureModel = brochureModel.Copy();
            keyLargoBrochureMaterial = brochureModel.Visuals[0].Material.Copy();
            keyLargoBrochureMaterial[MatParamName.DiffuseTex] = Tex.FromFile(rm.GetString("KeyLargoBrochureImage"));
            keyLargoBrochureModel.Visuals[0].Material = keyLargoBrochureMaterial;

            islamoradaBrochureModel = brochureModel.Copy();
            islamoradaBrochureMaterial = brochureModel.Visuals[0].Material.Copy();
            islamoradaBrochureMaterial[MatParamName.DiffuseTex] = Tex.FromFile(rm.GetString("IslamoradaBrochureImage"));
            islamoradaBrochureModel.Visuals[0].Material = islamoradaBrochureMaterial;

            marathonBrochureModel = brochureModel.Copy();
            marathonBrochureMaterial = brochureModel.Visuals[0].Material.Copy();
            marathonBrochureMaterial[MatParamName.DiffuseTex] = Tex.FromFile(rm.GetString("MarathonBrochureImage"));
            marathonBrochureModel.Visuals[0].Material = marathonBrochureMaterial;

            lowerkeysBrochureModel = brochureModel.Copy();
            lowerkeysBrochureMaterial = brochureModel.Visuals[0].Material.Copy();
            lowerkeysBrochureMaterial[MatParamName.DiffuseTex] = Tex.FromFile(rm.GetString("LowerKeysBrochureImage"));
            lowerkeysBrochureModel.Visuals[0].Material = lowerkeysBrochureMaterial;

            keywestBrochureModel = brochureModel.Copy();
            keywestBrochureMaterial = brochureModel.Visuals[0].Material.Copy();
            keywestBrochureMaterial[MatParamName.DiffuseTex] = Tex.FromFile(rm.GetString("KeyWestBrochureImage"));
            keywestBrochureModel.Visuals[0].Material = keywestBrochureMaterial;

            keylargoBrochurePose = originalkeylargoBrochurePose;
            islamoradaBrochurePose = originalislamoradaBrochurePose;
            marathonBrochurePose = originalmarathonBrochurePose;
            lowerkeysBrochurePose = originallowerkeysBrochurePose;
            keywestBrochurePose = originalkeywestBrochurePose;

            return true;
        }

        public void Step()
        {

            Hierarchy.Push(World.BoundsPose.ToMatrix());

            if (UI.Handle("keyLargoBrochure", ref keylargoBrochurePose, brochureBounds))
            {
                Globals.refEventDetailsDisplay.ClearTextDisplay();
                Globals.refEventsMenu.EnableDisplay(false, "");
                timeGrabbedKeyLargo = Time.Totalf;
                keylargoBrochureGrabbed = true;

                if (Globals.DataLoaded)
                {
                    Hierarchy.Push(keylargoBrochurePose.ToMatrix());
                    keylargoBrochureButtonPose.position = Vec3.Zero;
                    keylargoBrochureButtonPose.position.y -= 0.15f;
                    keylargoBrochureButtonPose.orientation = Quat.Identity;

                    UI.WindowBegin("KeyLargoButton", ref keylargoBrochureButtonPose, UIWin.Empty);
                    if (UI.Button("Show me events in Key Largo"))
                    {
                        Globals.refEventsMenu.EnableDisplay(true, "KeyLargo");
                        //This will force the brochure to return to original position immediately upon button push
                        timeGrabbedKeyLargo = 0f;
                    };
                    UI.WindowEnd();
                    Hierarchy.Pop();
                }
            }
            else
            {
                if (keylargoBrochureGrabbed)
                {
                    if ((Time.Totalf - timeGrabbedKeyLargo > waitTimeAfterRelease))
                    {
                        if (timeReturnKeyLargo.Equals(0f))
                        {
                            timeReturnKeyLargo = Time.Totalf;
                        }
                        ReturnToOriginalPosition(KeyLocation.KeyLargo, (Time.Totalf - timeReturnKeyLargo) / timeToLerpReturnBrochure);
                    }
                    else
                    {
                        if (Globals.DataLoaded)
                        {
                            Hierarchy.Push(keylargoBrochurePose.ToMatrix());
                            keylargoBrochureButtonPose.position = Vec3.Zero;
                            keylargoBrochureButtonPose.position.y -= 0.15f;
                            keylargoBrochureButtonPose.orientation = Quat.Identity;

                            UI.WindowBegin("KeyLargoButton", ref keylargoBrochureButtonPose, UIWin.Empty);
                            if (UI.Button("Show me events in Key Largo"))
                            {
                                Globals.refEventsMenu.EnableDisplay(true, "KeyLargo");
                                //This will force the brochure to return to original position immediately upon button push
                                timeGrabbedKeyLargo = 0f;
                            };
                            UI.WindowEnd();
                            Hierarchy.Pop();
                        }
                    }
                }
            }
            keyLargoBrochureModel.Draw(keylargoBrochurePose.ToMatrix());

            if (UI.Handle("islamoradaBrochure", ref islamoradaBrochurePose, brochureBounds))
            {
                Globals.refEventDetailsDisplay.ClearTextDisplay();
                Globals.refEventsMenu.EnableDisplay(false, "");
                timeGrabbedIslamorada = Time.Totalf;
                islamoradaBrochureGrabbed = true;
                if (Globals.DataLoaded)
                {
                    Hierarchy.Push(islamoradaBrochurePose.ToMatrix());

                    islamoradaBrochureButtonPose.position = Vec3.Zero;
                    islamoradaBrochureButtonPose.position.y -= 0.15f;
                    islamoradaBrochureButtonPose.orientation = Quat.Identity;
                    UI.WindowBegin("IslamoradaButton", ref islamoradaBrochureButtonPose, UIWin.Empty);
                    if (UI.Button("Show me events in Islamorada"))
                    {
                        Globals.refEventsMenu.EnableDisplay(true, "Islamorada");
                        //This will force the brochure to return to original position immediately upon button push
                        timeGrabbedIslamorada = 0f;
                    };
                    UI.WindowEnd();
                    Hierarchy.Pop();
                }
            }
            else
            {
                if (islamoradaBrochureGrabbed)
                {

                    if ((Time.Totalf - timeGrabbedIslamorada > waitTimeAfterRelease))
                    {
                        if (timeReturnIslamorada.Equals(0f))
                        {
                            timeReturnIslamorada = Time.Totalf;
                        }
                        ReturnToOriginalPosition(KeyLocation.Islamorada, (Time.Totalf - timeReturnIslamorada) / timeToLerpReturnBrochure);
                    }
                    else
                    {
                        if (Globals.DataLoaded)
                        {
                            Hierarchy.Push(islamoradaBrochurePose.ToMatrix());

                            islamoradaBrochureButtonPose.position = Vec3.Zero;
                            islamoradaBrochureButtonPose.position.y -= 0.15f;
                            islamoradaBrochureButtonPose.orientation = Quat.Identity;
                            UI.WindowBegin("IslamoradaButton", ref islamoradaBrochureButtonPose, UIWin.Empty);
                            if (UI.Button("Show me events in Islamorada"))
                            {
                                Globals.refEventsMenu.EnableDisplay(true, "Islamorada");
                                //This will force the brochure to return to original position immediately upon button push
                                timeGrabbedIslamorada = 0f;
                            };
                            UI.WindowEnd();
                            Hierarchy.Pop();
                        }

                    }
                }
            }
            islamoradaBrochureModel.Draw(islamoradaBrochurePose.ToMatrix());

            if (UI.Handle("marathonBrochure", ref marathonBrochurePose, brochureBounds))
            {
                Globals.refEventDetailsDisplay.ClearTextDisplay();
                Globals.refEventsMenu.EnableDisplay(false, "");
                timeGrabbedMarathon = Time.Totalf;
                marathonBrochureGrabbed = true;
                if (Globals.DataLoaded)
                {
                    Hierarchy.Push(marathonBrochurePose.ToMatrix());

                    marathonBrochureButtonPose.position = Vec3.Zero;
                    marathonBrochureButtonPose.position.y -= 0.15f;
                    marathonBrochureButtonPose.orientation = Quat.Identity;

                    UI.WindowBegin("marathonButton", ref marathonBrochureButtonPose, UIWin.Empty);
                    if (UI.Button("Show me events in Marathon"))
                    {
                        Globals.refEventsMenu.EnableDisplay(true, "Marathon");
                        //This will force the brochure to return to original position immediately upon button push
                        timeGrabbedMarathon = 0f;
                    };
                    UI.WindowEnd();
                    Hierarchy.Pop();
                }

            }
            else
            {
                if (marathonBrochureGrabbed)
                {
                    if ((Time.Totalf - timeGrabbedMarathon > waitTimeAfterRelease))
                    {
                        if (timeReturnMarathon.Equals(0f))
                        {
                            timeReturnMarathon = Time.Totalf;
                        }
                        ReturnToOriginalPosition(KeyLocation.Marathon, (Time.Totalf - timeReturnMarathon) / timeToLerpReturnBrochure);
                    }
                    else
                    {
                        if (Globals.DataLoaded)
                        {
                            Hierarchy.Push(marathonBrochurePose.ToMatrix());

                            marathonBrochureButtonPose.position = Vec3.Zero;
                            marathonBrochureButtonPose.position.y -= 0.15f;
                            marathonBrochureButtonPose.orientation = Quat.Identity;

                            UI.WindowBegin("marathonButton", ref marathonBrochureButtonPose, UIWin.Empty);
                            if (UI.Button("Show me events in Marathon"))
                            {
                                Globals.refEventsMenu.EnableDisplay(true, "Marathon");
                                //This will force the brochure to return to original position immediately upon button push
                                timeGrabbedMarathon = 0f;
                            };
                            UI.WindowEnd();
                            Hierarchy.Pop();
                        }

                    }
                }
            }
            marathonBrochureModel.Draw(marathonBrochurePose.ToMatrix());

            if (UI.Handle("lowerkeysBrochure", ref lowerkeysBrochurePose, brochureBounds))
            {
                Globals.refEventDetailsDisplay.ClearTextDisplay();
                Globals.refEventsMenu.EnableDisplay(false, "");
                timeGrabbedLowerKeys = Time.Totalf;
                lowerkeysBrochureGrabbed = true;
                if (Globals.DataLoaded)
                {
                    Hierarchy.Push(lowerkeysBrochurePose.ToMatrix());

                    lowerkeysBrochureButtonPose.position = Vec3.Zero;
                    lowerkeysBrochureButtonPose.position.y -= 0.15f;
                    lowerkeysBrochureButtonPose.orientation = Quat.Identity;

                    UI.WindowBegin("lowerkeysButton", ref lowerkeysBrochureButtonPose, UIWin.Empty);
                    if (UI.Button("Show me events in the Lower Keys"))
                    {
                        Globals.refEventsMenu.EnableDisplay(true, "LowerKeys");
                        //This will force the brochure to return to original position immediately upon button push
                        timeGrabbedLowerKeys = 0f;
                    };
                    UI.WindowEnd();
                    Hierarchy.Pop();
                }
            }
            else
            {
                if (lowerkeysBrochureGrabbed)
                {
                    if ((Time.Totalf - timeGrabbedLowerKeys > waitTimeAfterRelease))
                    {
                        if (timeReturnLowerKeys.Equals(0f))
                        {
                            timeReturnLowerKeys = Time.Totalf;
                        }
                        ReturnToOriginalPosition(KeyLocation.LowerKeys, (Time.Totalf - timeReturnLowerKeys) / timeToLerpReturnBrochure);
                    }
                    else
                    {
                        if (Globals.DataLoaded)
                        {
                            Hierarchy.Push(lowerkeysBrochurePose.ToMatrix());

                            lowerkeysBrochureButtonPose.position = Vec3.Zero;
                            lowerkeysBrochureButtonPose.position.y -= 0.15f;
                            lowerkeysBrochureButtonPose.orientation = Quat.Identity;

                            UI.WindowBegin("lowerkeysButton", ref lowerkeysBrochureButtonPose, UIWin.Empty);
                            if (UI.Button("Show me events in the Lower Keys"))
                            {
                                Globals.refEventsMenu.EnableDisplay(true, "LowerKeys");
                                //This will force the brochure to return to original position immediately upon button push
                                timeGrabbedLowerKeys = 0f;
                            };
                            UI.WindowEnd();
                            Hierarchy.Pop();
                        }

                    }
                }
            }
            lowerkeysBrochureModel.Draw(lowerkeysBrochurePose.ToMatrix());

            if (UI.Handle("keywestBrochure", ref keywestBrochurePose, brochureBounds))
            {

                Globals.refEventsMenu.EnableDisplay(false, "");
                timeGrabbedKeyWest = Time.Totalf;
                keywestBrochureGrabbed = true;
                if (Globals.DataLoaded)
                {
                    Hierarchy.Push(keywestBrochurePose.ToMatrix());

                    keywestBrochureButtonPose.position = Vec3.Zero;
                    keywestBrochureButtonPose.position.y -= 0.15f;
                    keywestBrochureButtonPose.orientation = Quat.Identity;

                    UI.WindowBegin("keywestButton", ref keywestBrochureButtonPose, UIWin.Empty);
                    if (UI.Button("Show me events in Key West"))
                    {
                        Globals.refEventsMenu.EnableDisplay(true, "KeyWest");
                        //This will force the brochure to return to original position immediately upon button push
                        timeGrabbedKeyWest = 0f;
                    };
                    UI.WindowEnd();
                    Hierarchy.Pop();
                }
            }
            else
            {
                if (keywestBrochureGrabbed)
                {
                    if ((Time.Totalf - timeGrabbedKeyWest > waitTimeAfterRelease))
                    {
                        if (timeReturnKeyWest.Equals(0f))
                        {
                            timeReturnKeyWest = Time.Totalf;
                        }
                        ReturnToOriginalPosition(KeyLocation.KeyWest, (Time.Totalf - timeReturnKeyWest) / timeToLerpReturnBrochure);
                    }
                    else
                    {
                        if (Globals.DataLoaded)
                        {
                            Hierarchy.Push(keywestBrochurePose.ToMatrix());

                            keywestBrochureButtonPose.position = Vec3.Zero;
                            keywestBrochureButtonPose.position.y -= 0.15f;
                            keywestBrochureButtonPose.orientation = Quat.Identity;

                            UI.WindowBegin("keywestButton", ref keywestBrochureButtonPose, UIWin.Empty);
                            if (UI.Button("Show me events in Key West"))
                            {
                                Globals.refEventsMenu.EnableDisplay(true, "KeyWest");
                                //This will force the brochure to return to original position immediately upon button push
                                timeGrabbedKeyWest = 0f;
                            };
                            UI.WindowEnd();
                            Hierarchy.Pop();
                        }

                    }
                }
            }
            keywestBrochureModel.Draw(keywestBrochurePose.ToMatrix());

            Hierarchy.Pop();

        }

        void ReturnToOriginalPosition(KeyLocation thisKeyLocation, float percentage)
        {

            switch (thisKeyLocation)
            {
                case KeyLocation.KeyLargo:

                    keylargoBrochurePose = Pose.Lerp(keylargoBrochurePose, originalkeylargoBrochurePose, percentage);
                    if (percentage > 1.0f)
                    {
                        keylargoBrochureGrabbed = false;
                        timeReturnKeyLargo = 0f;
                        keylargoBrochurePose = originalkeylargoBrochurePose;
                    }
                    break;

                case KeyLocation.Islamorada:

                    islamoradaBrochurePose = Pose.Lerp(islamoradaBrochurePose, originalislamoradaBrochurePose, percentage);
                    if (percentage > 1.0f)
                    {
                        islamoradaBrochureGrabbed = false;
                        timeReturnIslamorada = 0f;
                        islamoradaBrochurePose = originalislamoradaBrochurePose;
                    }
                    break;

                case KeyLocation.Marathon:

                    marathonBrochurePose = Pose.Lerp(marathonBrochurePose, originalmarathonBrochurePose, percentage);
                    if (percentage > 1.0f)
                    {
                        marathonBrochureGrabbed = false;
                        timeReturnMarathon = 0f;
                        marathonBrochurePose = originalmarathonBrochurePose;
                    }
                    break;

                case KeyLocation.LowerKeys:

                    lowerkeysBrochurePose = Pose.Lerp(lowerkeysBrochurePose, originallowerkeysBrochurePose, percentage);
                    if (percentage > 1.0f)
                    {
                        lowerkeysBrochureGrabbed = false;
                        timeReturnLowerKeys = 0f;
                        lowerkeysBrochurePose = originallowerkeysBrochurePose;
                    }
                    break;

                case KeyLocation.KeyWest:

                    keywestBrochurePose = Pose.Lerp(keywestBrochurePose, originalkeywestBrochurePose, percentage);
                    if (percentage > 1.0f)
                    {
                        keywestBrochureGrabbed = false;
                        timeReturnKeyWest = 0f;
                        keywestBrochurePose = originalkeywestBrochurePose;
                    }
                    break;

            }
        }


        public void Shutdown()
        { }
    }
}