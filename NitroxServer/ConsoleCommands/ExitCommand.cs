﻿using NitroxModel.DataStructures.Util;
using NitroxServer.ConsoleCommands.Abstract;
using NitroxModel.DataStructures.GameLogic;

namespace NitroxServer.ConsoleCommands
{
    internal class ExitCommand : Command
    {
        public ExitCommand() : base("stop", Perms.ADMIN, "Stops the server")
        {
            AddAlias("exit", "halt", "quit");
        }

        protected override void Execute(Optional<Player> sender)
        {
            Server.Instance.Stop();
        }
    }
}
