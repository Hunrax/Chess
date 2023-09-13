using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public abstract class Piece
    {
        public PieceColor color;
        public PieceType type;
        public char symbol;
        public ChessBoard chessBoard;
        public Game game;
        public bool firstMove;
        public Piece(ChessBoard setChessBoard, Game setGame, PieceColor setColor)
        {
            chessBoard = setChessBoard;
            game = setGame;
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
