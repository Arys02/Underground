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
        public static int compte_faces(string path)
        {
            FileStream fichier = File.Open(path, FileMode.Open);
            int compteur = 0;
            int carac_Lu = fichier.ReadByte();
            while (carac_Lu != -1)
            {
                if (carac_Lu == 'f')
                {
                    compteur++;
                }
                else
                {
                    while (carac_Lu != '\n')
                    {
                        carac_Lu = fichier.ReadByte();
                    }
                }
                carac_Lu = fichier.ReadByte();
            }
            fichier.Close();
            return compteur;
        }

        public static Vector4[] read_obj(string path, int maxfaces,float refx,float refy,float refz)
        {
            FileStream fichier = File.Open(path, FileMode.Open);
            Color3 couleur1 = Color.White.ToColor3();
            Color3 couleur2 = Color.YellowGreen.ToColor3();
            Color3 couleur3 = Color.YellowGreen.ToColor3();
            int nbsommets = 0;
            int a, b, c, compteur = 0;
            bool est_positif;
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
                        if (fichier.ReadByte() == ' ')
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
                                sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 10);
                                sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 100);
                                sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 1000);
                                sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 10000);
                                sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 100000);
                                sommets[nbsommets, j] = sommets[nbsommets, j] + (Convert.ToSingle(fichier.ReadByte() - '0') / 1000000);
                            }
                            else
                            {
                                sommets[nbsommets, j] = sommets[nbsommets, j] - (Convert.ToSingle(fichier.ReadByte() - '0') / 10);
                                sommets[nbsommets, j] = sommets[nbsommets, j] - (Convert.ToSingle(fichier.ReadByte() - '0') / 100);
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
            }
            return liste_vecteurs;
        }
    }
}

