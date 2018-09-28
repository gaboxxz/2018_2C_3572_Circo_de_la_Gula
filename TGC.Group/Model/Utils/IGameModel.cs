using TGC.Core.Camara;
using TGC.Core.Input;
using TGC.Core.Mathematica;
using TGC.Core.SceneLoader;
using TGC.Group.Camara;

namespace TGC.Group.Model.Utils
{
    public interface IGameModel
    {
        // I declare the accessible properties of the game model
        bool BoundingBox { get; set; }
        TgcD3dInput Input { get; set; }
        TgcCamera Camara { get; set; }
        TgcMesh Bandicoot { get; set; }
        float ElapsedTime { get; set; }
        TgcThirdPersonCamera BandicootCamera { get; set; }
        float DirectorAngle { get; set; }
        TGCVector3 BandicootMovement { get; set; }
        bool IsJumping { get; set; }
        int JumpDirection { get; set; }
    }
}
