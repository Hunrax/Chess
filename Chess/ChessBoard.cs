using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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
            board[0, 0] = new Piece("Black", "Rook", 'W');
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
        public string GetPieceTypeFromField(int row, int column)
        {
            return board[row, column].type;
        }
        public string getPieceColorFromField(int row, int column)
        {
            return board[row, column].color;
        }
        public bool isFieldEmpty(int row, int column)
        {
            if (row < 0 || row > 7 || column < 0 || column > 7)
                return false;
            if (board[row, column].type == "Empty")
                return true;
            return false;
        }
        public bool isFieldPossibleToCapture(int row, int column, string pieceColor)
        {
            if (row < 0 || row > 7 || column < 0 || column > 7)
                return false;
            if (board[row, column].type != "Empty" && pieceColor != board[row, column].color)
                return true;
            return false;
        }
        public bool isFieldAKing(int row, int column)
        {
            if (board[row, column].type == "King")
                return true;
            return false;
        }
        public bool firstMoveOfPiece(int pieceRow, int pieceColumn)
        {
            return board[pieceRow, pieceColumn].firstMove;
        }
        public bool arePointsEqual(Point point, int x, int y)
        {
            if (point.X == x && point.Y == y)
                return true;
            return false;
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
