using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly
{
    enum PropertyFamily
    {
        Brown,
        Cyan,
        Pink,
        Orange,
        Red,
        Yellow,
        Green,
        Blue,
        RailwayStation,
        PublicUtility,
    }

    abstract class FieldCell
    {
        public string Name { get; private set; }
        public GameManager gameManager { get; private set; }

        public FieldCell(GameManager gameManager, string name)
        {
            Name = name;
            this.gameManager = gameManager;
        }

        public virtual void OnPlayerStepped(Player player)
        {
            gameManager.TextCreator.ShowStepInfo(this, player);
            if ((this is BuyableCell) && (this as BuyableCell).Owner != null)
                gameManager.TextCreator.ShowMessage($"Владелец недвижимости - {(this as BuyableCell).Owner.Name}");
        }
    }

    class StartCell : FieldCell
    {
        public StartCell(GameManager gameManager, string name) : base(gameManager, name)
        {
        }

        public override void OnPlayerStepped(Player player)
        {
            base.OnPlayerStepped(player);
        }
    }

    /*    class PrisonCell : FieldCell
        {
            public PrisonCell(GameManager gameManager, string name) : base(gameManager, name)
            {
            }

            public override void OnPlayerStepped(Player player)
            {
                base.OnPlayerStepped(player);
            }
        }

        class GoToPrisonCell : FieldCell
        {
            public GoToPrisonCell(GameManager gameManager, string name) : base(gameManager, name)
            {
            }

            public override void OnPlayerStepped(Player player)
            {
                base.OnPlayerStepped(player);
                gameManager.MessageService.ShowMessage($"{player.Name} отправляется в тюрьму");
            }
        }
        class ParkingCell : FieldCell
        {
            public ParkingCell(GameManager gameManager, string name) : base(gameManager, name) { }

            public override void OnPlayerStepped(Player player)
            {
                base.OnPlayerStepped(player);
            }
        }*/

    class TaxCell : FieldCell
    {
        private decimal AmountTax { get; set; }

        public TaxCell(GameManager gameManager, string name, decimal amountTax) : base(gameManager, name)
        {
            AmountTax = amountTax;
        }

        public override void OnPlayerStepped(Player player)
        {
            base.OnPlayerStepped(player);

            if (gameManager.WriteOffMoney(player, AmountTax))
                gameManager.TextCreator.ShowPurchaseMessage($"У {player.Name} списали {AmountTax} руб. ", player);
            else
                gameManager.TextCreator.ShowMessage($"{player.Name} не смог заплатить {AmountTax} руб.");
        }
    }

    abstract class BuyableCell : FieldCell, IComparable
    {
        public Player Owner { get; set; }
        public decimal Cost { get; private set; }

        public virtual decimal RentalPrice { get; protected set; }

        public decimal AmountDeposit { get; private set; }

        public PropertyFamily Family { get; private set; }

        public bool IsActive { get; set; }

        public BuyableCell(GameManager gameManager, string name, Player owner, decimal cost, decimal rentalPrice, PropertyFamily family, bool isActive) : base(gameManager, name)
        {
            Owner = owner;
            Cost = cost;
            RentalPrice = rentalPrice;
            AmountDeposit = Cost / 2;
            Family = family;
            IsActive = isActive;
        }

        public override void OnPlayerStepped(Player player)
        {
            base.OnPlayerStepped(player);
            if (Owner == null)
            {
                gameManager.OfferToBuyProperty(player, this);
            }
            else if (Owner != player && IsActive)
            {
                if (gameManager.WriteOffMoney(player, RentalPrice))
                    gameManager.TextCreator.ShowPurchaseMessage($"{player.Name} заплатил игроку {Owner.Name} {RentalPrice} руб. ", player);
                else
                    gameManager.TextCreator.ShowMessage($"{player.Name} не смог заплатить игроку {Owner.Name} {RentalPrice} руб.");
            }
        }

        public int CompareTo(object obj)
        {
            return (int)(RentalPrice - (obj as BuyableCell).RentalPrice);
        }
    }

    class RailwayStation : BuyableCell
    {
        public RailwayStation(GameManager gameManager, string name, Player owner, decimal cost, decimal rentalPrice, PropertyFamily family, bool isActive) : base(gameManager, name, owner, cost, rentalPrice, family, isActive)
        {

        }

        public override void OnPlayerStepped(Player player)
        {
            base.OnPlayerStepped(player);
        }
    }


    class Street : BuyableCell
    {
        public decimal CostHouse { get; private set; }

        public Street(GameManager gameManager, string name, Player owner, decimal cost, decimal rentalPrice, PropertyFamily family, bool isActive, decimal costHouse) : base(gameManager, name, owner, cost, rentalPrice, family, isActive)
        {
            CostHouse = costHouse;
        }

        public override decimal RentalPrice => base.RentalPrice;
        virtual public Street GetNextHouseLevel()
        {
            return new FirstHouse(this);
        }

        virtual public Street GetPreviousHouseLevel()
        {
            return null;
        }

        public override void OnPlayerStepped(Player player)
        {
            base.OnPlayerStepped(player);
        }
    }

    abstract class HouseDecorator : Street
    {
        protected Street street;

        public HouseDecorator(GameManager gameManager, string name, Player owner, decimal cost, decimal rentalPrice, PropertyFamily family, bool isActive, decimal costHouse, Street street) : base(gameManager, name, owner, cost, rentalPrice, family, isActive, costHouse)
        {
            this.street = street;
        }
    }

    class FirstHouse : HouseDecorator
    {
        const int multiplier = 10;
        public FirstHouse(Street street) : base(street.gameManager, street.Name + " с 1 домом", street.Owner, street.Cost, street.RentalPrice, street.Family, street.IsActive, street.CostHouse, street)
        { }

        public override decimal RentalPrice { get => street.RentalPrice * multiplier; }

        public override Street GetNextHouseLevel()
        {
            return new SecondHouse(street);
        }
        public override Street GetPreviousHouseLevel()
        {
            return street;
        }
    }

    class SecondHouse : HouseDecorator
    {
        const int multiplier = 50;
        public SecondHouse(Street street) : base(street.gameManager, street.Name + " с 2 домами", street.Owner, street.Cost, street.RentalPrice, street.Family, street.IsActive, street.CostHouse, street)
        { }

        public override decimal RentalPrice { get => street.RentalPrice * multiplier; }

        public override Street GetNextHouseLevel()
        {
            return new ThirdHouse(street);
        }

        public override Street GetPreviousHouseLevel()
        {
            return new FirstHouse(street);
        }
    }

    class ThirdHouse : HouseDecorator
    {
        const int multiplier = 100;
        public ThirdHouse(Street street) : base(street.gameManager, street.Name + " с 3 домами", street.Owner, street.Cost, street.RentalPrice, street.Family, street.IsActive, street.CostHouse, street)
        { }
        public override decimal RentalPrice { get => street.RentalPrice * multiplier; }

        public override Street GetNextHouseLevel()
        {
            return new Hotel(street);
        }

        public override Street GetPreviousHouseLevel()
        {
            return new SecondHouse(street);
        }
    }

    class Hotel : HouseDecorator
    {
        const int multiplier = 200;
        public Hotel(Street street) : base(street.gameManager, street.Name + " c Отелем", street.Owner, street.Cost, street.RentalPrice, street.Family, street.IsActive, street.CostHouse, street)
        { }

        public override decimal RentalPrice { get => street.RentalPrice * multiplier; }

        public override Street GetNextHouseLevel()
        {
            return null;
        }

        public override Street GetPreviousHouseLevel()
        {
            return new ThirdHouse(street);
        }
    }

}
