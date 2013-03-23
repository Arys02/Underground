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
        #endregion
        
        public List<mtlstruct> MTLData = new List<mtlstruct>();

        public void read_mtl(string filename, string path)
        {
            byte[] mtl = File.ReadAllBytes(path + filename);
            int i = 0;
            bool on_en_a_deja_trouve_un = false;
            string type;
            string abc;
            float x, y, z;
            mtlstruct material = new mtlstruct("", Color.Zero, Color.Zero, Color.Zero, "null.bmp");
            while (i < mtl.Length)
            {
                type = ObjLoader.getstring(ref i, ref mtl);
                if (type == "newmtl")
                {
                    i++; // Espace
                    if (on_en_a_deja_trouve_un)
                    {
                        MTLData.Add(material);
                        material = new mtlstruct("", Color.Zero, Color.Zero, Color.Zero, "null.bmp");
                    }
                    else on_en_a_deja_trouve_un = true;
                    material.name = ObjLoader.getstring(ref i, ref mtl); // On recupère le nom

                    Console.Write("\t");
                    Program.WriteNicely("#", 4, "Nouveau MTL" + material.name);
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

                    Console.Write("\t");
                    Program.WriteNicely("#", 4, "Nouveau Ka " + x + " " + y + " " + z);

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

                    Console.Write("\t");
                    Program.WriteNicely("#", 4, "Nouveau Kd " + x + " " + y + " " + z);

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

                    Console.Write("\t");
                    Program.WriteNicely("#", 4, "Nouveau Ks " + x + " " + y + " " + z);

                    material.Ks = new Color(new Vector4(x, y, z, 1));
                }
                else if (type == "map_Kd")
                {
                    map_Kd = true;
                    i++; // Espace
                    abc = ObjLoader.getstring(ref i, ref mtl);

                    Console.Write("\t");
                    Program.WriteNicely("#", 4, "Nouveau map_Kd " + path + abc);
                    material.map_Kd = path + abc;
                    Program.Liste_textures.Add(new Texturestruct(material.map_Kd, Texture.FromFile(Program.device, material.map_Kd)));
                }
                else if (type == "") { }
                else if (type[0] == '#')
                {
                    Console.Write("\t");
                    Program.WriteNicely("#", 4, "Nouveau commentaire");
                }
                else
                {
                    Console.Write("\t");
                    Program.WriteNicely("!", 2, "Type inconnu");
                }
                ObjLoader.gotonextline(ref i, ref mtl);
            }
            MTLData.Add(material);
        }
    }
}
