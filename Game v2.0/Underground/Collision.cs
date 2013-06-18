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

        //public static List<BoundingBox> Initialize(structVertex[][] VerticesFinal)
        //{
        //    var ListeBoundingBoxes = new List<BoundingBox>();
        //    for (int k = 0; k <= VerticesFinal.GetUpperBound(0); k++)
        //    {
        //        Program.WriteNicely("#", 12, "Creation des BBoxes !");
        //        int t1 = VerticesFinal[k].GetUpperBound(0);

        //        int vecPerBox = 2;

        //        for (int i = 0; i <= t1 - vecPerBox; i = i + vecPerBox)
        //        {
        //            var tmp_vec = new List<Vector3>();

        //            for (int j = 0; j < vecPerBox; j++)
        //            {
        //                tmp_vec.Add((Vector3)VerticesFinal[k][i + j].Position);
        //            }
        //            var tmp2 = SharpDX.BoundingBox.FromPoints(tmp_vec.ToArray());
        //            ListeBoundingBoxes.Add(tmp2);
        //        }
        //    }
        //    bboxList = ListeBoundingBoxes;

        //    return ListeBoundingBoxes;
        //}

        public static List<BoundingBox> Initialize()
        {
            var ListeBoundingBoxes = new List<BoundingBox>();

            foreach (structOBJ obj in Program.Liste_OBJ)
            {
                foreach (structModel model in obj.data)
                {
                    var vecPerBox = 2;

                    for (int i = 0; i <= model.Sommets.GetUpperBound(0) - vecPerBox; i = i + vecPerBox)
                    {
                        var tmp_vec = new List<Vector3>();

                        for (int j = 0; j < vecPerBox; j++)
                        {
                            tmp_vec.Add((Vector3)model.Sommets[i + j].Position);
                        }
                        var tmp2 = SharpDX.BoundingBox.FromPoints(tmp_vec.ToArray());
                        ListeBoundingBoxes.Add(tmp2);
                    }
                }
            }

            bboxList = ListeBoundingBoxes;
            return ListeBoundingBoxes;
        }

        public static void AddVertex(List<Vector4> vertices)
        {
            var vecPerBox = 2;

            for (int i = 0; i <= vertices.Count - vecPerBox; i = i + vecPerBox)
            {
                var tmp_vec = new List<Vector3>();

                for (int j = 0; j < vecPerBox; j++)
                {
                    tmp_vec.Add((Vector3)vertices[i + j]);
                }
                var tmp2 = SharpDX.BoundingBox.FromPoints(tmp_vec.ToArray());
                bboxList.Add(tmp2);
            }


        }

        public static bool CheckCollisions(Vector3 pos)
        {
            var cameraSphere = new BoundingSphere(-pos, 0.15f);

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
