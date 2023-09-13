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
        public Button pressedButton;
        private Piece selectedPromotionPiece;
        private readonly ChessBoard chessBoard = new ChessBoard();
        private readonly ChessBoardGUI chessBoardGUI = new ChessBoardGUI();
        public readonly Game game = new Game(GameState.IN_PROGRESS);
        private readonly MainWindow mainWindow;
        public bool buttonClicked = false;

        public bool promotionPieceSelected = false;
        public int enPassantStatus = 0;

        public MainWindow()
        {
            InitializeComponent();
            mainWindow = this;
            game.window = mainWindow;
            game.chessBoard = chessBoard;
            chessBoardGUI.window = mainWindow;
            chessBoard.game = game;
            chessBoard.chessBoardGUI = chessBoardGUI;
            chessBoardGUI.chessBoard = chessBoard;

            chessBoard.Initialize();
            chessBoard.Print();
            game.DisablePieces();
        }
        private void PieceSelected(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;

            if (!buttonClicked) // select piece you want to move
            {
                SelectPiece(button);
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
            game.DisablePieces();
        }
        private void ChoosePromotionPiece(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            string pieceType = button.Name.ToString();

            selectedPromotionPiece = game.GetPieceFromPieceType(pieceType);
            promotionPieceSelected = true;
            int pieceRow = Grid.GetRow(pressedButton);
            int pieceColumn = Grid.GetColumn(pressedButton);

            chessBoardGUI.PromotePawn(pieceRow, pieceColumn);
            ImageBrush brush = new ImageBrush { ImageSource = new BitmapImage(new Uri($"images/{pieceType}.png", UriKind.Relative)) };
            pressedButton.Background = brush;
            pressedButton.Tag = pieceType;
            chessBoard.board[pieceRow, pieceColumn] = selectedPromotionPiece;
            chessBoard.board[pieceRow, pieceColumn].firstMove = false;
            promotionPieceSelected = false;

            chessBoard.Print();
            game.gameHistory.Add(chessBoard.ChessboardToString());
            game.DisablePieces();
            CheckForChecksAndGameOver();
        }
        private void SelectPiece(Button button)
        {
            buttonClicked = true;
            pressedButton = button;
            chessBoardGUI.ChangeChessBoardOpacity(0.5);
            pressedButton.Opacity = 1;
            pressedButton.IsEnabled = true;

            int pieceRow = Grid.GetRow(pressedButton);
            int pieceColumn = Grid.GetColumn(pressedButton);

            List<Point> possibleMoves = new List<Point>();
            chessBoard.GeneratePossibleMovesForPiece(pieceRow, pieceColumn, possibleMoves, true);
            chessBoardGUI.HighlightPossibleFields(possibleMoves);
        }
        private void SelectField(Button selectedField)
        {
            int fieldRow = Grid.GetRow(selectedField);
            int fieldColumn = Grid.GetColumn(selectedField);

            int pieceRow = Grid.GetRow(pressedButton);
            int pieceColumn = Grid.GetColumn(pressedButton);

            if (fieldColumn == pieceColumn && fieldRow == pieceRow)
            {
                chessBoardGUI.ChangeChessBoardOpacity(1);
                buttonClicked = false;
                game.DisablePieces();
                return;
            }
            string pieceType = pressedButton.Tag.ToString();
            string fieldType = selectedField.Tag.ToString();

            List<Point> possibleMoves = new List<Point>();
            chessBoard.GeneratePossibleMovesForPiece(pieceRow, pieceColumn, possibleMoves, true);

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
                chessBoardGUI.MovePieceToField(selectedField, pieceType, fieldType, fieldColumn, fieldRow, pieceColumn, pieceRow);

                if (!chessBoardGUI.PromotePawn(fieldRow, fieldColumn))
                {
                    chessBoard.Print();
                    game.gameHistory.Add(chessBoard.ChessboardToString());
                    CheckForChecksAndGameOver();
                }
                game.ChangeTurn();
                game.DisablePieces();
            }
        }
        private void CheckForChecksAndGameOver()
        {
            chessBoard.CheckBothKingsForChecks();
            chessBoardGUI.HighlightKingUnderCheck();
            game.CheckGameState(chessBoard.ChessboardToString());
            if (game.CheckIfGameOver())
            {
                foreach (Button field in gornaWarstwa.Children)
                    field.IsEnabled = false;
            }
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
    }
}
