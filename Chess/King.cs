using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class King : Piece
    {
        public King(MainWindow setWindow, ChessBoard setChessBoard, Game setGame, PieceColor setColor) : base(setWindow, setChessBoard, setGame, setColor)
        {
            window = setWindow;
            chessBoard = setChessBoard;
            game = setGame;
            color = setColor;
            type = PieceType.KING;
            symbol = 'K';
            firstMove = true;
        }
        public override void GeneratePossibleMoves(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            if (chessBoard.CanPieceMoveHere(pieceRow, pieceColumn - 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow, pieceColumn - 1));
            if (chessBoard.CanPieceMoveHere(pieceRow - 1, pieceColumn - 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn - 1));
            if (chessBoard.CanPieceMoveHere(pieceRow + 1, pieceColumn - 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn - 1));

            if (chessBoard.CanPieceMoveHere(pieceRow, pieceColumn + 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow, pieceColumn + 1));
            if (chessBoard.CanPieceMoveHere(pieceRow - 1, pieceColumn + 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn + 1));
            if (chessBoard.CanPieceMoveHere(pieceRow + 1, pieceColumn + 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn + 1));

            if (chessBoard.CanPieceMoveHere(pieceRow + 1, pieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn));
            if (chessBoard.CanPieceMoveHere(pieceRow - 1, pieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn));

            if (color == PieceColor.WHITE)
            {
                if (CheckCastling(chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn + 1) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn + 2))
                {
                    if ((checkForChecks && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 2) && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 1)) || !checkForChecks)
                    {
                        //if(!window.CheckIfKingUnderCheck(PieceColor.WHITE))
                        {
                            possibleMoves.Add(new Point(pieceRow, pieceColumn + 2));
                            chessBoard.whiteShortCastling = true;
                        }
                    }
                }
                if (CheckCastling(chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 1) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 2) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 3))
                {
                    if ((checkForChecks && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 2) && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 1)) || !checkForChecks)
                    {
                        //if (!window.CheckIfKingUnderCheck(PieceColor.WHITE))
                        {
                            possibleMoves.Add(new Point(pieceRow, pieceColumn - 2));
                            chessBoard.whiteLongCastling = true;
                        }
                    }
                }
            }
            if (color == PieceColor.BLACK)
            {
                if (CheckCastling(chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn + 1) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn + 2))
                {
                    if ((checkForChecks && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 2) && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 1)) || !checkForChecks)
                    {
                        //if (!window.CheckIfKingUnderCheck(PieceColor.BLACK))
                        {
                            possibleMoves.Add(new Point(pieceRow, pieceColumn + 2));
                            chessBoard.blackShortCastling = true;
                        }
                    }
                }
                if (CheckCastling(chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 1) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 2) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 3))
                {
                    if ((checkForChecks && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 2) && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 1)) || !checkForChecks)
                    {
                        //if (!window.CheckIfKingUnderCheck(PieceColor.BLACK))
                        {
                            possibleMoves.Add(new Point(pieceRow, pieceColumn - 2));
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
