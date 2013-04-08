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
    class ObjLoader
    {
        private static MtlLoader mtlloader;
        static public void Initialize()
        {
            mtlloader = new MtlLoader();
        }
        public static void gotonextvalue(ref int i, ref byte[] obj)
        {
            int carac_Lu = obj[i];
            i++;
            while ((!(
                (carac_Lu >= Convert.ToInt32('0') && carac_Lu <= Convert.ToInt32('9')) ||
                (carac_Lu >= Convert.ToInt32('a') && carac_Lu <= Convert.ToInt32('z')) ||
                (carac_Lu >= Convert.ToInt32('A') && carac_Lu <= Convert.ToInt32('Z')) ||
                carac_Lu == Convert.ToInt32('-') ||
                carac_Lu == Convert.ToInt32('.') ||
                carac_Lu == Convert.ToInt32('_')
                )) && i < obj.Length)
            {
                carac_Lu = obj[i];
                i++;
            }
            i--;
        }

        public static string getstring(ref int i, ref byte[] obj)
        {
            List<char> liste = new List<char>();
            int carac_Lu = obj[i];
            i++;
            while (carac_Lu != Convert.ToInt32('\n') &&
                carac_Lu != Convert.ToInt32(' ') &&
                i < obj.Length
            )
            {
                if (carac_Lu != '\r')
                {
                    liste.Add(Convert.ToChar(carac_Lu));
                }
                carac_Lu = obj[i];
                i++;
            }
            i--;
            return new string(liste.ToArray());
        }

        public static float getfloat(ref int i, ref byte[] obj)
        {
            bool est_negatif = false;
            bool virgule = false;
            float resultat = 0;
            int decalage = 0;
            int carac_Lu = carac_Lu = obj[i];
            i++;
            if (carac_Lu == Convert.ToInt32('-'))
            {
                est_negatif = true;
                carac_Lu = carac_Lu = obj[i];
                i++;
            }
            while (((carac_Lu >= Convert.ToInt32('0') && carac_Lu <= Convert.ToInt32('9')) || carac_Lu == Convert.ToInt32('.')) && i < obj.Length)
            {
                if (carac_Lu == Convert.ToInt32('.')) virgule = true;
                else
                {
                    if (virgule) decalage++;
                    if (est_negatif) resultat = resultat * 10 - (carac_Lu - Convert.ToInt32('0'));
                    else resultat = resultat * 10 + (carac_Lu - Convert.ToInt32('0'));
                }
                carac_Lu = carac_Lu = obj[i];
                i++;
            }
            i--;
            return Convert.ToSingle(resultat / (Math.Pow(10, decalage)));
        }

        public static void gotonextline(ref int i, ref byte[] obj)
        {
            int carac_Lu;
            do
            {
                carac_Lu = obj[i];
                i++;
            } while (carac_Lu != '\n' && i < obj.Length);
        }

        public static List<structModel> read_obj(byte[] obj, Matrix referentiel)
        {
            List<structModel> Liste_Models = new List<structModel>();
            int i = 0, nbsommets = 0, nbtextures = 0, nbnormales = 0;
            int material_used = 0;
            int precedent_pourcentage = 0;
            Program.WriteNicely("#", 2, "Ouverture du fichier " + obj[0].ToString());
            structModel model_actuel = new structModel(Program.VertexBufferZero, new structVertex[0], 0, "null.bmp");

            ///////////// Pour la construction des sommets /////////////
            List<Vector2> ListeCoordTextures = new List<Vector2>();
            List<Vector4> ListeCoordSommets = new List<Vector4>();
            List<Vector4> ListeNormales = new List<Vector4>();

            ///////////// Sommets /////////////
            List<structVertex> ListeVertex = new List<structVertex>();
            //List<Vertex[]> ListeVerticesComplete_avec_differentes_textures_separee = new List<Vertex[]>();

            float x = 1, y = 1, z = 1;
            string type;

            Color3[] couleur = new Color3[3] { 
                Color.White.ToColor3(),
                Color.White.ToColor3(),
                Color.White.ToColor3()
            };

            while (i < obj.Length)
            {
                type = getstring(ref i, ref obj);
                #region sommet
                if (type == "v")
                {
                    nbsommets++;
                    // On recupère les coordonnées du sommet
                    i++; // espace
                    x = getfloat(ref i, ref obj); // Coord x
                    i++; // espace
                    y = getfloat(ref i, ref obj); // Coord y
                    i++; // espace
                    z = getfloat(ref i, ref obj); // Coord z

                    // Ajout dans la liste le nouveau sommet
                    ListeCoordSommets.Add(new Vector4(x, y, z, 1));

                    // DEBUG
                    //string abc = "Nouveau sommet " + x + " " + y + " " + z;
                    //Program.WriteNicely("#", 3, abc);
                }
                #endregion
                #region textures
                else if (type == "vt")
                {
                    nbtextures++;
                    // On recupère les coordonnées de texture
                    i++; // espace
                    x = getfloat(ref i, ref obj); // Coord x
                    i++; // espace
                    y = getfloat(ref i, ref obj); // Coord y

                    // Ajout dans la liste la nouvelle texture
                    ListeCoordTextures.Add(new Vector2(x, y));

                    // DEBUG
                    //string abc = "Nouvelle texture " + x + " " + y;
                    //Program.WriteNicely("#", 3, abc);
                }
                #endregion
                #region normales
                else if (type == "vn")
                {
                    nbnormales++;
                    // On recupère les normales
                    i++; // espace
                    x = getfloat(ref i, ref obj); // Coord x
                    i++; // espace
                    y = getfloat(ref i, ref obj); // Coord y
                    i++; // espace
                    z = getfloat(ref i, ref obj); // Coord z

                    // Ajout dans la liste des normales
                    ListeNormales.Add(new Vector4(x, y, z, 1));

                    // DEBUG
                    //string abc = "Nouvelle normale " + x + " " + y;
                    //Program.WriteNicely("#", 3, abc);
                }
                #endregion
                #region faces
                else if (type == "f")
                {
                    bool testing = false;
                    if (mtlloader.MTLData.Count == 0)
                    {
                        testing = true;
                        mtlloader.MTLData.Add(new structMTL("", Color.White, Color.White, Color.White, "null.bmp"));
                    }
                    model_actuel.nbfaces++;
                    //string abc;
                    // CONSTRUCTION SOMMET 1
                    // On recupère les info du sommet
                    i++; // espace
                    x = getfloat(ref i, ref obj); // Coord x
                    i++; // slash
                    y = getfloat(ref i, ref obj); // Coord y
                    i++; // slash
                    z = getfloat(ref i, ref obj); // Coord z
                    ListeVertex.Add(new structVertex()
                    {
                        Position = Vector4.Transform(new Vector4(
                            ListeCoordSommets[Convert.ToInt32(x - 1)].X,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].Y,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].Z,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].W
                        ),referentiel),
                        CoordTextures = new Vector2(
                            ListeCoordTextures[Convert.ToInt32(y - 1)].X,
                            ListeCoordTextures[Convert.ToInt32(y - 1)].Y
                        ),
                        Color = mtlloader.MTLData[material_used].Kd.ToVector4(),
                        Normal = new Vector4(Vector3.TransformNormal(new Vector3(
                            ListeNormales[Convert.ToInt32(z - 1)].X,
                            ListeNormales[Convert.ToInt32(z - 1)].Y,
                            ListeNormales[Convert.ToInt32(z - 1)].Z),
                            referentiel),1),
                    });
                    //abc = "Construction sommet " + Convert.ToInt32(x-1) + " " + Convert.ToInt32(y-1) + " " + Convert.ToInt32(z-1);
                    //Program.WriteNicely("#", 3, abc);

                    // CONSTRUCTION SOMMET 2
                    // On recupère les info du sommet
                    i++; // espace
                    x = getfloat(ref i, ref obj); // Coord x
                    i++; // slash
                    y = getfloat(ref i, ref obj); // Coord y
                    i++; // slash
                    z = getfloat(ref i, ref obj); // Coord z

                    //var abcd = Vector4.Transform(new Vector4(0, 0, 1, 1), Matrix.Multiply(Matrix.RotationY(Convert.ToSingle(-Math.PI / 2)), Matrix.Translation(1, 0, 0)));

                    ListeVertex.Add(new structVertex()
                    {
                        Position = Vector4.Transform(new Vector4(
                            ListeCoordSommets[Convert.ToInt32(x-1)].X,
                            ListeCoordSommets[Convert.ToInt32(x-1)].Y,
                            ListeCoordSommets[Convert.ToInt32(x-1)].Z,
                            ListeCoordSommets[Convert.ToInt32(x-1)].W
                        ),referentiel),
                        CoordTextures = new Vector2(
                            ListeCoordTextures[Convert.ToInt32(y - 1)].X,
                            ListeCoordTextures[Convert.ToInt32(y - 1)].Y
                        ),
                        Color = mtlloader.MTLData[material_used].Kd.ToVector4(),
                        Normal = new Vector4(Vector3.TransformNormal(new Vector3(
                            ListeNormales[Convert.ToInt32(z - 1)].X,
                            ListeNormales[Convert.ToInt32(z - 1)].Y,
                            ListeNormales[Convert.ToInt32(z - 1)].Z),
                            referentiel), 1),
                    });
                    //abc = "Construction sommet " + Convert.ToInt32(x-1) + " " + Convert.ToInt32(y-1) + " " + Convert.ToInt32(z-1);
                    //Program.WriteNicely("#", 3, abc);

                    // CONSTRUCTION SOMMET 3
                    // On recupère les info du sommet
                    i++; // espace
                    x = getfloat(ref i, ref obj); // Coord x
                    i++; // slash
                    y = getfloat(ref i, ref obj); // Coord y
                    i++; // slash
                    z = getfloat(ref i, ref obj); // Coord z
                    ListeVertex.Add(new structVertex()
                    {
                        Position = Vector4.Transform(new Vector4(
                            ListeCoordSommets[Convert.ToInt32(x-1)].X,
                            ListeCoordSommets[Convert.ToInt32(x-1)].Y,
                            ListeCoordSommets[Convert.ToInt32(x-1)].Z,
                            ListeCoordSommets[Convert.ToInt32(x-1)].W
                        ),referentiel),
                        CoordTextures = new Vector2(
                            ListeCoordTextures[Convert.ToInt32(y - 1)].X,
                            ListeCoordTextures[Convert.ToInt32(y - 1)].Y
                        ),
                        Color = mtlloader.MTLData[material_used].Kd.ToVector4(),
                        Normal = new Vector4(Vector3.TransformNormal(new Vector3(
                            ListeNormales[Convert.ToInt32(z - 1)].X,
                            ListeNormales[Convert.ToInt32(z - 1)].Y,
                            ListeNormales[Convert.ToInt32(z - 1)].Z),
                            referentiel), 1),
                    });
                    //abc = "Construction sommet " + Convert.ToInt32(x-1) + " " + Convert.ToInt32(y-1) + " " + Convert.ToInt32(z-1);
                    //Program.WriteNicely("#", 3, abc);
                    if (testing)
                        mtlloader.MTLData.Clear();
                }
                #endregion
                #region objet
                else if (type == "o") // S'il s'agit d'un nouvel objet on ne fait rien
                {
                    i++;
                    Program.WriteNicely("#", 3, "Nouvel objet : " + getstring(ref i, ref obj));
                }
                #endregion
                #region groupe
                else if (type == "g") // S'il s'agit d'un nouveau groupe on ne fait rien
                {
                    i++;
                    Program.WriteNicely("#", 3, "Nouveau groupe : " + getstring(ref i, ref obj));
                }
                #endregion
                #region material
                else if (type == "mtllib") // S'il s'agit d'un nouveau fichier mtl on le lit et on le stocke
                {
                    i++;
                    string abc = getstring(ref i, ref obj);
                    Console.Write("\t");
                    Program.WriteNicely("#", 11, "Nouveau fichier .MTL : " + abc);
                    mtlloader.read_mtl(abc, @"Ressources/Game/");
                }
                else if (type == "usemtl")
                {
                    i++;
                    string abc = getstring(ref i, ref obj);
                    Program.WriteNicely("#", 11, "utilisation de : " + abc);
                    if (model_actuel.nbfaces != 0)
                    {
                        model_actuel.Sommets = ListeVertex.ToArray();
                        model_actuel.map_Kd = mtlloader.MTLData[material_used].map_Kd;
                        model_actuel.VertexBuffer = new VertexBuffer(Program.device,
                            (
                                Utilities.SizeOf<structVertex>() // NORMAL
                            ) * 3 * model_actuel.nbfaces, Usage.WriteOnly, VertexFormat.None, Pool.Managed);

                        ListeVertex.Clear();
                        Liste_Models.Add(model_actuel);
                        model_actuel = new structModel(Program.VertexBufferZero, new structVertex[0], 0, "null.bmp");
                    }
                    for (int j = 0; j < mtlloader.MTLData.Count; j++)
                    {
                        if (mtlloader.MTLData[j].name == abc)
                        {
                            material_used = j;
                            j = mtlloader.MTLData.Count;
                        }
                    }
                }
                else if (type == "s")
                {
                    //Program.WriteNicely("#", 11, "Lissage OFF");
                }
                #endregion
                else if (type == "")
                {
                    Program.WriteNicely("#", 5, "Ligne vide");
                }
                #region commentaire
                else if (type[0] == '#')
                {
                    Program.WriteNicely("#", 4, "Nouveau commentaire");
                }
                #endregion
                else
                {
                    Program.WriteNicely("!", 2, "Type inconnu");
                }

                // FIN DU TRAITEMENT
                if (precedent_pourcentage != Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(obj.Length) * 100))
                {
                    precedent_pourcentage = Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(obj.Length) * 100);
                    Console.WriteLine("\t{0} % lu", precedent_pourcentage);
                    Console.CursorTop--;
                }
                gotonextline(ref i, ref obj);
                //i++;
            }
            if (model_actuel.nbfaces != 0)
            {
                mtlloader.MTLData.Add(new structMTL("", Color.White, Color.White, Color.White, "null.bmp"));
                model_actuel.Sommets = ListeVertex.ToArray();
                model_actuel.map_Kd = mtlloader.MTLData[material_used].map_Kd;
                model_actuel.VertexBuffer = new VertexBuffer(Program.device,
                    (
                        Utilities.SizeOf<structVertex>() // NORMAL
                    ) * 3 * model_actuel.nbfaces, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                mtlloader.MTLData.Clear();
                ListeVertex.Clear();
                Liste_Models.Add(model_actuel);
                //model_actuel = new Model(Program.VertexBufferZero, new Vertex[0], 0, "null.bmp");
            }

            return Liste_Models;
            //return ListeVertex.ToArray();
        }
    }
}
