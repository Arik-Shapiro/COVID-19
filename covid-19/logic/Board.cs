using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using covid_19.logic.Event;

namespace covid_19.logic
{
    class Board
    {
        public Board(List<Person> people, int size)
        {
            this.size = size;
            SubscribeAll(people);
            board = new Person [size, size];
            locks = new int[size, size];
            InitializeBoard(people);
        }

        private int size;
        private Person[,] board;
        private int[,] locks;
        private void SubscribeAll(List<Person> people)
        {
            foreach(Person person in people){
                Subscribe(person);
            }
        }

        private void Subscribe(Person person)
        {
            person.OnMove += MovePerson;
            person.OnInfect += InfectPeople;
        }

        private void InitializeBoard(List<Person> people)
        {
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    locks[i, j] = 0;
                }
            }
            foreach(Person person in people)
            {
                board[person.X, person.Y] = person;
            }
        }

        private void MovePerson(object sender, MoveEvent e)
        {
            Person person = (Person)sender;
            if (IllegalMove(e))
            {
                Console.WriteLine("Person: {0} hit the wall, it was pretty painful.", person.Name);
                return;
            }
            if (Interlocked.Increment(ref locks[e.NewX, e.NewY]) == 1)
            {
                if (board[e.NewX, e.NewY] == null)
                {
                    board[e.NewX, e.NewY] = board[e.PrevX, e.PrevY];
                    board[e.PrevX, e.PrevY] = null;
                    person.X = e.NewX;
                    person.Y = e.NewY;
                }
                Interlocked.Decrement(ref locks[e.NewX, e.NewY]);
            }
            PrintBoard();
        }

        private void InfectPeople(object sender, InfectEvent e)
        {
            int startY = Math.Max(e.Y - e.InfectionRange, 0);
            int startX = Math.Max(e.X - e.InfectionRange, 0);
            int endY = Math.Min(e.Y + e.InfectionRange, size - 1);
            int endX = Math.Min(e.X + e.InfectionRange, size - 1);

            List<Person> peopleToInfect = new List<Person>();
            for(int i = startY; i <= endY; i++)
            {
                for(int j = startX; j <= endX; j++)
                {
                    Person person = board[i, j];
                    if(Interlocked.Increment(ref locks[i, j]) == 1)
                    {
                        if (person != null && !sender.Equals(person))
                        {
                            person.Infect();
                            PrintBoard();
                        }
                    }
                    Interlocked.Decrement(ref locks[i, j]);
                }
            }
        }

        private void PrintBoard()
        {
            lock (this)
            {
                Person[,] board = this.board;
                //Console.Clear();
                for (int i = 0; i < size; i++)
                {
                    Console.SetCursorPosition((Console.WindowWidth - size) / 2 , i);
                    for (int j = 0; j < size; j++)
                    {
                        Person person = board[i, j];
                        if (person == null)
                        {
                            Console.Write("_");
                        }
                        else
                        {
                            Console.Write(person.ToString());
                        }
                    }
                    Console.Write('\n');
                }
            }
        }

        private bool IllegalMove(MoveEvent e)
        {
            return e.NewX < 0 || e.NewY < 0 || e.NewX >= size || e.NewY >= size;
        }
    }
}
