using covid_19.service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace covid_19.logic.events
{
    class Doctor : Person
    {
        private const int HealRange = 4;
        private Thread _healThread;
        public EventHandler<HealEvent> OnHeal;
        public Doctor(int x, int y, State state) : base(x, y, state)
        {
            _healThread = new Thread(HealSurroundings);
        }

        private void HealSurroundings()
        {
            State currentState = this._state;
            while (!currentState.Equals(State.Sick))
            {
                int currentX = this.X;
                int currentY = this.Y;
                OnHeal?.Invoke(this, new HealEvent { X = currentX, Y = currentY, HealRange = Doctor.HealRange });
                currentState = this._state;
                Thread.Sleep(ThreadTimeIntervals.DoctorHeal);
            }
            _healThread = new Thread(HealSurroundings);
        }

        protected override void HealthyAction()
        {
            if (!_healThread.IsAlive)
            {
                _healThread.Start();
            }
            _moveAction[rnd.Next(_moveAction.Count)]();
        }
        public override BlockInfo GetBlockInfo()
        {
            return new BlockInfo(X, Y, _state == State.Healthy ? BlockType.HealthyDoctor : BlockType.Sick);
        }
    }
}
