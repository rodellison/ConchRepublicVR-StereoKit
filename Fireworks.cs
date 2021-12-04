using StereoKit;
using StereoKit.Framework;
using System;

namespace VRConchRepublic
{
    public class Fireworks : IStepper
    {
        readonly Random rand = new Random();

        public bool Enabled => false;
        private static readonly int spriteCount = 30;
        readonly float animationDelay = 0.035f;

        Mesh blueFireworkMesh;
        Mesh yellowFireworkMesh;
        Mesh pinkFireworkMesh;

        Material[] blueFireworkMaterials = new Material[spriteCount];
        Material[] yellowFireworkMaterials = new Material[spriteCount];
        Material[] pinkFireworkMaterials = new Material[spriteCount];
        int blueFireworkIndex;
        int yellowFireworkIndex;
        int pinkFireworkIndex;

        bool fireworksAvailable;

        bool blueFireworkPlaying;
        bool yellowFireworkPlaying;
        bool pinkFireworkPlaying;

        float blueFireworkSize;
        float yellowFireworkSize;
        float pinkFireworkSize;

        float blueFireworkX;
        float yellowFireworkX;
        float pinkFireworkX;
        float blueFireworkY;
        float yellowFireworkY;
        float pinkFireworkY;

        float timeToNextBlueFirework;
        float timeBlueFirework;
        float timeLastBlueFireworkFrame;
        float timeToNextYellowFirework;
        float timeLastYellowFireworkFrame;
        float timeYellowFirework;
        float timeToNextPinkFirework;
        float timeLastPinkFireworkFrame;
        float timePinkFirework;

        Material baseFireworksMaterial = Default.Material.Copy();

        public bool Initialize()
        {
            baseFireworksMaterial.Transparency = Transparency.Blend;

            blueFireworkMesh = Mesh.Quad;
            yellowFireworkMesh = Mesh.Quad;
            pinkFireworkMesh = Mesh.Quad;


            for (int i = 0; i < spriteCount; i++)
            {
                blueFireworkMaterials[i] = baseFireworksMaterial.Copy();
                blueFireworkMaterials[i][MatParamName.DiffuseTex] = Tex.FromFile("Fireworks/Blue/tile" + i.ToString("D3") + ".png");
                yellowFireworkMaterials[i] = baseFireworksMaterial.Copy();
                yellowFireworkMaterials[i][MatParamName.DiffuseTex] = Tex.FromFile("Fireworks/Yellow/tile" + i.ToString("D3") + ".png");
                pinkFireworkMaterials[i] = baseFireworksMaterial.Copy();
                pinkFireworkMaterials[i][MatParamName.DiffuseTex] = Tex.FromFile("Fireworks/Pink/tile" + i.ToString("D3") + ".png");
            }

            ResetNextBlueFirework();
            ResetNextYellowFirework();
            ResetNextPinkFirework();

            fireworksAvailable = true;

            return true;

        }
        private void ResetNextBlueFirework()
        {
            blueFireworkPlaying = false;
            timeToNextBlueFirework = (float)rand.Next(5, 10);
            blueFireworkSize = (float)rand.Next(50, 100);
            blueFireworkX = (float)rand.Next(-30, 30);
            blueFireworkY = (float)rand.Next(80, 125);
            timeBlueFirework = Time.Totalf;
        }
        private void ResetNextYellowFirework()
        {
            yellowFireworkPlaying = false;
            timeToNextYellowFirework = (float)rand.Next(5, 10);
            yellowFireworkSize = (float)rand.Next(50, 100);
            yellowFireworkX = (float)rand.Next(-30, 30);
            yellowFireworkY = (float)rand.Next(80, 125);
            timeYellowFirework = Time.Totalf;
        }
        private void ResetNextPinkFirework()
        {
            pinkFireworkPlaying = false;
            pinkFireworkSize = (float)rand.Next(50, 100);
            pinkFireworkX = (float)rand.Next(-30, 30);
            pinkFireworkY = (float)rand.Next(80, 125);
            timeToNextPinkFirework = (float)rand.Next(5, 10);

            timePinkFirework = Time.Totalf;
        }
        void DrawBlueFireworks()
        {
            Hierarchy.Push(Globals.shipMatrix);
            var positionToPlay = new Vec3(blueFireworkX, blueFireworkY, 5f);
            Globals.refBackgroundSound.PlayOnceShotFireworks(Hierarchy.ToWorld(positionToPlay), 1f, "Blue");
            blueFireworkMesh.Draw(blueFireworkMaterials[blueFireworkIndex],
                Matrix.TRS(positionToPlay,
                Quat.Identity, blueFireworkSize));
            Hierarchy.Pop();
        }
        void DrawYellowFireworks()
        {
            Hierarchy.Push(Globals.shipMatrix);
            var positionToPlay = new Vec3(yellowFireworkX, yellowFireworkY, 10f);
            Globals.refBackgroundSound.PlayOnceShotFireworks(Hierarchy.ToWorld(positionToPlay), 1f, "Yellow");
            yellowFireworkMesh.Draw(yellowFireworkMaterials[yellowFireworkIndex],
                Matrix.TRS(positionToPlay,
                Quat.Identity, yellowFireworkSize));
            Hierarchy.Pop();
        }
        void DrawPinkFireworks()
        {
            Hierarchy.Push(Globals.shipMatrix);
            var positionToPlay = new Vec3(pinkFireworkX, pinkFireworkY, 15f);
            Globals.refBackgroundSound.PlayOnceShotFireworks(Hierarchy.ToWorld(positionToPlay), 1f, "Pink");

            pinkFireworkMesh.Draw(pinkFireworkMaterials[pinkFireworkIndex],
                Matrix.TRS(positionToPlay,
                Quat.Identity, pinkFireworkSize));
            Hierarchy.Pop();
        }

        public void Step()
        {

            if (fireworksAvailable)
            {

                Hierarchy.Push(World.BoundsPose.ToMatrix());

                if (!blueFireworkPlaying)
                {
                    if ((Time.Totalf - timeBlueFirework > timeToNextBlueFirework))
                    {
                        timeLastBlueFireworkFrame = Time.Totalf;
                        blueFireworkIndex = 0;
                        blueFireworkPlaying = true;
                        DrawBlueFireworks();
                    }
                }
                else
                {
                    if (Time.Totalf - timeLastBlueFireworkFrame > animationDelay)
                    {
                        timeLastBlueFireworkFrame = Time.Totalf;
                        if (blueFireworkIndex + 1 < spriteCount)
                        {
                            blueFireworkIndex += 1;
                            DrawBlueFireworks();
                        }
                        else
                        {
                            ResetNextBlueFirework();
                        }
                    }
                    else
                    {
                        DrawBlueFireworks();
                    }

                }

                if (!yellowFireworkPlaying)
                {
                    if ((Time.Totalf - timeYellowFirework > timeToNextYellowFirework))
                    {
                        timeLastYellowFireworkFrame = Time.Totalf;
                        yellowFireworkIndex = 0;
                        yellowFireworkPlaying = true;
                        DrawYellowFireworks();

                    }
                }
                else
                {
                    if (Time.Totalf - timeLastYellowFireworkFrame > animationDelay)
                    {
                        timeLastYellowFireworkFrame = Time.Totalf;
                        if (yellowFireworkIndex + 1 < spriteCount)
                        {
                            yellowFireworkIndex += 1;
                            DrawYellowFireworks();
                        }
                        else
                        {
                            ResetNextYellowFirework();
                        }
                    }
                    else
                    {
                        DrawYellowFireworks();
                    }

                }

                if (!pinkFireworkPlaying)
                {
                    if ((Time.Totalf - timePinkFirework > timeToNextPinkFirework))
                    {
                        timeLastPinkFireworkFrame = Time.Totalf;
                        pinkFireworkIndex = 0;
                        pinkFireworkPlaying = true;
                        DrawPinkFireworks();

                    }
                }
                else
                {
                    if (Time.Totalf - timeLastPinkFireworkFrame > animationDelay)
                    {
                        timeLastPinkFireworkFrame = Time.Totalf;
                        if (pinkFireworkIndex + 1 < spriteCount)
                        {
                            pinkFireworkIndex += 1;
                            DrawPinkFireworks();
                        }
                        else
                        {
                            ResetNextPinkFirework();
                        }
                    }
                    else
                    {
                        DrawPinkFireworks();
                    }

                }

                Hierarchy.Pop();
            }
        }


        public void Shutdown()
        { }
    }
}