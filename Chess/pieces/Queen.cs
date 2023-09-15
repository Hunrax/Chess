using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Queen : Piece
    {
        public Queen(ChessBoard setChessBoard, PieceColor setColor) : base(setChessBoard, setColor)
        {
            chessBoard = setChessBoard; // We set it in a base constructor so we don't need to set it here - same for other classes
            color = setColor;
            type = PieceType.QUEEN;
            symbol = (color == PieceColor.WHITE) ? 'H' : 'h';
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

            newcolumn = column + 1;
            newrow = row + 1;
            while (newcolumn < chessBoard.size && newrow < chessBoard.size)
            {
                if (chessBoard.CanPieceMoveHere(newrow, newcolumn, color, checkForChecks, row, column))
                    possibleMoves.Add(new Point(newrow, newcolumn));
                if (!chessBoard.IsFieldEmpty(newrow, newcolumn))
                    break;
                newcolumn++;
                newrow++;
            }
            newcolumn = column - 1;
            newrow = row - 1;
            while (newcolumn >= chessBoard.minimumIndex && newrow >= chessBoard.minimumIndex)
            {
                if (chessBoard.CanPieceMoveHere(newrow, newcolumn, color, checkForChecks, row, column))
                    possibleMoves.Add(new Point(newrow, newcolumn));
                if (!chessBoard.IsFieldEmpty(newrow, newcolumn))
                    break;
                newcolumn--;
                newrow--;
            }
            newcolumn = column + 1;
            newrow = row - 1;
            while (newcolumn < chessBoard.size && newrow >= chessBoard.minimumIndex)
            {
                if (chessBoard.CanPieceMoveHere(newrow, newcolumn, color, checkForChecks, row, column))
                    possibleMoves.Add(new Point(newrow, newcolumn));
                if (!chessBoard.IsFieldEmpty(newrow, newcolumn))
                    break;
                newcolumn++;
                newrow--;
            }
            newcolumn = column - 1;
            newrow = row + 1;
            while (newcolumn >= chessBoard.minimumIndex && newrow < chessBoard.size)
            {
                if (chessBoard.CanPieceMoveHere(newrow, newcolumn, color, checkForChecks, row, column))
                    possibleMoves.Add(new Point(newrow, newcolumn));
                if (!chessBoard.IsFieldEmpty(newrow, newcolumn))
                    break;
                newcolumn--;
                newrow++;
            }
        }
    }
}
