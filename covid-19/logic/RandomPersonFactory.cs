using covid_19.logic.events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace covid_19.logic
{
    enum PersonType
    {
        Person,
        Doctor
    }
    class RandomPersonFactory
    {
        public delegate Person IFactory(int x, int y, State state);
        private Dictionary<PersonType, IFactory> factories;
        private List<(int, int)> indexPairs = new List<(int, int)>();
        private Random rnd = new Random();
        public RandomPersonFactory(int boardWidth, int boardHeight)
        {
            for (int i = 0; i < boardHeight; i++)
            {
                for (int j = 0; j < boardWidth; j++)
                {
                    indexPairs.Add((j, i));
                }
            }
            factories = new Dictionary<PersonType, IFactory>()
            {
                { PersonType.Person, (x, y, state) => new Person(x, y, state) },
                { PersonType.Doctor, (x, y, state) => new Doctor(x, y, state) }
            };
        }

        public Person GetPerson(PersonType type, State state)
        {
            var pair = indexPairs[rnd.Next(indexPairs.Count)];
            indexPairs.Remove(pair);
            return factories[type](pair.Item1, pair.Item2, state);
        }

        public List<Person> GetPeople(PersonType type, State state, int numOfPeople)
        {
            List<Person> people = new List<Person>();
            for(int i = 0; i < numOfPeople; i++)
            {
                people.Add(GetPerson(type, state));
            }
            return people;
        }

    }
}
