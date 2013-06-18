using System;
using System.Diagnostics;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;
using Color = SharpDX.Color;

namespace Underground
{
    class MtlLoader
    {
        #region Structure .mtl
        public bool Ka = false;
        public bool Kd = false;
        public bool Ks = false;
        public bool map_Kd = false;
        public bool map_Ns = false;
        #endregion

        public List<structMTL> MTLData = new List<structMTL>();

        public void read_mtl(string filename, string path)
        {
            MTLData.Clear();
            byte[] mtl = File.ReadAllBytes(path + filename);
            int i = 0;
            bool on_en_a_deja_trouve_un = false;
            string type;
            string abc;
            float x, y, z;
            structMTL material = new structMTL("", Color.Zero, Color.Zero, Color.Zero, "null.bmp", "null.bmp");
            while (i < mtl.Length)
            {
                type = ObjLoader.getstring(ref i, ref mtl);
                if (type == "newmtl")
                {
                    i++; // Espace
                    if (on_en_a_deja_trouve_un)
                    {
                        MTLData.Add(material);
                        material = new structMTL("", Color.Zero, Color.Zero, Color.Zero, "null.bmp", "null.bmp");
                    }
                    else on_en_a_deja_trouve_un = true;
                    material.name = ObjLoader.getstring(ref i, ref mtl); // On recupère le nom

                    Program.WriteNicely("\t#", 4, "Nouveau MTL" + material.name);
                }
                else if (type == "Ka")
                {
                    Ka = true;
                    i++; // Espace
                    x = ObjLoader.getfloat(ref i, ref mtl);
                    i++; // Espace
                    y = ObjLoader.getfloat(ref i, ref mtl);
                    i++; // Espace
                    z = ObjLoader.getfloat(ref i, ref mtl);

                    Program.WriteNicely("\t#", 4, "Nouveau Ka " + x + " " + y + " " + z);

                    material.Ka = new Color(new Vector4(x, y, z, 1));
                }
                else if (type == "Kd")
                {
                    Kd = true;
                    i++; // Espace
                    x = ObjLoader.getfloat(ref i, ref mtl);
                    i++; // Espace
                    y = ObjLoader.getfloat(ref i, ref mtl);
                    i++; // Espace
                    z = ObjLoader.getfloat(ref i, ref mtl);

                    Program.WriteNicely("\t#", 4, "Nouveau Kd " + x + " " + y + " " + z);

                    material.Kd = new Color(new Vector4(x, y, z, 1));
                }
                else if (type == "Ks")
                {
                    Ks = true;
                    i++; // Espace
                    x = ObjLoader.getfloat(ref i, ref mtl);
                    i++; // Espace
                    y = ObjLoader.getfloat(ref i, ref mtl);
                    i++; // Espace
                    z = ObjLoader.getfloat(ref i, ref mtl);

                    Program.WriteNicely("\t#", 4, "Nouveau Ks " + x + " " + y + " " + z);

                    material.Ks = new Color(new Vector4(x, y, z, 1));
                }
                else if (type == "map_Kd")
                {
                    map_Kd = true;
                    i++; // Espace
                    abc = ObjLoader.getstring(ref i, ref mtl);

                    Program.WriteNicely("\t#", 4, "Nouveau map_Kd " + path + abc);
                    material.map_Kd = path + abc;
                    Program.getTexture(material.map_Kd);
                    //Program.Liste_textures.Add(new Texturestruct(material.map_Kd, Texture.FromFile(Program.device, material.map_Kd)));
                }
                else if (type == "map_Ns")
                {
                    map_Ns = true;
                    i++; // Espace
                    abc = ObjLoader.getstring(ref i, ref mtl);
                    Program.WriteNicely("\t#", 4, "Nouveau map_Kd " + path + abc);
                    material.map_Ns = path + abc;
                    Program.getTexture(material.map_Ns);
                }
                else if (type == "") { }
                else if (type[0] == '#')
                {
                    Program.WriteNicely("\t#", 4, "Nouveau commentaire");
                }
                else
                {
                    Program.WriteNicely("\t!", 2, "Type inconnu");
                }
                ObjLoader.gotonextline(ref i, ref mtl);
            }
            MTLData.Add(material);
        }
    }
}
