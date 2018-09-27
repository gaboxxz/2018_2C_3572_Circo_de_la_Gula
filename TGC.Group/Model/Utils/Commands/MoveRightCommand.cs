using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveRightCommand : Command
    {
        private GameModelCanyon model;

        public MoveRightCommand(GameModelCanyon model)
        {
            this.model = model;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Right) || model.Input.keyDown(Key.D))
            {
                model.Bandicoot.RotateY(1 * model.ElapsedTime);
                model.banditcamara.rotateY(1 * model.ElapsedTime);
                model.anguloDirector += (1*model.ElapsedTime);
            }
        }
    }
}
