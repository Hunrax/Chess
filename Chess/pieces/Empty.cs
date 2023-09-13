using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Empty : Piece
    {
        public Empty(ChessBoard setChessBoard, Game setGame, PieceColor setColor) : base(setChessBoard, setGame, setColor)
        {
            chessBoard = setChessBoard;
            game = setGame;
            color = setColor;
            type = PieceType.EMPTY;
            symbol = '.';
        }
        public override void GeneratePossibleMoves(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            return;
        }
    }
}
