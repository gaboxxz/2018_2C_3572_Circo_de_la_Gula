using Microsoft.DirectX.DirectInput;

namespace TGC.Group.Model.Utils
{
    class ShowBoundingBoxCommand : Command
    {
        private GameModelCanyon model;

        public ShowBoundingBoxCommand(GameModelCanyon model)
        {
            this.model = model;
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
