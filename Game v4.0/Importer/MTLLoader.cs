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

namespace LOL_l.Importer
{
    struct structNewMTL
    {
        public string name;
        public string map_Kd;
        public string map_Ns;
        public Color Ka;
        public Color Kd;
        public Color Ks;
        public float Ns;
        public structNewMTL(string name, string map_Kd, string map_Ns, Color Ka, Color Kd, Color Ks, float Ns)
        {
            this.name = name;
            this.map_Kd = map_Kd;
            this.map_Ns = map_Ns;
            this.Kd = Kd;
            this.Ka = Ka;
            this.Ks = Ks;
            this.Ns = Ns;
        }
    }
    class MTLLoader
    {
        private structNewMTL[] ArrayMaterialSet;

        public structNewMTL getMaterialSet(string name)
        {
            Debug debug = new Debug();
            for (int i = 1; i < ArrayMaterialSet.Length; i++)
            {
                if (ArrayMaterialSet[i].name == name) return ArrayMaterialSet[i];
            }
            debug.WriteNicely("#", ConsoleColor.Red, "Set non trouvé " + name, 0);
            return ArrayMaterialSet[0]; // Si on a pas trouvé, on reenvoi le set par défaut
        }

        public MTLLoader()
        {
            List<structNewMTL> ListMaterialSet = new List<structNewMTL>();
            ListMaterialSet.Add(new structNewMTL("", "", "", Color.Black, Color.White, Color.White, 1));
            ArrayMaterialSet = ListMaterialSet.ToArray();
        }

        public MTLLoader(string path)
        {
            // Initialisation
            byte[] fileMTL = ResManager.ListBinaries[ResManager.getBinary(path)].Data;
            string type;
            float x, y, z;
            string chaine_Lu;
            int i = 0;
            List<structNewMTL> ListMaterialSet = new List<structNewMTL>();
            structNewMTL materialSet = new structNewMTL("", "", "", Color.Black, Color.White, Color.White, 1);
            ToolBox ToolBox = new ToolBox();
            Debug debug = new Debug();

            // Boucle du parser
            while (i < fileMTL.Length)
            {
                type = ToolBox.getstring(ref i, ref fileMTL);
                if (type == "newmtl")
                {
                    i++; // Espace
                    ListMaterialSet.Add(materialSet);
                    materialSet = new structNewMTL("", "", "", Color.Black, Color.White, Color.White, 1);
                    chaine_Lu = ToolBox.getstring(ref i, ref fileMTL);
                    materialSet.name = chaine_Lu;

                    debug.WriteNicely("#", ConsoleColor.DarkGreen, "nouveau set " + chaine_Lu, 3);
                }
                else if (type == "Ka")
                {
                    i++; // Espace
                    x = ToolBox.getfloat(ref i, ref fileMTL);
                    i++; // Espace
                    y = ToolBox.getfloat(ref i, ref fileMTL);
                    i++; // Espace
                    z = ToolBox.getfloat(ref i, ref fileMTL);

                    debug.WriteNicely("#", ConsoleColor.DarkGreen, "nouveau Ka " + x.ToString() + " " + y.ToString() + " " + z.ToString(), 3);
                    materialSet.Ka = new Color(x, y, z, 1);
                }
                else if (type == "Kd")
                {
                    i++; // Espace
                    x = ToolBox.getfloat(ref i, ref fileMTL);
                    i++; // Espace
                    y = ToolBox.getfloat(ref i, ref fileMTL);
                    i++; // Espace
                    z = ToolBox.getfloat(ref i, ref fileMTL);
                    materialSet.Kd = new Color(x, y, z, 1);

                    debug.WriteNicely("#", ConsoleColor.DarkGreen, "nouveau Kd " + x.ToString() + " " + y.ToString() + " " + z.ToString(), 3);
                }
                else if (type == "Ks")
                {
                    i++; // Espace
                    x = ToolBox.getfloat(ref i, ref fileMTL);
                    i++; // Espace
                    y = ToolBox.getfloat(ref i, ref fileMTL);
                    i++; // Espace
                    z = ToolBox.getfloat(ref i, ref fileMTL);
                    materialSet.Ks = new Color(x, y, z, 1);

                    debug.WriteNicely("#", ConsoleColor.DarkGreen, "nouveau Ks " + x.ToString() + " " + y.ToString() + " " + z.ToString(), 3);
                }
                else if (type == "map_Kd")
                {
                    i++; // Espace
                    chaine_Lu = ToolBox.getstring(ref i, ref fileMTL);
                    materialSet.map_Kd = chaine_Lu;

                    debug.WriteNicely("#", ConsoleColor.DarkGreen, "nouveau map_Kd " + chaine_Lu, 3);
                }
                else if (type == "map_Ns")
                {
                    i++; // Espace
                    chaine_Lu = ToolBox.getstring(ref i, ref fileMTL);
                    materialSet.map_Ns = chaine_Lu;

                    debug.WriteNicely("#", ConsoleColor.DarkGreen, "nouveau map_Ns " + chaine_Lu, 3);
                }
                else if (type == "") { }
                else if (type[0] == '#')
                {
                    debug.WriteNicely("#", ConsoleColor.Green, "Commentaire", 3);
                }
                else
                {
                    debug.WriteNicely("#", ConsoleColor.Red, "Type inconnu", 2);
                }
                ToolBox.gotonextline(ref i, ref fileMTL);
            }
            ListMaterialSet.Add(materialSet);
            ArrayMaterialSet = ListMaterialSet.ToArray();
        }
    }
}
