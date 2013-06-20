using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Underground
{
    class Animation
    {
        //Cette Liste stocke toutes les objets composants une animation. 
        List<List<Obj>> Anim;

        //La on charge notre liste avec les differents Objet
        public Animation(List<string> Pathlist)
        {
            for (int i = 0; i <= Pathlist.Count; i++ )
            {
                Anim.Add(Obj.Obj_parser(Pathlist[i]));
            }
        }
    }
}
