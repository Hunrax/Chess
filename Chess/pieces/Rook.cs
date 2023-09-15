using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Rook : Piece
    {
        public Rook(ChessBoard setChessBoard, PieceColor setColor) : base(setChessBoard, setColor)
        {
            chessBoard = setChessBoard;
            color = setColor;
            type = PieceType.ROOK;
            symbol = (color == PieceColor.WHITE) ? 'W' : 'w';
            firstMove = true;
        }
        public override void GeneratePossibleMoves(List<Point> possibleMoves, bool checkForChecks)
        {
            int newrow = row;
            int newcolumn = column;
            for (int i = column + 1; i < chessBoard.size; i++)
            {
                newcolumn += 1;
                if (chessBoard.CanPieceMoveHere(row, newcolumn, color, checkForChecks, row, column))
                    possibleMoves.Add(new Point(row, newcolumn));
                if (!chessBoard.IsFieldEmpty(row, newcolumn))
                    break;
            }
            newcolumn = column;
            for (int i = column - 1; i >= chessBoard.minimumIndex; i--)
            {
                newcolumn -= 1;
                if (chessBoard.CanPieceMoveHere(row, newcolumn, color, checkForChecks, row, column))
                    possibleMoves.Add(new Point(row, newcolumn));
                if (!chessBoard.IsFieldEmpty(row, newcolumn))
                    break;
            }
            for (int i = row + 1; i < chessBoard.size; i++)
            {
                newrow += 1;
                if (chessBoard.CanPieceMoveHere(newrow, column, color, checkForChecks, row, column))
                    possibleMoves.Add(new Point(newrow, column));
                if (!chessBoard.IsFieldEmpty(newrow, column))
                    break;
            }
            newrow = row;
            for (int i = row - 1; i >= chessBoard.minimumIndex; i--)
            {
                newrow -= 1;
                if (chessBoard.CanPieceMoveHere(newrow, column, color, checkForChecks, row, column))
                    possibleMoves.Add(new Point(newrow, column));
                if (!chessBoard.IsFieldEmpty(newrow, column))
                    break;
            }
        }
    }
}
