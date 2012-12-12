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

namespace MiniCube
{
    static class ObjLoader
    {
        private static void gotonextvalue(ref FileStream fichier)
        {
            int carac_Lu = fichier.ReadByte();
            while (!(
                (carac_Lu >= Convert.ToInt32('0') && carac_Lu <= Convert.ToInt32('9')) ||
                (carac_Lu >= Convert.ToInt32('a') && carac_Lu <= Convert.ToInt32('z')) ||
                (carac_Lu >= Convert.ToInt32('A') && carac_Lu <= Convert.ToInt32('Z')) ||
                carac_Lu == Convert.ToInt32('-') ||
                carac_Lu == Convert.ToInt32('.')
                ))
            {
                carac_Lu = fichier.ReadByte();
            }
            fichier.Position--;
        }

        private static float getfloat(ref FileStream fichier)
        {
            bool est_negatif = false;
            bool virgule = false;
            float resultat = 0;
            int decalage = 0;
            int carac_Lu = fichier.ReadByte();
            if (carac_Lu == Convert.ToInt32('-'))
            {
                est_negatif = true;
                carac_Lu = fichier.ReadByte();
            }
            while ((carac_Lu >= Convert.ToInt32('0') && carac_Lu <= Convert.ToInt32('9')) || carac_Lu == Convert.ToInt32('.'))
            {
                if (carac_Lu == Convert.ToInt32('.')) virgule = true;
                else
                {
                    if (virgule) decalage++;
                    if (est_negatif) resultat = resultat * 10 - (carac_Lu - Convert.ToInt32('0'));
                    else resultat = resultat * 10 + (carac_Lu - Convert.ToInt32('0'));
                }
                carac_Lu = fichier.ReadByte();
            }
            fichier.Position--;
            return Convert.ToSingle(resultat/(Math.Pow(10,decalage)));
        }

        private static void gotonextline(ref FileStream fichier)
        {
            int carac_Lu = fichier.ReadByte();
            while (carac_Lu != '\n')
            {
                carac_Lu = fichier.ReadByte();
            }
        }

        public static Vertex[] read_obj(string path, Vector4 referentiel, ref int nbfaces, ref int nbsommets, ref int nbnormales, ref int nbtextures)
        {
            Console.WriteLine("# Ouverture du fichier {0}", path);
            nbfaces = 0;
            nbsommets = 0;
            List<Vector2> ListeCoordTextures = new List<Vector2>();
            List<Vector4> ListeCoordSommets = new List<Vector4>();
            List<Vector4> ListeNormales = new List<Vector4>();
            List<Vertex> ListeVertex = new List<Vertex>();
            float x=1, y=1, z=1;

            FileStream fichier = File.Open(path, FileMode.Open);
            fichier.Seek(0, SeekOrigin.Begin);

            Color3[] couleur = new Color3[3] { 
                Color.White.ToColor3(),
                Color.Green.ToColor3(),
                Color.Green.ToColor3()
            };

            int carac_Lu = fichier.ReadByte();
            int count;
            while (carac_Lu != -1)
            {
                count = ListeVertex.Count;
                if (carac_Lu == Convert.ToInt32('v'))
                {
                    carac_Lu = fichier.ReadByte();
                    if (carac_Lu == Convert.ToInt32('t'))
                    {
                        carac_Lu = fichier.ReadByte();
                        if (carac_Lu == Convert.ToInt32(' '))
                        {
                            nbtextures++;
                            // Il s'agit d'une coordonnée de texture
                            gotonextvalue(ref fichier);
                            x = getfloat(ref fichier);
                            gotonextvalue(ref fichier);
                            y = getfloat(ref fichier);
                            ListeCoordTextures.Add(new Vector2(x, y));
                        }
                        else gotonextvalue(ref fichier);
                    }
                    else if (carac_Lu == Convert.ToInt32('n'))
                    {
                        carac_Lu = fichier.ReadByte();
                        if (carac_Lu == Convert.ToInt32(' '))
                        {
                            nbnormales++;
                            // Il s'agit d'une normale
                            gotonextvalue(ref fichier);
                            x = getfloat(ref fichier);
                            gotonextvalue(ref fichier);
                            y = getfloat(ref fichier);
                            gotonextvalue(ref fichier);
                            z = getfloat(ref fichier);
                            ListeNormales.Add(new Vector4(x, y, z, 1.0f));
                        }
                        else gotonextvalue(ref fichier);
                    }
                    else if (carac_Lu == Convert.ToInt32(' '))
                    {
                        nbsommets++;
                        // Il s'agit d'une coordonnée de sommet
                        gotonextvalue(ref fichier);
                        x = getfloat(ref fichier);
                        gotonextvalue(ref fichier);
                        y = getfloat(ref fichier);
                        gotonextvalue(ref fichier);
                        z = getfloat(ref fichier);
                        ListeCoordSommets.Add(new Vector4(x, y, z, 1.0f));
                    }
                }
                else if (carac_Lu == Convert.ToInt32('f'))
                {
                    carac_Lu = fichier.ReadByte();
                    if (carac_Lu == ' ')
                    {
                        nbfaces++;
                        // Il s'agit d'une face

                        // Sommet 1 2 3
                        for (int i = 0; i < 3; i++)
                        {
                            gotonextvalue(ref fichier);
                            x = getfloat(ref fichier);
                            carac_Lu = fichier.ReadByte();
                            if (carac_Lu == Convert.ToInt32('/'))
                            {
                                gotonextvalue(ref fichier);
                                y = getfloat(ref fichier);
                                gotonextvalue(ref fichier);
                                z = getfloat(ref fichier);
                            }
                            else fichier.Position--;

                            ListeVertex.Add(
                                new Vertex()
                                {
                                    Position = new Vector4(
                                        ListeCoordSommets[Convert.ToInt32(x) - 1][0] + referentiel.X,
                                        ListeCoordSommets[Convert.ToInt32(x) - 1][1] + referentiel.Y,
                                        ListeCoordSommets[Convert.ToInt32(x) - 1][2] + referentiel.Z,
                                        ListeCoordSommets[Convert.ToInt32(x) - 1][3] + referentiel.W
                                    ),
                                    //Color = new Color(couleur[i].Red, couleur[i].Green, couleur[i].Blue, 1.0f),
                                    CoordTextures = new Vector2(
                                        ListeCoordTextures[Convert.ToInt32(y) - 1][0],
                                        ListeCoordTextures[Convert.ToInt32(y) - 1][1]
                                    ),
                                    Color = new Vector4(couleur[i].Red, couleur[i].Green, couleur[i].Blue, 1.0f),
                                }
                            );
                        }
                    }
                }
                Console.WriteLine("\t{0} % lu", Convert.ToSingle(fichier.Position) / Convert.ToSingle(fichier.Length) * 100);
                Console.CursorTop--;
                gotonextline(ref fichier);
                carac_Lu = fichier.ReadByte();
            }
            Console.WriteLine("# Le fichier {0} a été lu", path);
            fichier.Close();
            return ListeVertex.ToArray();
        }
    }
}
