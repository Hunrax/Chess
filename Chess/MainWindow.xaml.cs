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
        void selectPiece(Button button, string piece)
        {
            if (!buttonClicked && blackPiecesWithKing.Contains(piece) && turn == "White")
            {
                MessageBox.Show("It's white's turn!");
            }
            else if (!buttonClicked && whitePiecesWithKing.Contains(piece) && turn == "Black")
            {
                MessageBox.Show("It's black's turn!");
            }
            else if (!buttonClicked && button.Background != Brushes.Transparent)
            {
                buttonClicked = true;
                pressedButton = button;

                changeChessBoardOpacity(0.5);
                pressedButton.Opacity = 1;
            }
            else if (!buttonClicked && button.Background == Brushes.Transparent)
            {
                MessageBox.Show("Select a piece first.");
            }
        }
        void selectField(Button selectedField)
        {
            var fieldRow = Grid.GetRow(selectedField);
            var fieldColumn = Grid.GetColumn(selectedField);

            var pieceRow = Grid.GetRow(pressedButton);
            var pieceColumn = Grid.GetColumn(pressedButton);

            if (fieldColumn == pieceColumn && fieldRow == pieceRow)
            {
                changeChessBoardOpacity(1);

                buttonClicked = false;
                return;
            }
            string pieceType = pressedButton.Tag.ToString();
            string fieldType = selectedField.Tag.ToString();

            ArrayList possibleMovesRow = new ArrayList();
            ArrayList possibleMovesColumn = new ArrayList();

            int newPieceRow = pieceRow;
            int newPieceColumn = pieceColumn;

            switch (pieceType)
            {
                case "WhitePawn":
                    movePawn(fieldType, pieceRow, pieceColumn, newPieceRow, fieldRow, fieldColumn, possibleMovesRow, possibleMovesColumn, blackPiecesNoKing, 6, -1, 1);
                    break;

                case "BlackPawn":
                    movePawn(fieldType, pieceRow, pieceColumn, newPieceRow, fieldRow, fieldColumn, possibleMovesRow, possibleMovesColumn, whitePiecesNoKing, 1, 1, -1);
                    break;

                case "WhiteRook":
                case "BlackRook":
                    moveRook(pieceType, fieldType, pieceRow, pieceColumn, newPieceRow, newPieceColumn, possibleMovesRow, possibleMovesColumn);
                    break;

                case "WhiteKnight":
                case "BlackKnight":
                    moveKnight(pieceType, fieldType, pieceRow, pieceColumn, fieldRow, fieldColumn, possibleMovesRow, possibleMovesColumn);
                    break;

                case "WhiteBishop":
                case "BlackBishop":
                    moveBishop(pieceType, fieldType, pieceRow, pieceColumn, newPieceRow, newPieceColumn, possibleMovesRow, possibleMovesColumn);
                    break;

                case "WhiteQueen":
                case "BlackQueen":
                    moveQueen(pieceType, fieldType, pieceRow, pieceColumn, newPieceRow, newPieceColumn, possibleMovesRow, possibleMovesColumn);
                    break;

                case "WhiteKing":
                case "BlackKing":
                    moveKing(pieceType, fieldType, pieceRow, pieceColumn, possibleMovesRow, possibleMovesColumn);
                    break;
            }

            bool correctFieldSelected = false;
            for (int i = 0; i < possibleMovesColumn.Count; i++)
            {
                if ((int)possibleMovesColumn[i] == fieldColumn && (int)possibleMovesRow[i] == fieldRow)
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
            {
                MessageBox.Show("You can't move this piece here.");
            }
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
        void movePawn(string fieldType, int pieceRow, int pieceColumn, int newPieceRow, int fieldRow, int fieldColumn, ArrayList possibleMovesRow, ArrayList possibleMovesColumn, ArrayList piecesNoKing, int pieceRowValueCheck, int value1, int value2)
        {
            if (fieldType == "Empty")
            {
                newPieceRow = pieceRow + value1;
                possibleMovesRow.Add(newPieceRow);
                possibleMovesColumn.Add(pieceColumn);
                if (pieceRow == pieceRowValueCheck)
                {
                    possibleMovesRow.Add(newPieceRow + value1);
                    possibleMovesColumn.Add(pieceColumn);
                }
            }
            else if (piecesNoKing.Contains(fieldType))
            {
                if (fieldRow == pieceRow + value1 && fieldColumn == pieceColumn + value1)
                {
                    possibleMovesRow.Add(pieceRow + value1);
                    possibleMovesColumn.Add(pieceColumn + value1);
                }
                if (fieldRow == pieceRow + value1 && fieldColumn == pieceColumn + value2)
                {
                    possibleMovesRow.Add(pieceRow + value1);
                    possibleMovesColumn.Add(pieceColumn + value2);
                }
            }
        }
        void moveKnight(string pieceType, string fieldType, int pieceRow, int pieceColumn, int fieldRow, int fieldColumn, ArrayList possibleMovesRow, ArrayList possibleMovesColumn)
        {
            if (fieldType == "Empty" ||
               (pieceType == "WhiteKnight" && blackPiecesNoKing.Contains(fieldType)) ||
               (pieceType == "BlackKnight" && whitePiecesNoKing.Contains(fieldType)))
            {
                if (pieceColumn - 2 >= 0 && pieceRow - 1 >= 0)
                {
                    possibleMovesColumn.Add(pieceColumn - 2);
                    possibleMovesRow.Add(pieceRow - 1);
                }
                if (pieceColumn - 2 >= 0 && pieceRow + 1 <= 7)
                {
                    possibleMovesColumn.Add(pieceColumn - 2);
                    possibleMovesRow.Add(pieceRow + 1);
                }
                if (pieceColumn + 2 <= 7 && pieceRow - 1 >= 0)
                {
                    possibleMovesColumn.Add(pieceColumn + 2);
                    possibleMovesRow.Add(pieceRow - 1);
                }
                if (pieceColumn + 2 <= 7 && pieceRow + 1 <= 7)
                {
                    possibleMovesColumn.Add(pieceColumn + 2);
                    possibleMovesRow.Add(pieceRow + 1);
                }
                if (pieceColumn + 1 <= 7 && pieceRow - 2 >= 0)
                {
                    possibleMovesColumn.Add(pieceColumn + 1);
                    possibleMovesRow.Add(pieceRow - 2);
                }
                if (pieceColumn + 1 <= 7 && pieceRow + 2 <= 7)
                {
                    possibleMovesColumn.Add(pieceColumn + 1);
                    possibleMovesRow.Add(pieceRow + 2);
                }
                if (pieceColumn - 1 >= 0 && pieceRow - 2 >= 0)
                {
                    possibleMovesColumn.Add(pieceColumn - 1);
                    possibleMovesRow.Add(pieceRow - 2);
                }
                if (pieceColumn - 1 >= 0 && pieceRow + 2 <= 7)
                {
                    possibleMovesColumn.Add(pieceColumn - 1);
                    possibleMovesRow.Add(pieceRow + 2);
                }
            }
        }
        void moveBishop(string pieceType, string fieldType, int pieceRow, int pieceColumn, int newPieceRow, int newPieceColumn, ArrayList possibleMovesRow, ArrayList possibleMovesColumn)
        {
            if (fieldType == "Empty" ||
               (pieceType == "WhiteBishop" && blackPiecesNoKing.Contains(fieldType)) ||
               (pieceType == "BlackBishop" && whitePiecesNoKing.Contains(fieldType)))
            {
                newPieceColumn = pieceColumn + 1;
                newPieceRow = pieceRow + 1;
                while (newPieceColumn < 8 && newPieceRow < 8)
                {
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, newPieceColumn].type != "Empty")
                        break;
                    newPieceColumn++;
                    newPieceRow++;
                }
                newPieceColumn = pieceColumn - 1;
                newPieceRow = pieceRow - 1;
                while (newPieceColumn >= 0 && newPieceRow >= 0)
                {
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, newPieceColumn].type != "Empty")
                        break;
                    newPieceColumn--;
                    newPieceRow--;
                }
                newPieceColumn = pieceColumn + 1;
                newPieceRow = pieceRow - 1;
                while (newPieceColumn < 8 && newPieceRow >= 0)
                {
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, newPieceColumn].type != "Empty")
                        break;
                    newPieceColumn++;
                    newPieceRow--;
                }
                newPieceColumn = pieceColumn - 1;
                newPieceRow = pieceRow + 1;
                while (newPieceColumn >= 0 && newPieceRow < 8)
                {
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, newPieceColumn].type != "Empty")
                        break;
                    newPieceColumn--;
                    newPieceRow++;
                }
            }
        }
        void moveRook(string pieceType, string fieldType, int pieceRow, int pieceColumn, int newPieceRow, int newPieceColumn, ArrayList possibleMovesRow, ArrayList possibleMovesColumn)
        {
            if (fieldType == "Empty" ||
               (pieceType == "WhiteRook" && blackPiecesNoKing.Contains(fieldType)) ||
               (pieceType == "BlackRook" && whitePiecesNoKing.Contains(fieldType)))
            {
                for (int i = pieceColumn + 1; i < 8; i++)
                {
                    newPieceColumn += 1;
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(pieceRow);
                    if (chessBoard.board[pieceRow, newPieceColumn].type != "Empty")
                        break;
                }
                newPieceColumn = pieceColumn;
                for (int i = pieceColumn - 1; i >= 0; i--)
                {
                    newPieceColumn -= 1;
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(pieceRow);
                    if (chessBoard.board[pieceRow, newPieceColumn].type != "Empty")
                        break;
                }
                for (int i = pieceRow + 1; i < 8; i++)
                {
                    newPieceRow += 1;
                    possibleMovesColumn.Add(pieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, pieceColumn].type != "Empty")
                        break;
                }
                newPieceRow = pieceRow;
                for (int i = pieceRow - 1; i >= 0; i--)
                {
                    newPieceRow -= 1;
                    possibleMovesColumn.Add(pieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, pieceColumn].type != "Empty")
                        break;
                }
            }
        }
        void moveQueen(string pieceType, string fieldType, int pieceRow, int pieceColumn, int newPieceRow, int newPieceColumn, ArrayList possibleMovesRow, ArrayList possibleMovesColumn)
        {
            if (fieldType == "Empty" ||
               (pieceType == "WhiteQueen" && blackPiecesNoKing.Contains(fieldType)) ||
               (pieceType == "BlackQueen" && whitePiecesNoKing.Contains(fieldType)))
            {
                for (int i = pieceColumn + 1; i < 8; i++)
                {
                    newPieceColumn += 1;
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(pieceRow);
                    if (chessBoard.board[pieceRow, newPieceColumn].type != "Empty")
                        break;
                }
                newPieceColumn = pieceColumn;
                for (int i = pieceColumn - 1; i >= 0; i--)
                {
                    newPieceColumn -= 1;
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(pieceRow);
                    if (chessBoard.board[pieceRow, newPieceColumn].type != "Empty")
                        break;
                }
                for (int i = pieceRow + 1; i < 8; i++)
                {
                    newPieceRow += 1;
                    possibleMovesColumn.Add(pieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, pieceColumn].type != "Empty")
                        break;
                }
                newPieceRow = pieceRow;
                for (int i = pieceRow - 1; i >= 0; i--)
                {
                    newPieceRow -= 1;
                    possibleMovesColumn.Add(pieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, pieceColumn].type != "Empty")
                        break;
                }

                newPieceColumn = pieceColumn + 1;
                newPieceRow = pieceRow + 1;
                while (newPieceColumn < 8 && newPieceRow < 8)
                {
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, newPieceColumn].type != "Empty")
                        break;
                    newPieceColumn++;
                    newPieceRow++;
                }
                newPieceColumn = pieceColumn - 1;
                newPieceRow = pieceRow - 1;
                while (newPieceColumn >= 0 && newPieceRow >= 0)
                {
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, newPieceColumn].type != "Empty")
                        break;
                    newPieceColumn--;
                    newPieceRow--;
                }
                newPieceColumn = pieceColumn + 1;
                newPieceRow = pieceRow - 1;
                while (newPieceColumn < 8 && newPieceRow >= 0)
                {
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, newPieceColumn].type != "Empty")
                        break;
                    newPieceColumn++;
                    newPieceRow--;
                }
                newPieceColumn = pieceColumn - 1;
                newPieceRow = pieceRow + 1;
                while (newPieceColumn >= 0 && newPieceRow < 8)
                {
                    possibleMovesColumn.Add(newPieceColumn);
                    possibleMovesRow.Add(newPieceRow);
                    if (chessBoard.board[newPieceRow, newPieceColumn].type != "Empty")
                        break;
                    newPieceColumn--;
                    newPieceRow++;
                }
            }
        }
        void moveKing(string pieceType, string fieldType, int pieceRow, int pieceColumn, ArrayList possibleMovesRow, ArrayList possibleMovesColumn)
        {
            if (fieldType == "Empty" ||
               (pieceType == "WhiteKing" && blackPiecesNoKing.Contains(fieldType)) ||
               (pieceType == "BlackKing" && whitePiecesNoKing.Contains(fieldType)))
            {
                if (pieceColumn - 1 >= 0)
                {
                    possibleMovesColumn.Add(pieceColumn - 1);
                    possibleMovesRow.Add(pieceRow);
                    if (pieceRow - 1 >= 0)
                    {
                        possibleMovesColumn.Add(pieceColumn - 1);
                        possibleMovesRow.Add(pieceRow - 1);
                    }
                    if (pieceRow + 1 < 8)
                    {
                        possibleMovesColumn.Add(pieceColumn - 1);
                        possibleMovesRow.Add(pieceRow + 1);
                    }
                }
                if (pieceColumn + 1 < 8)
                {
                    possibleMovesColumn.Add(pieceColumn + 1);
                    possibleMovesRow.Add(pieceRow);
                    if (pieceRow - 1 >= 0)
                    {
                        possibleMovesColumn.Add(pieceColumn + 1);
                        possibleMovesRow.Add(pieceRow - 1);
                    }
                    if (pieceRow + 1 < 8)
                    {
                        possibleMovesColumn.Add(pieceColumn + 1);
                        possibleMovesRow.Add(pieceRow + 1);
                    }
                }
                if (pieceRow + 1 < 8)
                {
                    possibleMovesColumn.Add(pieceColumn);
                    possibleMovesRow.Add(pieceRow + 1);
                }
                if (pieceRow - 1 >= 0)
                {
                    possibleMovesColumn.Add(pieceColumn);
                    possibleMovesRow.Add(pieceRow - 1);
                }
            }
        }
    }
}
