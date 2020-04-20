using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace covid_19.logic.Event
{
    class MoveEvent : EventArgs
    {
        public int NewX { get; set; }
        public int NewY { get; set; }
        public int PrevX { get; set; }
        public int PrevY { get; set; }
    }
}
