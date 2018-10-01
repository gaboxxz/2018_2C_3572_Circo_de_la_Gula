using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveLeftCommand : Command
    {
        private IGameModel model;

        public MoveLeftCommand(IGameModel ctx)
        {
            model = ctx;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Left) || model.Input.keyDown(Key.A))
            {
                model.BandicootCamera.rotateY(-1 * model.ElapsedTime);
                model.DirectorAngle -= (1*model.ElapsedTime); 
                model.Rotation = TGC.Core.Mathematica.TGCMatrix.RotationY(model.DirectorAngle);
            }
        }

    }
}
