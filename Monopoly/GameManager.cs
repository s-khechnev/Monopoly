using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly
{
    class GameManager
    {
        const decimal moneyPerCircle = 200;

        public TextCreator TextCreator { get; private set; }

        LinkedList<Player> players;
        FieldCell[] gameField;

        public GameManager(TextCreator textCreator, LinkedList<Player> players)
        {
            TextCreator = textCreator;
            this.players = players;
            InitGameField();
            Play();
        }

        private void InitGameField()
        {
            gameField = new FieldCell[] {
                new StartCell(this, "Старт"),
                new Street(this, "Житная ул.", null, 60, 2, PropertyFamily.Brown,true,50),
                new Street(this, "Нагатинская ул.", null, 60, 4, PropertyFamily.Brown,true,50),
                new TaxCell(this, "Подоходный налог", 400),
                new Street(this, "Варшавское шоссе", null, 100, 6, PropertyFamily.Cyan, true,50),
                new Street(this, "ул. Огарева", null, 100, 6, PropertyFamily.Cyan, true,50),
                new Street(this, "Первая парковая ул.", null, 120, 8, PropertyFamily.Cyan,true, 50),
                new Street(this, "Полянка ул.", null, 140, 10, PropertyFamily.Pink, true,100),
                new Street(this, "ул. Сретенка", null, 140, 10, PropertyFamily.Pink, true,100),
                new Street(this, "Ростовская наб.", null, 160, 12, PropertyFamily.Pink, true,100),
                new Street(this, "Рязанский проспект", null, 180, 14, PropertyFamily.Orange, true,100),
                new Street(this, "ул. Вавилова", null, 180, 14, PropertyFamily.Orange, true,100),
                new Street(this, "Рублевское шоссе", null, 200, 16, PropertyFamily.Orange, true,100),
                new Street(this, "ул. Тверская",  null, 220, 18, PropertyFamily.Red, true,150),
                new Street(this, "Большая Дмитровка ул.", null, 220, 18, PropertyFamily.Red,true, 150),
                new Street(this, "Площадь маяковского",null, 240, 20, PropertyFamily.Red, true,150),
                new Street(this, "ул. Малая Бронная", null, 350, 35, PropertyFamily.Blue,true, 200),
                new TaxCell(this, "Сверхналог", 500),
                new Street(this, "ул. Арбат", null, 400, 50, PropertyFamily.Blue, true,200),
                new Street(this, "ул. Щусева",  null, 300, 26, PropertyFamily.Green,true, 200),
                new Street(this, "Гоголевский бульвар",  null, 300, 26, PropertyFamily.Green,true, 200),
                new Street(this, "Кутузовский проспект",  null, 300, 28, PropertyFamily.Green,true, 200),
            };
        }

        private void Play()
        {
            TextCreator.ShowStartState(players);
            Console.ReadKey();
            Console.Clear();

            while (players.Count > 1)
            {
                for (var playerIter = players.First; playerIter != null; playerIter = playerIter.Next)
                {
                    var player = playerIter.Value;

                    DoStep(player);
                    OfferToBuildHouse(player);
                    TryActivateProperty(player);
                    TextCreator.ShowPlayerProperty(player);
                    Console.WriteLine();

                    if (player.IsLoser)
                    {
                        player.ActivateLoserProperty();
                        TextCreator.ShowMessage($"========== {player.Name} выбывает из игры ==========");
                        players.Remove(player);
                    }
                }
                //Console.ReadLine();
                Console.WriteLine("\n");
            }
            TextCreator.ShowMessage($"========== {players.First.Value.Name} выиграл ==========");
        }

        private void TryActivateProperty(Player player)
        {
            foreach (var key in player.Property.Keys)
            {
                foreach (var item in player.Property[key])
                {
                    if (!item.IsActive && player.Money > item.AmountDeposit)
                        ActivateProperty(player, item);
                }
            }
        }

        private void ActivateProperty(Player player, BuyableCell item)
        {
            player.WriteOffMoney(item.AmountDeposit);
            item.IsActive = true;
            TextCreator.ShowPurchaseMessage($"{player.Name} внёс залог за {item.Name} в размере {item.AmountDeposit} руб. ", player);
        }

        private void OfferToBuildHouse(Player player)
        {
            if (player.Property.Count == 0)
                return;

            foreach (var propFam in player.Property.Keys)
            {
                if (propFam == PropertyFamily.RailwayStation || propFam == PropertyFamily.PublicUtility)
                    continue;

                int countPropFam = 0;
                foreach (var item in gameField)
                {
                    if (item is Street && (item as Street).Family == propFam)
                        countPropFam++;
                }

                if (countPropFam == player.Property[propFam].Count)
                    TryBuildHouse(player, propFam);
            }
        }

        private void TryBuildHouse(Player player, PropertyFamily propFam)
        {
            if (player.Money < (player.Property[propFam].First.Value as Street).CostHouse)
                return;
            else
                FindSuitableStreet(player, propFam);
        }

        private void FindSuitableStreet(Player player, PropertyFamily propFam)
        {
            Street minStreet = player.Property[propFam].First.Value as Street;

            foreach (var item in player.Property[propFam])
            {
                if (item.CompareTo(minStreet) <= 0)
                    minStreet = item as Street;
            }

            BuildHouse(player, minStreet);
        }

        private void BuildHouse(Player player, Street minStreet)
        {
            var nextHouse = minStreet.GetNextHouseLevel();

            if (nextHouse != null)
            {
                for (int i = 0; i < gameField.Length; i++)
                {
                    if (gameField[i] == minStreet)
                    {
                        gameField[i] = nextHouse;
                        break;
                    }
                }
                player.Property[minStreet.Family].Find(minStreet).Value = nextHouse;


                player.WriteOffMoney(minStreet.CostHouse);
                TextCreator.ShowPurchaseMessage($"{player.Name} построил дом на {minStreet.Name} за {minStreet.CostHouse} руб. ", player);
            }
        }

        public void OfferToBuyProperty(Player player, BuyableCell buyableCell)
        {
            if (player.Money >= buyableCell.Cost)
            {
                BuyProperty(player, buyableCell);
            }
        }

        private void BuyProperty(Player player, BuyableCell buyableCell)
        {
            player.AddProperty(buyableCell);
            buyableCell.Owner = player;
            player.WriteOffMoney(buyableCell.Cost);
            TextCreator.ShowPurchaseMessage($"{player.Name} купил {buyableCell.Name} за {buyableCell.Cost} руб. ", player);
        }

        private void DoStep(Player player)
        {
            int countStep = new Random().Next(1, 7) + new Random().Next(1, 7);

            TextCreator.ShowMessage($"Кубики: {countStep}");

            player.ChangePosition(player.Position + countStep);

            if (player.Position >= gameField.Length)
            {
                player.AddMoney(moneyPerCircle);
                player.ChangePosition(player.Position % gameField.Length);
                TextCreator.ShowMessage($"{player.Name} прошел Старт! +200 руб.");
            }

            gameField[player.Position].OnPlayerStepped(player);
        }

        public bool WriteOffMoney(Player player, decimal amount)
        {
            if (player.Money >= amount || TryGetMoney(player, amount))
            {
                player.WriteOffMoney(amount);
                return true;
            }
            else
            {
                Lose(player);
                return false;
            }
        }

        private void Lose(Player player)
        {
            player.SetIsLoser();
        }

        private bool TryGetMoney(Player player, decimal amount)
        {
            foreach (var key in player.Property.Keys)
            {
                if (TrySellHouses(player, key, amount))
                    return true;
            }

            foreach (var key in player.Property.Keys)
            {
                foreach (var item in player.Property[key])
                {
                    if (TryMortgage(player, item as Street, amount))
                        return true;
                }
            }

            return false;
        }

        private bool TrySellHouses(Player player, PropertyFamily key, decimal amount)
        {
            bool flag = true;
            while (flag)
            {
                foreach (var item in player.Property[key].OrderBy(value => value.RentalPrice))
                {
                    var street = item as Street;

                    var prevHouse = street.GetPreviousHouseLevel();

                    if (prevHouse == null)
                    {
                        flag = false;
                        continue;
                    }

                    for (int i = 0; i < gameField.Length; i++)
                    {
                        if (gameField[i] == street)
                        {
                            gameField[i] = prevHouse;
                            break;
                        }
                    }

                    player.Property[street.Family].Find(street).Value = prevHouse;
                    ///

                    player.AddMoney(street.CostHouse);
                    TextCreator.ShowPurchaseMessage($"{player.Name} продал дом на {street.Name} за {street.CostHouse} руб. ", player);

                    if (player.Money > amount)
                        flag = false;
                }
            }
            return player.Money > amount;
        }

        private bool TryMortgage(Player player, Street street, decimal amount)
        {
            if (!street.IsActive)
                return false;

            street.IsActive = false;
            player.AddMoney(street.AmountDeposit);
            TextCreator.ShowPurchaseMessage($"{player.Name} заложил {street.Name} за {street.AmountDeposit} руб. ", player);

            return player.Money > amount;
        }
    }
}
