using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace covid_19.service.DTO
{
    public enum BlockType
    {
        Empty,
        HealthyPerson,
        HealthyDoctor,
        Sick
    }
    class BlockInfo
    {

        //add enum of block
        public BlockInfo(int x, int y, BlockType type)
        {
            X = x;
            Y = y;
            Type = type;
        }
        public int X { get; set; }
        public int Y { get; set; }

        public BlockType Type { get; set; }
    }
}
