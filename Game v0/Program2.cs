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
    internal static class Program
    {
        //[STAThread]


        private static int compte_faces(string path)
        {
            FileStream fichier = File.Open(path,FileMode.Open);
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

        private static Vector4[] read_obj(string path,int maxfaces)
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
                    a=0;
                    for (int temp = fichier.ReadByte(); temp != Convert.ToInt32(' '); temp = fichier.ReadByte())
                    {
                        a = a*10 + (temp-Convert.ToInt32('0'));
                    }
                    b = 0;
                    for (int temp = fichier.ReadByte(); temp != Convert.ToInt32(' '); temp = fichier.ReadByte())
                    {
                        b = b * 10 + (temp - Convert.ToInt32('0'));
                    }
                    c = 0;
                    for (int temp = fichier.ReadByte(); temp != Convert.ToInt32('\n'); temp = fichier.ReadByte())
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
                    while (carac_Lu != '\n')
                    {
                        carac_Lu = fichier.ReadByte();
                    }
                }
                carac_Lu = fichier.ReadByte();
            }
            return liste_vecteurs;
        }

        private static void Main()
        {
            int[] position = new int[3];
            position[0] = 0;
            position[1] = 0;
            position[2] = 0;
            //string path = "untitled.obj";
            //string path = "FilmNoirTriangl.obj";
            string path = "ressource.obj";
            //string path = "FilmNoirTriangl.obj";
            var form = new RenderForm("SharpDX - MiniCube Direct3D9 Sample");
            var direct3D = new Direct3D();
            var device = new Device(direct3D, 0, DeviceType.Reference, form.Handle, CreateFlags.HardwareVertexProcessing, new PresentParameters(form.ClientSize.Width, form.ClientSize.Height));

            // Creates the VertexBuffer
            var vertices = new VertexBuffer(device, (Utilities.SizeOf<Vector4>() + Utilities.SizeOf<Vector4>()) * 3 * compte_faces(path), Usage.WriteOnly, VertexFormat.None, Pool.Managed);

            int nbfaces = compte_faces(path);
            vertices.Lock(0, 0, LockFlags.None).WriteRange(read_obj(path, nbfaces));
            vertices.Unlock();

            // Compiles the effect
            var effect = Effect.FromFile(device, "MiniCube.fx", ShaderFlags.None);

            // Allocate Vertex Elements
            var vertexElems = new[] {
        		new VertexElement(0, 0, DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
        		new VertexElement(0, Convert.ToInt16(Utilities.SizeOf<Vector4>()), DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
				VertexElement.VertexDeclarationEnd
        	};

            // Creates and sets the Vertex Declaration
            var vertexDecl = new VertexDeclaration(device, vertexElems);
            device.SetStreamSource(0, vertices, 0, Utilities.SizeOf<Vector4>() + Utilities.SizeOf<Vector4>());
            device.VertexDeclaration = vertexDecl;

            // Get the technique
            var technique = effect.GetTechnique(0);
            var pass = effect.GetPass(technique, 0);

            // Prepare matrices
            var view = Matrix.LookAtLH(new Vector3(0, 0, -5), new Vector3(0, 0, 0), Vector3.UnitY);
            var proj = Matrix.PerspectiveFovLH((float)Math.PI / 4.0f, form.ClientSize.Width / (float)form.ClientSize.Height, 0.1f, 100.0f);
            var viewProj = Matrix.Multiply(view, proj);

            // Use clock
            var clock = new Stopwatch();
            clock.Start();

            RenderLoop.Run(form, () =>
            {
                //Input input;
                var time = clock.ElapsedMilliseconds / 1000.0f;

                device.Clear(ClearFlags.Target | ClearFlags.ZBuffer, Color.Black, 1.0f, 0);
                device.BeginScene();

                effect.Technique = technique;
                effect.Begin();
                effect.BeginPass(0);
                var worldViewProj = Matrix.Translation(new Vector3(10+time, -20, 0)) * Matrix.RotationX(-(float)Math.PI / 6)  * viewProj;
                //var worldViewProj = Matrix.Translation(new Vector3(position[0],position[1],position[2])) * Matrix.RotationX(time) * Matrix.RotationY(time * 2) * Matrix.RotationZ(time * .7f) * viewProj;
                effect.SetValue("worldViewProj", worldViewProj);

                device.DrawPrimitives(PrimitiveType.TriangleList, 0, nbfaces);

                effect.EndPass();
                effect.End();

                device.EndScene();
                device.Present();
            });

            effect.Dispose();
            vertices.Dispose();
            device.Dispose();
            direct3D.Dispose();
        }
    }
}