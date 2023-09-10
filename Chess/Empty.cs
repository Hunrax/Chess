using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Empty : Piece
    {
        public Empty(MainWindow setWindow, ChessBoard setChessBoard, Game setGame, PieceColor setColor) : base(setWindow, setChessBoard, setGame, setColor)
        {
            window = setWindow;
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
