using Microsoft.DirectX.DirectInput;
using TGC.Core.Example;
using TGC.Core.Input;

namespace TGC.Group.Model.Utils
{
    class ShowBoundingBoxCommand : Command
    {
        private IGameModel model;

        public ShowBoundingBoxCommand(IGameModel ctx)
        {
            model = ctx;
        }

        public void execute()
        {
            if (model.Input.keyPressed(Key.F))
            {
                model.BoundingBox = !model.BoundingBox;
            }
        }
    }
}
