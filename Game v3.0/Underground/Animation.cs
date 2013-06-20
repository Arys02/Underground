using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Test2
{
    class Animation
    {
        //Cette Liste stocke toutes les objets composants une animation. 
        List<Object> Anim;

        //La on charge notre liste avec les differents Objet
        public Animation(List<string> Pathlist)
        {
            foreach(string path in Pathlist)
            {
                Object Alphatest = new Object(path);
                Anim.Add(Alphatest);
            }
        }        
        

    }
}
