using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class ChessBoard
    {
        public Piece[,] board = new Piece[8,8];


        public void Clear()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new Piece("None", "Empty", '.');
                }
            }
        }
        public void Initialize()
        {
            board[0, 0] = new Piece("White", "Rook", 'W');
            board[0, 1] = new Piece("Black", "Knight", 'S');
            board[0, 2] = new Piece("Black", "Bishop", 'G');
            board[0, 3] = new Piece("Black", "Queen", 'H');
            board[0, 4] = new Piece("Black", "King", 'K');
            board[0, 5] = new Piece("Black", "Bishop", 'G');
            board[0, 6] = new Piece("Black", "Knight", 'S');
            board[0, 7] = new Piece("Black", "Rook", 'W');

            for(int i = 0; i < 8; i++)
                board[1, i] = new Piece("Black", "Pawn", 'P');

            board[7, 0] = new Piece("White", "Rook", 'W');
            board[7, 1] = new Piece("White", "Knight", 'S');
            board[7, 2] = new Piece("White", "Bishop", 'G');
            board[7, 3] = new Piece("White", "Queen", 'H');
            board[7, 4] = new Piece("White", "King", 'K');
            board[7, 5] = new Piece("White", "Bishop", 'G');
            board[7, 6] = new Piece("White", "Knight", 'S');
            board[7, 7] = new Piece("White", "Rook", 'W');

            for (int i = 0; i < 8; i++)
                board[6, i] = new Piece("White", "Pawn", 'P');
        }
        public void Print()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    Trace.Write(board[i, j].symbol + " ");
                }
                Trace.WriteLine("");
            }
            Trace.WriteLine("");
        }
    }
}
