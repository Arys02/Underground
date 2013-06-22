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
        public static Vector4 CalcTangentVector(Vector3[] position, Vector2[] texCoord, Vector3 normal)
        {
            Vector4 tangent;
            // Une fois les 3 vertices (position / coordonnéesde texture d'un triangle) donnés, on calcule et retourne le vecteur tangent du triangle.
            // La bitangente est cross(normal, tangent.xyz) * tangent.w

            // edge1 est le vecteur allant du sommet 1 au sommet 2
            // edge2 est le vecteur allant du sommet 2 au sommet 3
            Vector3 edge1 = new Vector3(position[1].X - position[0].X, position[1].Y - position[0].Y, position[1].Z - position[0].Z);
            Vector3 edge2 = new Vector3(position[2].X - position[1].X, position[2].Y - position[1].Y, position[2].Z - position[1].Z);

            edge1.Normalize();
            edge2.Normalize();

            // Creation de 2 vecteur (les textures) dans l'espace tangentiel qui pointent vers la même direction qu'edge1 et edge2
            // texEdge1 est le vecteur allant de la coordonnée de texture 1 à 2
            // texEdge2 est le vecteur allant de la coordonnée de texture 2 à 3
            Vector2 texEdge1 = new Vector2(texCoord[1].X - texCoord[0].X, texCoord[1].Y - texCoord[0].Y);
            Vector2 texEdge2 = new Vector2(texCoord[2].X - texCoord[1].X, texCoord[2].Y - texCoord[1].Y);

            texEdge1.Normalize();
            texEdge2.Normalize();

            // Ces 2 jeux de vecteurs correspondent à ce système:
            //
            //  edge1 = (texEdge1.x * tangent) + (texEdge1.y * bitangent)
            //  edge2 = (texEdge2.x * tangent) + (texEdge2.y * bitangent)
            //
            // Ce qui en utilisant la notation des matrices ressemble à
            //
            //  [ edge1 ]     [ texEdge1.x  texEdge1.y ]  [ tangent   ]
            //  [       ]  =  [                        ]  [           ]
            //  [ edge2 ]     [ texEdge2.x  texEdge2.y ]  [ bitangent ]
            //
            // donc la solution est :
            //
            //  [ tangent   ]        1     [ texEdge2.y  -texEdge1.y ]  [ edge1 ]
            //  [           ]  =  -------  [                         ]  [       ]
            //  [ bitangent ]      det A   [-texEdge2.x   texEdge1.x ]  [ edge2 ]
            //
            //  où :
            //        [ texEdge1.x  texEdge1.y ]
            //    A = [                        ]
            //        [ texEdge2.x  texEdge2.y ]
            //
            //    det A = (texEdge1.x * texEdge2.y) - (texEdge1.y * texEdge2.x)
            //
            // A partir de cette solution, les vecteur de base de l'espace tangentiel sont
            //
            //    tangent = (1 / det A) * ( texEdge2.y * edge1 - texEdge1.y * edge2)
            //  bitangent = (1 / det A) * (-texEdge2.x * edge1 + texEdge1.x * edge2)
            //     normal = cross(tangent, bitangent)

            Vector3 bitangent;
            float det = (texEdge1.X * texEdge2.Y) - (texEdge1.Y * texEdge2.X);

            if (Math.Abs(det) < 1e-6f)    // almost equal to zero
            {
                tangent.X = 1.0f;
                tangent.Y = 0.0f;
                tangent.Z = 0.0f;

                bitangent = new Vector3(0.0f, 1.0f, 0.0f);
            }
            else
            {
                det = 1.0f / det;

                tangent.X = (texEdge2.Y * edge1.X - texEdge1.Y * edge2.X) * det;
                tangent.Y = (texEdge2.Y * edge1.Y - texEdge1.Y * edge2.Y) * det;
                tangent.Z = (texEdge2.Y * edge1.Z - texEdge1.Y * edge2.Z) * det;
                tangent.W = 0.0f;
        
                bitangent = new Vector3((-texEdge2.X * edge1.X + texEdge1.X * edge2.X) * det,
                                            (-texEdge2.X * edge1.Y + texEdge1.X * edge2.Y) * det,
                                            (-texEdge2.X * edge1.Z + texEdge1.X * edge2.Z) * det);

                tangent.Normalize();
                bitangent.Normalize();
            }

            // Calculate the handedness of the local tangent space.
            // The bitangent vector is the cross product between the triangle face
            // normal vector and the calculated tangent vector. The resulting bitangent
            // vector should be the same as the bitangent vector calculated from the
            // set of linear equations above. If they point in different directions
            // then we need to invert the cross product calculated bitangent vector. We
            // store this scalar multiplier in the tangent vector's 'w' component so
            // that the correct bitangent vector can be generated in the normal mapping
            // shader's vertex shader.
            Vector3 n = normal;
            Vector3 t = new Vector3(tangent.X, tangent.Y, tangent.Z);
            Vector3 b = Vector3.Cross(n, t);
            tangent.W = (Vector3.Dot(b, bitangent) < 0.0f) ? -1.0f : 1.0f;
            return tangent;
        }
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

        public static List<structModel> read_obj(byte[] obj, Matrix transformation)
        {
            List<structModel> Liste_Models = new List<structModel>();
            int i = 0, nbsommets = 0, nbtextures = 0, nbnormales = 0;
            int material_used = 0;
            int precedent_pourcentage = 0;
            Program.WriteNicely("#", 2, "Ouverture du fichier " + obj[0].ToString());
            structModel model_actuel = new structModel(Program.VertexBufferZero, new structVertex[0], 0, "null.bmp", "null.bmp", transformation);

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
                    structVertex[] face = new structVertex[3];
                    bool testing = false;
                    if (mtlloader.MTLData.Count == 0)
                    {
                        testing = true;
                        mtlloader.MTLData.Add(new structMTL("", Color.White, Color.White, Color.White, "null.bmp", "null.bmp"));
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
                    face[0] = (new structVertex()
                    {
                        Position = new Vector4(
                            ListeCoordSommets[Convert.ToInt32(x - 1)].X,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].Y,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].Z,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].W
                        ),
                        CoordTextures = new Vector2(
                            ListeCoordTextures[Convert.ToInt32(y - 1)].X,
                            ListeCoordTextures[Convert.ToInt32(y - 1)].Y
                        ),
                        Color = mtlloader.MTLData[material_used].Kd.ToVector4(),
                        Normal = new Vector4(
                            ListeNormales[Convert.ToInt32(z - 1)].X,
                            ListeNormales[Convert.ToInt32(z - 1)].Y,
                            ListeNormales[Convert.ToInt32(z - 1)].Z,
                            1),
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

                    face[1] = (new structVertex()
                    {
                        Position = new Vector4(
                            ListeCoordSommets[Convert.ToInt32(x - 1)].X,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].Y,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].Z,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].W
                        ),
                        CoordTextures = new Vector2(
                            ListeCoordTextures[Convert.ToInt32(y - 1)].X,
                            ListeCoordTextures[Convert.ToInt32(y - 1)].Y
                        ),
                        Color = mtlloader.MTLData[material_used].Kd.ToVector4(),
                        Normal = new Vector4(
                            ListeNormales[Convert.ToInt32(z - 1)].X,
                            ListeNormales[Convert.ToInt32(z - 1)].Y,
                            ListeNormales[Convert.ToInt32(z - 1)].Z,
                            1),
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
                    face[2] = (new structVertex()
                    {
                        Position = new Vector4(
                            ListeCoordSommets[Convert.ToInt32(x - 1)].X,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].Y,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].Z,
                            ListeCoordSommets[Convert.ToInt32(x - 1)].W
                        ),
                        CoordTextures = new Vector2(
                            ListeCoordTextures[Convert.ToInt32(y - 1)].X,
                            ListeCoordTextures[Convert.ToInt32(y - 1)].Y
                        ),
                        Color = mtlloader.MTLData[material_used].Kd.ToVector4(),
                        Normal = new Vector4(
                            ListeNormales[Convert.ToInt32(z - 1)].X,
                            ListeNormales[Convert.ToInt32(z - 1)].Y,
                            ListeNormales[Convert.ToInt32(z - 1)].Z,
                            1),
                    });

                    Vector4 Tangent = CalcTangentVector(
                        new Vector3[] {
                        new Vector3(face[0].Position.X, face[0].Position.Y, face[0].Position.Z),
                        new Vector3(face[1].Position.X, face[1].Position.Y, face[1].Position.Z),
                        new Vector3(face[2].Position.X, face[2].Position.Y, face[2].Position.Z),
                    },
                        new Vector2[] {
                        new Vector2(face[0].CoordTextures.X, face[0].CoordTextures.Y),
                        new Vector2(face[1].CoordTextures.X, face[1].CoordTextures.Y),
                        new Vector2(face[2].CoordTextures.X, face[2].CoordTextures.Y),
                    },
                        new Vector3(face[0].Normal.X, face[0].Normal.Y, face[0].Normal.Z));

                    face[0].Tangent = Tangent;
                    face[1].Tangent = Tangent;
                    face[2].Tangent = Tangent;
                    if (mtlloader.MTLData[material_used].map_Ns != "null.bmp")
                    {
                        face[0].bool_normal_map = 1;
                        face[1].bool_normal_map = 1;
                        face[2].bool_normal_map = 1;
                    }
                    else
                    {
                        face[0].bool_normal_map = -1;
                        face[1].bool_normal_map = -1;
                        face[2].bool_normal_map = -1;
                    }

                    ListeVertex.Add(face[0]);
                    ListeVertex.Add(face[1]);
                    ListeVertex.Add(face[2]);

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
                    Program.WriteNicely("\t#", 11, "Nouveau fichier .MTL : " + abc);
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
                        model_actuel.map_Ns = mtlloader.MTLData[material_used].map_Ns;
                        model_actuel.VertexBuffer = new VertexBuffer(Program.device,
                            (
                                Utilities.SizeOf<structVertex>() // NORMAL
                            ) * 3 * model_actuel.nbfaces, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                        ListeVertex.Clear();
                        Liste_Models.Add(model_actuel);
                        model_actuel = new structModel(Program.VertexBufferZero, new structVertex[0], 0, "null.bmp", "null.bmp", transformation);
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
                mtlloader.MTLData.Add(new structMTL("", Color.White, Color.White, Color.White, "null.bmp", "null.bmp"));
                model_actuel.Sommets = ListeVertex.ToArray();
                model_actuel.map_Kd = mtlloader.MTLData[material_used].map_Kd;
                model_actuel.map_Ns = mtlloader.MTLData[material_used].map_Ns;
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
