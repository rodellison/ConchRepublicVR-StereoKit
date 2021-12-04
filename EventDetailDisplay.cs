using StereoKit;
using StereoKit.Framework;
using System;
using System.Resources;
using VRConchRepublic.Data;

namespace VRConchRepublic
{
    public class EventDetailDisplay : IStepper
    {

        ResourceManager rm;
        int generalImageCount;
        int generalImageIndex;
        Tex randomGeneralImage;

        Matrix tempMatrix;
        Matrix tempImageMatrix;
        readonly Mesh backplate = Mesh.GenerateCube(new Vec3(1.75f, 1.3f, 0.05f));
        private Pose backplatePose = new Pose(0.89f, 1.725f, -2.2f, Quat.FromAngles(0f, -200f, 0f));
        private Matrix titlePose = Matrix.TRS(V.XYZ(0f, 0.6f, -0.1f), Quat.Identity, 3);
        private Matrix descPose = Matrix.TRS(V.XYZ(0f, 0.3f, -0.1f), Quat.Identity, 2);

        //Making the image box Vec3.one so that the UV is stretched to cover the side..
        readonly Mesh randomImageBox = Mesh.GenerateCube(Vec3.One);
        private Pose imagePose = new Pose(-0.82f, 1.725f, -2.2f, Quat.FromAngles(0f, 20f, 0f));
        private Matrix attributionPose = Matrix.TRS(V.XYZ(-0.5f, 0.5f, 0.75f), Quat.LookDir(0, 0, 1), 1f);

        float targetScale;
        bool scaleUp;
        bool fullyScaled;

        TextStyle titleStyle;
        TextStyle descriptionStyle;
        TextStyle attributionStyle;

        string eventTitle = "";
        string eventDescription = "";
        string attributionText = "";

        bool displayEnabled;
        Material backplateMaterial;
        Material imageMaterial;

        readonly Font displayFont = Font.FromFile("comic.ttf");

        //Blue
        Color textColor = Color.HSV(240 / 360f, 100 / 100f, 100 / 100f);
        //Transparent White
        Color backplateColor = new Color(0.75f, 0.75f, 0.75f, 0.75f);
        //White
        Color attributionTextColor = Color.White;


        float startingTime;
        readonly float timeToLerp = 0.5f;
        public bool Enabled => true;

        public bool Initialize()
        {

            rm = new ResourceManager(typeof(ConchRepublicStrings));
            try
            {
                if (rm == null)
                    Log.Err("Could not load Resource Manager to obtain GeneralImages");
                else
                {
                    generalImageCount = int.Parse(rm.GetString("GeneralImageCount"));
                    Log.Info("General Image Count = " + generalImageCount);
                    generalImageIndex = 0;
                }

            }
            catch (Exception e)
            {
                Log.Err("Error in loading Resource Manager in EventDetailDisplay: " + e.Message);
                generalImageCount = 0;
                generalImageIndex = 0;
            }


            //Using values from this site: https://alloyui.com/examples/color-picker/hsv.html
            //H is value from 0 to 359   degrees
            //S is value from 0 to 100   percent
            //V is value from 0 to 100   percent

            titleStyle = Text.MakeStyle(Default.Font, 2 * U.cm, textColor);
            descriptionStyle = Text.MakeStyle(Default.Font, 2 * U.cm, textColor);
            attributionStyle = Text.MakeStyle(Default.Font, 2 * U.cm, attributionTextColor);

            if (displayFont != null)
            {
                titleStyle = Text.MakeStyle(displayFont, 2 * U.cm, textColor);
                descriptionStyle = Text.MakeStyle(displayFont, 2 * U.cm, textColor);
                attributionStyle = Text.MakeStyle(displayFont, 2 * U.cm, attributionTextColor);
            }
            displayEnabled = false;

            backplateMaterial = Default.Material.Copy();
            imageMaterial = Default.Material.Copy();

            return true;

        }

        public void Shutdown()
        {
        }

        public void ClearTextDisplay()
        {
            //This will trigger scale down in the Step method.
            startingTime = Time.Totalf;
            fullyScaled = false;

        }

        public Tex GetNextImage()
        {
            if (generalImageCount > 0)
            {
                if (generalImageIndex + 1 == generalImageCount)
                {
                    generalImageIndex = 0;
                }

                try
                {
                    Log.Info("Setting sprite image path ");
                    string imageName = rm.GetString("GeneralImage" + generalImageIndex);
                    attributionText = "Photo by " + rm.GetString("Attribution" + generalImageIndex);
                    var imagePath = "GeneralImages/" + imageName;

                    Log.Info("Attempting to load image resource: " + imagePath);
                    generalImageIndex += 1;

                    return Tex.FromFile(imagePath);
                }
                catch (ArgumentException e)
                {
                    Log.Err("Arg Exception: " + e.Message);
                }
                catch (InvalidOperationException e)
                {
                    Log.Err("Invalid Op Exception: " + e.Message);
                }
                catch (MissingManifestResourceException e)
                {
                    Log.Err("Missing Manifest Resource Exception: " + e.Message);
                }

                return null;
            }
            else
                return null;

        }

        public string ConvertDateString(string dateToConvert)
        {
            return string.Format("{0}/{1}/{2}", dateToConvert.Substring(4, 2), dateToConvert.Substring(6, 2), dateToConvert.Substring(0, 4));
        }

        public void SetTextDisplay(EventDataDetails eventDataDetails)
        {
            eventTitle = eventDataDetails.eventName;
            eventDescription = "From " +
                ConvertDateString(eventDataDetails.eventStartDate) +
                " to " +
                ConvertDateString(eventDataDetails.eventEndDate) + "\n\n" +
                eventDataDetails.eventDescription;


            randomGeneralImage = GetNextImage();
            if (randomGeneralImage != null)
            {
                imageMaterial[MatParamName.DiffuseTex] = randomGeneralImage;
            }

            //If the display is enabled, then we've already scaled up
            if (!displayEnabled)
            {
                //This triggers the scale up in the step method
                startingTime = Time.Totalf;
                displayEnabled = true;
                fullyScaled = false;
                scaleUp = true;

            }

        }

        public void Step()
        {

            if (!fullyScaled)
            {
                if (scaleUp)
                {
                    ScaleUpBackplate((Time.Totalf - startingTime) / timeToLerp);
                }
                else
                {
                    ScaleDownBackplate((Time.Totalf - startingTime) / timeToLerp);
                }

            }


            if (displayEnabled)
            {
                Hierarchy.Push(World.BoundsPose.ToMatrix());

                if (randomGeneralImage != null)
                {
                    tempImageMatrix = imagePose.ToMatrix(new Vec3(1.75f * targetScale, 1.3f * targetScale, 0.05f * targetScale));
                    randomImageBox.Draw(imageMaterial, tempImageMatrix);

                    Hierarchy.Push(tempImageMatrix);
                    Text.Add(attributionText, attributionPose, attributionStyle, TextAlign.XLeft | TextAlign.YTop, TextAlign.TopLeft | TextAlign.YTop, -0.01f, -0.01f);
                    Hierarchy.Pop();

                }


                tempMatrix = backplatePose.ToMatrix(targetScale);
                // Draw a backplate to create some contrast with the scene text
                backplate.Draw(backplateMaterial, tempMatrix, backplateColor);

                //Push the tempMatrix (backplate) into the hierarchy, so that the 'text' items will use it as their parent.
                Hierarchy.Push(tempMatrix);

                Text.Add(eventTitle, titlePose, V.XY(0.5f, 0), TextFit.Wrap, titleStyle, TextAlign.XCenter | TextAlign.YTop, TextAlign.XCenter | TextAlign.YTop);
                Text.Add(eventDescription, descPose, V.XY(0.7f, 0), TextFit.Wrap, descriptionStyle, TextAlign.XCenter | TextAlign.YTop, TextAlign.XCenter | TextAlign.YTop);

                Hierarchy.Pop();
                Hierarchy.Pop();
            }

        }

        public void ScaleUpBackplate(float percentage)
        {
            targetScale = SKMath.Lerp(0f, 1f, percentage);
            if (percentage > 1.0f)
            {
                fullyScaled = true;
                scaleUp = false;
            }

        }
        public void ScaleDownBackplate(float percentage)
        {
            targetScale = SKMath.Lerp(1f, 0f, percentage);
            if (percentage > 1.0f)
            {
                fullyScaled = true;
                displayEnabled = false;
            }

        }

    }
}
