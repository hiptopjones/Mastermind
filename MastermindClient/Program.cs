using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MastermindClient
{
    class Program
    {
        static int Main(string[] args)
        {
            try
            {
                if (args.Length < 1)
                {
                    Console.WriteLine("Missing a URL to communicate with.");
                    return 1;
                }

                string serverUrl = args[0];

                GameClient client = new GameClient(serverUrl);
                client.Run();

                return 0;
            }
            finally
            {
                Console.WriteLine("Hit a key to continue...");
                Console.ReadKey();
            }
        }
    }
}
