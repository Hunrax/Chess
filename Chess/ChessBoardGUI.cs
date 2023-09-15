using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;

namespace Chess
{
    public class ChessBoardGUI
    {
        public MainWindow window;
        public ChessBoard chessBoard;
        public void ChangeChessBoardOpacity(double opacity)
        {
            foreach (Grid field in window.dolnaWarstwa.Children)
            {
                field.Opacity = opacity;
            }
            foreach (Button field in window.gornaWarstwa.Children)
            {
                field.Opacity = opacity;
                if (field.Opacity < 1)
                    field.IsEnabled = false;
                else
                    field.IsEnabled = true;
            }
        }
        public void HighlightPossibleFields(List<Point> possibleMoves)
        {
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                Grid gridDolnaWarstwa = window.dolnaWarstwa.Children.Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == possibleMoves[i].X && Grid.GetColumn(e) == possibleMoves[i].Y) as Grid;

                Button gridGornaWarstwa = window.gornaWarstwa.Children.Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == Grid.GetRow(gridDolnaWarstwa) && Grid.GetColumn(e) == Grid.GetColumn(gridDolnaWarstwa)) as Button;

                if (!chessBoard.IsFieldAKing((int)possibleMoves[i].X, (int)possibleMoves[i].Y))
                {
                    gridDolnaWarstwa.Opacity = 1;
                    gridGornaWarstwa.IsEnabled = true;
                }
            }
        }
        public void HighlightKingUnderCheck()
        {
            int blackKingRow = (int)chessBoard.GetKingPosition(PieceColor.BLACK).X;
            int blackKingColumn = (int)chessBoard.GetKingPosition(PieceColor.BLACK).Y;

            int whiteKingRow = (int)chessBoard.GetKingPosition(PieceColor.WHITE).X;
            int whiteKingColumn = (int)chessBoard.GetKingPosition(PieceColor.WHITE).Y;

            Grid blackKing = window.dolnaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == blackKingRow && Grid.GetColumn(e) == blackKingColumn) as Grid;

            Grid whiteKing = window.dolnaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == whiteKingRow && Grid.GetColumn(e) == whiteKingColumn) as Grid;

            if (chessBoard.blackKingUnderCheck)
                blackKing.Background = Brushes.DarkRed;

            if (chessBoard.whiteKingUnderCheck)
                whiteKing.Background = Brushes.DarkRed;

            if (!chessBoard.blackKingUnderCheck)
                UnhighlightRedFields(PieceColor.BLACK);

            if (!chessBoard.whiteKingUnderCheck)
                UnhighlightRedFields(PieceColor.WHITE);
        }
        private void UnhighlightRedFields(PieceColor pieceColor)
        {
            for (int i = 0; i < chessBoard.size; i++)
            {
                for (int j = 0; j < chessBoard.size; j++)
                {
                    Grid field = window.dolnaWarstwa.Children.Cast<UIElement>()
                        .FirstOrDefault(e => Grid.GetRow(e) == i && Grid.GetColumn(e) == j) as Grid;

                    if (field.Background == Brushes.DarkRed && (chessBoard.GetPieceColorFromField(i, j) == pieceColor || chessBoard.GetPieceColorFromField(i, j) == PieceColor.NONE))
                    {
                        Grid fieldNeighbour;
                        if (i < chessBoard.maximumIndex)
                            fieldNeighbour = window.dolnaWarstwa.Children.Cast<UIElement>()
                                .FirstOrDefault(e => Grid.GetRow(e) == i + 1 && Grid.GetColumn(e) == j) as Grid;
                        else
                            fieldNeighbour = window.dolnaWarstwa.Children.Cast<UIElement>()
                                .FirstOrDefault(e => Grid.GetRow(e) == i - 1 && Grid.GetColumn(e) == j) as Grid;

                        if (fieldNeighbour.Background == Brushes.Wheat)
                        {
                            BrushConverter converter = new BrushConverter();
                            field.Background = (Brush)converter.ConvertFromString("#4E3524");
                        }
                        else
                            field.Background = Brushes.Wheat;
                    }
                }
            }
        }
        public void PerformCastling(int rookRow, int rookColumn, int emptyFieldRow, int emptyFieldColumn)
        {
            Piece emptyField = chessBoard.GetPieceFromField(emptyFieldRow, emptyFieldColumn);
            chessBoard.board[emptyFieldRow, emptyFieldColumn] = chessBoard.GetPieceFromField(rookRow, rookColumn);
            chessBoard.board[rookRow, rookColumn] = emptyField;
        }
        public void DeletePawnEnPassant(int pieceColumn, int pieceRow)
        {
            int pawnToDeleteColumn = pieceColumn + window.enPassantStatus;

            Button pawnToDelete = window.gornaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == pieceRow && Grid.GetColumn(e) == pawnToDeleteColumn) as Button;

            pawnToDelete.Tag = "Empty";
            pawnToDelete.Background = Brushes.Transparent;
            chessBoard.board[pieceRow, pawnToDeleteColumn] = new Empty(chessBoard, PieceColor.NONE);
        }
        public void MovePieceToField(Button selectedField, int fieldColumn, int fieldRow, int pieceColumn, int pieceRow)
        {
            chessBoard.board[fieldRow, fieldColumn] = chessBoard.board[pieceRow, pieceColumn];
            chessBoard.board[pieceRow, pieceColumn] = new Empty(chessBoard, PieceColor.NONE);

            if (window.enPassantStatus != 0) // remove pawn captured en passant
                DeletePawnEnPassant(pieceColumn, pieceRow);

            chessBoard.CheckAllCastlings(fieldRow, fieldColumn);
            chessBoard.board[fieldRow, fieldColumn].firstMove = false;

            if ((string)selectedField.Tag != "Empty")
            {
                selectedField.Background = Brushes.Transparent;
                selectedField.Tag = "Empty";
            }
            chessBoard.board[pieceRow, pieceColumn].button = selectedField;
            chessBoard.SetPiecesPositions();
            UpdateGUI();
            ChangeChessBoardOpacity(1);
            window.buttonClicked = false;
        }
        public bool PromotePawn(int row, int column)
        {
            PieceType type = chessBoard.GetPieceTypeFromField(row, column);
            PieceColor color = chessBoard.GetPieceColorFromField(row, column);
            if (type == PieceType.PAWN)
            {
                if (color == PieceColor.WHITE && row == chessBoard.minimumIndex)
                {
                    if (!window.promotionPieceSelected)
                    {
                        SetWhitePromotionButtonsVisibility(Visibility.Visible);
                        ChangeChessBoardOpacity(0.5);
                        return true;
                    }
                    else
                    {
                        SetWhitePromotionButtonsVisibility(Visibility.Hidden);
                        ChangeChessBoardOpacity(1);
                        return true;
                    }
                }
                if (color == PieceColor.BLACK && row == chessBoard.maximumIndex)
                {
                    if (!window.promotionPieceSelected)
                    {
                        SetBlackPromotionButtonsVisibility(Visibility.Visible);
                        ChangeChessBoardOpacity(0.5);
                        return true;
                    }
                    else
                    {
                        SetBlackPromotionButtonsVisibility(Visibility.Hidden);
                        ChangeChessBoardOpacity(1);
                        return true;
                    }
                }
            }
            return false;
        }
        private void SetWhitePromotionButtonsVisibility(Visibility visibility)
        {
            window.PromoteWhiteBishop.Visibility = visibility;
            window.PromoteWhiteKnight.Visibility = visibility;
            window.PromoteWhiteQueen.Visibility = visibility;
            window.PromoteWhiteRook.Visibility = visibility;
        }
        private void SetBlackPromotionButtonsVisibility(Visibility visibility)
        {
            window.PromoteBlackBishop.Visibility = visibility;
            window.PromoteBlackKnight.Visibility = visibility;
            window.PromoteBlackQueen.Visibility = visibility;
            window.PromoteBlackRook.Visibility = visibility;
        }
        public void ConnectPiecesAndButtons()
        {
            for (int i = 0; i < chessBoard.size; i++)
            {
                for (int j = 0; j < chessBoard.size; j++)
                {
                    Piece piece = chessBoard.GetPieceFromField(i, j);
                    Button pieceButton = window.gornaWarstwa.Children.Cast<UIElement>()
                        .FirstOrDefault(e => Grid.GetRow(e) == piece.row && Grid.GetColumn(e) == piece.column) as Button;
                    piece.button = pieceButton;
                }
            }
        }
        public void UpdateGUI()
        {
            for (int i = 0; i < chessBoard.size; i++)
            {
                for (int j = 0; j < chessBoard.size; j++)
                {
                    Piece piece = chessBoard.GetPieceFromField(i, j);
                    Grid.SetRow(piece.button, piece.row);
                    Grid.SetColumn(piece.button, piece.column);
                }
            }
        }
    }
}
