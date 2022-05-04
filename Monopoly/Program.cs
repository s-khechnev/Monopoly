using System;
using System.Collections.Generic;

namespace Monopoly
{
    class Program
    {

        static void Main(string[] args)
        {
            IMessageService messageService = new MessageService();

            TextCreator textCreator = new TextCreator(messageService);

            LinkedList<Player> players = new LinkedList<Player>();
            players.AddLast(new Player("Петя"));
            players.AddLast(new Player("Вася"));


            GameManager gameManager = new GameManager(textCreator, players);



        }
    }
}
