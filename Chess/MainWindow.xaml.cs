using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
        string turn = "White";
        int movesCounter = 0;
        ChessBoard chessBoard = new ChessBoard();
        bool testMode = false;

        ArrayList whitePiecesNoKing = new ArrayList{ "WhitePawn", "WhiteKnight", "WhiteBishop", "WhiteRook", "WhiteQueen" };
        ArrayList whitePiecesWithKing = new ArrayList { "WhitePawn", "WhiteKnight", "WhiteBishop", "WhiteRook", "WhiteQueen", "WhiteKing" };
        ArrayList blackPiecesNoKing = new ArrayList { "BlackPawn", "BlackKnight", "BlackBishop", "BlackRook", "BlackQueen" };
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

            if(!buttonClicked) // select piece you want to move
            {
                selectPiece(button, piece);
            }
            else // choose where you want to move your piece
            {
                Button selectedField = e.Source as Button;
                selectField(selectedField);
                chessBoard.Print();
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

                string pieceType = pressedButton.Tag.ToString();
                int pieceRow = Grid.GetRow(pressedButton);
                int pieceColumn = Grid.GetColumn(pressedButton);

                List<Point> possibleMoves = new List<Point>();
                generatePossibleMoves(pieceType, pieceRow, pieceColumn, possibleMoves);
                highlightPossibleFields(possibleMoves);
            }
            else if (!buttonClicked && button.Background == Brushes.Transparent)
            {
                MessageBox.Show("Select a piece first.");
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
                if (arePointsEqual(possibleMoves[i], fieldRow, fieldColumn))
                {
                    correctFieldSelected = true;
                    break;
                }
            }

            if (correctFieldSelected)
            {
                movePieceToField(selectedField, pieceType, fieldType, fieldColumn, fieldRow, pieceColumn, pieceRow);
            }
            else
                MessageBox.Show("You can't move this piece here.");
        }
        bool arePointsEqual(Point point, int x, int y)
        {
            if (point.X == x && point.Y == y)
                return true;
            return false;
        }
        void movePieceToField(Button selectedField, string pieceType, string fieldType, int fieldColumn, int fieldRow, int pieceColumn, int pieceRow)
        {
            Grid.SetColumn(pressedButton, fieldColumn); // move piece to field
            Grid.SetRow(pressedButton, fieldRow);
            chessBoard.board[fieldRow, fieldColumn] = getPieceFromPieceType(pieceType);

            Grid.SetColumn(selectedField, pieceColumn); // set former piece field to empty
            Grid.SetRow(selectedField, pieceRow);
            chessBoard.board[pieceRow, pieceColumn] = new Piece("None", "Empty", '.');

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
                field.Opacity = opacity;
            foreach (Button field in gornaWarstwa.Children)
                field.Opacity = opacity;
        }
        void generatePossibleMoves(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
        {
            switch (pieceType)
            {
                case "WhitePawn":
                    movePawn(pieceType, pieceRow, pieceColumn, possibleMoves, 6, -1, 1);
                    break;

                case "BlackPawn":
                    movePawn(pieceType, pieceRow, pieceColumn, possibleMoves, 1, 1, -1);
                    break;

                case "WhiteRook":
                case "BlackRook":
                    moveRook(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;

                case "WhiteKnight":
                case "BlackKnight":
                    moveKnight(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;

                case "WhiteBishop":
                case "BlackBishop":
                    moveBishop(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;

                case "WhiteQueen":
                case "BlackQueen":
                    moveQueen(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;

                case "WhiteKing":
                case "BlackKing":
                    moveKing(pieceType, pieceRow, pieceColumn, possibleMoves);
                    break;
            }
        }
        void highlightPossibleFields(List<Point> possibleMoves)
        {
            for(int i = 0; i < possibleMoves.Count; i++)
            {
                Grid gridDolnaWarstwa = dolnaWarstwa.Children.Cast<UIElement>()
                    .FirstOrDefault(e => Grid.GetRow(e) == possibleMoves[i].X && Grid.GetColumn(e) == possibleMoves[i].Y) as Grid;

                gridDolnaWarstwa.Opacity = 1;
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
            else
                return "Black";
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
            if (chessBoard.board[row, column].type != "Empty" && chessBoard.board[row, column].type != "King" && pieceColor != chessBoard.board[row, column].color)
                return true;
            return false;
        }
        bool canPieceMoveHere(int row, int column, string pieceColor)
        {
            if (isFieldEmpty(row, column) || isFieldPossibleToCapture(row, column, pieceColor))
                return true;
            return false;
        }
        void movePawn(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves, int pieceRowValueCheck, int value1, int value2)
        {
            if(isFieldEmpty(pieceRow + value1, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + value1, pieceColumn));
            if(isFieldEmpty(pieceRow + (2 * value1), pieceColumn) && pieceRow == pieceRowValueCheck)
                possibleMoves.Add(new Point(pieceRow + (2 * value1), pieceColumn));

            if(isFieldPossibleToCapture(pieceRow + value1, pieceColumn + value1, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow + value1, pieceColumn + value1));
            if (isFieldPossibleToCapture(pieceRow + value1, pieceColumn + value2, getPieceColorFromPieceType(pieceType)))
                possibleMoves.Add(new Point(pieceRow + value1, pieceColumn + value2));
        }
        void moveKnight(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
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
        void moveBishop(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
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
        void moveRook(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
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
        void moveQueen(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
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

            newPieceColumn = pieceColumn + 1;
            newPieceRow = pieceRow + 1;
            while (newPieceColumn < 8 && newPieceRow < 8)
            {
                if (canPieceMoveHere(newPieceRow, newPieceColumn, getPieceColorFromPieceType(pieceType)))
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
        void moveKing(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves)
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
        }
    }
}
