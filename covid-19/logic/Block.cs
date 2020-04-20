using covid_19.service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace covid_19.logic
{
    abstract class Block
    {
        public int X { get; set; }
        public int Y { get; set; }
        public abstract void Infect();
        public abstract void Heal();

        public abstract BlockInfo GetBlockInfo();

        public Block(int x, int y)
        {
            X = x;
            Y = y;
        }

        public virtual bool isEmpty()
        {
            return false;
        }

    }
}
