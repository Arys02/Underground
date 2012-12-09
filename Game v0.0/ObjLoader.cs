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

        public static Vector4[] read_obj(string path,Vector4 referentiel)
        {
            List<Vector2> ListeCoordTextures = new List<Vector2>();
            List<Vector4> ListeCoordSommets = new List<Vector4>();
            List<Vector4> ListeNormales = new List<Vector4>();
            List<Vector4> ListeVertex = new List<Vector4>();
            float x=1, y=1, z=1;

            FileStream fichier = File.Open(path, FileMode.Open);
            fichier.Seek(0, SeekOrigin.Begin);


            Color3 couleur1 = Color.White.ToColor3();
            Color3 couleur2 = Color.YellowGreen.ToColor3();
            Color3 couleur3 = Color.YellowGreen.ToColor3();
            /*int nbsommets = 0;
            int a, b, c, compteur = 0;
            bool est_positif;*/

            int carac_Lu = fichier.ReadByte();
            while (carac_Lu != -1)
            {
                if (carac_Lu == Convert.ToInt32('v'))
                {
                    carac_Lu = fichier.ReadByte();
                    if (carac_Lu == Convert.ToInt32('t'))
                    {
                        carac_Lu = fichier.ReadByte();
                        if (carac_Lu == Convert.ToInt32(' '))
                        {
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
                        // Il s'agit d'une face

                        // Sommet 1
                        //for (int i = 0; i < 3; i++)
                        //{
                            // Sommet 1
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
                            ListeVertex.Add(
                                new Vector4(
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][0] + referentiel.X,
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][1] + referentiel.Y,
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][2] + referentiel.Z,
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][3] + referentiel.W
                                )
                            );
                            ListeVertex.Add(
                                new Vector4(couleur1.Red, couleur1.Green, couleur1.Blue, 1.0f)
                            );

                            // Sommet 2
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
                            ListeVertex.Add(
                                new Vector4(
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][0] + referentiel.X,
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][1] + referentiel.Y,
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][2] + referentiel.Z,
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][3] + referentiel.W
                                )
                            );
                            ListeVertex.Add(
                                new Vector4(couleur2.Red, couleur2.Green, couleur2.Blue, 1.0f)
                            );

                            // Sommet 3
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
                            ListeVertex.Add(
                                new Vector4(
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][0] + referentiel.X,
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][1] + referentiel.Y,
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][2] + referentiel.Z,
                                    ListeCoordSommets[Convert.ToInt32(x) - 1][3] + referentiel.W
                                )
                            );
                            ListeVertex.Add(
                                new Vector4(couleur3.Red, couleur3.Green, couleur3.Blue, 1.0f)
                            );
                        //}
                    }
                }
                gotonextline(ref fichier);
                carac_Lu = fichier.ReadByte();
            }
            fichier.Close();
            return ListeVertex.ToArray();
        }
    }
}






/*
int carac_Lu = fichier.ReadByte();
float[,] sommets = new float[maxfaces, 3];
fichier.Seek(0, SeekOrigin.Begin);
carac_Lu = fichier.ReadByte();

for (int i = 0; i < maxfaces; i++)
{
    sommets[i, 0] = 0.0f;
    sommets[i, 1] = 0.0f;
    sommets[i, 2] = 0.0f;
}
Vector4[] liste_vecteurs = new Vector4[maxfaces * 6];



while (carac_Lu != -1) // Tant que la fin du fichier n'est pas atteinte
{
    if (carac_Lu == 'v')
    {
        for (int j = 0; j < 3; j++)
        {
            est_positif = true;
            carac_Lu = fichier.ReadByte();
            if (carac_Lu == ' ')
            {
                carac_Lu = fichier.ReadByte();
                // Partie >1
                if (carac_Lu == '-')
                {
                    est_positif = false;
                    carac_Lu = fichier.ReadByte();
                }
                while (carac_Lu != '.')
                {
                    if (est_positif) sommets[nbsommets, j] = sommets[nbsommets, j] * 10 + Convert.ToSingle(carac_Lu - '0');
                    else sommets[nbsommets, j] = sommets[nbsommets, j] * 10 - Convert.ToSingle(carac_Lu - '0');
                    carac_Lu = fichier.ReadByte();
                }
                // Partie <1
                if (est_positif)
                {
                    sommets[nbsommets, j] += (Convert.ToSingle(fichier.ReadByte() - '0') / 10);
                    sommets[nbsommets, j] += (Convert.ToSingle(fichier.ReadByte() - '0') / 100);
                    sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 1000);
                    sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 10000);
                    sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 100000);
                    sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 1000000);
                }
                else
                {
                    sommets[nbsommets, j] -= (Convert.ToSingle(fichier.ReadByte() - '0') / 10);
                    sommets[nbsommets, j] -= (Convert.ToSingle(fichier.ReadByte() - '0') / 100);
                    sommets[nbsommets, j] = sommets[nbsommets, j] - (Convert.ToSingle(fichier.ReadByte() - '0') / 1000);
                    sommets[nbsommets, j] = sommets[nbsommets, j] - (Convert.ToSingle(fichier.ReadByte() - '0') / 10000);
                    sommets[nbsommets, j] = sommets[nbsommets, j] - (Convert.ToSingle(fichier.ReadByte() - '0') / 100000);
                    sommets[nbsommets, j] = sommets[nbsommets, j] - (Convert.ToSingle(fichier.ReadByte() - '0') / 1000000);
                }
            }
            else
            {
                while (carac_Lu != '\n')
                {
                    carac_Lu = fichier.ReadByte();
                }
            }
        }
        nbsommets++;
    }
    else if (carac_Lu == 'f')
    {
        // On lit quel sommet on doit assembler pour
        fichier.ReadByte();
        a = 0;
        for (int temp = fichier.ReadByte(); temp != Convert.ToInt32(' '); temp = fichier.ReadByte())
        {
            a = a * 10 + (temp - Convert.ToInt32('0'));
        }

        b = 0;
        for (int temp = fichier.ReadByte(); temp != Convert.ToInt32(' '); temp = fichier.ReadByte())
        {
            b = b * 10 + (temp - Convert.ToInt32('0'));
        }

        c = 0;
        for (int temp = fichier.ReadByte(); temp >= Convert.ToInt32('0') && temp <= Convert.ToInt32('9'); temp = fichier.ReadByte())
        {
            c = c * 10 + (temp - Convert.ToInt32('0'));
        }

        // Va me stocker un triangle ...
        liste_vecteurs[compteur * 6 + 0] = new Vector4(sommets[a - 1, 0], sommets[a - 1, 1], sommets[a - 1, 2], 1.0f);
        liste_vecteurs[compteur * 6 + 1] = new Vector4(couleur1.Red, couleur1.Green, couleur1.Blue, 1.0f);
        liste_vecteurs[compteur * 6 + 2] = new Vector4(sommets[b - 1, 0], sommets[b - 1, 1], sommets[b - 1, 2], 1.0f);
        liste_vecteurs[compteur * 6 + 3] = new Vector4(couleur2.Red, couleur2.Green, couleur2.Blue, 1.0f);
        liste_vecteurs[compteur * 6 + 4] = new Vector4(sommets[c - 1, 0], sommets[c - 1, 1], sommets[c - 1, 2], 1.0f);
        liste_vecteurs[compteur * 6 + 5] = new Vector4(couleur3.Red, couleur3.Green, couleur3.Blue, 1.0f);
        compteur++;
    }
    else
    {
        while (carac_Lu != '\n' && carac_Lu != -1)
        {
            carac_Lu = fichier.ReadByte();
        }
    }
    carac_Lu = fichier.ReadByte();
}*/
