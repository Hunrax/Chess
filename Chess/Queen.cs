﻿using System.Collections.Generic;
using System.Windows;

namespace Chess
{
    public class Queen : Piece
    {
        public Queen(MainWindow setWindow, ChessBoard setChessBoard, Game setGame, PieceColor setColor) : base(setWindow, setChessBoard, setGame, setColor)
        {
            window = setWindow;
            chessBoard = setChessBoard;
            game = setGame;
            color = setColor;
            type = PieceType.QUEEN;
            symbol = 'H';
        }
        public override void GeneratePossibleMoves(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            int newPieceRow = pieceRow;
            int newPieceColumn = pieceColumn;
            for (int i = pieceColumn + 1; i < chessBoard.size; i++)
            {
                newPieceColumn += 1;
                if (window.CanPieceMoveHere(pieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(pieceRow, newPieceColumn));
                if (!chessBoard.IsFieldEmpty(pieceRow, newPieceColumn))
                    break;
            }
            newPieceColumn = pieceColumn;
            for (int i = pieceColumn - 1; i >= chessBoard.minimumIndex; i--)
            {
                newPieceColumn -= 1;
                if (window.CanPieceMoveHere(pieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(pieceRow, newPieceColumn));
                if (!chessBoard.IsFieldEmpty(pieceRow, newPieceColumn))
                    break;
            }
            for (int i = pieceRow + 1; i < chessBoard.size; i++)
            {
                newPieceRow += 1;
                if (window.CanPieceMoveHere(newPieceRow, pieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, pieceColumn));
                if (!chessBoard.IsFieldEmpty(newPieceRow, pieceColumn))
                    break;
            }
            newPieceRow = pieceRow;
            for (int i = pieceRow - 1; i >= chessBoard.minimumIndex; i--)
            {
                newPieceRow -= 1;
                if (window.CanPieceMoveHere(newPieceRow, pieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, pieceColumn));
                if (!chessBoard.IsFieldEmpty(newPieceRow, pieceColumn))
                    break;
            }

            newPieceColumn = pieceColumn + 1;
            newPieceRow = pieceRow + 1;
            while (newPieceColumn < chessBoard.size && newPieceRow < chessBoard.size)
            {
                if (window.CanPieceMoveHere(newPieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
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
                if (window.CanPieceMoveHere(newPieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
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
                if (window.CanPieceMoveHere(newPieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
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
                if (window.CanPieceMoveHere(newPieceRow, newPieceColumn, color, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!chessBoard.IsFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn--;
                newPieceRow++;
            }
        }
    }
}