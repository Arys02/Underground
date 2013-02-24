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
        private static void gotonextvalue(ref int i, byte[] obj)
        {
            int carac_Lu = obj[i];
            i++;
            while (!(
                (carac_Lu >= Convert.ToInt32('0') && carac_Lu <= Convert.ToInt32('9')) ||
                (carac_Lu >= Convert.ToInt32('a') && carac_Lu <= Convert.ToInt32('z')) ||
                (carac_Lu >= Convert.ToInt32('A') && carac_Lu <= Convert.ToInt32('Z')) ||
                carac_Lu == Convert.ToInt32('-') ||
                carac_Lu == Convert.ToInt32('.')
                ))
            {
                carac_Lu = obj[i];
                i++;
            }
            i--;
        }



        private static string getstring(ref FileStream fichier)
        {
            List<char> liste = new List<char>();
            int carac_Lu = fichier.ReadByte();
            while (carac_Lu != Convert.ToInt32('\n') &&
                carac_Lu != Convert.ToInt32(' ') &&
                carac_Lu != -1
            )
            {
                liste.Add(Convert.ToChar(carac_Lu));
                carac_Lu = fichier.ReadByte();
            }
            fichier.Position--;
            return new string(liste.ToArray());
        }

        private static float getfloat(ref int i, byte[] obj)
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
            while (((carac_Lu >= Convert.ToInt32('0') && carac_Lu <= Convert.ToInt32('9')) || carac_Lu == Convert.ToInt32('.')) && carac_Lu != -1)
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

        private static void gotonextline(ref int i, byte[] obj)
        {
            int carac_Lu = obj[i];
            i++;
            while (carac_Lu != '\n')
            {
                carac_Lu = obj[i];
                i++;
            }
        }

        public static Vertex[] read_obj(byte[] obj, Vector4 referentiel, ref int nbfaces, ref int nbsommets, ref int nbnormales, ref int nbtextures)
        {
            int i = 0;
            bool is_4 = true;

            int precedent_pourcentage = 0;
            Ingame.WriteNicely("#", 2, "Ouverture du fichier " + obj[0].ToString());
            List<Vector2> ListeCoordTextures = new List<Vector2>();
            List<Vector4> ListeCoordSommets = new List<Vector4>();
            List<Vector4> ListeNormales = new List<Vector4>();
            List<Vertex> ListeVertex = new List<Vertex>();
            float x = 1, y = 1, z = 1;


            Color3[] couleur = new Color3[3] { 
                Color.White.ToColor3(),
                Color.White.ToColor3(),
                Color.White.ToColor3()
            };

            int carac_Lu = obj[i];
            i++;
            int count;
            while (carac_Lu != -1)
            {
                count = ListeVertex.Count;
                if (carac_Lu == Convert.ToInt32('v'))
                {
                    carac_Lu = obj[i];
                    i++;
                    if (carac_Lu == Convert.ToInt32('t'))
                    {
                        carac_Lu = obj[i];
                        i++;
                        if (carac_Lu == Convert.ToInt32(' '))
                        {
                            nbtextures++;
                            // Il s'agit d'une coordonnée de texture
                            gotonextvalue(ref i, obj);
                            x = getfloat(ref i, obj);
                            gotonextvalue(ref i, obj);
                            y = getfloat(ref i, obj);
                            ListeCoordTextures.Add(new Vector2(x, y));
                        }
                    }
                    else if (carac_Lu == Convert.ToInt32('n'))
                    {
                        carac_Lu = obj[i];
                        i++;
                        if (carac_Lu == Convert.ToInt32(' '))
                        {
                            nbnormales++;
                            // Il s'agit d'une normale
                            gotonextvalue(ref i, obj);
                            x = getfloat(ref i, obj);
                            gotonextvalue(ref i, obj);
                            y = getfloat(ref i, obj);
                            gotonextvalue(ref i, obj);
                            z = getfloat(ref i, obj);
                            ListeNormales.Add(new Vector4(x, y, z, 1.0f));
                        }
                    }
                    else if (carac_Lu == Convert.ToInt32(' '))
                    {
                        nbsommets++;
                        // Il s'agit d'une coordonnée de sommet
                        gotonextvalue(ref i, obj);
                        x = getfloat(ref i, obj);
                        gotonextvalue(ref i, obj);
                        y = getfloat(ref i, obj);
                        gotonextvalue(ref i, obj);
                        z = getfloat(ref i, obj);
                        ListeCoordSommets.Add(new Vector4(x, y, z, 1.0f));
                    }
                }
                else if (carac_Lu == Convert.ToInt32('f'))
                {
                    carac_Lu = obj[i];
                    i++;
                    if (carac_Lu == ' ')
                    {
                        nbfaces++;
                        // Il s'agit d'une face

                        // Sommet 1 2 3
                        float[] values = new float[4*3]; // 4*2 sans NORMAL
                        for (int k = 0; k < 3; k++)
                        {
                            gotonextvalue(ref i, obj);
                            x = getfloat(ref i, obj);
                            values[k * 3] = x;
                            carac_Lu = obj[i];
                            i++;
                            if (carac_Lu == Convert.ToInt32('/'))
                            {
                                carac_Lu = obj[i];
                                if (carac_Lu == '/')
                                {
                                    y = 1.0f;
                                    ListeCoordTextures.Add(new Vector2(1, 1));
                                }
                                else
                                {
                                    gotonextvalue(ref i, obj);
                                    y = getfloat(ref i, obj);
                                }
                                values[k * 3 + 1] = y;
                                gotonextvalue(ref i, obj);
                                z = getfloat(ref i, obj);
                                values[k * 3 + 2] = z;
                            }
                            else i--;
                        }
                        if ((obj[i] < '0' || obj[i] > '9') && obj[i]!=' ') is_4 = false;
                        else
                        {
                            int k = 3;
                            gotonextvalue(ref i, obj);
                            x = getfloat(ref i, obj);
                            values[k * 3] = x;
                            carac_Lu = obj[i];
                            i++;
                            if (carac_Lu == Convert.ToInt32('/'))
                            {
                                carac_Lu = obj[i];
                                if (carac_Lu == '/')
                                {
                                    y = 1.0f;
                                    ListeCoordTextures.Add(new Vector2(1, 1));
                                }
                                else
                                {
                                    gotonextvalue(ref i, obj);
                                    y = getfloat(ref i, obj);
                                }
                                values[k * 3 + 1] = y;
                                gotonextvalue(ref i, obj);
                                z = getfloat(ref i, obj);
                                values[k * 3 + 2] = z;
                            }
                            else i--;
                        }
                        for (int k=0;k<3;k++) {
                            ListeVertex.Add(
                                new Vertex()
                                {
                                    Position = new Vector4(
                                        ListeCoordSommets[Convert.ToInt32(values[k * 3]) - 1][0] + referentiel.X,
                                        ListeCoordSommets[Convert.ToInt32(values[k * 3]) - 1][1] + referentiel.Y,
                                        ListeCoordSommets[Convert.ToInt32(values[k * 3]) - 1][2] + referentiel.Z,
                                        ListeCoordSommets[Convert.ToInt32(values[k * 3]) - 1][3] + referentiel.W
                                    ),
                                    //Color = new Color(couleur[i].Red, couleur[i].Green, couleur[i].Blue, 1.0f),
                                    CoordTextures = new Vector2(
                                        ListeCoordTextures[Convert.ToInt32(values[k * 3 + 1]) - 1][0],
                                        ListeCoordTextures[Convert.ToInt32(values[k * 3 + 1]) - 1][1]
                                    ),
                                    Color = new Vector4(couleur[k].Red, couleur[k].Green, couleur[k].Blue, 1.0f),

                                    // NORMAL
                                    Normal = ListeNormales[Convert.ToInt32(values[k * 3 + 2]) - 1]
                                }
                            );
                        }
                        if (is_4)
                        {
                            for (int k = 0; k < 4; k++)
                            {
                                if (k != 1)
                                {
                                    ListeVertex.Add(
                                        new Vertex()
                                        {
                                            Position = new Vector4(
                                                ListeCoordSommets[Convert.ToInt32(values[k * 3]) - 1][0] + referentiel.X,
                                                ListeCoordSommets[Convert.ToInt32(values[k * 3]) - 1][1] + referentiel.Y,
                                                ListeCoordSommets[Convert.ToInt32(values[k * 3]) - 1][2] + referentiel.Z,
                                                ListeCoordSommets[Convert.ToInt32(values[k * 3]) - 1][3] + referentiel.W
                                            ),
                                            //Color = new Color(couleur[i].Red, couleur[i].Green, couleur[i].Blue, 1.0f),
                                            CoordTextures = new Vector2(
                                                ListeCoordTextures[Convert.ToInt32(values[k * 3 + 1]) - 1][0],
                                                ListeCoordTextures[Convert.ToInt32(values[k * 3 + 1]) - 1][1]
                                            ),
                                            Color = new Vector4(couleur[0].Red, couleur[0].Green, couleur[0].Blue, 1.0f),
                                            // NORMAL
                                            Normal = ListeNormales[Convert.ToInt32(values[k * 3 + 2]) - 1]
                                        }
                                    );
                                }
                            }
                        }
                    }
                }
                else if (carac_Lu == Convert.ToInt32('m'))
                {
                    carac_Lu = obj[i];
                    i++;
                    if (carac_Lu == 't')
                    {
                        carac_Lu = obj[i];
                        i++;
                        if (carac_Lu == 'l')
                        {
                            carac_Lu = obj[i];
                            i++;
                            if (carac_Lu == 'l')
                            {
                                carac_Lu = obj[i];
                                i++;
                                if (carac_Lu == 'i')
                                {
                                    carac_Lu = obj[i];
                                    i++;
                                    if (carac_Lu == 'b')
                                    {
                                        carac_Lu = obj[i];
                                        i++;
                                        if (carac_Lu == ' ')
                                        {
                                            // Console.WriteLine("# Material: {0}", getstring(ref i, obj));

                                            // Ouvrir MTLLIB
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                if (precedent_pourcentage != Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(obj.Length) * 100))
                {
                    precedent_pourcentage = Convert.ToInt32(Convert.ToSingle(i) / Convert.ToSingle(obj.Length) * 100);
                    Console.WriteLine("\t{0} % lu", precedent_pourcentage);
                    Console.CursorTop--;
                }
                gotonextline(ref i, obj);
                if (i != obj.Length)
                {
                    carac_Lu = obj[i];
                    i++;
                }
                else carac_Lu = -1;
            }

            if (is_4) nbfaces *= 2;

            return ListeVertex.ToArray();
        }
    }
}
