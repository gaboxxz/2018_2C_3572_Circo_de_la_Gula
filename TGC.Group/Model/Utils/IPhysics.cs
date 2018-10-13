using BulletSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TGC.Group.Model.Utils
{
    public interface IPhysics
    {
        RigidBody BandicootRigidBody { get; set; }

        void Init(IGameModel ctx);
        void Update();
        void Render();
        void Dispose();
    }
}
