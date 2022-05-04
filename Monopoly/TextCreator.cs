using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly
{
    class TextCreator
    {
        IMessageService messageService;
        public TextCreator(IMessageService messageService)
        {
            this.messageService = messageService;
        }

        public void ShowMessage(string message)
        {
            messageService.ShowMessage(message);
        }

        public void ShowStepInfo(FieldCell fieldCell, Player player)
        {
            messageService.ShowMessage($"У игрока {player.Name} {player.Money} руб.\nИгрок {player.Name} находится на {fieldCell.Name}");
        }

        public void ShowStartState(LinkedList<Player> players)
        {
            messageService.ShowMessage($"Количество игроков: {players.Count}");

            foreach (var player in players)
            {
                messageService.ShowMessage($"У игрока {player.Name} {player.Money} руб. ");
            }

            messageService.ShowMessage("");
        }

        public void ShowPurchaseMessage(string message, Player player)
        {
            messageService.ShowMessage(message + $"Остаток на счёте {player.Money} руб.");
        }

        public void ShowPlayerProperty(Player player)
        {
            if (player.Property.Count == 0)
                return;

            Console.WriteLine($"Недвижимость игрока {player.Name}: ");
            foreach (var pair in player.Property)
            {
                messageService.ShowMessage(pair.Key.ToString() + ": ");
                foreach (var item in pair.Value)
                {
                    messageService.ShowMessage($"{item.Name} [{item.IsActive}], ");
                }
                messageService.ShowMessage("");
            }
            messageService.ShowMessage("");
        }
    }
}
