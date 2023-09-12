using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Chess
{
    public partial class MainWindow : Window
    {
        private Button pressedButton;
        private Piece selectedPromotionPiece;
        private readonly ChessBoard chessBoard = new ChessBoard();
        private readonly Game game = new Game(GameState.IN_PROGRESS);
        private readonly MainWindow mainWindow;
        private bool buttonClicked = false;

        private bool promotionPieceSelected = false;
        private int enPassantStatus = 0;

        public bool whiteKingUnderCheck = false;
        public bool blackKingUnderCheck = false;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            game.window = mainWindow;
            game.chessBoard = chessBoard;
            chessBoard.window = mainWindow;
            chessBoard.game = game;

            chessBoard.Initialize();
            chessBoard.Print();
        }
        private void PieceSelected(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            string pieceType = button.Tag.ToString();

            if (!buttonClicked) // select piece you want to move
            {
                SelectPiece(button, pieceType);
            }
            else // choose where you want to move your piece
            {
                Button selectedField = e.Source as Button;
                SelectField(selectedField);
            }
        }
        private void TestMode(object sender, RoutedEventArgs e)
        {
            game.testMode = !game.testMode;
            if(game.testMode)
                TestModeButton.Content = "TestMode: ON";
            else
                TestModeButton.Content = "TestMode: OFF";
        }
        private void ChoosePromotionPiece(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            string pieceType = button.Name.ToString();

            selectedPromotionPiece = game.GetPieceFromPieceType(pieceType);
            promotionPieceSelected = true;
            int pieceRow = Grid.GetRow(pressedButton);
            int pieceColumn = Grid.GetColumn(pressedButton);

            PromotePawn(pieceRow, pieceColumn);
            ImageBrush brush = new ImageBrush { ImageSource = new BitmapImage(new Uri($"images/{pieceType}.png", UriKind.Relative)) };
            pressedButton.Background = brush;
            pressedButton.Tag = pieceType;
            chessBoard.board[pieceRow, pieceColumn] = selectedPromotionPiece;
            chessBoard.board[pieceRow, pieceColumn].firstMove = false;
            promotionPieceSelected = false;

            chessBoard.Print();
            game.gameHistory.Add(chessBoard.ChessboardToString());
            CheckForChecksAndGameOver();
        }
        private void SelectPiece(Button button, string pieceType)
        {
            if (!buttonClicked && game.GetPieceColorFromPieceType(pieceType) == PieceColor.BLACK && game.turn == PieceColor.WHITE && !game.testMode)
            {
                MessageBox.Show("It's white's turn!");
            }
            else if (!buttonClicked && game.GetPieceColorFromPieceType(pieceType) == PieceColor.WHITE && game.turn == PieceColor.BLACK && !game.testMode)
            {
                MessageBox.Show("It's black's turn!");
            }
            else if (!buttonClicked && button.Background != Brushes.Transparent)
            {
                buttonClicked = true;
                pressedButton = button;
                chessBoard.ChangeChessBoardOpacity(0.5);
                pressedButton.Opacity = 1;
                pressedButton.IsEnabled = true;

                int pieceRow = Grid.GetRow(pressedButton);
                int pieceColumn = Grid.GetColumn(pressedButton);

                List<Point> possibleMoves = new List<Point>();
                GeneratePossibleMovesForPiece(pieceRow, pieceColumn, possibleMoves, true);
                chessBoard.HighlightPossibleFields(possibleMoves);
            }
        }
        private void SelectField(Button selectedField)
        {
            int fieldRow = Grid.GetRow(selectedField);
            int fieldColumn = Grid.GetColumn(selectedField);

            int pieceRow = Grid.GetRow(pressedButton);
            int pieceColumn = Grid.GetColumn(pressedButton);

            if (fieldColumn == pieceColumn && fieldRow == pieceRow)
            {
                chessBoard.ChangeChessBoardOpacity(1);
                buttonClicked = false;
                return;
            }
            string pieceType = pressedButton.Tag.ToString();
            string fieldType = selectedField.Tag.ToString();

            List<Point> possibleMoves = new List<Point>();
            GeneratePossibleMovesForPiece(pieceRow, pieceColumn, possibleMoves, true);

            bool correctFieldSelected = false;
            for(int i = 0; i < possibleMoves.Count; i++)
            {
                if (chessBoard.ArePointsEqual(possibleMoves[i], fieldRow, fieldColumn) && !chessBoard.IsFieldAKing(fieldRow, fieldColumn))
                {
                    correctFieldSelected = true;
                    break;
                }
            }
            if (correctFieldSelected)
            {
                if (chessBoard.GetPieceTypeFromField(pieceRow, pieceColumn) == PieceType.PAWN)
                    (chessBoard.GetPieceFromField(pieceRow, pieceColumn) as Pawn).CheckPawnDoubleMove(pieceRow, fieldRow);

                UpdateEnPassantStatus(pieceType, fieldType, pieceRow, pieceColumn, fieldRow, fieldColumn);
                MovePieceToField(selectedField, pieceType, fieldType, fieldColumn, fieldRow, pieceColumn, pieceRow);

                if (!PromotePawn(fieldRow, fieldColumn))
                {
                    chessBoard.Print();
                    game.gameHistory.Add(chessBoard.ChessboardToString());
                }
                CheckForChecksAndGameOver();
            }
        }
        private void CheckForChecksAndGameOver()
        {
            CheckBothKingsForChecks();
            chessBoard.HighlightKingUnderCheck();
            game.CheckGameState(chessBoard.ChessboardToString());
            if (game.CheckIfGameOver())
            {
                foreach (Button field in gornaWarstwa.Children)
                    field.IsEnabled = false;
            }
        }
        public void PerformCastling(int rookRow, int rookColumn, int emptyFieldRow, int emptyFieldColumn)
        {
            Button rook = gornaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == rookRow && Grid.GetColumn(e) == rookColumn) as Button;

            Button empty = gornaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == emptyFieldRow && Grid.GetColumn(e) == emptyFieldColumn) as Button;

            PieceColor pieceColor = chessBoard.GetPieceColorFromField(rookRow, rookColumn);

            Grid.SetColumn(rook, emptyFieldColumn);
            Grid.SetRow(rook, emptyFieldRow);
            chessBoard.board[emptyFieldRow, emptyFieldColumn] = new Rook(mainWindow, chessBoard, game, pieceColor);

            Grid.SetColumn(empty, rookColumn);
            Grid.SetRow(empty, rookRow);
            chessBoard.board[rookRow, rookColumn] = new Empty(mainWindow, chessBoard, game, PieceColor.NONE);
        }
        private void DeletePawnEnPassant(int pieceColumn, int pieceRow)
        {
            int pawnToDeleteColumn = pieceColumn + enPassantStatus;

            Button pawnToDelete = gornaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == pieceRow && Grid.GetColumn(e) == pawnToDeleteColumn) as Button;

            pawnToDelete.Tag = "Empty";
            pawnToDelete.Background = Brushes.Transparent;

            Grid.SetColumn(pawnToDelete, pawnToDeleteColumn);
            Grid.SetRow(pawnToDelete, pieceRow);
            chessBoard.board[pieceRow, pawnToDeleteColumn] = new Empty(mainWindow, chessBoard, game, PieceColor.NONE);
        }
        private void MovePieceToField(Button selectedField, string pieceType, string fieldType, int fieldColumn, int fieldRow, int pieceColumn, int pieceRow)
        {
            Grid.SetColumn(pressedButton, fieldColumn); // move piece to field
            Grid.SetRow(pressedButton, fieldRow);
            chessBoard.board[fieldRow, fieldColumn] = chessBoard.board[pieceRow, pieceColumn];
            chessBoard.board[fieldRow, fieldColumn].firstMove = false;

            Grid.SetColumn(selectedField, pieceColumn); // set former piece field to empty
            Grid.SetRow(selectedField, pieceRow);
            chessBoard.board[pieceRow, pieceColumn] = new Empty(mainWindow, chessBoard, game, PieceColor.NONE);

            if (enPassantStatus != 0) // remove pawn captured en passant
                DeletePawnEnPassant(pieceColumn, pieceRow);

            chessBoard.CheckAllCastlings(pieceType, fieldColumn);

            if (fieldType != "Empty")
            {
                selectedField.Background = Brushes.Transparent;
                selectedField.Tag = "Empty";
            }
            chessBoard.ChangeChessBoardOpacity(1);
            game.ChangeTurn();
            buttonClicked = false;
        }
        private void GeneratePossibleMovesForPiece(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            Piece piece = chessBoard.GetPieceFromField(pieceRow, pieceColumn);
            piece.GeneratePossibleMoves(pieceRow, pieceColumn, possibleMoves, checkForChecks);
        }
        private void UpdateEnPassantStatus(string pieceType, string fieldType, int pieceRow, int pieceColumn, int fieldRow, int fieldColumn)
        {
            if (fieldType == "Empty" && pieceType.EndsWith("Pawn"))
            {
                if (pieceType.StartsWith("White"))
                {
                    if (fieldRow < pieceRow && fieldColumn < pieceColumn)
                        enPassantStatus = -1;
                    else if (fieldRow < pieceRow && fieldColumn > pieceColumn)
                        enPassantStatus = 1;
                    else
                        enPassantStatus = 0;
                }
                if (pieceType.StartsWith("Black"))
                {
                    if (fieldRow > pieceRow && fieldColumn < pieceColumn)
                        enPassantStatus = -1;
                    else if (fieldRow > pieceRow && fieldColumn > pieceColumn)
                        enPassantStatus = 1;
                    else
                        enPassantStatus = 0;
                }
            }
            else
                enPassantStatus = 0;
        }
        public List<Point> CheckForKingsDefence(PieceColor pieceColor)
        {
            List<Point> possibleMoves = new List<Point>();
            for (int i = 0; i < chessBoard.size; i++)
            {
                for (int j = 0; j < chessBoard.size; j++)
                {
                    if (!chessBoard.IsFieldEmpty(i, j) && chessBoard.GetPieceColorFromField(i, j) == pieceColor)
                    {
                        GeneratePossibleMovesForPiece(i, j, possibleMoves, true);
                    }
                }
            }
            return possibleMoves;
        }
        public bool CheckIfKingUnderCheck(PieceColor pieceColor)
        {
            List<Point> possibleMoves = new List<Point>();
            for (int i = 0; i < chessBoard.size; i++)
            {
                for (int j = 0; j < chessBoard.size; j++)
                {
                    if (!chessBoard.IsFieldEmpty(i, j) && chessBoard.GetPieceColorFromField(i, j) != pieceColor)
                    {
                        GeneratePossibleMovesForPiece(i, j, possibleMoves, false);
                        if(possibleMoves.Contains(chessBoard.GetKingPosition(pieceColor)))
                        {
                            return true;
                        }
                    } 
                }
            }
            return false;
        }
        private void CheckBothKingsForChecks()
        {
            blackKingUnderCheck = CheckIfKingUnderCheck(PieceColor.BLACK);
            whiteKingUnderCheck = CheckIfKingUnderCheck(PieceColor.WHITE);
        }
        public bool IsKingSafeAfterMove(int pieceRow, int pieceColumn, int fieldRow, int fieldColumn)
        {
            Piece piece = chessBoard.board[pieceRow, pieceColumn];
            if (piece.type != PieceType.EMPTY)
            {
                Piece field = chessBoard.board[fieldRow, fieldColumn];

                chessBoard.board[fieldRow, fieldColumn] = piece;
                chessBoard.board[pieceRow, pieceColumn] = new Empty(mainWindow, chessBoard, game, PieceColor.NONE);

                bool kingSafe = !CheckIfKingUnderCheck(piece.color);

                chessBoard.board[fieldRow, fieldColumn] = field;
                chessBoard.board[pieceRow, pieceColumn] = piece;

                return kingSafe;
            }
            return true;
        }
        private bool PromotePawn(int row, int column)
        {
            PieceType type = chessBoard.GetPieceTypeFromField(row, column);
            PieceColor color = chessBoard.GetPieceColorFromField(row, column);
            if (type == PieceType.PAWN)
            {
                if (color == PieceColor.WHITE && row == chessBoard.minimumIndex)
                {
                    if (!promotionPieceSelected)
                    {
                        SetWhitePromotionButtonsVisibility(Visibility.Visible);
                        chessBoard.ChangeChessBoardOpacity(0.5);
                        return true;
                    }
                    else
                    {
                        SetWhitePromotionButtonsVisibility(Visibility.Hidden);
                        chessBoard.ChangeChessBoardOpacity(1);
                        return true;
                    }
                }
                if (color == PieceColor.BLACK && row == chessBoard.maximumIndex)
                {
                    if (!promotionPieceSelected)
                    {
                        SetBlackPromotionButtonsVisibility(Visibility.Visible);
                        chessBoard.ChangeChessBoardOpacity(0.5);
                        return true;
                    }
                    else
                    {
                        SetBlackPromotionButtonsVisibility(Visibility.Hidden);
                        chessBoard.ChangeChessBoardOpacity(1);
                        return true;
                    }
                }
            }
            return false;
        }
        private void SetWhitePromotionButtonsVisibility(Visibility visibility)
        {
            PromoteWhiteBishop.Visibility = visibility;
            PromoteWhiteKnight.Visibility = visibility;
            PromoteWhiteQueen.Visibility = visibility;
            PromoteWhiteRook.Visibility = visibility;
        }
        private void SetBlackPromotionButtonsVisibility(Visibility visibility)
        {
            PromoteBlackBishop.Visibility = visibility;
            PromoteBlackKnight.Visibility = visibility;
            PromoteBlackQueen.Visibility = visibility;
            PromoteBlackRook.Visibility = visibility;
        }
    }
}
