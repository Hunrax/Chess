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
        public bool firstMove;
        public Piece(ChessBoard setChessBoard, PieceColor setColor)
        {
            chessBoard = setChessBoard;
            color = setColor;
        }
        public abstract void GeneratePossibleMoves(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks);
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
