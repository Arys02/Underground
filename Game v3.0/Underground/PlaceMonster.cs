using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SharpDX;

namespace Underground
{
    class PlaceMonster
    {
        private Vector3[] Transform_mazetype_in_V3array(int type)
        {
            Vector3[] resultat = new Vector3[5];
            //Program.newmaze.maze[a,b].h
            return resultat;
        }
        public Vector3 GetPositionMonster(out float ang)
        {
            // Variables
            Vector3 positionMob = new Vector3();
            int slender_distancenbunite = 10 * 70;
            float slender_distanceapprocheunite = 2.1f * 70;
            float angle;
            int a = Convert.ToInt32(Ingame.macamera.position.X + Ingame.rayon_salles) / (2 * Ingame.rayon_salles);
            int b = Convert.ToInt32(Ingame.macamera.position.Z + Ingame.rayon_salles) / (2 * Ingame.rayon_salles);
            Ingame.ab_stay_in_bounds(ref a, ref b);
            float a2 = a;
            float b2 = b;

            Matrix.Translation(0, 0, 2 * Ingame.rayon_salles / 3);
            // On détermine dans quelle direction regarde le joueur
            if (Ingame.macamera.angle.Y < Math.PI / 4)
                angle = 0;
            else if (Ingame.macamera.angle.Y < 3 * Math.PI / 4)
                angle = Convert.ToSingle(1 * Math.PI / 2);
            else if (Ingame.macamera.angle.Y < 5 * Math.PI / 4)
                angle = Convert.ToSingle(2 * Math.PI / 2);
            else if (Ingame.macamera.angle.Y < 7 * Math.PI / 4)
                angle = Convert.ToSingle(3 * Math.PI / 2);
            else
                angle = 0;

            // On détermine la position et dans quelle direction regardera le mob
            ang = 0;
            float calc = 1 - (slender_distanceapprocheunite * (float)Ingame.compteur_slender) / (float)slender_distancenbunite;
            #region 0 * Math.PI / 2
            if (angle == 0)
            {
                if (Program.newmaze.maze[a, b].b == 1)
                {
                    ang = Convert.ToSingle(Math.PI);
                    b2 -= calc;
                }
                else if (Program.newmaze.maze[a, b].g == 1)
                {
                    ang = Convert.ToSingle(3 * Math.PI / 2);
                    a2 -= calc;
                }
                else if (Program.newmaze.maze[a, b].d == 1)
                {
                    ang = Convert.ToSingle(1 * Math.PI / 2);
                    a2 += calc;
                }
                else if (Program.newmaze.maze[a, b].h == 1)
                {
                    ang = 0;
                    b2 += calc;
                }
                else
                {
                    Console.WriteLine("wtf ?");
                }
            }
            #endregion
            #region 1 * Math.PI / 2
            if (angle == Convert.ToSingle(Math.PI / 2))
            {
                if (Program.newmaze.maze[a, b].d == 1)
                {
                    ang = Convert.ToSingle(1 * Math.PI / 2);
                    a2 += calc;
                }
                else if (Program.newmaze.maze[a, b].h == 1)
                {
                    ang = 0;
                    b2 += calc;
                }
                else if (Program.newmaze.maze[a, b].b == 1)
                {
                    ang = Convert.ToSingle(Math.PI);
                    b2 -= calc;
                }
                else if (Program.newmaze.maze[a, b].g == 1)
                {
                    ang = Convert.ToSingle(3 * Math.PI / 2);
                    a2 -= calc;
                }
                else
                {
                    Console.WriteLine("wtf ?");
                }
            }
            #endregion
            #region 2 * Math.PI / 2
            if (angle == Convert.ToSingle(2 * Math.PI / 2))
            {
                if (Program.newmaze.maze[a, b].h == 1)
                {
                    ang = 0;
                    b2 += calc;
                }
                else if (Program.newmaze.maze[a, b].g == 1)
                {
                    ang = Convert.ToSingle(3 * Math.PI / 2);
                    a2 -= calc;
                }
                else if (Program.newmaze.maze[a, b].d == 1)
                {
                    ang = Convert.ToSingle(1 * Math.PI / 2);
                    a2 += calc;
                }
                else if (Program.newmaze.maze[a, b].b == 1)
                {
                    ang = Convert.ToSingle(Math.PI);
                    b2 -= calc;
                }
                else
                {
                    Console.WriteLine("wtf ?");
                }
            }
            #endregion
            #region 3 * Math.PI / 2
            if (angle == Convert.ToSingle(3 * Math.PI / 2))
            {
                if (Program.newmaze.maze[a, b].g == 1)
                {
                    ang = Convert.ToSingle(3 * Math.PI / 2);
                    a2 -= calc;
                }
                else if (Program.newmaze.maze[a, b].b == 1)
                {
                    ang = Convert.ToSingle(Math.PI);
                    b2 -= calc;
                }
                else if (Program.newmaze.maze[a, b].h == 1)
                {
                    ang = 0;
                    b2 += calc;
                }
                else if (Program.newmaze.maze[a, b].d == 1)
                {
                    ang = Convert.ToSingle(1 * Math.PI / 2);
                    a2 += calc;
                }
                else
                {
                    Console.WriteLine("wtf ?");
                }
            }
            #endregion

            positionMob.X = a2 * Ingame.rayon_salles * 2;
            positionMob.Y = 0;
            positionMob.Z = b2 * Ingame.rayon_salles * 2;

            Console.WriteLine("{0} {1}", Ingame.macamera.position.X, Ingame.macamera.position.Z);


            Vector3 resultat = new Vector3(
                -(float)
                (positionMob.X /*+
                (slender_distancenbunite - slender_distanceapprocheunite * Ingame.compteur_slender) *
                Math.Sin(angle)*/),
                0,
                -(float)
                (positionMob.Z /*-
                (slender_distancenbunite - slender_distanceapprocheunite * Ingame.compteur_slender) *
                (Math.Cos(angle))*/)
            );
            return resultat;
        }
    }
}
