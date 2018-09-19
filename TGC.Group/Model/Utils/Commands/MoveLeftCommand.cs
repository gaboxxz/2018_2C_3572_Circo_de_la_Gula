using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveLeftCommand : Command
    {
        private GameModelCanyon model;

        public MoveLeftCommand(GameModelCanyon model)
        {
            this.model = model;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Left) || model.Input.keyDown(Key.A))
            {
                model.bandicootMovement.X = 1;
            }
        }

    }
}
