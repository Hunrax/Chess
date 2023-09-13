using System;
using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Pawn : Piece
    {
        public int pawnDoubleMoveTurn;
        public int moveDirection1, moveDirection2;
        public Pawn(ChessBoard setChessBoard, Game setGame, PieceColor setColor) : base(setChessBoard, setGame, setColor)
        {
            chessBoard = setChessBoard;
            game = setGame;
            color = setColor;
            type = PieceType.PAWN;
            symbol = (color == PieceColor.WHITE) ? 'P' : 'p';
            firstMove = true;
            pawnDoubleMoveTurn = -1;
            SetMoveDirection();
        }
        public override void GeneratePossibleMoves(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            if (chessBoard.IsFieldEmpty(pieceRow + moveDirection1, pieceColumn))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + moveDirection1, pieceColumn)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + moveDirection1, pieceColumn));

                if (chessBoard.IsFieldEmpty(pieceRow + (2 * moveDirection1), pieceColumn) && firstMove)
                {
                    if ((checkForChecks && chessBoard.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + (2 * moveDirection1), pieceColumn)) || !checkForChecks)
                        possibleMoves.Add(new Point(pieceRow + (2 * moveDirection1), pieceColumn));
                }
            }

            if (chessBoard.IsFieldPossibleToCapture(pieceRow + moveDirection1, pieceColumn + moveDirection1, color))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + moveDirection1, pieceColumn + moveDirection1)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + moveDirection1, pieceColumn + moveDirection1));
            }
            if (chessBoard.IsFieldPossibleToCapture(pieceRow + moveDirection1, pieceColumn + moveDirection2, color))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + moveDirection1, pieceColumn + moveDirection2)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + moveDirection1, pieceColumn + moveDirection2));
            }
            if (chessBoard.IsFieldEmpty(pieceRow + moveDirection1, pieceColumn + moveDirection1) && EnPassant(pieceRow, pieceColumn, moveDirection1))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + moveDirection1, pieceColumn + moveDirection1)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + moveDirection1, pieceColumn + moveDirection1));
            }
            if (chessBoard.IsFieldEmpty(pieceRow + moveDirection1, pieceColumn + moveDirection2) && EnPassant(pieceRow, pieceColumn, moveDirection2))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + moveDirection1, pieceColumn + moveDirection2)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + moveDirection1, pieceColumn + moveDirection2));
            }
        }
        private void SetMoveDirection()
        {
            if(color == PieceColor.WHITE)
            {
                moveDirection1 = -1;
                moveDirection2 = 1;
            }
            else if (color == PieceColor.BLACK)
            {
                moveDirection1 = 1;
                moveDirection2 = -1;
            }
        }
        private bool EnPassant(int pieceRow, int pieceColumn, int columnShift)
        {
            Piece piece = chessBoard.GetPieceFromField(pieceRow, pieceColumn + columnShift);
            if (piece.type == PieceType.PAWN && color != piece.color)
            {
                Pawn pawn = (Pawn)piece;
                if (pawn.pawnDoubleMoveTurn == game.movesCounter && color == PieceColor.WHITE)
                    return true;
                if (pawn.pawnDoubleMoveTurn == game.movesCounter - 1 && color == PieceColor.BLACK)
                    return true;
            }
            return false;
        }
        public void CheckPawnDoubleMove(int pieceRow, int fieldRow)
        {
            if (Math.Abs(pieceRow - fieldRow) == 2)
                pawnDoubleMoveTurn = game.movesCounter;
        }
    }
}
