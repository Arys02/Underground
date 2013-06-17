﻿/*
 * ,// globalement ce qu'il faut dans le program.cs pour générer le laby
 *         Maze newmaze = new Maze(50, 100);
            newmaze.initecelles();
            newmaze.Generate(newmaze.maze[0,0]);
            newmaze.Setaffichage();
 * au niveau de la geule des salles : ["x"] = y,z avec y le type de salle, et z la modification sur les salles.
 *  x,1 = no rotation,
 *  x,2 = rotation de pi/2
 *  x,3 = rotation de pi
 *  x,4 = rotation de 3pi/2
 *     testaffiche[1, 1, 1, 1] = "┼";   = 1
        testaffiche[1, 0, 1, 1] = "┴";  = 2,1 
 *     testaffiche[1, 1, 0, 1] = "├";  = 2,2
        testaffiche[0, 1, 1, 1] = "┬";  = 2,3
 *     testaffiche[1, 1, 1, 0] = "┤";  = 2,4
        testaffiche[0, 1, 1, 0] = "┐"; = 3,1
        testaffiche[1, 0, 1, 0] = "┘"; = 3,2
 *     testaffiche[1, 0, 0, 1] = "└"; = 3,3
        testaffiche[0, 1, 0, 1] = "┌"; = 3,4
        testaffiche[1, 1, 0, 0] = "|"; =  4,1
        testaffiche[0, 0, 1, 1] = "─"; = 4,2
        testaffiche[0, 0, 0, 1] = "←"; = 5,1
 *     testaffiche[0, 1, 0, 0] = "↑"; = 5,2
        testaffiche[0, 0, 1, 0] = "→"; = 5,3
        testaffiche[1, 0, 0, 0] = "↓"; = 5,4
        testaffiche[0, 0, 0, 0] = "0"; = 0
 * 
 * 
 */
using System;
using System.Collections.Generic;
using System.Linq;


    public class Case
    {
        public List<Case> voisines ;// liste contenant les cases voisines 
        public List<Case> listrandom;
        public List<Case> see; 
        public bool isvisited = false;
        // de h  a  g ,  lien si a 1 , mure fermer si a 0.
        public int h = 0;   // lien en haut
        public int b = 0;
        public int d = 0;
        public int g = 0;
        public float exportaffiche;
        public string affiche;
        // position dans le tableau maze
        public int x;
        public int y;

        // constructeur de case.
        public Case()
        {
            voisines = new List<Case>();
            listrandom = new List<Case>();
            isvisited = false;
        }
        public void addvoisins(Case C)
        {
            voisines.Add(C);
        }
        public List<Case> randomizeNeighbors(ref Random random) // trie la liste dans un ordre aléatoire.
        {
            listrandom = voisines.OrderBy(emp => Guid.NewGuid()).ToList();
            return listrandom;
        }
  
    }

internal class Maze
{
    public Random random = new Random();
    public uint width = 0;
    public uint heigth = 0;
    public Case[,] maze;
    public string[,,,] testaffiche;
    public float[,] exportetab;
    // constructeur du labyrinth
    public Maze(uint width, uint heigth)
    {
        this.heigth = heigth;
        this.width = width;
        maze = new Case[this.width,this.heigth];
        exportetab = new float[this.width,this.heigth];
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < heigth; j++)
            {
                maze[i, j] = new Case();
                exportetab[i,j] = new float();
            }
        }
        testaffiche = new string[2,2,2,2];
        testaffiche[1, 1, 1, 1] = "┼";
        testaffiche[1, 1, 1, 0] = "┤";
        testaffiche[1, 1, 0, 1] = "├";
        testaffiche[1, 0, 1, 1] = "┴";
        testaffiche[0, 1, 1, 1] = "┬";
        testaffiche[0, 1, 1, 0] = "┐";
        testaffiche[1, 0, 1, 0] = "┘";
        testaffiche[0, 1, 0, 1] = "┌";
        testaffiche[1, 0, 0, 1] = "└";
        testaffiche[1, 1, 0, 0] = "|";
        testaffiche[0, 0, 1, 1] = "─";
        testaffiche[0, 0, 0, 1] = "←";
        testaffiche[0, 0, 1, 0] = "→";
        testaffiche[0, 1, 0, 0] = "↑";
        testaffiche[1, 0, 0, 0] = "↓";
        testaffiche[0, 0, 0, 0] = "0";


    }
    public void Export()
    {
        for (int j = 0; j < heigth; j++)
        {
            for (int i = 0; i < width; i++)
            {
                switch (maze[i,j].affiche)
                {
                    case "┼":
                        maze[i, j].exportaffiche = 1;
                        break;
                    case "┴":
                        maze[i, j].exportaffiche = 2.1f;
                        break;
                    case "├":
                        maze[i, j].exportaffiche = 2.2f;
                        break;
                    case "┬":
                        maze[i, j].exportaffiche = 2.3f;
                        break;
                    case "┤":
                        maze[i, j].exportaffiche =2.4f;
                        break;
                    case "┐":
                        maze[i, j].exportaffiche = 3.1f;
                        break;
                    case "┘":
                        maze[i, j].exportaffiche = 3.2f;
                        break;
                    case "└":
                        maze[i, j].exportaffiche = 3.3f;
                        break;
                    case "┌":
                        maze[i, j].exportaffiche = 3.4f;
                        break;
                    case "|":
                        maze[i, j].exportaffiche = 4.1f;
                        break;
                    case "─":
                        maze[i, j].exportaffiche = 4.2f;
                        break;
                    case "←":
                        maze[i, j].exportaffiche = 5.1f;
                        break;
                    case "↑":
                        maze[i, j].exportaffiche = 5.2f;
                        break;
                    case "→":
                        maze[i, j].exportaffiche = 5.3f;
                        break;
                    case "↓":
                        maze[i, j].exportaffiche = 5.4f;
                        break;
                    default:
                        maze[i, j].exportaffiche = 0f;
                        break;
                }
                exportetab[i, j] = maze[i, j].exportaffiche;
            }
        }
    }
    public void Initecelles()
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < heigth; j++)
            {
                maze[i, j].affiche = "0";
                maze[i, j].isvisited = false;
                maze[i, j].x = i;
                maze[i, j].y = j;

                if (i + 1 < width)
                    maze[i, j].addvoisins(maze[i + 1, j]);

                if (i - 1 >= 0)
                    maze[i, j].addvoisins(maze[i - 1, j]);

                if (j + 1 < heigth)
                    maze[i, j].addvoisins(maze[i, j + 1]);

                if (j - 1 >= 0)
                    maze[i, j].addvoisins(maze[i, j - 1]);

            }
        }
    }
    public void Generate(Case C)
    {
        C.isvisited = true;
        foreach (Case C2 in C.randomizeNeighbors(ref random))
        {
            if (!C2.isvisited)
            {
                if (C2.x > C.x)
                {
                    C.d = 1;
                    C2.g = 1;
                    Generate(C2);
                    continue;
                }
                if (C2.x < C.x)
                {
                    C.g = 1;
                    C2.d = 1;
                    Generate(C2);
                    continue;
                }
                if (C2.y > C.y)
                {
                    C.h = 1;
                    C2.b = 1;
                    Generate(C2);
                    continue;
                }
                if (C2.y < C.y)
                {
                    C.b = 1;
                    C2.h = 1;
                    Generate(C2);
                }

            }
        }
    }
    /* la liste des cases qui sont "vue" par une case.

    public List<Case> whichsee(int x, int y, string s, List<Case> liste, Case celles)
    {

        if (maze[x,y] !=null)
        {
            switch (s)
            {
                case "g":
                    if (celles.g == 0)
                        return liste;
                    liste.Add(maze[x, y]);
                    x++;

                    return whichsee(x, y, s, liste, maze[x, y]);

                case "d":
                    if (celles.d == 0)
                        return liste;
                    liste.Add(maze[x, y]);

                    x--;

                    return whichsee(x, y, s, liste, maze[x, y]);

                case "h":
                    if (celles.h == 0)
                        return liste;
                    y--;
                    liste.Add(maze[x, y]);
                    return whichsee(x, y, s, liste, maze[x, y]);

                case "b":
                    if (celles.b == 0)
                        return liste;
                    y++;
                    liste.Add(maze[x, y]);
                    return whichsee(x, y, s, liste, maze[x, y]);
            }
        }
        return null;
    }

    public void cellessee(int x, int y)
    {
        if (maze[x,y].d == 1)
        {
            maze[x,y].see.Concat(whichsee(x, y,"d", maze[x, y].see, maze[x, y]));
        }
        
        if (maze[x,y].h == 1)
        {
            maze[x,y].see.Concat(whichsee(x, y,"h", maze[x, y].see, maze[x, y]));
        }
        if (maze[x,y].b == 1)
        {
            maze[x,y].see.Concat(whichsee(x, y,"b", maze[x, y].see, maze[x, y]));
        }
        if (maze[x,y].g == 1)
        {
            maze[x,y].see.Concat(whichsee(x, y,"g", maze[x, y].see, maze[x, y]));
        }
    }*/

    public void Setaffichage()
    {
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                maze[i, j].affiche = testaffiche[maze[i, j].g, maze[i, j].d, maze[i, j].b, maze[i, j].h];
                //cellessee(i,j);

                Console.Write(maze[i, j].affiche/* +" | "*/);
                
            }
            Console.WriteLine();
        }
        for (int i = 0; i < maze.GetLength(0); i++)
        {
            for (int j = 0; j < maze.GetLength(1); j++)
            {
                maze[i, j].affiche = testaffiche[maze[i, j].g, maze[i, j].d, maze[i, j].b, maze[i, j].h];
                //cellessee(i,j);
                Export();
                Console.Write(exportetab[i,j] + "|");

            }
            Console.WriteLine();
        }
        /*
        foreach (var celles in maze[3,3].see)
                {
                    Console.WriteLine("[ "+celles.x+","+celles.y+" ]");
                }*/
    }
   
}
