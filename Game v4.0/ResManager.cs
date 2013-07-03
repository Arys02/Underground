using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using SharpDX.Windows;


namespace LOL_l
{
    public struct structTexture
    {
        public Texture Texture;
        public string path;
        public structTexture(string path, ref Device device)
        {
            this.Texture = Texture.FromFile(device, path);
            this.path = path;
        }
    }
    public struct structBinary
    {
        public string path;
        public byte[] Data;
        public structBinary(string path, byte[] Data)
        {
            this.path = path;
            this.Data = Data;
        }
    }
    public static class ResManager
    {
        public static List<structTexture> ListTextures = new List<structTexture>();
        public static List<structBinary> ListBinaries = new List<structBinary>();
        public static Effect Basic_Effect;

        public static void Initialize(ref Device device, ref RenderForm form)
        {
            Macro macro = new Macro("nblights", 2.ToString());
            Basic_Effect = Effect.FromFile(device, "Effect.fx", new Macro[] { macro }, null, "", ShaderFlags.OptimizationLevel3);
            Basic_Effect.Technique = Basic_Effect.GetTechnique(0);
            Basic_Effect.SetValue("AmbientLightColor", new Vector4(0f, 0f, 0f, 0f));
            Matrix proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 7000.0f);
        }
        public static void Dispose()
        {
            Basic_Effect.Dispose();
            foreach (structTexture texture in ListTextures)
            {
                texture.Texture.Dispose();
            }
        }
        public static int getTexture(string filename, ref Device device, bool it_is_bumpTexture)
        {
            if (filename == "")
            {
                if (it_is_bumpTexture) filename = "null_NRM.png";
                else filename = "null.bmp";
            }
            for (int i = 0; i < ListTextures.Count; i++)
            {
                if (ListTextures[i].path == filename)
                {
                    return i;
                }
            }
            ListTextures.Add(new structTexture(filename, ref device));
            return ListTextures.Count - 1;
        }
        public static int getBinary(string filename)
        {
            for (int i = 0; i < ListBinaries.Count; i++)
            {
                if (ListBinaries[i].path == filename)
                {
                    return i;
                }
            }
            ListBinaries.Add(new structBinary(filename, File.ReadAllBytes(filename)));
            return ListBinaries.Count - 1;
        }
    }
}
