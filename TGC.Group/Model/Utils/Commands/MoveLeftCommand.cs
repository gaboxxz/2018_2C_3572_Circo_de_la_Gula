using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveLeftCommand : Command
    {
        private GameModel model;

        public MoveLeftCommand(GameModel model)
        {
            this.model = model;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Left))
            {
                model.bandicootMovement.X = 1;
            }
        }

    }
}
