using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.Windows;

namespace Underground
{
    class Collision
    {
        public static IntervalKDTree<string> tree;

        /// <summary>
        /// Créée une bounding contenant tout les points compris dans le tableau mis en paramètre.
        /// </summary>
        private static void PutFromPoints(Vector3[] points)
        {
            if (points == null)
                throw new ArgumentNullException("points");

            Vector3 result1 = new Vector3(float.MaxValue);
            Vector3 result2 = new Vector3(float.MinValue);
            for (int index = 0; index < points.Length; ++index)
            {
                Vector3.Min(ref result1, ref points[index], out result1);
                Vector3.Max(ref result2, ref points[index], out result2);
            }

            //BUGFIX : car les distances entre les sommets des murs sont trop petites O_O
            //result1 -= new Vector3(0.05f);
            //result2 += new Vector3(0.05f);

            tree.Put(result1.X, result1.Y, result1.Z, result2.X, result2.Y, result2.Z, "wall" + result1.X + "," + result1.Y + "," + result1.Z + "-" + result2.X + "," + result2.Y + "," + result2.Z);    
        }


        public static void Initialize()
        {
            tree = new IntervalKDTree<string>(70000, 700);

            foreach (structOBJ obj in Program.Liste_OBJ)
            {
                if (obj.sera_affiche)
                {
                    foreach (structModel model in obj.data)
                    {
                        var vecPerBox = 2;

                        for (int i = 0; i <= model.Sommets.GetUpperBound(0) - vecPerBox; i = i + vecPerBox)
                        {
                            var tmp_vec = new List<Vector3>();

                            for (int j = 0; j < vecPerBox; j++)
                            {
                                tmp_vec.Add(
                                    (Vector3) Vector4.Transform(model.Sommets[i + j].Position, obj.Transformation));
                            }

                            PutFromPoints(tmp_vec.ToArray());
                        }
                    }
                }
            }
        }

        public static void AddVertex(structOBJ obj)
        {
            var vecPerBox = 2;

            foreach (structModel model in obj.data)
            {
                for (int i = 0; i <= model.Sommets.GetUpperBound(0) - vecPerBox; i = i + vecPerBox)
                {
                    var tmp_vec = new List<Vector3>();

                    for (int j = 0; j < vecPerBox; j++)
                    {
                        tmp_vec.Add(
                            (Vector3)Vector4.Transform(model.Sommets[i + j].Position, obj.Transformation));
                    }

                    PutFromPoints(tmp_vec.ToArray());
                }
            }
        }

        public static bool CheckCollisions(Vector3 pos)
        {
            bool res = tree.GetIntersect(-pos.X - 0.15 * 70, -pos.Y - 0.15 * 70, -pos.Z - 0.15 * 70, -pos.X + 0.15 * 70, -pos.Y + 0.15 * 70, -pos.Z + 0.15 * 70);
            
            // Pour savoir quel mur à été touché : 
            //HashSet<string> foundValues;
            //foundValues = tree.GetValues(-pos.X - 0.1, -pos.Y - 0.1, -pos.Z - 0.1, -pos.X + 0.1, -pos.Y + 0.1, -pos.Z + 0.1, new HashSet<string>());

            //foreach (var foundValue in foundValues)
            //{
            //    Console.WriteLine(foundValue);
            //}

            return res;
        }

    }
}
