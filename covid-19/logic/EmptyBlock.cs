using covid_19.service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace covid_19.logic
{
    class EmptyBlock : Block
    {
        public EmptyBlock(int x, int y) : base(x, y)
        {
        }

        public override BlockInfo GetBlockInfo()
        {
            return new BlockInfo(X, Y, BlockType.Empty);
        }

        public override void Heal()
        {
            //DO NOTHING
        }

        public override void Infect()
        {
            //DO NOTHING
        }
        public override bool isEmpty()
        {
            return true;
        }
    }
}
