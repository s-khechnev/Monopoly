using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly
{
    class Player
    {
        const decimal defaultAmountMoney = 1500;

        public string Name { get; private set; }
        public decimal Money { get; private set; }
        public int Position { get; private set; }
        public bool IsLoser { get; private set; }
        public Dictionary<PropertyFamily, LinkedList<BuyableCell>> Property { get; private set; }

        public Player(string name)
        {
            Name = name;
            Init();
        }

        void Init()
        {
            Money = defaultAmountMoney;
            Position = 0;
            IsLoser = false;
            Property = new Dictionary<PropertyFamily, LinkedList<BuyableCell>>();
        }

        public void ChangePosition(int toPosition)
        {
            Position = toPosition;
        }

        public void WriteOffMoney(decimal amount)
        {
            Money -= amount;
        }

        public void AddMoney(decimal amount)
        {
            Money += amount;
        }

        public void AddProperty(BuyableCell buyableCell)
        {
            if (!Property.ContainsKey(buyableCell.Family))
                Property.Add(buyableCell.Family, new LinkedList<BuyableCell>());
            Property[buyableCell.Family].AddLast(buyableCell);

        }

        public void SetIsLoser()
        {
            IsLoser = true;
            Money = -1;
            SellPlayerProperty();
        }

        private void SellPlayerProperty()
        {
            foreach (var key in Property.Keys)
            {
                foreach (var item in Property[key])
                {
                    item.Owner = null;
                }
            }
        }

        public void ActivateLoserProperty()
        {
            foreach (var key in Property.Keys)
            {
                foreach (var item in Property[key])
                {
                    item.IsActive = true;
                }
            }
        }
    }
}