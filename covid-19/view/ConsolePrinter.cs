using covid_19.logic;
using covid_19.service.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace covid_19.view
{
    class ConsolePrinter : IObserver<BlockInfo>
    {
        private int _width;
        private int _height;

        private Dictionary<BlockType, string> _BlockTypeToString
            = new Dictionary<BlockType, string>() { 
                { BlockType.Empty, "_" }, 
                { BlockType.HealthyPerson, "#" }, 
                { BlockType.HealthyDoctor, "+" }, 
                { BlockType.Sick, "*" } 
            };

        public ConsolePrinter(int width, int height)
        {
            _width = width;
            _height = height;
            _lock = new object();
        }

        public void OnCompleted()
        {
            throw new NotImplementedException();
        }

        public void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        private object _lock;
        public void OnNext(BlockInfo value)
        {
            lock (_lock)
            {
                Console.SetCursorPosition((Console.WindowWidth - _width) / 2 + value.X, value.Y);
                Console.Write(_BlockTypeToString[value.Type]);
            }
        }
    }
}
