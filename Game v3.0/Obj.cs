using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Underground
{
    class Obj
    {
        //Ces 4 tableau contiennent toutes les coordonnees qui composent un obj
        public static float[,] V;
        public static float[,] VT;
        public static float[,] VN;
        public static float[,] F;
        //Contient le nom du fichier mtl a utiliser
        public string Materialpath;
        //Contient la liste des materiaux a lire dans le fichier mtl du dessus
        public List<string> UseMaterials;

        //Cette struc ne sert  qu a la clarte du code;
        struct Coor
        {
            public string Materialpath;
            public List<string> v;
            public List<string> vt;
            public List<string> vn;
            public List<string> f;
            public List<string> UseMaterials;
        }

        //Constructeur ceux dernier ne fait que remplir les tableau a partir d une struc Coor
        public Obj(Coor coor)
        {
            char[] delimiterChars = { ' ', '/' };
            string[] Nomb;            

            V = new float[coor.v.Count, 3];
            for (int i = 0; i <= (coor.v.Count); i++)
            {
                Nomb = coor.v[i].Split(delimiterChars);
                V[i, 0] = float.Parse(Nomb[0]);
                V[i, 1] = float.Parse(Nomb[1]);
                V[i, 2] = float.Parse(Nomb[2]);
            }

            VT = new float[coor.vt.Count, 2];
            for (int i = 0; i <= (coor.vt.Count); i++)
            {
                Nomb = coor.vt[i].Split(delimiterChars);
                VT[i, 0] = float.Parse(Nomb[0]);
                VT[i, 1] = float.Parse(Nomb[1]);
            }

            VN = new float[coor.vn.Count, 3];
            for (int i = 0; i <= (coor.vn.Count); i++)
            {
                Nomb = coor.vn[i].Split(delimiterChars);
                VN[i, 0] = float.Parse(Nomb[0]);
                VN[i, 1] = float.Parse(Nomb[1]);
                VN[i, 2] = float.Parse(Nomb[2]);
            }

            F = new float[coor.f.Count, 3];
            for (int i = 0; i <= (coor.f.Count); i++)
            {
                Nomb = coor.f[i].Split(delimiterChars);
                F[i, 0] = float.Parse(Nomb[0]);
                F[i, 1] = float.Parse(Nomb[1]);
                F[i, 2] = float.Parse(Nomb[2]);
            }
        }

        //Mon parser il se sert du constructeur pour cree une liste Objet contenue dans un meme fichier objet
        //Exemple : si on lui donne le fichier FFFsql.obj notre list contiendra les obj suivant: CageThorax.005_Cube.228,
        //CageThorax.004_Cube.227,CageThorax.003_Cube.226,...
        public static List<Obj> Obj_parser(string filename)
        {
            StreamReader obj;

            obj = new StreamReader(filename);

            Coor coor = new Coor();
            List<Obj> Objlist = new List<Obj>();
            string Line;
            Line = obj.ReadLine();

            do
            {                           
                switch (Line[0])
                {
                    case 'm':
                        {
                            string[] tab = Line.Split(' ');
                            if (tab[0] == "mtllib")
                            {
                                coor.Materialpath = tab[1];
                            }
                        }
                        break;
                    case 'u':
                        {
                            string[] tab = Line.Split(' ');
                            if (tab[0] == "usemtl")
                            {
                                coor.UseMaterials.Add(tab[1]);
                            }
                        }
                        break;
                    case 'v':
                        if (Line[1] == 't')
                        {
                            Line.Remove(0, 3);
                            coor.vt.Add(Line);
                        }
                        else if (Line[1] == 'n')
                        {
                            Line.Remove(0, 3);
                            coor.vn.Add(Line);
                        }
                        else
                        {
                            Line.Remove(0, 2);
                            coor.v.Add(Line);
                        }
                        break;

                    case 'f':
                        Line.Remove(0, 2);
                        coor.f.Add(Line);
                        break;

                    case 'o':
                        Obj temp = new Obj(coor);
                        Objlist.Add(temp);
                        coor.v.Clear();
                        coor.vt.Clear();
                        coor.vn.Clear();
                        coor.f.Clear();
                        coor.UseMaterials.Clear();                        
                        break;

                    default :
                        break;
                }

                Line = obj.ReadLine();
            } while (Line != null) ;

            obj.Close();

            Obj tamp = new Obj(coor);
            Objlist.Add(tamp);

            return Objlist;

        }

    }
}
