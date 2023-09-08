using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    [Serializable]
    class Piece
    {
        public string color;
        public string type;
        public char symbol;
        public bool firstMove;
        public int pawnDoubleMoveTurn;
        public Piece(string setColor, string setType, char setSymbol)
        {
            color = setColor;
            type = setType;
            symbol = setSymbol;
            firstMove = true;
            pawnDoubleMoveTurn = -1;
        }
    }
}
