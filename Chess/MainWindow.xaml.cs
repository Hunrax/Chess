using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chess
{
    public partial class MainWindow : Window
    {
        bool buttonClicked = false;
        Button pressedButton;
        Piece selectedPromotionPiece;
        bool promotionPieceSelected = false;
        string turn = "White";
        int movesCounter = 0;
        ChessBoard chessBoard = new ChessBoard();
        bool testMode = false;

        bool whiteShortCastling = false;
        bool whiteLongCastling = false;
        bool blackShortCastling = false;
        bool blackLongCastling = false;
        bool whiteKingUnderCheck = false;
        bool blackKingUnderCheck = false;

        ArrayList whitePiecesWithKing = new ArrayList { "WhitePawn", "WhiteKnight", "WhiteBishop", "WhiteRook", "WhiteQueen", "WhiteKing" };
        ArrayList blackPiecesWithKing = new ArrayList { "BlackPawn", "BlackKnight", "BlackBishop", "BlackRook", "BlackQueen", "BlackKing" };
        public MainWindow()
        {
            InitializeComponent();
            chessBoard.Clear();
            chessBoard.Initialize();
            chessBoard.Print();
        }
        void PieceSelected(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            string piece = button.Tag.ToString();

            if (!buttonClicked) // select piece you want to move
            {
                selectPiece(button, piece);
            }
            else // choose where you want to move your piece
            {
                Button selectedField = e.Source as Button;
                selectField(selectedField);
            }
        }
        void TestMode(object sender, RoutedEventArgs e)
        {
            testMode = !testMode;
            if(testMode)
                TestModeButton.Content = "TestMode: ON";
            else
                TestModeButton.Content = "TestMode: OFF";
        }
        void ChoosePromotionPiece(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            string tag = button.Tag.ToString();
            string pieceType = button.Name.ToString();

            selectedPromotionPiece = getPieceFromPieceType(pieceType);
            promotionPieceSelected = true;
                int pieceRow = Grid.GetRow(pressedButton);
                int pieceColumn = Grid.GetColumn(pressedButton);

            promotePawn(pieceRow, pieceColumn);
            var brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri($"C:/Users/user/Desktop/Chess/Chess/images/{pieceType}.png", UriKind.Relative));
            pressedButton.Background = brush;
            pressedButton.Tag = selectedPromotionPiece.color + selectedPromotionPiece.type;
            chessBoard.board[pieceRow, pieceColumn] = selectedPromotionPiece;
            chessBoard.board[pieceRow, pieceColumn].firstMove = false;
            promotionPieceSelected = false;
        }
        void selectPiece(Button button, string piece)
        {
            if (!buttonClicked && blackPiecesWithKing.Contains(piece) && turn == "White" && !testMode)
            {
                MessageBox.Show("It's white's turn!");
            }
            else if (!buttonClicked && whitePiecesWithKing.Contains(piece) && turn == "Black" && !testMode)
            {
                MessageBox.Show("It's black's turn!");
            }
            else if (!buttonClicked && button.Background != Brushes.Transparent)
            {
                buttonClicked = true;
                pressedButton = button;

                changeChessBoardOpacity(0.5);
                pressedButton.Opacity = 1;
                pressedButton.IsEnabled = true;

                string pieceType = pressedButton.Tag.ToString();
                int pieceRow = Grid.GetRow(pressedButton);
                int pieceColumn = Grid.GetColumn(pressedButton);

                List<Point> possibleMoves = new List<Point>();
                generatePossibleMoves(pieceType, pieceRow, pieceColumn, possibleMoves);
                highlightPossibleFields(possibleMoves);
            }
        }
        void selectField(Button selectedField)
        {
            int fieldRow = Grid.GetRow(selectedField);
            int fieldColumn = Grid.GetColumn(selectedField);

            int pieceRow = Grid.GetRow(pressedButton);
            int pieceColumn = Grid.GetColumn(pressedButton);

            if (fieldColumn == pieceColumn && fieldRow == pieceRow)
            {
                changeChessBoardOpacity(1);
                buttonClicked = false;
                return;
            }
            string pieceType = pressedButton.Tag.ToString();
            string fieldType = selectedField.Tag.ToString();

            List<Point> possibleMoves = new List<Point>();
            generatePossibleMoves(pieceType, pieceRow, pieceColumn, possibleMoves);

            bool correctFieldSelected = false;
            for(int i = 0; i < possibleMoves.Count; i++)
            {
                if (arePointsEqual(possibleMoves[i], fieldRow, fieldColumn) && !isFieldAKing(fieldRow, fieldColumn))
                {
                    correctFieldSelected = true;
                    break;
                }
            }
            if (correctFieldSelected)
            {
                movePieceToField(selectedField, pieceType, fieldType, fieldColumn, fieldRow, pieceColumn, pieceRow);
                promotePawn(fieldRow, fieldColumn);
                chessBoard.Print();
                checkIfAnyKingUnderCheck(chessBoard.board);
            }
        }
        void performCastling(int rookRow, int rookColumn, int emptyFieldRow, int emptyFieldColumn)
        {
            Button rook = gornaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == rookRow && Grid.GetColumn(e) == rookColumn) as Button;

            Button empty = gornaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == emptyFieldRow && Grid.GetColumn(e) == emptyFieldColumn) as Button;

            Grid.SetColumn(rook, emptyFieldColumn);
            Grid.SetRow(rook, emptyFieldRow);
            chessBoard.board[emptyFieldRow, emptyFieldColumn] = new Piece("White", "Rook", 'W');

            Grid.SetColumn(empty, rookColumn);
            Grid.SetRow(empty, rookRow);
            chessBoard.board[rookRow, rookColumn] = new Piece("None", "Empty", '.');
        }
        void movePieceToField(Button selectedField, string pieceType, string fieldType, int fieldColumn, int fieldRow, int pieceColumn, int pieceRow)
        {
            Grid.SetColumn(pressedButton, fieldColumn); // move piece to field
            Grid.SetRow(pressedButton, fieldRow);
            chessBoard.board[fieldRow, fieldColumn] = getPieceFromPieceType(pieceType);
            chessBoard.board[fieldRow, fieldColumn].firstMove = false;

            Grid.SetColumn(selectedField, pieceColumn); // set former piece field to empty
            Grid.SetRow(selectedField, pieceRow);
            chessBoard.board[pieceRow, pieceColumn] = new Piece("None", "Empty", '.');

            if (whiteShortCastling && pieceType == "WhiteKing" && fieldColumn == 6)
            {
                performCastling(7, 7, 7, 5);
                whiteShortCastling = false;
            }
            if (whiteLongCastling && pieceType == "WhiteKing" && fieldColumn == 2)
            {
                performCastling(7, 0, 7, 3);
                whiteLongCastling = false;
            }

            if (blackShortCastling && pieceType == "BlackKing" && fieldColumn == 6)
            {
                performCastling(0, 7, 0, 5);
                blackShortCastling = false;
            }
            if (blackLongCastling && pieceType == "BlackKing" && fieldColumn == 2)
            {
                performCastling(0, 0, 0, 3);
                blackLongCastling = false;
            }

            if (fieldType != "Empty")
            {
                selectedField.Background = Brushes.Transparent;
                selectedField.Tag = "Empty";
            }
            changeChessBoardOpacity(1);
            changeTurn();
            buttonClicked = false;
        }
        void changeTurn()
        {
            if (turn == "White")
            {
                movesCounter++;
                turn = "Black";
                turnTextBox.Background = Brushes.Black;
                turnTextBox.Foreground = Brushes.White;
            }
            else
            {
                turn = "White";
                turnTextBox.Background = Brushes.White;
                turnTextBox.Foreground = Brushes.Black;
            }
            movesCounterTextBox.Text = movesCounter.ToString();
        }
        void changeChessBoardOpacity(double opacity)
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
        void generatePossibleMoves(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
        {
            switch (pieceType)
            {
                case "WhitePawn":
                    generateMovesPawn(pieceType, pieceRow, pieceColumn, possibleMoves, -1, 1);
                    break;

                case "BlackPawn":
                    generateMovesPawn(pieceType, pieceRow, pieceColumn, possibleMoves, 1, -1);
                    break;

                case "WhiteRook":
                case "BlackRook":
                    generateMovesRook(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;

                case "WhiteKnight":
                case "BlackKnight":
                    generateMovesKnight(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;

                case "WhiteBishop":
                case "BlackBishop":
                    generateMovesBishop(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;

                case "WhiteQueen":
                case "BlackQueen":
                    generateMovesQueen(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;

                case "WhiteKing":
                case "BlackKing":
                    generateMovesKing(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;
            }
        }
        void highlightPossibleFields(List<Point> possibleMoves)
        {
            for(int i = 0; i < possibleMoves.Count; i++)
            {
                Grid gridDolnaWarstwa = dolnaWarstwa.Children.Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == possibleMoves[i].X && Grid.GetColumn(e) == possibleMoves[i].Y) as Grid;

                Button gridGornaWarstwa = gornaWarstwa.Children.Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == Grid.GetRow(gridDolnaWarstwa) && Grid.GetColumn(e) == Grid.GetColumn(gridDolnaWarstwa)) as Button;

                if (!isFieldAKing((int)possibleMoves[i].X, (int)possibleMoves[i].Y))
                {
                    gridDolnaWarstwa.Opacity = 1;
                    gridGornaWarstwa.IsEnabled = true;
                }
            }
        }
        Piece getPieceFromPieceType(string pieceType)
        {
            switch(pieceType)
            {
                case "WhitePawn":
                    return new Piece("White", "Pawn", 'P');
                case "BlackPawn":
                    return new Piece("Black", "Pawn", 'P');
                case "WhiteRook":
                    return new Piece("White", "Rook", 'W');
                case "BlackRook":
                    return new Piece("Black", "Rook", 'W');
                case "WhiteKnight":
                    return new Piece("White", "Knight", 'S');
                case "BlackKnight":
                    return new Piece("Black", "Knight", 'S');
                case "WhiteBishop":
                    return new Piece("White", "Bishop", 'G');
                case "BlackBishop":
                    return new Piece("Black", "Bishop", 'G');
                case "WhiteQueen":
                    return new Piece("White", "Queen", 'H');
                case "BlackQueen":
                    return new Piece("Black", "Queen", 'H');
                case "WhiteKing":
                    return new Piece("White", "King", 'K');
                case "BlackKing":
                    return new Piece("Black", "King", 'K');
                default:
                    return new Piece("None", "Empty", '.');
            }
        }
        string getPieceColorFromPieceType(string pieceType)
        {
            if (pieceType.StartsWith("White"))
                return "White";
            else if(pieceType.StartsWith("Black"))
                return "Black";
            return "None";
        }
        string GetPieceTypeFromField(int row, int column)
        {
            return chessBoard.board[row, column].type;
        }
        string getPieceColorFromField(int row, int column)
        {
            return chessBoard.board[row, column].color;
        }
        bool arePointsEqual(Point point, int x, int y)
        {
            if (point.X == x && point.Y == y)
                return true;
            return false;
        }
        bool isFieldEmpty(int row, int column)
        {
            if (row < 0 || row > 7 || column < 0 || column > 7)
                return false;
            if (chessBoard.board[row, column].type == "Empty")
                return true;
            return false;
        }
        bool isFieldPossibleToCapture(int row, int column, string pieceColor)
        {
            if (row < 0 || row > 7 || column < 0 || column > 7)
                return false;
            if (chessBoard.board[row, column].type != "Empty" && pieceColor != chessBoard.board[row, column].color)
                return true;
            return false;
        }
        bool canPieceMoveHere(int fieldRow, int fieldColumn, string pieceColor)
        {
            if (isFieldEmpty(fieldRow, fieldColumn) || isFieldPossibleToCapture(fieldRow, fieldColumn, pieceColor))
                return true;
            return false;
        }
        bool isFieldAKing(int row, int column)
        {
            if (chessBoard.board[row, column].type == "King")
                return true;
            return false;
        }
        bool firstMoveOfPiece(int pieceRow, int pieceColumn)
        {
            return chessBoard.board[pieceRow, pieceColumn].firstMove;
        }
        void generateMovesPawn(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves, int value1, int value2)
        {
            if(isFieldEmpty(pieceRow + value1, pieceColumn) /*&& isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + value1, pieceColumn)*/)
                possibleMoves.Add(new Point(pieceRow + value1, pieceColumn));
            if(isFieldEmpty(pieceRow + (2 * value1), pieceColumn) && firstMoveOfPiece(pieceRow, pieceColumn)/* && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + (2 * value1), pieceColumn)*/)
                possibleMoves.Add(new Point(pieceRow + (2 * value1), pieceColumn));

            if(isFieldPossibleToCapture(pieceRow + value1, pieceColumn + value1, getPieceColorFromPieceType(pieceType))/* && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + value1, pieceColumn + value1)*/)
                possibleMoves.Add(new Point(pieceRow + value1, pieceColumn + value1));
            if (isFieldPossibleToCapture(pieceRow + value1, pieceColumn + value2, getPieceColorFromPieceType(pieceType))/* && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + value1, pieceColumn + value2)*/)
                possibleMoves.Add(new Point(pieceRow + value1, pieceColumn + value2));
        }
        void generateMovesKnight(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
        {
            if(canPieceMoveHere(pieceRow - 1, pieceColumn - 2, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn - 2));
            if(canPieceMoveHere(pieceRow + 1, pieceColumn - 2, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn - 2));

            if (canPieceMoveHere(pieceRow - 1, pieceColumn + 2, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn + 2));
            if (canPieceMoveHere(pieceRow + 1, pieceColumn + 2, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn + 2));

            if (canPieceMoveHere(pieceRow - 2, pieceColumn + 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow - 2, pieceColumn + 1));
            if (canPieceMoveHere(pieceRow + 2, pieceColumn + 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow + 2, pieceColumn + 1));

            if (canPieceMoveHere(pieceRow - 2, pieceColumn - 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow - 2, pieceColumn - 1));
            if (canPieceMoveHere(pieceRow + 2, pieceColumn - 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow + 2, pieceColumn - 1));
        }
        void generateMovesBishop(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
        {
            int newPieceColumn = pieceColumn + 1;
            int newPieceRow = pieceRow + 1;
            while (newPieceColumn < 8 && newPieceRow < 8)
            {
                if(canPieceMoveHere(newPieceRow, newPieceColumn, getPieceColorFromPieceType(pieceType)))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!isFieldEmpty(newPieceRow, newPieceColumn)) 
                    break;
                newPieceColumn++;
                newPieceRow++;
            }
            newPieceColumn = pieceColumn - 1;
            newPieceRow = pieceRow - 1;
            while (newPieceColumn >= 0 && newPieceRow >= 0)
            {
                if (canPieceMoveHere(newPieceRow, newPieceColumn, getPieceColorFromPieceType(pieceType)))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!isFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn--;
                newPieceRow--;
            }
            newPieceColumn = pieceColumn + 1;
            newPieceRow = pieceRow - 1;
            while (newPieceColumn < 8 && newPieceRow >= 0)
            {
                if (canPieceMoveHere(newPieceRow, newPieceColumn, getPieceColorFromPieceType(pieceType)))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!isFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn++;
                newPieceRow--;
            }
            newPieceColumn = pieceColumn - 1;
            newPieceRow = pieceRow + 1;
            while (newPieceColumn >= 0 && newPieceRow < 8)
            {
                if (canPieceMoveHere(newPieceRow, newPieceColumn, getPieceColorFromPieceType(pieceType)))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!isFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn--;
                newPieceRow++;
            }
        }
        void generateMovesRook(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
        {
            int newPieceRow = pieceRow;
            int newPieceColumn = pieceColumn;
            for (int i = pieceColumn + 1; i < 8; i++)
            {
                newPieceColumn += 1;
                if (canPieceMoveHere(pieceRow, newPieceColumn, getPieceColorFromPieceType(pieceType)))
                    possibleMoves.Add(new Point(pieceRow, newPieceColumn));
                if (!isFieldEmpty(pieceRow, newPieceColumn))
                    break;
            }
            newPieceColumn = pieceColumn;
            for (int i = pieceColumn - 1; i >= 0; i--)
            {
                newPieceColumn -= 1;
                if (canPieceMoveHere(pieceRow, newPieceColumn, getPieceColorFromPieceType(pieceType)))
                    possibleMoves.Add(new Point(pieceRow, newPieceColumn));
                if (!isFieldEmpty(pieceRow, newPieceColumn))
                    break;
            }
            for (int i = pieceRow + 1; i < 8; i++)
            {
                newPieceRow += 1;
                if (canPieceMoveHere(newPieceRow, pieceColumn, getPieceColorFromPieceType(pieceType)))
                    possibleMoves.Add(new Point(newPieceRow, pieceColumn));
                if (!isFieldEmpty(newPieceRow, pieceColumn))
                    break;
            }
            newPieceRow = pieceRow;
            for (int i = pieceRow - 1; i >= 0; i--)
            {
                newPieceRow -= 1;
                if (canPieceMoveHere(newPieceRow, pieceColumn, getPieceColorFromPieceType(pieceType)))
                    possibleMoves.Add(new Point(newPieceRow, pieceColumn));
                if (!isFieldEmpty(newPieceRow, pieceColumn))
                    break;
            }
        }
        void generateMovesQueen(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
        {
            generateMovesRook(pieceType, pieceRow, pieceColumn, possibleMoves);
            generateMovesBishop(pieceType, pieceRow, pieceColumn, possibleMoves);
        }
        void generateMovesKing(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
        {
            if (canPieceMoveHere(pieceRow, pieceColumn - 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow, pieceColumn - 1));
            if (canPieceMoveHere(pieceRow - 1, pieceColumn - 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn - 1));
            if (canPieceMoveHere(pieceRow + 1, pieceColumn - 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn - 1));

            if (canPieceMoveHere(pieceRow, pieceColumn + 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow, pieceColumn + 1));
            if (canPieceMoveHere(pieceRow - 1, pieceColumn + 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn + 1));
            if (canPieceMoveHere(pieceRow + 1, pieceColumn + 1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn + 1));

            if (canPieceMoveHere(pieceRow + 1, pieceColumn, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn));
            if (canPieceMoveHere(pieceRow - 1, pieceColumn, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn));

            if(getPieceColorFromPieceType(pieceType) == "White")
            {
                if (checkCastling(pieceRow, pieceColumn, 7, 7) && isFieldEmpty(pieceRow, pieceColumn + 1) && isFieldEmpty(pieceRow, pieceColumn + 2))
                {
                    possibleMoves.Add(new Point(pieceRow, pieceColumn + 2));
                    whiteShortCastling = true;
                }
                if(checkCastling(pieceRow, pieceColumn, 7, 0) && isFieldEmpty(pieceRow, pieceColumn - 1) && isFieldEmpty(pieceRow, pieceColumn - 2) && isFieldEmpty(pieceRow, pieceColumn - 3))
                {
                    possibleMoves.Add(new Point(pieceRow, pieceColumn - 2));
                    whiteLongCastling = true;
                }
            }
            if(getPieceColorFromPieceType(pieceType) == "Black")
            {
                if (checkCastling(pieceRow, pieceColumn, 0, 7) && isFieldEmpty(pieceRow, pieceColumn + 1) && isFieldEmpty(pieceRow, pieceColumn + 2))
                {
                    possibleMoves.Add(new Point(pieceRow, pieceColumn + 2));
                    blackShortCastling = true;
                }
                if (checkCastling(pieceRow, pieceColumn, 0, 0) && isFieldEmpty(pieceRow, pieceColumn - 1) && isFieldEmpty(pieceRow, pieceColumn - 2) && isFieldEmpty(pieceRow, pieceColumn - 3))
                {
                    possibleMoves.Add(new Point(pieceRow, pieceColumn - 2));
                    blackLongCastling = true;
                }
            }
        }
        List<Point> generateAllPossibleMoves(Piece[,] chessBoard)
        {
            List<Point> possibleMoves = new List<Point>();
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (!isFieldEmpty(i, j))
                    {
                        string pieceColor = chessBoard[i, j].color;
                        string pieceType = GetPieceTypeFromField(i, j);
                        string type = pieceColor + pieceType;
                        generatePossibleMoves(type, i, j, possibleMoves);
                    }
                }
            }
            return possibleMoves;
        }
        bool checkIfAnyKingUnderCheck(Piece[,] chessBoard)
        {
            List<Point> possibleMoves = generateAllPossibleMoves(chessBoard);
            bool check = false;
            foreach (Point possibleMove in possibleMoves)
            {
                if (isFieldAKing((int)possibleMove.X, (int)possibleMove.Y))
                {
                    check = true;
                    string color = getPieceColorFromField((int)possibleMove.X, (int)possibleMove.Y);
                    if(color == "Black")
                    {
                        blackKingUnderCheck = true;
                        Trace.WriteLine("Black king under check!");
                    }
                    else
                    {
                        whiteKingUnderCheck = true;
                        Trace.WriteLine("White king under check!");
                    }
                }
            }
            if(!check)
            {
                whiteKingUnderCheck = false;
                blackKingUnderCheck = false;
                Trace.WriteLine("No check!");
            }
            return check;
        }
        bool isKingSafeAfterMove(int pieceRow, int pieceColumn, int fieldRow, int fieldColumn)
        {
            string pieceType = GetPieceTypeFromField(pieceRow, pieceColumn);
            if (pieceType != "Empty")
            {
                Piece piece = chessBoard.board[pieceRow, pieceColumn];
                Piece piece2 = chessBoard.board[fieldRow, fieldColumn];

                Piece[,] tempBoard = new Piece[8,8];
                tempBoard = cloneChessboard(chessBoard.board);

                tempBoard[fieldRow, fieldColumn] = piece;
                tempBoard[pieceRow, pieceColumn] = new Piece("None", "Empty", '.');

                bool kingSafe = !checkIfAnyKingUnderCheck(tempBoard);

                tempBoard[fieldRow, fieldColumn] = piece2;
                tempBoard[pieceRow, pieceColumn] = piece;

                return kingSafe;
            }
            return true;
        } 
        bool checkCastling(int kingRow, int kingColumn, int rookRow, int rookColumn)
        {
            if (firstMoveOfPiece(kingRow, kingColumn) && GetPieceTypeFromField(kingRow, kingColumn) == "King" && firstMoveOfPiece(rookRow, rookColumn) && GetPieceTypeFromField(rookRow, rookColumn) == "Rook")
                return true;
            return false;
        }
        void promotePawn(int row, int column)
        {
            string type = GetPieceTypeFromField(row, column);
            string color = getPieceColorFromField(row, column);
            if (type == "Pawn")
            {
                if(color == "White" && row == 0)
                {
                    if (!promotionPieceSelected)
                    {
                        PromoteWhiteBishop.Visibility = Visibility.Visible;
                        PromoteWhiteKnight.Visibility = Visibility.Visible;
                        PromoteWhiteQueen.Visibility = Visibility.Visible;
                        PromoteWhiteRook.Visibility = Visibility.Visible;
                        changeChessBoardOpacity(0.5);
                    }
                    else
                    {
                        PromoteWhiteBishop.Visibility = Visibility.Hidden;
                        PromoteWhiteKnight.Visibility = Visibility.Hidden;
                        PromoteWhiteQueen.Visibility = Visibility.Hidden;
                        PromoteWhiteRook.Visibility = Visibility.Hidden;
                        changeChessBoardOpacity(1);
                    }
                }
                if(color == "Black" && row == 7)
                {
                    if(!promotionPieceSelected)
                    {
                        PromoteBlackBishop.Visibility = Visibility.Visible;
                        PromoteBlackKnight.Visibility = Visibility.Visible;
                        PromoteBlackQueen.Visibility = Visibility.Visible;
                        PromoteBlackRook.Visibility = Visibility.Visible;
                        changeChessBoardOpacity(0.5);
                    }
                    else
                    {
                        PromoteBlackBishop.Visibility = Visibility.Hidden;
                        PromoteBlackKnight.Visibility = Visibility.Hidden;
                        PromoteBlackQueen.Visibility = Visibility.Hidden;
                        PromoteBlackRook.Visibility = Visibility.Hidden;
                        changeChessBoardOpacity(1);
                    }
                }
            }
        }
        Piece[,] cloneChessboard(Piece[,] board)
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    board[i, j] = DeepClone(chessBoard.board[i, j]);
                }
            }
            return board;
        }
        public static Piece DeepClone<Piece>(Piece obj)
        {
            using (var ms = new MemoryStream())
            {
                var formatter = new BinaryFormatter();
                formatter.Serialize(ms, obj);
                ms.Position = 0;

                return (Piece)formatter.Deserialize(ms);
            }
        }
    }
}
