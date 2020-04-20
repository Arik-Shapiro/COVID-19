using covid_19.logic;
using covid_19.view;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace covid_19.service
{
    class GameController
    {
        public GameController(Board board, List<Person> people, ConsolePrinter printer)
        {
            _board = board;
            _people = people;
            _printer = printer;
        }
        private Board _board;
        private List<Person> _people;
        private ConsolePrinter _printer;
        public async Task Start()
        {
            _board.Subscribe(_printer);
            _board.InitializeBoard(_people);
            List<Task> tasks = new List<Task>();
            foreach(Person p in _people)
            {
                tasks.Add(Task.Run(p.Run));
            }
            ThreadPool.SetMinThreads(_people.Count, 0);
            await Task.WhenAll(tasks);
        }
    }
}
