using System;
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
            Console.WriteLine(meshes.Count);
            Console.WriteLine(listaMeshes.Count);
            return listaMeshes;
        }
        
        public void AgregarMeshesDeEscena(List<Mesh> lista, String FilePath)
        {
            var meshes = sceneLoader.loadSceneFromFile(FilePath).Meshes;

            foreach (var mesh in meshes)
            {
                    lista.Add(new Mesh(mesh));
            }
            
        }

        

        public void LimpiarEscena()
        {
            listaMeshes.Clear();
        }

    }
}
