using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Test2
{
    public class Object
    {
        List<Obj> Mono;

        private struct Obj
        {
            //Contient le nom du fichier mtl a utiliser
            public string Materialpath;
            //Contient la liste des materiaux a lire dans le fichier mtl du dessus
            public List<string> UseMaterials;
            //Ces 4 tableau contiennent toutes les coordonnees qui composent un obj
            public static float[,] V;
            public static float[,] VT;
            public static float[,] VN;
            public static float[,] F;

            //constructeur de structure
            public Obj(List<string> v, List<string> vn, List<string> vt, List<string> f, List<string> usematerial, string materialpath)
            {
                char[] delimiterChars = { ' ', '/' };
                string[] Nomb;

                V = new float[v.Count, 3];
                for (int i = 0; i < (v.Count); i++)
                {
                    Nomb = v[i].Split(delimiterChars);
                    V[i, 0] = float.Parse(Replace(Nomb[0]));
                    V[i, 1] = float.Parse(Replace(Nomb[1]));
                    V[i, 2] = float.Parse(Replace(Nomb[2]));
                }

                VT = new float[vt.Count, 2];
                for (int i = 0; i < (vt.Count); i++)
                {
                    Nomb = vt[i].Split(delimiterChars);
                    VT[i, 0] = float.Parse(Replace(Nomb[0]));
                    VT[i, 1] = float.Parse(Replace(Nomb[1]));
                }

                VN = new float[vn.Count, 3];
                for (int i = 0; i < (vn.Count); i++)
                {
                    Nomb = vn[i].Split(delimiterChars);
                    VN[i, 0] = float.Parse(Replace(Nomb[0]));
                    VN[i, 1] = float.Parse(Replace(Nomb[1]));
                    VN[i, 2] = float.Parse(Replace(Nomb[2]));
                }

                F = new float[f.Count, 9];
                for (int i = 0; i < (f.Count); i++)
                {
                    Nomb = f[i].Split(delimiterChars);
                    F[i, 0] = float.Parse(Replace(Nomb[0]));
                    F[i, 1] = float.Parse(Replace(Nomb[1]));
                    F[i, 2] = float.Parse(Replace(Nomb[2]));

                    F[i, 3] = float.Parse(Replace(Nomb[3]));
                    F[i, 4] = float.Parse(Replace(Nomb[4]));
                    F[i, 5] = float.Parse(Replace(Nomb[5]));

                    F[i, 6] = float.Parse(Replace(Nomb[6]));
                    F[i, 7] = float.Parse(Replace(Nomb[7]));
                    F[i, 8] = float.Parse(Replace(Nomb[8]));
                }

                UseMaterials = usematerial;
                Materialpath = materialpath;
            }
        }

        public Object(string filename)
        {   
            Mono = new List<Obj>();
            StreamReader obj;            
            obj = new StreamReader(filename);
            
            List<string> v = new List<string>();
            List<string> vn = new List<string>();
            List<string> vt = new List<string>();
            List<string> f = new List<string>();
            List<string> usematerials = new List<string>();
            string materialpath = "";

            bool charge = false;
            Obj test;

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
                                materialpath = tab[1];
                            }
                        }
                        break;

                    case 'u':
                        {
                            string[] tab = Line.Split(' ');
                            if (tab[0] == "usemtl")
                            {
                                usematerials.Add(tab[1]);
                            }
                        }
                        break;

                    case 'v':
                        if (Line[1] == 't')
                        {
                            vt.Add(Line.Remove(0, 3));
                        }
                        else if (Line[1] == 'n')
                        {
                            vn.Add(Line.Remove(0, 3));
                        }
                        else
                        {
                            v.Add(Line.Remove(0, 2));
                        }
                        break;

                    case 'f':
                        f.Add(Line.Remove(0, 2));
                        break;

                    case 'o':
                        if (charge == true)
                        {             
                            test = new Obj(v,vn,vt,f,usematerials,materialpath);
                            Mono.Add(test);
                            f.Clear();
                            v.Clear();
                            vt.Clear();
                            vn.Clear();
                            usematerials.Clear();
                        }
                        else
                        {
                            charge = true;
                        }
                        break;

                    default:
                        break;
                }

                Line = obj.ReadLine();
            } while (Line != null);

            obj.Close();

            test = new Obj(v,vn,vt,f,usematerials,materialpath);
            Mono.Add(test);        
        }

        //Fonction remplacent '.' par ',' pour la convertion en float
        private static string Replace(string a)
        {
            string[] tab = a.Split('.');
            if (tab.Length > 1)
            {
                return (tab[0] + ',' + tab[1]);
            }
            else
            {
                return (tab[0]);
            }
        }

    }
}

