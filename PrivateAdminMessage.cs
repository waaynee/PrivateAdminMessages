using Oxide.Core.Libraries.Covalence;
using System.Collections.Generic;

namespace Oxide.Plugins
{
    [Info("Private Admin Message", "waayne", "0.1.1")]
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
        protected override void LoadDefaultMessages()
        {
            lang.RegisterMessages(new Dictionary<string, string>
            {
                ["AdminToPlayerMessageSent"] = "Admin to {0}: {1}\n(reply via /am <message>)",
                ["PlayerToAdminMessageSent"] = "{0} to Admin: {1}"
            }, this);
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

                IPlayer receiver = covalence.Players.FindPlayer(receiverName);
                receiver.Reply(string.Format(lang.GetMessage("AdminToPlayerMessageSent", this, player.Id), receiverName, message));
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

                string replyMessage = string.Format(lang.GetMessage("PlayerToAdminMessageSent", this, player.Id), player.Name, message);
                foreach (var connectedPlayer in covalence.Players.Connected)
                {
                    if (connectedPlayer.HasPermission("privateadminmessage.receive"))
                    {
                        connectedPlayer.Reply(replyMessage);
                    }
                }
                Puts(replyMessage);
            }
        }
    }
}