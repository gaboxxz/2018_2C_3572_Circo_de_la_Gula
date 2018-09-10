using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveRightCommand : Command
    {
        private GameModel model;

        public MoveRightCommand(GameModel model)
        {
            this.model = model;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Right))
            {
                model.bandicootMovement.X = -1;
            }
        }
    }
}
