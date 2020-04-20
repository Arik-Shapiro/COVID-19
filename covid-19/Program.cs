using covid_19.logic;
using covid_19.logic.events;
using covid_19.service;
using covid_19.view;
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
            const int boardWidth = 50;
            const int boardHeight = 25;
            const int numOfHealthyPerson = 5;
            const int numOfSickPerson = 5;
            const int numOfDoctors = 4;
            RandomPersonFactory factory = new RandomPersonFactory(boardWidth, boardHeight);
            List<Person> people = factory.GetPeople(PersonType.Person, State.Healthy, numOfHealthyPerson)
                .Concat(factory.GetPeople(PersonType.Person, State.Sick, numOfSickPerson))
                .Concat(factory.GetPeople(PersonType.Doctor, State.Healthy, numOfDoctors))
                .ToList();
            ConsolePrinter printer = new ConsolePrinter(boardWidth, boardHeight);
            Board board = new Board(people, boardWidth, boardHeight);
            GameController controller = new GameController(board, people, printer);
            await controller.Start();
        }
    }
}
