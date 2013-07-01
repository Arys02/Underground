using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using SharpDX.Windows;

namespace LOL_l.Importer
{
    class ToolBox
    {
        public Vector4 CalcTangentVector(Vector3[] position, Vector2[] texCoord, Vector3 normal)
        {
            Vector4 tangent;

            Vector3 edge1 = new Vector3(position[1].X - position[0].X, position[1].Y - position[0].Y, position[1].Z - position[0].Z);
            Vector3 edge2 = new Vector3(position[2].X - position[1].X, position[2].Y - position[1].Y, position[2].Z - position[1].Z);

            edge1.Normalize();
            edge2.Normalize();

            Vector2 texEdge1 = new Vector2(texCoord[1].X - texCoord[0].X, texCoord[1].Y - texCoord[0].Y);
            Vector2 texEdge2 = new Vector2(texCoord[2].X - texCoord[1].X, texCoord[2].Y - texCoord[1].Y);

            texEdge1.Normalize();
            texEdge2.Normalize();

            Vector3 bitangent;
            float det = (texEdge1.X * texEdge2.Y) - (texEdge1.Y * texEdge2.X);

            if (Math.Abs(det) < 1e-6f)
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

            Vector3 n = normal;
            Vector3 t = new Vector3(tangent.X, tangent.Y, tangent.Z);
            Vector3 b = Vector3.Cross(n, t);
            tangent.W = (Vector3.Dot(b, bitangent) < 0.0f) ? -1.0f : 1.0f;
            return tangent;
        }
        public void gotonextvalue(ref int i, ref byte[] obj)
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
        public string getstring(ref int i, ref byte[] obj)
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
        public float getfloat(ref int i, ref byte[] obj)
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
        public void gotonextline(ref int i, ref byte[] obj)
        {
            int carac_Lu;
            do
            {
                carac_Lu = obj[i];
                i++;
            } while (carac_Lu != '\n' && i < obj.Length);
        }
        public Vector3 Vec4to3(Vector4 Source)
        {
            return new Vector3(Source.X, Source.Y, Source.Z);
        }
    }
}
