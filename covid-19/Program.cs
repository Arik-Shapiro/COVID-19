using covid_19.service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace covid_19
{
    class Program
    {
        static async Task Main(string[] args)
        {
            GameController controller = new GameController();
            await controller.Start();

        }
    }
}
