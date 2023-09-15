using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Knight : Piece
    {
        public Knight(ChessBoard setChessBoard, PieceColor setColor) : base(setChessBoard, setColor)
        {
            chessBoard = setChessBoard;
            color = setColor;
            type = PieceType.KNIGHT;
            symbol = (color == PieceColor.WHITE) ? 'S' : 's';
        }
        public override void GeneratePossibleMoves(List<Point> possibleMoves, bool checkForChecks)
        {
            if (chessBoard.CanPieceMoveHere(row - 1, column - 2, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row - 1, column - 2));
            if (chessBoard.CanPieceMoveHere(row + 1, column - 2, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row + 1, column - 2));

            if (chessBoard.CanPieceMoveHere(row - 1, column + 2, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row - 1, column + 2));
            if (chessBoard.CanPieceMoveHere(row + 1, column + 2, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row + 1, column + 2));

            if (chessBoard.CanPieceMoveHere(row - 2, column + 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row - 2, column + 1));
            if (chessBoard.CanPieceMoveHere(row + 2, column + 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row + 2, column + 1));

            if (chessBoard.CanPieceMoveHere(row - 2, column - 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row - 2, column - 1));
            if (chessBoard.CanPieceMoveHere(row + 2, column - 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row + 2, column - 1));
        }
    }
}
