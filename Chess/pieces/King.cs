using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace Chess
{
    public class King : Piece
    {
        public King(ChessBoard setChessBoard, PieceColor setColor) : base(setChessBoard, setColor)
        {
            chessBoard = setChessBoard;
            color = setColor;
            type = PieceType.KING;
            symbol = (color == PieceColor.WHITE) ? 'K' : 'k';
            firstMove = true;
        }
        public override void GeneratePossibleMoves(List<Point> possibleMoves, bool checkForChecks)
        {
            if (chessBoard.CanPieceMoveHere(row, column - 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row, column - 1));
            if (chessBoard.CanPieceMoveHere(row - 1, column - 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row - 1, column - 1));
            if (chessBoard.CanPieceMoveHere(row + 1, column - 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row + 1, column - 1));

            if (chessBoard.CanPieceMoveHere(row, column + 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row, column + 1));
            if (chessBoard.CanPieceMoveHere(row - 1, column + 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row - 1, column + 1));
            if (chessBoard.CanPieceMoveHere(row + 1, column + 1, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row + 1, column + 1));

            if (chessBoard.CanPieceMoveHere(row + 1, column, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row + 1, column));
            if (chessBoard.CanPieceMoveHere(row - 1, column, color, checkForChecks, row, column))
                possibleMoves.Add(new Point(row - 1, column));

            if (color == PieceColor.WHITE)
            {
                if (CheckCastling(chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(row, column + 1) && chessBoard.IsFieldEmpty(row, column + 2))
                {
                    if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row, column + 2) && chessBoard.IsKingSafeAfterMove(row, column, row, column + 1)) || !checkForChecks)
                    {
                        if (!chessBoard.whiteKingUnderCheck)
                        {
                            possibleMoves.Add(new Point(row, column + 2));
                            chessBoard.whiteShortCastling = true;
                        }
                    }
                }
                if (CheckCastling(chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(row, column - 1) && chessBoard.IsFieldEmpty(row, column - 2) && chessBoard.IsFieldEmpty(row, column - 3))
                {
                    if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row, column - 2) && chessBoard.IsKingSafeAfterMove(row, column, row, column - 1)) || !checkForChecks)
                    {
                        if (!chessBoard.whiteKingUnderCheck)
                        {
                            possibleMoves.Add(new Point(row, column - 2));
                            chessBoard.whiteLongCastling = true;
                        }
                    }
                }
            }
            if (color == PieceColor.BLACK)
            {
                if (CheckCastling(chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(row, column + 1) && chessBoard.IsFieldEmpty(row, column + 2))
                {
                    if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row, column + 2) && chessBoard.IsKingSafeAfterMove(row, column, row, column + 1)) || !checkForChecks)
                    {
                        if (!chessBoard.blackKingUnderCheck)
                        {
                            possibleMoves.Add(new Point(row, column + 2));
                            chessBoard.blackShortCastling = true;
                        }
                    }
                }
                if (CheckCastling(chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(row, column - 1) && chessBoard.IsFieldEmpty(row, column - 2) && chessBoard.IsFieldEmpty(row, column - 3))
                {
                    if ((checkForChecks && chessBoard.IsKingSafeAfterMove(row, column, row, column - 2) && chessBoard.IsKingSafeAfterMove(row, column, row, column - 1)) || !checkForChecks)
                    {
                        if (!chessBoard.blackKingUnderCheck)
                        {
                            possibleMoves.Add(new Point(row, column - 2));
                            chessBoard.blackLongCastling = true;
                        }
                    }
                }
            }
        }
        private bool CheckCastling(int rookRow, int rookColumn)
        {
            if (firstMove && chessBoard.FirstMoveOfPiece(rookRow, rookColumn) && chessBoard.GetPieceTypeFromField(rookRow, rookColumn) == PieceType.ROOK)
                return true;
            return false;
        }
    }
}
