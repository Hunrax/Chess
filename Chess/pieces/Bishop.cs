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
        public override void GeneratePossibleMoves(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            int newPieceColumn = pieceColumn + 1;
            int newPieceRow = pieceRow + 1;
            while (newPieceColumn < chessBoard.size && newPieceRow < chessBoard.size)
            {
                if (chessBoard.CanPieceMoveHere(newPieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!chessBoard.IsFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn++;
                newPieceRow++;
            }
            newPieceColumn = pieceColumn - 1;
            newPieceRow = pieceRow - 1;
            while (newPieceColumn >= chessBoard.minimumIndex && newPieceRow >= chessBoard.minimumIndex)
            {
                if (chessBoard.CanPieceMoveHere(newPieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!chessBoard.IsFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn--;
                newPieceRow--;
            }
            newPieceColumn = pieceColumn + 1;
            newPieceRow = pieceRow - 1;
            while (newPieceColumn < chessBoard.size && newPieceRow >= chessBoard.minimumIndex)
            {
                if (chessBoard.CanPieceMoveHere(newPieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!chessBoard.IsFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn++;
                newPieceRow--;
            }
            newPieceColumn = pieceColumn - 1;
            newPieceRow = pieceRow + 1;
            while (newPieceColumn >= chessBoard.minimumIndex && newPieceRow < chessBoard.size)
            {
                if (chessBoard.CanPieceMoveHere(newPieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!chessBoard.IsFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn--;
                newPieceRow++;
            }
        }
    }
}
