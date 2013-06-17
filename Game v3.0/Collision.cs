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

        private static List<BoundingBox> bboxList;

        public static List<BoundingBox> Initialize(structVertex[][] VerticesFinal)
        {
            var ListeBoundingBoxes = new List<BoundingBox>();
            for (int k = 0; k <= VerticesFinal.GetUpperBound(0); k++)
            {
                Program.WriteNicely("#", 12, "Creation des BBoxes !");
                int t1 = VerticesFinal[k].GetUpperBound(0);

                int vecPerBox = 2;

                for (int i = 0; i <= t1 - vecPerBox; i = i + vecPerBox)
                {
                    var tmp_vec = new List<Vector3>();

                    for (int j = 0; j < vecPerBox; j++)
                    {
                        tmp_vec.Add((Vector3)VerticesFinal[k][i + j].Position);
                    }
                    var tmp2 = SharpDX.BoundingBox.FromPoints(tmp_vec.ToArray());
                    ListeBoundingBoxes.Add(tmp2);
                }
            }
            bboxList = ListeBoundingBoxes;

            return ListeBoundingBoxes;
        }

        public static bool CheckCollisions(Vector3 pos)
        {
            var cameraSphere = new BoundingSphere(-pos, 0.1f);

            foreach (var BBox in bboxList)
            {
                if (BBox.Intersects(ref cameraSphere))
                {
                    Program.WriteNicely("!", 12, "Intersection");
                    return true;
                }
            }
            return false;
        }
    }
}
