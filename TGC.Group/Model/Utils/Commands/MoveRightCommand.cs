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
                model.bandicootMovement.X = -1;
            }
        }
    }
}
