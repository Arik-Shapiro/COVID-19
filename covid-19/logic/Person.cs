using covid_19.logic.Event;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace covid_19.logic
{
    class Person
    {
        private enum State { Sick, Healthy, Carrying, SickQuarantined }
        private enum Command { Infect, Heal }

        private State getState(State current, Command command)
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

        private object stateLock;

        private void ChangeState(Command command)
        {
            lock (stateLock)
            { 
                state = getState(state, command);
            }
        }

        private State state;

        private static Random rnd = new System.Random();
        public event EventHandler<MoveEvent> OnMove;
        public event EventHandler<InfectEvent> OnInfect;

        public bool ShouldStop { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string Name { get; set; }
        public int MaxRnd { get; set; }


        private List<Action> moveAction;
        private readonly Dictionary<State, Action> actions;
        public Person(string name, int x, int y, int maxRnd, bool isSick)
        {
            Name = name;
            X = x;
            Y = y;
            MaxRnd = maxRnd;
            moveAction = new List<Action>() { MoveUp, MoveDown, MoveLeft, MoveRight };
            actions = new Dictionary<State, Action>() { { State.Healthy, HealthyAction }, { State.Sick, SickAction} };
            state = isSick ? State.Sick : State.Healthy;
            ShouldStop = false;
            stateLock = new object();
            infectThread = new Thread(InfectSurroundings);
        }


        public void Run()
        {
            int rndX = rnd.Next(MaxRnd);
            int rndY = rnd.Next(MaxRnd);
            OnMove?.Invoke(this, new MoveEvent { PrevX = X, PrevY = Y, NewX = rndX, NewY = Y });
            while (!ShouldStop)
            {
                State currentState = state;
                actions[currentState]();
                Thread.Sleep(rnd.Next(500, 1500));
            }
        }

        private void HealthyAction()
        {
            moveAction[rnd.Next(moveAction.Count)]();
        }

        private Thread infectThread;
        private void SickAction()
        {
            if (!infectThread.IsAlive)
            {
                infectThread.Start();
            }
            moveAction[rnd.Next(moveAction.Count)]();
        }

        private void InfectSurroundings()
        {
            State currentState = this.state;
            while (currentState.Equals(State.Sick)){
                Thread.Sleep(100);
                int currentX = this.X;
                int currentY = this.Y;
                OnInfect?.Invoke(this, new InfectEvent { X = currentX, Y = currentY, InfectionRange = 1 });
                currentState = this.state;
            }
        }
        public void Infect()
        {
            ChangeState(Command.Infect);
        }
        public void Heal()
        {
            ChangeState(Command.Heal);
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

        public override string ToString()
        {
            if (state.Equals(State.Sick)) return "*";
            else return "#";
        }
    }
}
