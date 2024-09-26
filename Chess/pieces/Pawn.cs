using System;
using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Pawn : Piece
    {
        public int pawnDoubleMoveTurn;
        public int moveDirection1, moveDirection2;
        public Pawn(ChessBoard setChessBoard, PieceColor setColor) : base(setChessBoard, setColor)
        {
            chessBoard = setChessBoard;
            color = setColor;
            type = PieceType.PAWN;
            symbol = (color == PieceColor.WHITE) ? 'P' : 'p';
            firstMove = true;
            pawnDoubleMoveTurn = -1;
            SetMoveDirection();
        }
        public override void GeneratePossibleMoves(List<Point> possibleMoves, bool checkForChecks)
        {
            if (chessBoard.IsFieldEmpty(row + moveDirection1, column))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row + moveDirection1, column)) || !checkForChecks)
                    possibleMoves.Add(new Point(row + moveDirection1, column));

                if (chessBoard.IsFieldEmpty(row + (2 * moveDirection1), column) && firstMove)
                {
                    if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row + (2 * moveDirection1), column)) || !checkForChecks)
                        possibleMoves.Add(new Point(row + (2 * moveDirection1), column));
                }
            }

            if (chessBoard.IsFieldPossibleToCapture(row + moveDirection1, column + moveDirection1, color))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row + moveDirection1, column + moveDirection1)) || !checkForChecks)
                    possibleMoves.Add(new Point(row + moveDirection1, column + moveDirection1));
            }
            if (chessBoard.IsFieldPossibleToCapture(row + moveDirection1, column + moveDirection2, color))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row + moveDirection1, column + moveDirection2)) || !checkForChecks)
                    possibleMoves.Add(new Point(row + moveDirection1, column + moveDirection2));
            }
            if (chessBoard.IsFieldEmpty(row + moveDirection1, column + moveDirection1) && EnPassant(moveDirection1))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row + moveDirection1, column + moveDirection1)) || !checkForChecks)
                    possibleMoves.Add(new Point(row + moveDirection1, column + moveDirection1));
            }
            if (chessBoard.IsFieldEmpty(row + moveDirection1, column + moveDirection2) && EnPassant(moveDirection2))
            {
                if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row + moveDirection1, column + moveDirection2)) || !checkForChecks)
                    possibleMoves.Add(new Point(row + moveDirection1, column + moveDirection2));
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
        private bool EnPassant(int columnShift)
        {
            Piece piece = chessBoard.GetPieceFromField(row, column + columnShift);
            if (piece.type == PieceType.PAWN && color != piece.color)
            {
                Pawn pawn = (Pawn)piece;
                if (pawn.pawnDoubleMoveTurn == chessBoard.game.movesCounter && color == PieceColor.WHITE)
                    return true;
                if (pawn.pawnDoubleMoveTurn == chessBoard.game.movesCounter - 1 && color == PieceColor.BLACK)
                    return true;
            }
            return false;
        }
        public void CheckPawnDoubleMove(int fieldRow)
        {
            if (Math.Abs(row - fieldRow) == 2)
                pawnDoubleMoveTurn = chessBoard.game.movesCounter; // Maybe this can become a boolean value "pawnLastMoveWasDoubleMove"? It would let us remove Game from ChessBoard
        }
    }
}
