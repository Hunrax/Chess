using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace Chess
{
    public abstract class Piece
    {
        public PieceColor color;
        public PieceType type;
        public char symbol;
        public ChessBoard chessBoard;
        public bool firstMove; // I suggest to initialise it here in constructor - good practise is not to leave fields uninitialised
        public Button button;
        public int row, column;
        public Piece(ChessBoard setChessBoard, PieceColor setColor)
        {
            chessBoard = setChessBoard;
            color = setColor;
            button = new Button();
        }
        public abstract void GeneratePossibleMoves(List<Point> possibleMoves, bool checkForChecks);
    }
    public enum PieceColor
    {
        BLACK,
        WHITE,
        NONE
    }
    public enum PieceType
    {
        PAWN,
        BISHOP,
        KNIGHT,
        ROOK,
        QUEEN,
        KING,
        EMPTY
    }
}
