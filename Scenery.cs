using StereoKit;
using StereoKit.Framework;
using System;

namespace VRConchRepublic
{
    public class Scenery : IStepper
    {
        private const float shipY = 0.75f;
        readonly Random rand = new Random();

        readonly Pose bambooBeachChairPose = new Pose(-2f, 0.35f, -1.5f, Quat.FromAngles(0f, -35f, 0f));
        readonly Pose bambooBeachChair2Pose = new Pose(2f, 0.35f, -1.5f, Quat.FromAngles(0f, 35f, 0f));

        readonly Pose beachPose = new Pose(0, 0.35f, 0f, Quat.Identity);
        Model beachModel;
        Model shipModel;
        readonly Pose beachTablePose = new Pose(0, 0.35f, 0f, Quat.Identity);
        Model beachTableModel;
        Pose signPostPose;
        Model signPostModel;

        private readonly int numPalmTrees = 50;
        private readonly int numBananaTrees = 30;

        Model PalmTreeModel;
        Model BananaTreeModel;
        Model BambooBeachChairModel;
        Model BambooBeachChairModel2;
        Model[] palmTreeModels;
        Model[] bananaTreeModels;

        Pose[] palmTreeModelsPose;
        Matrix[] palmTreeModelsMatrix;
        Pose[] bananaTreeModelsPose;
        Matrix[] bananaTreeModelsMatrix;

        readonly double rangeMax = 1f;
        readonly double rangeMin = 0.25f;
        readonly double rangeBananaScaleMax = 3.5f;
        readonly double rangeBananaScaleMin = 2f;

        double range;
        double range2;
        public bool Enabled => false;
        public bool Initialize()
        {
            palmTreeModels = new Model[numPalmTrees];
            palmTreeModelsPose = new Pose[numPalmTrees];
            palmTreeModelsMatrix = new Matrix[numPalmTrees];

            bananaTreeModels = new Model[numBananaTrees];
            bananaTreeModelsPose = new Pose[numBananaTrees];
            bananaTreeModelsMatrix = new Matrix[numBananaTrees];

            range = rangeMax - rangeMin;
            range2 = rangeBananaScaleMax - rangeBananaScaleMin;

            // Set up some nice lighting, and a background.
            Renderer.SkyTex = Tex.FromCubemapEquirectangular("001.png", out SphericalHarmonics lighting);
            Renderer.SkyTex.AddressMode = TexAddress.Clamp;
            //A bit higher value for png, instead of .hdr
            lighting.Brightness(15f);
            Renderer.SkyLight = lighting;

            Renderer.SetClip(0.08f, 76f);

            beachModel = Model.FromFile("Beach.glb");
            BananaTreeModel = Model.FromFile("Banana_Plant_01.glb");
            PalmTreeModel = Model.FromFile("PalmTree2.glb");
            beachTableModel = Model.FromFile("BeachTable.glb");
            BambooBeachChairModel = Model.FromFile("beach_chair.glb");
            BambooBeachChairModel2 = BambooBeachChairModel.Copy();

            for (int i = 0; i < numPalmTrees; i++)
            {
                palmTreeModels[i] = PalmTreeModel.Copy();
                var randNegPos = rand.Next(0, 100) > 50 ? 1 : -1;
                var x = (float)rand.Next(5, 50) * randNegPos;
                var z = (float)rand.Next(-50, 50);

                var thisPostion = new Vec3(x, 0, z);
                thisPostion.y = GetRangeCorrectedYPosition(thisPostion, 0, 50, 0f, -1.25f);

                palmTreeModelsPose[i] = new Pose(thisPostion, Quat.FromAngles((float)rand.Next(-10, 10), (float)rand.Next(-180, 180), (float)rand.Next(-5, 5)));
                palmTreeModelsMatrix[i] = palmTreeModelsPose[i].ToMatrix(GetRandomScaleValue());

                var anim = rand.Next(0, 100) > 50 ? "PalmSway1" : "PalmSway2";
                palmTreeModels[i].PlayAnim(anim, AnimMode.Loop);

            }
            for (int i = 0; i < numBananaTrees; i++)
            {
                bananaTreeModels[i] = BananaTreeModel.Copy();
                var randNegPos = rand.Next(0, 100) > 50 ? 1 : -1;
                var x = (float)rand.Next(8, 35) * randNegPos;
                var z = (float)rand.Next(-50, 50);

                var thisPostion = new Vec3(x, 0, z);
                thisPostion.y = GetRangeCorrectedYPosition(thisPostion, 0, 50, 0.5f, -1f);

                bananaTreeModelsPose[i] = new Pose(thisPostion, Quat.FromAngles((float)rand.Next(-10, 10), (float)rand.Next(-180, 180), (float)rand.Next(-5, 5)));
                bananaTreeModelsMatrix[i] = bananaTreeModelsPose[i].ToMatrix(GetRandomScaleValue2());

                bananaTreeModels[i].PlayAnim("BananaTreeSway", AnimMode.Loop);

            }

            shipModel = Model.FromFile("ship.glb");
            shipModel.PlayAnim("BoatRock", AnimMode.Loop);
            signPostModel = Model.FromFile("SignPost.glb");
            signPostPose = new Pose(-4.0f, 0.25f, -3f, Quat.FromAngles(new Vec3((float)rand.Next(-10, 10), -115f, (float)rand.Next(-5, 5))));

            return true;

        }

        private float GetRangeCorrectedYPosition(Vec3 thisPostion, float distanceMin, float distanceMax, float yMin, float yMax)
        {
            //This routine comes from: https://stackoverflow.com/questions/14224535/scaling-between-two-number-ranges

            //The beach slopes down as distance increases from the viewer (in all directions).
            //So, we want the trees Y position to go down the further they get created from the viewer.. (so they dont appear to be floating in the distance)

            float distanceSquared = Vec3.DistanceSq(Vec3.Zero, thisPostion);
            var actualDistance = SKMath.Sqrt(distanceSquared);
            var percent = (actualDistance - distanceMin) / (distanceMax - distanceMin);
            return percent * (yMax - yMin) + yMin;

        }


        public void Step()
        {
            Hierarchy.Push(World.BoundsPose.ToMatrix());

            beachModel.Draw(beachPose.ToMatrix());
            beachTableModel.Draw(beachTablePose.ToMatrix());
            signPostModel.Draw(signPostPose.ToMatrix());
            BambooBeachChairModel.Draw(bambooBeachChairPose.ToMatrix());
            BambooBeachChairModel2.Draw(bambooBeachChair2Pose.ToMatrix());

            for (int i = 0; i < numPalmTrees; i++)
            {
                palmTreeModels[i].Draw(palmTreeModelsMatrix[i]);
            }
            for (int i = 0; i < numBananaTrees; i++)
            {
                bananaTreeModels[i].Draw(bananaTreeModelsMatrix[i]);
            }

            //Move the boat in a circular pattern around the island
            float shipRotateScale = -65f;
            float shipSpeedFactor = 0.003f;
            Vec3 atShipPosition = V.XYZ((float)Math.Sin(Time.Totalf * shipSpeedFactor) * shipRotateScale, shipY, (float)Math.Cos(Time.Totalf * shipSpeedFactor) * shipRotateScale);
            Quat rotBoat = Quat.LookAt(atShipPosition, new Vec3(0, 1, 0));
            Globals.shipMatrix = Matrix.TRS(atShipPosition, rotBoat, 0.1f);
            shipModel.Draw(Globals.shipMatrix);

            Hierarchy.Pop();
        }

        private float GetRandomScaleValue()
        {
            double sample = rand.NextDouble();
            double scaled = (sample * range) + rangeMin;
            return (float)scaled;
        }
        private float GetRandomScaleValue2()
        {
            double sample = rand.NextDouble();
            double scaled = (sample * range2) + rangeBananaScaleMin;
            return (float)scaled;
        }

        public void Shutdown()
        { }
    }
}