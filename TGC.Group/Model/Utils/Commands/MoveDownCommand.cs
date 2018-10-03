using Microsoft.DirectX.DirectInput;
using TGC.Core.Mathematica;

namespace TGC.Group.Model.Utils.Commands
{
    class MoveDownCommand : Command
    {
        private IGameModel model;

        public MoveDownCommand(IGameModel ctx)
        {
            model = ctx;
        }

        public void execute()
        {
            if (model.Input.keyDown(Key.Down) || model.Input.keyDown(Key.S))
            {
                TGCVector3 movement = new TGCVector3
                {
                    X = (-1) * FastMath.Sin(model.DirectorAngle),
                    Y = 0,
                    Z = (-1) * FastMath.Cos(model.DirectorAngle)
                };
                model.BandicootMovement = movement;
                model.BandicootCamera.Target = model.Bandicoot.Position;
            }
        }
    }
}
