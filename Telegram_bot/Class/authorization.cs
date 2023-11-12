using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL;
using WTelegram;
namespace Telegram_bot.Class
{
    static class authorization
    {
        public static async Task Main()
        {
            using var client = new WTelegram.Client();
            var myself = await client.LoginUserIfNeeded();
            Console.WriteLine($"We are logged-in as {myself} (id {myself.id})");

            var resolved = await client.Contacts_ResolveUsername("Hakaton_telegram_bot"); // username without the @
            await client.SendMessageAsync(resolved, "Привет друзья");

            var dialogs = await client.Messages_GetAllDialogs();
            foreach (Dialog dialog in dialogs.dialogs)
            {
                switch (dialogs.UserOrChat(dialog))
                {
                    case User user when user.IsActive: Console.WriteLine("User " + user); break;
                    case ChatBase chat when chat.IsActive: Console.WriteLine(chat); break;
                }
            }

            var chats = await client.Messages_GetAllDialogs();
            foreach (var (id, chat) in chats.users)
                Console.WriteLine($"{id} : {chat}");
            Console.Write("Choose a chat ID to send a message to: ");
            long chatId = long.Parse(Console.ReadLine());
            await client.SendMessageAsync(chats.users[chatId], "я живой");

            InputPeer peer = chats.users[1130467807]; // the chat (or User) we want
            //await client.GetMessages(peer);
            for (int offset_id = 0; ;)
            {
                var messages = await client.Messages_GetHistory(peer, offset_id);
                if (messages.Messages.Length == 0) break;
                foreach (var msgBase in messages.Messages)
                {
                    var from = messages.UserOrChat(msgBase.From ?? msgBase.Peer); // from can be User/Chat/Channel
                    if (msgBase is Message msg)
                        Console.WriteLine($"{from}> {msg.message} {msg.media}");
                    else if (msgBase is MessageService ms)
                        Console.WriteLine($"{from} [{ms.action.GetType().Name[13..]}]");
                }
                offset_id = messages.Messages[^1].ID;
            }
        }
    }
}
