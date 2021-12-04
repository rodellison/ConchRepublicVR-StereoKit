
using StereoKit;
using StereoKit.Framework;
using System;


namespace VRConchRepublic
{
    public class BackgroundSound : IStepper
    {

        Sound wavesSound;
        SoundInst wavesSoundInst;
        Sound musicSound;
        SoundInst musicSoundInst;
        Sound blueFireworkSound;
        Sound yellowFireworkSound;
        Sound pinkFireworkSound;

        public bool Enabled => true;
        public bool Initialize()
        {
            wavesSound = Sound.FromFile("Smallwavesbeach.mp3");
            musicSound = Sound.FromFile("happy-steeldrum.mp3");
            blueFireworkSound = Sound.FromFile("FireWorks.mp3");
            yellowFireworkSound = Sound.FromFile("FireWorks2.mp3");
            pinkFireworkSound = Sound.FromFile("FireWorks3.mp3");

            return true;
        }

        public void PlayOnceShotFireworks(Vec3 fireworkPosition, float Volume, string fireworkColor)
        {
            //Trying to find a position that is in the direction of, but a bit closer than the firework.
            var soundPosition = Vec3.Lerp(Vec3.Zero, fireworkPosition, 0.055f);
            switch (fireworkColor)
            {
                case "Blue":
                    blueFireworkSound.Play(soundPosition, Volume);
                    break;
                case "Yellow":
                    yellowFireworkSound.Play(soundPosition, Volume);
                    break;
                case "Pink":
                    pinkFireworkSound.Play(soundPosition, Volume);
                    break;

            }
        }


        public void Step()
        {

            if (!wavesSoundInst.IsPlaying)
            {
                wavesSoundInst = wavesSound.Play(Vec3.Zero, 0.75f);
            }
            if (!musicSoundInst.IsPlaying)
            {
                musicSoundInst = musicSound.Play(Vec3.Zero, 0.75f);
            }

            Hierarchy.Push(World.BoundsPose.ToMatrix());

            //Move the Ocean Waves sound in a figure8 pattern
            //From: https://gamedev.stackexchange.com/questions/43691/how-can-i-move-an-object-in-an-infinity-or-figure-8-trajectory
            //scale = 2 / (3 - cos(2 * t));
            //x = scale * cos(t);
            //y = scale * sin(2 * t) / 2;

            float scale = 2 / (3 - (float)Math.Cos(2 * Time.Totalf));
            float x = scale * (float)Math.Cos(Time.Totalf);
            float z = scale * (float)Math.Sin(2 * Time.Totalf) / 2;
            Vec3 atWaveSound = V.XYZ(x, 1.5f, z);
            wavesSoundInst.Position = Hierarchy.ToWorld(atWaveSound);
            //Helper to see where the sound is being 'drawn'
            //   Default.MeshSphere.Draw(Default.Material, Matrix.TS(atWaveSound, 0.2f));

            //Move the music in a circular pattern
            float musicScale = 1.5f;
            Vec3 atMusicSound = V.XYZ((float)Math.Sin(Time.Totalf) * musicScale, 3, (float)Math.Cos(Time.Totalf) * musicScale);
            musicSoundInst.Position = Hierarchy.ToWorld(atMusicSound);

            //Helper to see where the sound is being 'drawn'
            //   Default.MeshSphere.Draw(Default.Material, Matrix.TS(atMusicSound, 0.2f));

            Hierarchy.Pop();

        }

        public void Shutdown()
        {
            wavesSoundInst.Stop();
            musicSoundInst.Stop();

        }
    }
}