using covid_19.logic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace covid_19.service
{
    class GameController
    {
        public async Task Start()
        {
            List<Person> people = new List<Person>
            {
                new Person("1", 0, 0 , 20, true),
                new Person("2", 2, 2, 20, true),
                new Person("3", 3, 3, 20, true),
                new Person("4", 5, 5, 20, false),
                new Person("5", 6, 6, 20, false),
                new Person("6", 7, 7, 20, false)
            };
            Board board = new Board(people, 20);
            List<Task> tasks = new List<Task>();
            foreach(Person p in people)
            {
                tasks.Add(Task.Run(() => p.Run()));
            }
            await Task.WhenAll(tasks);
        }
    }
}
