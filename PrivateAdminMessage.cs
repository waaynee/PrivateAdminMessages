using System;
using System.Collections.Generic;
using System.Linq;
using Oxide.Core;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("PrivateAdminMessage", "waayne", "0.1.0")]
    [Description("Allows admins to send private messages to players via console/chat")]
    class PrivateAdminMessage : CovalencePlugin
    {
        private void Init()
        {
            if (!permission.PermissionExists("privateadminmessage.use", this))
                permission.RegisterPermission("privateadminmessage.use", this);

            if (!permission.PermissionExists("privateadminmessage.receive", this))
                permission.RegisterPermission("privateadminmessage.receive", this);
        }

        [Command("pam"), Permission("privateadminmessage.use")]
        private void AdminToPlayerMessageCommand(IPlayer player, string command, string[] args)
        {
            if(args.Length > 1)
            {
                string receiverName = args[0];
                string message = args[1];
                for(int i = 2; i < args.Length; i++)
                {
                    message += " " + args[i];
                }

                IPlayer receiver = FindPlayer(receiverName);
                receiver.Reply($"Admin to {receiverName}: {message}");
                receiver.Reply("(reply via /am <message>)");
                Puts($"You to {receiverName}: {message}");
            }
        }

        [Command("am")]
        private void PlayerToAdminMessageCommand(IPlayer player, string command, string[] args)
        {
            if (args.Length > 0)
            {
                string message = args[0];
                for (int i = 1; i < args.Length; i++)
                {
                    message += " " + args[i];
                }

                foreach (var connectedPlayer in covalence.Players.Connected)
                {
                    if (connectedPlayer.HasPermission("privateadminmessage.receive"))
                    {
                        connectedPlayer.Reply($"{player.Name} to Admin: {message}");
                    }
                }

                Puts($"{player.Name} to Admin: {message}");
            }
        }

        #region Helper
        private IPlayer FindPlayer(string nameOrIdOrIp)
        {
            foreach (var activePlayer in covalence.Players.Connected)
            {
                if (activePlayer.Id == nameOrIdOrIp)
                    return activePlayer;
                if (activePlayer.Name.Contains(nameOrIdOrIp))
                    return activePlayer;
                if (activePlayer.Name.ToLower().Contains(nameOrIdOrIp.ToLower()))
                    return activePlayer;
                if (activePlayer.Address == nameOrIdOrIp)
                    return activePlayer;
            }

            return null;
        }
        #endregion
    }
}