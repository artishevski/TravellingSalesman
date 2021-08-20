using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Cities
    {
        private int[,] citiesDistance = {
            {0, 1, 5, 2, 3, 2, 8},
            {1, 0, 1, 1, 2, 1, 3},
            {5, 1, 0, 2, 3, 5, 6},
            {2, 1, 2, 0, 1, 2, 3},
            {3, 2, 3, 1, 0, 1, 2},
            {2, 1, 5, 2, 1, 0, 1},
            {8, 3, 6, 3, 2, 1, 0},
        };
        private int citiesNum = 7;
        public Cities(){}
        public int[,] CitiesDistance { get => citiesDistance; set => citiesDistance = value; }
        public int CitiesNum { get => citiesNum; set => citiesNum = value; }
    }
}
