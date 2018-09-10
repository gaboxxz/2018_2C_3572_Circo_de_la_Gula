using System.Collections.Generic;
using Microsoft.DirectX.DirectInput;

using TGC.Group.Model.Utils.Commands;

namespace TGC.Group.Model.Utils
{
    /* Esta clase se encarga de manejar las entradas de usuario y ejecutar el comando correspondiente.
    *  Utilizamos el Patron de diseño Command.
    */
    class InputHandler
    {
        private Dictionary<Key, Command> commands = new Dictionary<Key, Command>();

        // Utiliza el modelo y bindea un comando a un evento.
        public InputHandler(GameModel model) {
            commands[Key.F] = new ShowBoundingBoxCommand(model);
            commands[Key.Left] = new MoveLeftCommand(model);
            commands[Key.Right] = new MoveRightCommand(model);
            commands[Key.Up] = new MoveUpCommand(model);
            commands[Key.Down] = new MoveDownCommand(model);
            commands[Key.Space] = new JumpCommand(model);
        }

        public void HandleInput(Key key) {
            Command command = commands[key];

            if (command != null) {
                command.execute();
            }
        }
    }
}
