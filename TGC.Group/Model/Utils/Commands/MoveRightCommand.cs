using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveRightCommand : Command
    {
        private IGameModel model;

        public MoveRightCommand(IGameModel ctx)
        {
            model = ctx;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Right) || model.Input.keyDown(Key.D))
            {
                model.Bandicoot.RotateY(1 * model.ElapsedTime);
                model.BandicootCamera.rotateY(1 * model.ElapsedTime);
                model.DirectorAngle += (1*model.ElapsedTime);
            }
        }
    }
}
