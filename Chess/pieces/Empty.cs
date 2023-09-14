using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Empty : Piece
    {
        public Empty(ChessBoard setChessBoard, PieceColor setColor) : base(setChessBoard, setColor)
        {
            chessBoard = setChessBoard;
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
