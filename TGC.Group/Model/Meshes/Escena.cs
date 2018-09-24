﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TGC.Core.SceneLoader;
using TGC.Group.Model.Meshes;

namespace TGC.Group.Model.Meshes
{

    class Escena
    {
        private TgcSceneLoader sceneLoader = new TgcSceneLoader();
        private List<Mesh> listaMeshes = new List<Mesh>();


        public List<Mesh> ObtenerMeshesDeEscena(String FilePath)
        {
            //listaMeshes.Clear();
            var meshes = sceneLoader.loadSceneFromFile(FilePath).Meshes;

            foreach(var mesh in meshes)
            {
                listaMeshes.Add(new Mesh(mesh));
            }

            return listaMeshes;
        }
        
        public void AgregarMeshesDeEscena(List<Mesh> lista, String FilePath)
        {
            //listaMeshes.Clear();
            var meshes = sceneLoader.loadSceneFromFile(FilePath).Meshes;

            foreach (var mesh in meshes)
            {
                /*if(mesh.Name.Equals("fruta3"))
                    lista.Add(new Mesh(mesh));
                else*/
                    lista.Add(new Mesh(mesh));
            }

            //return lista;
        }

        public void LimpiarEscena()
        {
            listaMeshes.Clear();
        }

    }
}