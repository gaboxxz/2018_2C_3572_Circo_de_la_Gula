using Microsoft.DirectX.DirectInput;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveUpCommand : Command
    {
        private IGameModel model;

        public MoveUpCommand(IGameModel ctx)
        {
            model = ctx;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Up) || model.Input.keyDown(Key.W))
            {
                TGCVector3 movement = new TGCVector3
                {
                    X = FastMath.Sin(model.DirectorAngle),
                    Y = 0,
                    Z = FastMath.Cos(model.DirectorAngle)
                };
                model.BandicootMovement = movement;

                model.BandicootCamera.OffsetForward -= 1 * model.ElapsedTime;
                model.BandicootCamera.Target = model.Bandicoot.Position;
            }
        }
    }
}
