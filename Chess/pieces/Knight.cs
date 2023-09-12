using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Knight : Piece
    {
        public Knight(MainWindow setWindow, ChessBoard setChessBoard, Game setGame, PieceColor setColor) : base(setWindow, setChessBoard, setGame, setColor)
        {
            window = setWindow;
            chessBoard = setChessBoard;
            game = setGame;
            color = setColor;
            type = PieceType.KNIGHT;
            symbol = (color == PieceColor.WHITE) ? 'S' : 's';
        }
        public override void GeneratePossibleMoves(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            if (chessBoard.CanPieceMoveHere(pieceRow - 1, pieceColumn - 2, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn - 2));
            if (chessBoard.CanPieceMoveHere(pieceRow + 1, pieceColumn - 2, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn - 2));

            if (chessBoard.CanPieceMoveHere(pieceRow - 1, pieceColumn + 2, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn + 2));
            if (chessBoard.CanPieceMoveHere(pieceRow + 1, pieceColumn + 2, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn + 2));

            if (chessBoard.CanPieceMoveHere(pieceRow - 2, pieceColumn + 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 2, pieceColumn + 1));
            if (chessBoard.CanPieceMoveHere(pieceRow + 2, pieceColumn + 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 2, pieceColumn + 1));

            if (chessBoard.CanPieceMoveHere(pieceRow - 2, pieceColumn - 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 2, pieceColumn - 1));
            if (chessBoard.CanPieceMoveHere(pieceRow + 2, pieceColumn - 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 2, pieceColumn - 1));
        }
    }
}
