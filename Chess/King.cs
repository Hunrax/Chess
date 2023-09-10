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
            if (window.CanPieceMoveHere(pieceRow, pieceColumn - 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow, pieceColumn - 1));
            if (window.CanPieceMoveHere(pieceRow - 1, pieceColumn - 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn - 1));
            if (window.CanPieceMoveHere(pieceRow + 1, pieceColumn - 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn - 1));

            if (window.CanPieceMoveHere(pieceRow, pieceColumn + 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow, pieceColumn + 1));
            if (window.CanPieceMoveHere(pieceRow - 1, pieceColumn + 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn + 1));
            if (window.CanPieceMoveHere(pieceRow + 1, pieceColumn + 1, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn + 1));

            if (window.CanPieceMoveHere(pieceRow + 1, pieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn));
            if (window.CanPieceMoveHere(pieceRow - 1, pieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn));

            if (color == PieceColor.WHITE)
            {
                if (window.CheckCastling(pieceRow, pieceColumn, chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn + 1) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn + 2))
                {
                    if ((checkForChecks && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 2) && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 1)) || !checkForChecks)
                    {
                        //if(!CheckIfAnyKingUnderCheck("White"))
                        {
                            possibleMoves.Add(new Point(pieceRow, pieceColumn + 2));
                            window.whiteShortCastling = true;
                        }
                    }
                }
                if (window.CheckCastling(pieceRow, pieceColumn, chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 1) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 2) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 3))
                {
                    if ((checkForChecks && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 2) && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 1)) || !checkForChecks)
                    {
                        //if (!CheckIfAnyKingUnderCheck("White"))
                        {
                            possibleMoves.Add(new Point(pieceRow, pieceColumn - 2));
                            window.whiteLongCastling = true;
                        }
                    }
                }
            }
            if (color == PieceColor.BLACK)
            {
                if (window.CheckCastling(pieceRow, pieceColumn, chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn + 1) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn + 2))
                {
                    if ((checkForChecks && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 2) && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 1)) || !checkForChecks)
                    {
                        //if (!CheckIfAnyKingUnderCheck("Black"))
                        {
                            possibleMoves.Add(new Point(pieceRow, pieceColumn + 2));
                            window.blackShortCastling = true;
                        }
                    }
                }
                if (window.CheckCastling(pieceRow, pieceColumn, chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_ROOK_COLUMN) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 1) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 2) && chessBoard.IsFieldEmpty(pieceRow, pieceColumn - 3))
                {
                    if ((checkForChecks && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 2) && window.IsKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 1)) || !checkForChecks)
                    {
                        //if (!CheckIfAnyKingUnderCheck("Black"))
                        {
                            possibleMoves.Add(new Point(pieceRow, pieceColumn - 2));
                            window.blackLongCastling = true;
                        }
                    }
                }
            }
        }
    }
}
