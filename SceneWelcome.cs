using StereoKit;
using StereoKit.Framework;

namespace VRConchRepublic
{
    class SceneWelcome : IStepper
    {
        Sprite logo;
        float logoHalfWidth;

        public bool Enabled => true;

        public bool Initialize()
        {
            logo = Sprite.FromFile("welcome.png", SpriteType.Single);
            logoHalfWidth = logo.Aspect * 0.5f;
            Renderer.SkyTex = Tex.FromCubemapEquirectangular("black.png", out SphericalHarmonics lighting);
            Renderer.SkyLight = lighting;

            return true;
        }

        public void Shutdown()
        {
        }

        public void Step()
        {
            Hierarchy.Push(World.BoundsPose.ToMatrix());
            Hierarchy.Push(Matrix.T(0, 1f, -2f));
            logo.Draw(Matrix.TRS(V.XYZ(logoHalfWidth * 2, 1.5f, 0), Quat.LookDir(0, 0, 1), V.XYZ(logo.Aspect, 1, 1) * 2f));
            Hierarchy.Pop();
            Hierarchy.Pop();
        }


    }
}
