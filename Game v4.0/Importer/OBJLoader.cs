using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.IO;
using SharpDX;
using SharpDX.Direct3D9;
using SharpDX.XAudio2;
using SharpDX.Windows;

namespace LOL_l.Importer
{
    public struct structVertex
    {
        public Vector4 CoordVertex;
        public Vector2 CoordTex;
        public Vector4 Couleur;
        public Vector4 Normale;
        public Vector4 Tangent;

        public structVertex(Vector4 CoordVertex, Vector2 CoordTex, Vector4 Normale, Color Couleur, Vector4 Tangent)
        {
            this.CoordTex = CoordTex;
            this.CoordVertex = CoordVertex;
            this.Normale = Normale;
            this.Couleur = Couleur.ToVector4();
            this.Tangent = Tangent;
        }
    }
    enum ObjectState
    {
        Not_prepared = 1,
        Ready = 2,
        Disposed = 3
    }
    class SubObject
    {
        private DataStream DataStream;
        private Effect Effect;
        private VertexDeclaration vertexElems3D;
        private EffectHandle Technique, Pass;
        private Debug Debug = new Debug();
        private ObjectState State = ObjectState.Not_prepared;

        public VertexBuffer VertexBuffer;
        public Matrix SubTransformation;
        public structNewMTL materialSet;
        public structVertex[] Vertices;

        public bool sera_affiche;

        public SubObject(Matrix SubTransformation, structNewMTL materialSet, structVertex[] Vertices)
        {
            this.materialSet = materialSet;
            this.SubTransformation = SubTransformation;
            this.sera_affiche = true;
            this.Vertices = Vertices;
        }
        public void PrepareSubObject(ref Device device)
        {
            if (State != ObjectState.Disposed)
            {
                if (State != ObjectState.Ready)
                {
                    this.Effect = ResManager.Basic_Effect.Clone(device);
                    this.Technique = this.Effect.GetTechnique(0);
                    this.Pass = this.Effect.GetPass(this.Technique, 0);
                    VertexBuffer = new VertexBuffer(device, Utilities.SizeOf<structVertex>() * Vertices.Length, Usage.WriteOnly, VertexFormat.None, Pool.Managed);
                    DataStream = VertexBuffer.Lock(0, 0, LockFlags.None);
                    DataStream.WriteRange(Vertices);
                    VertexBuffer.Unlock();
                    this.vertexElems3D = new VertexDeclaration(device, new VertexElement[] {
        		        new VertexElement(0, // POSITION
                            0,
                            DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Position, 0),
				        new VertexElement(0, // TEXCOORD0
                            Convert.ToInt16(Utilities.SizeOf<Vector3>()),
                            DeclarationType.Float2, DeclarationMethod.Default,DeclarationUsage.TextureCoordinate,0),
                        new VertexElement(0, // COLOR0
                            Convert.ToInt16(Utilities.SizeOf<Vector3>()+Utilities.SizeOf<Vector2>()),
                            DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Color, 0),
                        new VertexElement(0, // NORMAL0
                            Convert.ToInt16(Utilities.SizeOf<Vector3>()+Utilities.SizeOf<Vector2>()+Utilities.SizeOf<Vector4>()),
                            DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Normal, 0),
                        new VertexElement(0, // TANGENT
                            Convert.ToInt16(Utilities.SizeOf<Vector3>()+Utilities.SizeOf<Vector2>()+Utilities.SizeOf<Vector4>()+Utilities.SizeOf<Vector3>()),
                            DeclarationType.Float4, DeclarationMethod.Default, DeclarationUsage.Tangent, 0),
                        VertexElement.VertexDeclarationEnd,
        	        });
                    State = ObjectState.Ready;
                }
                else
                {
                    Debug.WriteNicely("!", ConsoleColor.DarkRed, "ATTENTION DUPLICATION DU MODEL", 0);
                }
            }
            else
            {
                Debug.WriteNicely("!", ConsoleColor.DarkRed, "ATTENTION, VOUS TENTEZ D'AFFICHER UN MESH N'EXISTANT PLUS", 0);
            }
        }
        public void FreeSubObject()
        {
            if (State == ObjectState.Ready)
            {
                DataStream.Dispose();
                VertexBuffer.Dispose();
                vertexElems3D.Dispose();
                Effect.Dispose();
                Pass.Dispose();
                Technique.Dispose();
                State = ObjectState.Disposed;
            }
            else
            {
                Debug.WriteNicely("!", ConsoleColor.DarkRed, "ATTENTION LIBERATION INSENSEE", 0);
            }
        }
        public void Draw(ref Device device, Matrix Transformation, Matrix View, Matrix Proj)
        {
            if (State != ObjectState.Not_prepared)
            {
                if (State != ObjectState.Disposed)
                {
                    this.Effect.Technique = Technique;
                    this.Effect.Begin();
                    this.Effect.BeginPass(0);
                    //this.Effect.SetValue("worldViewProj", worldViewProj);
                    this.Effect.SetValue("worldViewProj", SubTransformation * Transformation * View * Proj);
                    device.SetStreamSource(0, VertexBuffer, 0, Utilities.SizeOf<structVertex>());
                    device.VertexDeclaration = this.vertexElems3D;
                    device.SetTexture(0, ResManager.ListTextures[ResManager.getTexture(materialSet.map_Kd, ref device, false)].Texture);
                    device.SetTexture(1, ResManager.ListTextures[ResManager.getTexture(materialSet.map_Ns, ref device, true)].Texture);
                    device.DrawPrimitives(PrimitiveType.TriangleList, 0, Vertices.Length / 3);
                    this.Effect.EndPass();
                    this.Effect.End();
                }
                else
                {
                    Debug.WriteNicely("!", ConsoleColor.DarkRed, "ATTENTION, VOUS TENTEZ D'AFFICHER UN MESH N'EXISTANT PLUS", 0);
                }
            }
            else
            {
                Debug.WriteNicely("!", ConsoleColor.DarkRed, "ATTENTION, VOUS TENTEZ D'AFFICHER UN MESH PAS PREPARE", 0);
            }
        }

    }
    class OBJLoader
    {
        public Matrix Transformation;
        private List<SubObject> ListSubObject = new List<SubObject>();
        public OBJLoader(string path, Matrix Transformation)
        {
            this.Transformation = Transformation;
            MTLLoader currentMaterial = new MTLLoader();
            structNewMTL currentMaterialSet = currentMaterial.getMaterialSet("");
            byte[] FileOBJ = ResManager.ListBinaries[ResManager.getBinary(path)].Data;
            int i = 0;
            float x = 1, y = 1, z = 1;
            string type, chaine_Lu;
            ToolBox ToolBox = new ToolBox();
            Debug Debug = new Debug();
            List<Vector2> ListeCoordTextures = new List<Vector2>();
            List<Vector4> ListeCoordSommets = new List<Vector4>();
            List<Vector4> ListeNormales = new List<Vector4>();
            List<structVertex> ListeVertex = new List<structVertex>();
            while (i < FileOBJ.Length)
            {
                type = ToolBox.getstring(ref i, ref FileOBJ);
                #region Vertex (OK)
                if (type == "v")
                {
                    i++; // Espace
                    x = ToolBox.getfloat(ref i, ref FileOBJ);
                    i++; // Espace
                    y = ToolBox.getfloat(ref i, ref FileOBJ);
                    i++; // Espace
                    z = ToolBox.getfloat(ref i, ref FileOBJ);

                    ListeCoordSommets.Add(new Vector4(x, y, z, 1));
                    Debug.WriteNicely("#", ConsoleColor.Gray, "Nouveau sommet " + x + " " + y + " " + z, 4);
                }
                #endregion
                #region Texture coordinate (OK)
                else if (type == "vt")
                {
                    i++; // Espace
                    x = ToolBox.getfloat(ref i, ref FileOBJ);
                    i++; // Espace
                    y = ToolBox.getfloat(ref i, ref FileOBJ);

                    ListeCoordTextures.Add(new Vector2(x, y));
                    Debug.WriteNicely("#", ConsoleColor.Gray, "Nouvelle coordonnée de texture " + x + " " + y + " ", 4);
                }
                #endregion
                #region Normale (OK)
                else if (type == "vn")
                {
                    i++; // Espace
                    x = ToolBox.getfloat(ref i, ref FileOBJ);
                    i++; // Espace
                    y = ToolBox.getfloat(ref i, ref FileOBJ);
                    i++; // Espace
                    z = ToolBox.getfloat(ref i, ref FileOBJ);

                    ListeNormales.Add(new Vector4(x, y, z, 1));
                    Debug.WriteNicely("#", ConsoleColor.Gray, "Nouvelle normale " + x + " " + y + " " + z, 4);
                }
                #endregion
                #region Face (OK)
                else if (type == "f")
                {
                    const int nbVerticesPerFace = 3;
                    structVertex[] Vertices3 = new structVertex[3];
                    for (int j = 0; j < nbVerticesPerFace; j++)
                    {
                        i++; // Espace
                        x = ToolBox.getfloat(ref i, ref FileOBJ); // Coord x
                        i++; // slash
                        y = ToolBox.getfloat(ref i, ref FileOBJ); // Coord y
                        i++; // slash
                        z = ToolBox.getfloat(ref i, ref FileOBJ); // Coord z
                        Vertices3[j] = new structVertex(
                            ListeCoordSommets[Convert.ToInt32(x) - 1],
                            ListeCoordTextures[Convert.ToInt32(y) - 1],
                            ListeNormales[Convert.ToInt32(z) - 1],
                            currentMaterialSet.Kd,
                            new Vector4()
                        );
                    }
                    Vector4 Tangent = ToolBox.CalcTangentVector(
                        new Vector3[] { ToolBox.Vec4to3(Vertices3[0].CoordVertex), ToolBox.Vec4to3(Vertices3[1].CoordVertex), ToolBox.Vec4to3(Vertices3[2].CoordVertex) },
                        new Vector2[] { Vertices3[0].CoordTex, Vertices3[1].CoordTex, Vertices3[2].CoordTex },
                        ToolBox.Vec4to3(Vertices3[0].Normale));
                    for (int j = 0; j < nbVerticesPerFace; j++)
                    {
                        Vertices3[j].Tangent = Tangent;
                        ListeVertex.Add(Vertices3[j]);
                    }
                    Debug.WriteNicely("#", ConsoleColor.DarkBlue, "Nouvelle face", 3);
                }
                #endregion
                #region Object (OK)
                else if (type == "o")
                {
                    i++;
                    chaine_Lu = ToolBox.getstring(ref i, ref FileOBJ);
                    Debug.WriteNicely("#", ConsoleColor.Green, "Nouvel objet : " + chaine_Lu, 3);
                    if (ListeVertex.Count != 0)
                    {
                        ListSubObject.Add(new SubObject(Matrix.Identity, currentMaterialSet, ListeVertex.ToArray()));
                        ListeVertex.Clear();
                    }
                }
                #endregion
                #region Group (OK -- Non implémenté)
                else if (type == "g")
                {
                    i++;
                    chaine_Lu = ToolBox.getstring(ref i, ref FileOBJ);
                    Debug.WriteNicely("#", ConsoleColor.Green, "Nouveau groupe : " + chaine_Lu, 3);
                }
                #endregion
                #region MTLLib (OK)
                else if (type == "mtllib")
                {
                    i++;
                    chaine_Lu = ToolBox.getstring(ref i, ref FileOBJ);
                    Debug.WriteNicely("#", ConsoleColor.Green, "Nouveau fichier .MTL : " + chaine_Lu, 2);
                    currentMaterial = new MTLLoader(chaine_Lu);
                    currentMaterialSet = currentMaterial.getMaterialSet("");
                }
                #endregion
                #region UseMTL (OK)
                else if (type == "usemtl")
                {
                    if (ListeVertex.Count != 0)
                    {
                        ListSubObject.Add(new SubObject(Matrix.Identity, currentMaterialSet, ListeVertex.ToArray()));
                        ListeVertex.Clear();
                    }
                    i++;
                    chaine_Lu = ToolBox.getstring(ref i, ref FileOBJ);
                    Debug.WriteNicely("#", ConsoleColor.Green, "Nouveau Set : " + chaine_Lu, 2);
                    currentMaterialSet = currentMaterial.getMaterialSet(chaine_Lu);
                }
                #endregion
                #region Lissage (OK -- Non implémenté)
                else if (type == "s")
                {
                }
                #endregion
                #region New line (OK)
                else if (type == "") { }
                #endregion
                #region Commentaire (OK)
                else if (type[0] == '#')
                {
                    Debug.WriteNicely("#", ConsoleColor.Green, "Commentaire", 3);
                }
                #endregion
                #region Unknown (OK)
                else
                {
                    Debug.WriteNicely("!", ConsoleColor.Red, "Type inconnu", 3);
                }
                #endregion
                ToolBox.gotonextline(ref i, ref FileOBJ);
            }
            if (ListeVertex.Count != 0)
            {
                ListSubObject.Add(new SubObject(Matrix.Identity, currentMaterialSet, ListeVertex.ToArray()));
                ListeVertex.Clear();
            }
        }
        public void Prepare(ref Device device)
        {
            foreach (SubObject SubObject in ListSubObject)
            {
                SubObject.PrepareSubObject(ref device);
            }
        }
        public void Free()
        {
            foreach (SubObject SubObject in ListSubObject)
            {
                SubObject.FreeSubObject();
            }
            ListSubObject.Clear();
        }
        public void Disable()
        {
            foreach (SubObject SubObject in ListSubObject)
            {
                SubObject.sera_affiche = false;
            }
        }
        public void Draw(ref Device device, Matrix View, Matrix Proj)
        {
            foreach (SubObject SubObject in ListSubObject)
            {
                SubObject.Draw(ref device, Transformation, View, Proj);
            }
        }
    }
}
