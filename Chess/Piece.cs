using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Piece
    {
        public string color;
        public string type;
        public char symbol;

        public Piece(string setColor, string setType, char setSymbol)
        {
            color = setColor;
            type = setType;
            symbol = setSymbol;
        }
    }
}
