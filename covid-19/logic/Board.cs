using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using covid_19.logic.Event;
using covid_19.logic.events;
using covid_19.service.DTO;

namespace covid_19.logic
{
    class Board
    {
        public Board(List<Person> people, int width, int height)
        {
            _width = width;
            _height = height;
            board = new Block[_height, _width];
            _locks = new object[_height, _width];
            observers = new List<IObserver<BlockInfo>>();
            SubscribeAll(people);
        }

        private int _width;
        private int _height;
        private Block[,] board;
        private object[,] _locks;
        private void SubscribeAll(List<Person> people)
        {
            foreach (Person person in people)
            {
                Subscribe((dynamic)person);
            }
        }

        private void Subscribe(Person person)
        {
            person.OnMove += MovePerson;
            person.OnInfect += InfectPeople;
        }
        private void Subscribe(Doctor doctor)
        {
            doctor.OnMove += MovePerson;
            doctor.OnInfect += InfectPeople;
            doctor.OnHeal += HealPeople;
        }

        public void InitializeBoard(List<Person> people)
        {

            foreach (Person person in people)
            {
                board[person.Y, person.X] = person;
            }
            for (int i = 0; i < _height; i++)
            {
                for (int j = 0; j < _width; j++)
                {
                    if(board[i, j] == null)
                    {
                        board[i, j] = new EmptyBlock(j, i);
                    }
                    _locks[i, j] = new object();
                    observers.ForEach(o => o.OnNext(board[i, j].GetBlockInfo()));
                }
            }
        }

        private void MovePerson(object sender, MoveEvent e)
        {
            Person person = (Person)sender;
            if (IllegalMove(e))
            {
                return;
            }
            object locker = _locks[e.NewY, e.NewX];
            if (Monitor.TryEnter(locker))
            {
                try
                {
                    if (board[e.NewY, e.NewX].isEmpty())
                    {
                        Block temp = board[e.NewY, e.NewX];
                        board[e.NewY, e.NewX] = person;
                        board[e.PrevY, e.PrevX] = temp;
                        temp.X = e.PrevX;
                        temp.Y = e.PrevY;
                        person.X = e.NewX;
                        person.Y = e.NewY;
                        UpdateBoard(temp);
                        UpdateBoard(person);
                    }
                }
                finally
                {
                    Monitor.Exit(locker);
                }
            }
        }

        private void UpdateBoard(Block block)
        {
            observers.ForEach(o => o.OnNext(block.GetBlockInfo()));
        }

        private void InfectPeople(object sender, InfectEvent e)
        {
            PerformRangedAction(e.X, e.Y, e.InfectionRange, sender, block => { block.Infect(); UpdateBoard(block); });
        }

        private void HealPeople(object sender, HealEvent e)
        {
            PerformRangedAction(e.X, e.Y, e.HealRange, sender, block => { block.Heal(); UpdateBoard(block); });
        }

        private void PerformRangedAction(int x, int y, int range, object actionPerformer, Action<Block> action)
        {
            int startY = Math.Max(y - range, 0);
            int startX = Math.Max(x - range, 0);
            int endY = Math.Min(y + range, _height - 1);
            int endX = Math.Min(x + range, _width - 1);

            for (int i = startY; i <= endY; i++)
            {
                for (int j = startX; j <= endX; j++)
                {
                    Block block = board[i, j];
                    object locker = _locks[i, j];
                    if (Monitor.TryEnter(locker))
                    {
                        try
                        {
                            if (block != null && !actionPerformer.Equals(block))
                            {
                                action(block);
                            }
                        }
                        finally
                        {
                            Monitor.Exit(_locks[i, j]);
                        }
                    }
                }
            }
        }

        private bool IllegalMove(MoveEvent e)
        {
            return e.NewX < 0 || e.NewY < 0 || e.NewX >= _width || e.NewY >= _height;
        }

        List<IObserver<BlockInfo>> observers;
        public IDisposable Subscribe(IObserver<BlockInfo> observer)
        {
            observers.Add(observer);
            return new Unsubscriber<BlockInfo>(observers, observer);
        }
        internal class Unsubscriber<PersonInfo> : IDisposable
        {
            private List<IObserver<PersonInfo>> _observers;
            private IObserver<PersonInfo> _observer;

            internal Unsubscriber(List<IObserver<PersonInfo>> observers, IObserver<PersonInfo> observer)
            {
                this._observers = observers;
                this._observer = observer;
            }

            public void Dispose()
            {
                if (_observers.Contains(_observer))
                    _observers.Remove(_observer);
            }
        }
    }
}
