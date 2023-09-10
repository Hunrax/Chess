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

        public bool whiteShortCastling = false;
        public bool whiteLongCastling = false;
        public bool blackShortCastling = false;
        public bool blackLongCastling = false;

        public bool whiteKingUnderCheck = false;
        public bool blackKingUnderCheck = false;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            game.window = mainWindow;

            chessBoard.Clear(mainWindow, game);
            chessBoard.Initialize(mainWindow, game);
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

            selectedPromotionPiece = GetPieceFromPieceType(pieceType);
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
        }
        private void SelectPiece(Button button, string pieceType)
        {
            if (!buttonClicked && GetPieceColorFromPieceType(pieceType) == PieceColor.BLACK && game.turn == PieceColor.WHITE && !game.testMode)
            {
                MessageBox.Show("It's white's turn!");
            }
            else if (!buttonClicked && GetPieceColorFromPieceType(pieceType) == PieceColor.WHITE && game.turn == PieceColor.BLACK && !game.testMode)
            {
                MessageBox.Show("It's black's turn!");
            }
            else if (!buttonClicked && button.Background != Brushes.Transparent)
            {
                buttonClicked = true;
                pressedButton = button;
                ChangeChessBoardOpacity(0.5);
                pressedButton.Opacity = 1;
                pressedButton.IsEnabled = true;

                int pieceRow = Grid.GetRow(pressedButton);
                int pieceColumn = Grid.GetColumn(pressedButton);

                List<Point> possibleMoves = new List<Point>();
                GeneratePossibleMovesForPiece(pieceRow, pieceColumn, possibleMoves, true);
                HighlightPossibleFields(possibleMoves);
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
                ChangeChessBoardOpacity(1);
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
                PromotePawn(fieldRow, fieldColumn);
                chessBoard.Print();
                CheckIfAnyKingUnderCheck(game.turn);
                HighlightKingUnderCheck();
                game.CheckGameState();
                if (game.CheckIfGameOver())
                {
                    foreach (Button field in gornaWarstwa.Children)
                        field.IsEnabled = false;
                }
            }
        }
        private void PerformCastling(int rookRow, int rookColumn, int emptyFieldRow, int emptyFieldColumn)
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
        private void CheckAllCastlings(string pieceType, int fieldColumn)
        {
            if (whiteShortCastling && pieceType == "WhiteKing" && fieldColumn == chessBoard.SHORT_CASTLING_KING_COLUMN)
            {
                PerformCastling(chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_ROOK_COLUMN, chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_EMPTYFIELD_COLUMN);
                whiteShortCastling = false;
            }
            if (whiteLongCastling && pieceType == "WhiteKing" && fieldColumn == chessBoard.LONG_CASTLING_KING_COLUMN)
            {
                PerformCastling(chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_ROOK_COLUMN, chessBoard.WHITE_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_EMPTYFIELD_COLUMN);
                whiteLongCastling = false;
            }

            if (blackShortCastling && pieceType == "BlackKing" && fieldColumn == chessBoard.SHORT_CASTLING_KING_COLUMN)
            {
                PerformCastling(chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_ROOK_COLUMN, chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.SHORT_CASTLING_EMPTYFIELD_COLUMN);
                blackShortCastling = false;
            }
            if (blackLongCastling && pieceType == "BlackKing" && fieldColumn == chessBoard.LONG_CASTLING_KING_COLUMN)
            {
                PerformCastling(chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_ROOK_COLUMN, chessBoard.BLACK_CASTLING_ROOK_ROW, chessBoard.LONG_CASTLING_EMPTYFIELD_COLUMN);
                blackLongCastling = false;
            }
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

            CheckAllCastlings(pieceType, fieldColumn);

            if (fieldType != "Empty")
            {
                selectedField.Background = Brushes.Transparent;
                selectedField.Tag = "Empty";
            }
            ChangeChessBoardOpacity(1);
            game.ChangeTurn();
            buttonClicked = false;
        }
        private void ChangeChessBoardOpacity(double opacity)
        {
            foreach (Grid field in dolnaWarstwa.Children)
            {
                field.Opacity = opacity;
            }
            foreach (Button field in gornaWarstwa.Children)
            {
                field.Opacity = opacity;
                if (field.Opacity < 1)
                    field.IsEnabled = false;
                else
                    field.IsEnabled = true;
            }
        }
        private void GeneratePossibleMovesForPiece(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            Piece piece = chessBoard.GetPieceFromField(pieceRow, pieceColumn);
            piece.GeneratePossibleMoves(pieceRow, pieceColumn, possibleMoves, checkForChecks);
        }
        private void HighlightPossibleFields(List<Point> possibleMoves)
        {
            for (int i = 0; i < possibleMoves.Count; i++)
            {
                Grid gridDolnaWarstwa = dolnaWarstwa.Children.Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == possibleMoves[i].X && Grid.GetColumn(e) == possibleMoves[i].Y) as Grid;

                Button gridGornaWarstwa = gornaWarstwa.Children.Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == Grid.GetRow(gridDolnaWarstwa) && Grid.GetColumn(e) == Grid.GetColumn(gridDolnaWarstwa)) as Button;

                if (!chessBoard.IsFieldAKing((int)possibleMoves[i].X, (int)possibleMoves[i].Y))
                {
                    gridDolnaWarstwa.Opacity = 1;
                    gridGornaWarstwa.IsEnabled = true;
                }
            }
        }
        private Piece GetPieceFromPieceType(string pieceType)
        {
            switch (pieceType)
            {
                case "WhitePawn":
                    return new Pawn(mainWindow, chessBoard, game, PieceColor.WHITE);
                case "BlackPawn":
                    return new Pawn(mainWindow, chessBoard, game, PieceColor.BLACK);
                case "WhiteRook":
                    return new Rook(mainWindow, chessBoard, game, PieceColor.WHITE);
                case "BlackRook":
                    return new Rook(mainWindow, chessBoard, game, PieceColor.BLACK);
                case "WhiteKnight":
                    return new Knight(mainWindow, chessBoard, game, PieceColor.WHITE);
                case "BlackKnight":
                    return new Knight(mainWindow, chessBoard, game, PieceColor.BLACK);
                case "WhiteBishop":
                    return new Bishop(mainWindow, chessBoard, game, PieceColor.WHITE);
                case "BlackBishop":
                    return new Bishop(mainWindow, chessBoard, game, PieceColor.BLACK);
                case "WhiteQueen":
                    return new Queen(mainWindow, chessBoard, game, PieceColor.WHITE);
                case "BlackQueen":
                    return new Queen(mainWindow, chessBoard, game, PieceColor.BLACK);
                case "WhiteKing":
                    return new King(mainWindow, chessBoard, game, PieceColor.WHITE);
                case "BlackKing":
                    return new King(mainWindow, chessBoard, game, PieceColor.BLACK);
                default:
                    return new Empty(mainWindow, chessBoard, game, PieceColor.NONE);
            }
        }
        public PieceColor GetPieceColorFromPieceType(string pieceType)
        {
            if (pieceType.StartsWith("White"))
                return PieceColor.WHITE;
            else if (pieceType.StartsWith("Black"))
                return PieceColor.BLACK;
            return PieceColor.NONE;
        }
        public bool CanPieceMoveHere(int fieldRow, int fieldColumn, PieceColor pieceColor, bool checkForChecks, int pieceRow, int pieceColumn)
        {
            if (chessBoard.IsFieldEmpty(fieldRow, fieldColumn) || chessBoard.IsFieldPossibleToCapture(fieldRow, fieldColumn, pieceColor))
            {
                if ((checkForChecks && IsKingSafeAfterMove(pieceRow, pieceColumn, fieldRow, fieldColumn)) || !checkForChecks)
                    return true;
                return false;
            }
            return false;
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
        private List<Point> GenerateAllPossibleMoves(PieceColor pieceColor)
        {
            List<Point> possibleMoves = new List<Point>();
            for (int i = 0; i < chessBoard.size; i++)
            {
                for (int j = 0; j < chessBoard.size; j++)
                {
                    if (!chessBoard.IsFieldEmpty(i, j) && chessBoard.GetPieceColorFromField(i, j) == pieceColor)
                    {
                        GeneratePossibleMovesForPiece(i, j, possibleMoves, false);
                    }
                }
            }
            return possibleMoves;
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
        public bool CheckIfAnyKingUnderCheck(PieceColor pieceColor)
        {
            if (pieceColor == PieceColor.WHITE)
                pieceColor = PieceColor.BLACK;
            else if (pieceColor == PieceColor.BLACK)
                pieceColor = PieceColor.WHITE;

            List<Point> possibleMoves = GenerateAllPossibleMoves(pieceColor);
            foreach (Point possibleMove in possibleMoves)
            {
                int row = (int)possibleMove.X;
                int column = (int)possibleMove.Y;
                if (chessBoard.IsFieldAKing(row, column) && pieceColor == PieceColor.WHITE)
                {
                    blackKingUnderCheck = true;
                    return true;
                }
                if (chessBoard.IsFieldAKing(row, column) && pieceColor == PieceColor.BLACK)
                {
                    whiteKingUnderCheck = true;
                    return true;
                }
            }
            whiteKingUnderCheck = false;
            blackKingUnderCheck = false;
            return false;
        }
        public bool IsKingSafeAfterMove(int pieceRow, int pieceColumn, int fieldRow, int fieldColumn)
        {
            Piece piece = chessBoard.board[pieceRow, pieceColumn];
            if (piece.type != PieceType.EMPTY)
            {
                Piece field = chessBoard.board[fieldRow, fieldColumn];

                chessBoard.board[fieldRow, fieldColumn] = piece;
                chessBoard.board[pieceRow, pieceColumn] = new Empty(mainWindow, chessBoard, game, PieceColor.NONE);

                bool kingSafe = !CheckIfAnyKingUnderCheck(piece.color);

                chessBoard.board[fieldRow, fieldColumn] = field;
                chessBoard.board[pieceRow, pieceColumn] = piece;

                return kingSafe;
            }
            return true;
        }
        private void HighlightKingUnderCheck()
        {
            int blackKingRow = (int)GetKingPosition(PieceColor.BLACK).X;
            int blackKingColumn = (int)GetKingPosition(PieceColor.BLACK).Y;

            int whiteKingRow = (int)GetKingPosition(PieceColor.WHITE).X;
            int whiteKingColumn = (int)GetKingPosition(PieceColor.WHITE).Y;

            Grid blackKing = dolnaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == blackKingRow && Grid.GetColumn(e) == blackKingColumn) as Grid;

            Grid whiteKing = dolnaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == whiteKingRow && Grid.GetColumn(e) == whiteKingColumn) as Grid;

            if (blackKingUnderCheck)
                blackKing.Background = Brushes.DarkRed;

            if (whiteKingUnderCheck)
                whiteKing.Background = Brushes.DarkRed;

            else if (!whiteKingUnderCheck && !blackKingUnderCheck)
                UnhighlightRedFields();
        }
        private void UnhighlightRedFields()
        {
            for (int i = 0; i < chessBoard.size; i++)
            {
                for (int j = 0; j < chessBoard.size; j++)
                {
                    Grid field = dolnaWarstwa.Children.Cast<UIElement>()
                        .FirstOrDefault(e => Grid.GetRow(e) == i && Grid.GetColumn(e) == j) as Grid;

                    if(field.Background == Brushes.DarkRed)
                    {
                        Grid fieldNeighbour;
                        if (i < chessBoard.maximumIndex)
                            fieldNeighbour = dolnaWarstwa.Children.Cast<UIElement>()
                                .FirstOrDefault(e => Grid.GetRow(e) == i + 1 && Grid.GetColumn(e) == j) as Grid;
                        else
                            fieldNeighbour = dolnaWarstwa.Children.Cast<UIElement>()
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
        private Point GetKingPosition(PieceColor kingColor)
        {
            for (int i = 0; i < chessBoard.size; i++)
            {
                for (int j = 0; j < chessBoard.size; j++)
                {
                    if (chessBoard.IsFieldAKing(i, j) && chessBoard.GetPieceColorFromField(i, j) == kingColor)
                        return new Point(i, j);
                }
            }
            return new Point(-1, -1);
        }
        public bool CheckCastling(int kingRow, int kingColumn, int rookRow, int rookColumn)
        {
            if (chessBoard.FirstMoveOfPiece(kingRow, kingColumn) && chessBoard.GetPieceTypeFromField(kingRow, kingColumn) == PieceType.KING && chessBoard.FirstMoveOfPiece(rookRow, rookColumn) && chessBoard.GetPieceTypeFromField(rookRow, rookColumn) == PieceType.ROOK)
                return true;
            return false;
        }
        private void PromotePawn(int row, int column)
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
                        ChangeChessBoardOpacity(0.5);
                    }
                    else
                    {
                        SetWhitePromotionButtonsVisibility(Visibility.Hidden);
                        ChangeChessBoardOpacity(1);
                    }
                }
                if (color == PieceColor.BLACK && row == chessBoard.maximumIndex)
                {
                    if (!promotionPieceSelected)
                    {
                        SetBlackPromotionButtonsVisibility(Visibility.Visible);
                        ChangeChessBoardOpacity(0.5);
                    }
                    else
                    {
                        SetBlackPromotionButtonsVisibility(Visibility.Hidden);
                        ChangeChessBoardOpacity(1);
                    }
                }
            }
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
