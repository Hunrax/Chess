using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Bishop : Piece
    {
        public Bishop(ChessBoard setChessBoard, PieceColor setColor) : base(setChessBoard, setColor)
        {
            chessBoard = setChessBoard;
            color = setColor;
            type = PieceType.BISHOP;
            symbol = (color == PieceColor.WHITE) ? 'G' : 'g' ;
        }
        public override void GeneratePossibleMoves(List<Point> possibleMoves, bool checkForChecks)
        {
            int newcolumn = column + 1;
            int newrow = row + 1;
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
