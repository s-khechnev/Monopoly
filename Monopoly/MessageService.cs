using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Monopoly
{
    interface IMessageService
    {
        void ShowMessage(string message);
    }

    class MessageService : IMessageService
    {
        public void ShowMessage(string message)
        {
            Console.WriteLine(message);
        }
    }
}
