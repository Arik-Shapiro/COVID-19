using covid_19.logic.Event;
using covid_19.service.DTO;
using covid_19.view;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace covid_19.logic
{
    public enum State 
    {
        Sick,
        Healthy,
        Carrying,
        SickQuarantined 
    }
    class Person : Block
    {
        protected enum Command 
        {
            Infect,
            Heal
        }


        protected static class Probabilities
        {
            public const int Infect = 5;
            public const int Heal = 20;
        }

        protected static class ThreadTimeIntervals
        {
            public const int MinAction = 100;
            public const int MaxAction = 500;
            public const int DoctorHeal = 50;
            public const int InfectPerson = 50;
        }

        private const int InfectionRange = 2;

        protected State getState(State current, Command command)
        {
            return (current, command) switch
            {
                (State.Healthy, Command.Heal) => State.Healthy,
                (State.Healthy, Command.Infect) => State.Sick,
                (State.Sick, Command.Heal) => State.Healthy,
                (State.Sick, Command.Infect) => State.Sick,
                _ => throw new NotSupportedException(
                    $"{current} has no transition on {command}")
            };
        }

        protected object stateLock;

        protected void ChangeState(Command command)
        {
            lock (stateLock)
            {
                _state = getState(_state, command);
            }
        }

        protected State _state;

        public static Random rnd = new Random();
        public event EventHandler<MoveEvent> OnMove;
        public event EventHandler<InfectEvent> OnInfect;

        public bool ShouldStop { get; set; }

        protected List<Action> _moveAction;
        private readonly Dictionary<State, Action> _actions;
        public Person(int x, int y, State state) : base(x, y)
        {
            _moveAction = new List<Action>() {
                MoveUp,
                MoveDown,
                MoveLeft,
                MoveRight
            };
            _actions = new Dictionary<State, Action>() {
                { State.Healthy, HealthyAction },
                { State.Sick, SickAction }
            };
            _state = state;
            ShouldStop = false;
            stateLock = new object();
            _infectThread = new Thread(InfectSurroundings);
        }


        public void Run()
        {
            while (!ShouldStop)
            {
                State currentState = _state;
                _actions[currentState]();
                Thread.Sleep(rnd.Next(ThreadTimeIntervals.MinAction, ThreadTimeIntervals.MaxAction));
            }
        }

        protected virtual void HealthyAction()
        {
            _moveAction[rnd.Next(_moveAction.Count)]();
        }

        private Thread _infectThread;
        protected void SickAction()
        {
            if (!_infectThread.IsAlive)
            {
                _infectThread.Start();
            }
            _moveAction[rnd.Next(_moveAction.Count)]();
        }

        private void InfectSurroundings()
        {
            State currentState = this._state;
            while (currentState.Equals(State.Sick))
            {
                int currentX = this.X;
                int currentY = this.Y;
                OnInfect?.Invoke(this, new InfectEvent { X = currentX, Y = currentY, InfectionRange = Person.InfectionRange });
                currentState = this._state;
                Thread.Sleep(ThreadTimeIntervals.InfectPerson);
            }
            _infectThread = new Thread(InfectSurroundings);
        }
        public override void Infect()
        {
            int chance = rnd.Next(1, 100);
            if (chance <= Probabilities.Infect)
            {
                ChangeState(Command.Infect);
            }
        }
        public override void Heal()
        {
            int chance = rnd.Next(1, 100);
            if (chance <= Probabilities.Heal)
            {
                ChangeState(Command.Heal);
            }
        }

        private void MoveUp()
        {
            OnMove?.Invoke(this, new MoveEvent { PrevX = X, PrevY = Y, NewX = X, NewY = Y - 1 });
        }
        public void MoveDown()
        {
            OnMove?.Invoke(this, new MoveEvent { PrevX = X, PrevY = Y, NewX = X, NewY = Y + 1 });
        }
        public void MoveLeft()
        {
            OnMove?.Invoke(this, new MoveEvent { PrevX = X, PrevY = Y, NewX = X - 1, NewY = Y });
        }
        public void MoveRight()
        {
            OnMove?.Invoke(this, new MoveEvent { PrevX = X, PrevY = Y, NewX = X + 1, NewY = Y });
        }
        public override BlockInfo GetBlockInfo()
        {
            return new BlockInfo(X, Y, _state == State.Healthy ? BlockType.HealthyPerson : BlockType.Sick);
        }
    }
}
