using System;
using System.Collections;
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
        private const int LONG_CASTLING_KING_COLUMN = 2;
        private const int SHORT_CASTLING_KING_COLUMN = 6;
        private const int WHITE_CASTLING_ROOK_ROW = 7;
        private const int BLACK_CASTLING_ROOK_ROW = 0;
        private const int SHORT_CASTLING_ROOK_COLUMN = 7;
        private const int SHORT_CASTLING_EMPTYFIELD_COLUMN = 5;
        private const int LONG_CASTLING_ROOK_COLUMN = 0;
        private const int LONG_CASTLING_EMPTYFIELD_COLUMN = 3;

        private const int CHESSBOARD_MINIMUM_INDEX = 0;
        private const int CHESSBOARD_MAXIMUM_INDEX = 7;
        private const int CHESSBOARD_SIZE = 8;

        Button pressedButton;
        Piece selectedPromotionPiece;
        ChessBoard chessBoard = new ChessBoard();
        Game game = new Game(GameState.IN_PROGRESS);
        bool buttonClicked = false;

        bool promotionPieceSelected = false;
        int enPassantStatus = 0;

        bool whiteShortCastling = false;
        bool whiteLongCastling = false;
        bool blackShortCastling = false;
        bool blackLongCastling = false;

        bool whiteKingUnderCheck = false;
        bool blackKingUnderCheck = false;

        public MainWindow()
        {
            InitializeComponent();
            chessBoard.Clear();
            chessBoard.Initialize();
            chessBoard.Print();
            game.window = this;
        }
        void PieceSelected(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            string pieceType = button.Tag.ToString();

            if (!buttonClicked) // select piece you want to move
            {
                selectPiece(button, pieceType);
            }
            else // choose where you want to move your piece
            {
                Button selectedField = e.Source as Button;
                selectField(selectedField);
            }
        }
        void TestMode(object sender, RoutedEventArgs e)
        {
            game.testMode = !game.testMode;
            if(game.testMode)
                TestModeButton.Content = "TestMode: ON";
            else
                TestModeButton.Content = "TestMode: OFF";
        }
        void ChoosePromotionPiece(object sender, RoutedEventArgs e)
        {
            Button button = e.Source as Button;
            string pieceType = button.Name.ToString();

            selectedPromotionPiece = getPieceFromPieceType(pieceType);
            promotionPieceSelected = true;
            int pieceRow = Grid.GetRow(pressedButton);
            int pieceColumn = Grid.GetColumn(pressedButton);

            promotePawn(pieceRow, pieceColumn);
            var brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri($"images/{pieceType}.png", UriKind.Relative));
            pressedButton.Background = brush;
            pressedButton.Tag = selectedPromotionPiece.color + selectedPromotionPiece.type;
            chessBoard.board[pieceRow, pieceColumn] = selectedPromotionPiece;
            chessBoard.board[pieceRow, pieceColumn].firstMove = false;
            promotionPieceSelected = false;
        }
        void selectPiece(Button button, string pieceType)
        {
            if (!buttonClicked && getPieceColorFromPieceType(pieceType) == "Black" && game.turn == "White" && !game.testMode)
            {
                MessageBox.Show("It's white's turn!");
            }
            else if (!buttonClicked && getPieceColorFromPieceType(pieceType) == "White" && game.turn == "Black" && !game.testMode)
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

                int pieceRow = Grid.GetRow(pressedButton);
                int pieceColumn = Grid.GetColumn(pressedButton);

                List<Point> possibleMoves = new List<Point>();
                generatePossibleMoves(pieceType, pieceRow, pieceColumn, possibleMoves, true);
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
            generatePossibleMoves(pieceType, pieceRow, pieceColumn, possibleMoves, true);

            bool correctFieldSelected = false;
            for(int i = 0; i < possibleMoves.Count; i++)
            {
                if (chessBoard.arePointsEqual(possibleMoves[i], fieldRow, fieldColumn) && !chessBoard.isFieldAKing(fieldRow, fieldColumn))
                {
                    correctFieldSelected = true;
                    break;
                }
            }
            if (correctFieldSelected)
            {
                checkPawnDoubleMove(pieceRow, pieceColumn, fieldRow);
                updateEnPassantStatus(pieceType, fieldType, pieceRow, pieceColumn, fieldRow, fieldColumn);
                movePieceToField(selectedField, pieceType, fieldType, fieldColumn, fieldRow, pieceColumn, pieceRow);
                promotePawn(fieldRow, fieldColumn);
                chessBoard.Print();
                checkIfAnyKingUnderCheck(game.turn);
                highlightKingUnderCheck();

                game.checkGameState();
                if(game.checkIfGameOver())
                {
                    foreach (Button field in gornaWarstwa.Children)
                        field.IsEnabled = false;
                }
            }
        }
        void performCastling(int rookRow, int rookColumn, int emptyFieldRow, int emptyFieldColumn)
        {
            Button rook = gornaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == rookRow && Grid.GetColumn(e) == rookColumn) as Button;

            Button empty = gornaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == emptyFieldRow && Grid.GetColumn(e) == emptyFieldColumn) as Button;

            string pieceColor = chessBoard.getPieceColorFromField(rookRow, rookColumn);

            Grid.SetColumn(rook, emptyFieldColumn);
            Grid.SetRow(rook, emptyFieldRow);
            chessBoard.board[emptyFieldRow, emptyFieldColumn] = new Piece(pieceColor, "Rook", 'W');

            Grid.SetColumn(empty, rookColumn);
            Grid.SetRow(empty, rookRow);
            chessBoard.board[rookRow, rookColumn] = new Piece("None", "Empty", '.');
        }
        void checkAllCastlings(string pieceType, int fieldColumn)
        {
            if (whiteShortCastling && pieceType == "WhiteKing" && fieldColumn == SHORT_CASTLING_KING_COLUMN)
            {
                performCastling(WHITE_CASTLING_ROOK_ROW, SHORT_CASTLING_ROOK_COLUMN, WHITE_CASTLING_ROOK_ROW, SHORT_CASTLING_EMPTYFIELD_COLUMN);
                whiteShortCastling = false;
            }
            if (whiteLongCastling && pieceType == "WhiteKing" && fieldColumn == LONG_CASTLING_KING_COLUMN)
            {
                performCastling(WHITE_CASTLING_ROOK_ROW, LONG_CASTLING_ROOK_COLUMN, WHITE_CASTLING_ROOK_ROW, LONG_CASTLING_EMPTYFIELD_COLUMN);
                whiteLongCastling = false;
            }

            if (blackShortCastling && pieceType == "BlackKing" && fieldColumn == SHORT_CASTLING_KING_COLUMN)
            {
                performCastling(BLACK_CASTLING_ROOK_ROW, SHORT_CASTLING_ROOK_COLUMN, BLACK_CASTLING_ROOK_ROW, SHORT_CASTLING_EMPTYFIELD_COLUMN);
                blackShortCastling = false;
            }
            if (blackLongCastling && pieceType == "BlackKing" && fieldColumn == LONG_CASTLING_KING_COLUMN)
            {
                performCastling(BLACK_CASTLING_ROOK_ROW, LONG_CASTLING_ROOK_COLUMN, BLACK_CASTLING_ROOK_ROW, LONG_CASTLING_EMPTYFIELD_COLUMN);
                blackLongCastling = false;
            }
        }
        void deletePawnEnPassant(int pieceColumn, int pieceRow)
        {
            int pawnToDeleteColumn = pieceColumn + enPassantStatus;

            Button pawnToDelete = gornaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == pieceRow && Grid.GetColumn(e) == pawnToDeleteColumn) as Button;

            pawnToDelete.Tag = "Empty";
            pawnToDelete.Background = Brushes.Transparent;

            Grid.SetColumn(pawnToDelete, pawnToDeleteColumn);
            Grid.SetRow(pawnToDelete, pieceRow);
            chessBoard.board[pieceRow, pawnToDeleteColumn] = new Piece("None", "Empty", '.');
        }
        void movePieceToField(Button selectedField, string pieceType, string fieldType, int fieldColumn, int fieldRow, int pieceColumn, int pieceRow)
        {
            Grid.SetColumn(pressedButton, fieldColumn); // move piece to field
            Grid.SetRow(pressedButton, fieldRow);
            chessBoard.board[fieldRow, fieldColumn] = chessBoard.board[pieceRow, pieceColumn];
            chessBoard.board[fieldRow, fieldColumn].firstMove = false;

            Grid.SetColumn(selectedField, pieceColumn); // set former piece field to empty
            Grid.SetRow(selectedField, pieceRow);
            chessBoard.board[pieceRow, pieceColumn] = new Piece("None", "Empty", '.');

            if (enPassantStatus != 0) // remove pawn captured en passant
                deletePawnEnPassant(pieceColumn, pieceRow);

            checkAllCastlings(pieceType, fieldColumn);

            if (fieldType != "Empty")
            {
                selectedField.Background = Brushes.Transparent;
                selectedField.Tag = "Empty";
            }
            changeChessBoardOpacity(1);
            game.changeTurn();
            buttonClicked = false;
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
        void generatePossibleMoves(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            switch (pieceType)
            {
                case "WhitePawn":
                    generateMovesPawn(pieceType, pieceRow, pieceColumn, possibleMoves, -1, 1, checkForChecks);
                    break;

                case "BlackPawn":
                    generateMovesPawn(pieceType, pieceRow, pieceColumn, possibleMoves, 1, -1, checkForChecks);
                    break;

                case "WhiteRook":
                case "BlackRook":
                    generateMovesRook(pieceType, pieceRow, pieceColumn, possibleMoves, checkForChecks);
                    break;

                case "WhiteKnight":
                case "BlackKnight":
                    generateMovesKnight(pieceType, pieceRow, pieceColumn, possibleMoves, checkForChecks);
                    break;

                case "WhiteBishop":
                case "BlackBishop":
                    generateMovesBishop(pieceType, pieceRow, pieceColumn, possibleMoves, checkForChecks);
                    break;

                case "WhiteQueen":
                case "BlackQueen":
                    generateMovesQueen(pieceType, pieceRow, pieceColumn, possibleMoves, checkForChecks);
                    break;

                case "WhiteKing":
                case "BlackKing":
                    generateMovesKing(pieceType, pieceRow, pieceColumn, possibleMoves, checkForChecks);
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

                if (!chessBoard.isFieldAKing((int)possibleMoves[i].X, (int)possibleMoves[i].Y))
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
        bool canPieceMoveHere(int fieldRow, int fieldColumn, string pieceColor, bool checkForChecks, int pieceRow, int pieceColumn)
        {
            if (chessBoard.isFieldEmpty(fieldRow, fieldColumn) || chessBoard.isFieldPossibleToCapture(fieldRow, fieldColumn, pieceColor))
            {
                if((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, fieldRow, fieldColumn)) || !checkForChecks)
                    return true;
                return false;
            }
            return false;
        }
        void generateMovesPawn(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves, int value1, int value2, bool checkForChecks)
        {
            if (chessBoard.isFieldEmpty(pieceRow + value1, pieceColumn))
            {
                if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + value1, pieceColumn)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + value1, pieceColumn));

                if (chessBoard.isFieldEmpty(pieceRow + (2 * value1), pieceColumn) && chessBoard.firstMoveOfPiece(pieceRow, pieceColumn))
                {
                    if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + (2 * value1), pieceColumn)) || !checkForChecks)
                        possibleMoves.Add(new Point(pieceRow + (2 * value1), pieceColumn));
                }
            }

            if (chessBoard.isFieldPossibleToCapture(pieceRow + value1, pieceColumn + value1, getPieceColorFromPieceType(pieceType)))
            {
                if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + value1, pieceColumn + value1)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + value1, pieceColumn + value1));
            }                
            if (chessBoard.isFieldPossibleToCapture(pieceRow + value1, pieceColumn + value2, getPieceColorFromPieceType(pieceType)))
            {
                if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + value1, pieceColumn + value2)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + value1, pieceColumn + value2));
            } 
            
            if (chessBoard.isFieldEmpty(pieceRow + value1, pieceColumn + value1) && enPassant(pieceRow, pieceColumn, value1))
            {
                if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + value1, pieceColumn + value1)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + value1, pieceColumn + value1));
            }
            if (chessBoard.isFieldEmpty(pieceRow + value1, pieceColumn + value2) && enPassant(pieceRow, pieceColumn, value2))
            {
                if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow + value1, pieceColumn + value2)) || !checkForChecks)
                    possibleMoves.Add(new Point(pieceRow + value1, pieceColumn + value2));
            }
        }
        bool enPassant(int pieceRow, int pieceColumn, int columnShift)
        {
            if (chessBoard.board[pieceRow, pieceColumn + columnShift].type == "Pawn" && chessBoard.getPieceColorFromField(pieceRow, pieceColumn) != chessBoard.getPieceColorFromField(pieceRow, pieceColumn + columnShift))
            {
                if(chessBoard.board[pieceRow, pieceColumn + columnShift].pawnDoubleMoveTurn == game.movesCounter && chessBoard.getPieceColorFromField(pieceRow, pieceColumn) == "White")
                    return true;
                if (chessBoard.board[pieceRow, pieceColumn + columnShift].pawnDoubleMoveTurn == game.movesCounter - 1 && chessBoard.getPieceColorFromField(pieceRow, pieceColumn) == "Black")
                    return true;
            }        
            return false;
        }
        int updateEnPassantStatus(string pieceType, string fieldType, int pieceRow, int pieceColumn, int fieldRow, int fieldColumn)
        {
            if(fieldType == "Empty" && pieceType.EndsWith("Pawn"))
            {
                if(pieceType.StartsWith("White"))
                {
                    if (fieldRow < pieceRow && fieldColumn < pieceColumn)
                        enPassantStatus = - 1;
                    else if (fieldRow < pieceRow && fieldColumn > pieceColumn)
                        enPassantStatus = + 1;
                    else
                        enPassantStatus = 0;
                }
                if (pieceType.StartsWith("Black"))
                {
                    if (fieldRow > pieceRow && fieldColumn < pieceColumn)
                        enPassantStatus = - 1;
                    else if (fieldRow > pieceRow && fieldColumn > pieceColumn)
                        enPassantStatus = + 1;
                    else
                        enPassantStatus = 0;
                }
            }
            else
                enPassantStatus = 0;

            return enPassantStatus;
        }
        void generateMovesKnight(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            string pieceColor = getPieceColorFromPieceType(pieceType);
            if (canPieceMoveHere(pieceRow - 1, pieceColumn - 2, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn - 2));
            if(canPieceMoveHere(pieceRow + 1, pieceColumn - 2, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn - 2));

            if (canPieceMoveHere(pieceRow - 1, pieceColumn + 2, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn + 2));
            if (canPieceMoveHere(pieceRow + 1, pieceColumn + 2, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn + 2));

            if (canPieceMoveHere(pieceRow - 2, pieceColumn + 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 2, pieceColumn + 1));
            if (canPieceMoveHere(pieceRow + 2, pieceColumn + 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 2, pieceColumn + 1));

            if (canPieceMoveHere(pieceRow - 2, pieceColumn - 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 2, pieceColumn - 1));
            if (canPieceMoveHere(pieceRow + 2, pieceColumn - 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 2, pieceColumn - 1));
        }
        void generateMovesBishop(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            string pieceColor = getPieceColorFromPieceType(pieceType);
            int newPieceColumn = pieceColumn + 1;
            int newPieceRow = pieceRow + 1;
            while (newPieceColumn < CHESSBOARD_SIZE && newPieceRow < CHESSBOARD_SIZE)
            {
                if(canPieceMoveHere(newPieceRow, newPieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!chessBoard.isFieldEmpty(newPieceRow, newPieceColumn)) 
                    break;
                newPieceColumn++;
                newPieceRow++;
            }
            newPieceColumn = pieceColumn - 1;
            newPieceRow = pieceRow - 1;
            while (newPieceColumn >= CHESSBOARD_MINIMUM_INDEX && newPieceRow >= CHESSBOARD_MINIMUM_INDEX)
            {
                if (canPieceMoveHere(newPieceRow, newPieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!chessBoard.isFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn--;
                newPieceRow--;
            }
            newPieceColumn = pieceColumn + 1;
            newPieceRow = pieceRow - 1;
            while (newPieceColumn < CHESSBOARD_SIZE && newPieceRow >= CHESSBOARD_MINIMUM_INDEX)
            {
                if (canPieceMoveHere(newPieceRow, newPieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!chessBoard.isFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn++;
                newPieceRow--;
            }
            newPieceColumn = pieceColumn - 1;
            newPieceRow = pieceRow + 1;
            while (newPieceColumn >= CHESSBOARD_MINIMUM_INDEX && newPieceRow < CHESSBOARD_SIZE)
            {
                if (canPieceMoveHere(newPieceRow, newPieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, newPieceColumn));
                if (!chessBoard.isFieldEmpty(newPieceRow, newPieceColumn))
                    break;
                newPieceColumn--;
                newPieceRow++;
            }
        }
        void generateMovesRook(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            string pieceColor = getPieceColorFromPieceType(pieceType);
            int newPieceRow = pieceRow;
            int newPieceColumn = pieceColumn;
            for (int i = pieceColumn + 1; i < CHESSBOARD_SIZE; i++)
            {
                newPieceColumn += 1;
                if (canPieceMoveHere(pieceRow, newPieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(pieceRow, newPieceColumn));
                if (!chessBoard.isFieldEmpty(pieceRow, newPieceColumn))
                    break;
            }
            newPieceColumn = pieceColumn;
            for (int i = pieceColumn - 1; i >= CHESSBOARD_MINIMUM_INDEX; i--)
            {
                newPieceColumn -= 1;
                if (canPieceMoveHere(pieceRow, newPieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(pieceRow, newPieceColumn));
                if (!chessBoard.isFieldEmpty(pieceRow, newPieceColumn))
                    break;
            }
            for (int i = pieceRow + 1; i < CHESSBOARD_SIZE; i++)
            {
                newPieceRow += 1;
                if (canPieceMoveHere(newPieceRow, pieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, pieceColumn));
                if (!chessBoard.isFieldEmpty(newPieceRow, pieceColumn))
                    break;
            }
            newPieceRow = pieceRow;
            for (int i = pieceRow - 1; i >= CHESSBOARD_MINIMUM_INDEX; i--)
            {
                newPieceRow -= 1;
                if (canPieceMoveHere(newPieceRow, pieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                    possibleMoves.Add(new Point(newPieceRow, pieceColumn));
                if (!chessBoard.isFieldEmpty(newPieceRow, pieceColumn))
                    break;
            }
        }
        void generateMovesQueen(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            generateMovesRook(pieceType, pieceRow, pieceColumn, possibleMoves, checkForChecks);
            generateMovesBishop(pieceType, pieceRow, pieceColumn, possibleMoves, checkForChecks);
        }
        void generateMovesKing(string pieceType, int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            string pieceColor = getPieceColorFromPieceType(pieceType);
            if (canPieceMoveHere(pieceRow, pieceColumn - 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow, pieceColumn - 1));
            if (canPieceMoveHere(pieceRow - 1, pieceColumn - 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn - 1));
            if (canPieceMoveHere(pieceRow + 1, pieceColumn - 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn - 1));

            if (canPieceMoveHere(pieceRow, pieceColumn + 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow, pieceColumn + 1));
            if (canPieceMoveHere(pieceRow - 1, pieceColumn + 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn + 1));
            if (canPieceMoveHere(pieceRow + 1, pieceColumn + 1, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn + 1));

            if (canPieceMoveHere(pieceRow + 1, pieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow + 1, pieceColumn));
            if (canPieceMoveHere(pieceRow - 1, pieceColumn, pieceColor, checkForChecks, pieceRow, pieceColumn))
                possibleMoves.Add(new Point(pieceRow - 1, pieceColumn));

            if(pieceColor == "White" && !whiteKingUnderCheck)
            {
                if (checkCastling(pieceRow, pieceColumn, WHITE_CASTLING_ROOK_ROW, SHORT_CASTLING_ROOK_COLUMN) && chessBoard.isFieldEmpty(pieceRow, pieceColumn + 1) && chessBoard.isFieldEmpty(pieceRow, pieceColumn + 2))
                {
                    if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 2) && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 1)) || !checkForChecks)
                    {
                        possibleMoves.Add(new Point(pieceRow, pieceColumn + 2));
                        whiteShortCastling = true;
                    }
                }
                if(checkCastling(pieceRow, pieceColumn, WHITE_CASTLING_ROOK_ROW, LONG_CASTLING_ROOK_COLUMN) && chessBoard.isFieldEmpty(pieceRow, pieceColumn - 1) && chessBoard.isFieldEmpty(pieceRow, pieceColumn - 2) && chessBoard.isFieldEmpty(pieceRow, pieceColumn - 3))
                {
                    if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 2) && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 1)) || !checkForChecks)
                    {
                        possibleMoves.Add(new Point(pieceRow, pieceColumn - 2));
                        whiteLongCastling = true;
                    }
                }
            }
            if(pieceColor == "Black" && !blackKingUnderCheck)
            {
                if (checkCastling(pieceRow, pieceColumn, BLACK_CASTLING_ROOK_ROW, SHORT_CASTLING_ROOK_COLUMN) && chessBoard.isFieldEmpty(pieceRow, pieceColumn + 1) && chessBoard.isFieldEmpty(pieceRow, pieceColumn + 2))
                {
                    if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 2) && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn + 1)) || !checkForChecks)
                    {
                        possibleMoves.Add(new Point(pieceRow, pieceColumn + 2));
                        blackShortCastling = true;
                    }
                }
                if (checkCastling(pieceRow, pieceColumn, BLACK_CASTLING_ROOK_ROW, LONG_CASTLING_ROOK_COLUMN) && chessBoard.isFieldEmpty(pieceRow, pieceColumn - 1) && chessBoard.isFieldEmpty(pieceRow, pieceColumn - 2) && chessBoard.isFieldEmpty(pieceRow, pieceColumn - 3))
                {
                    if ((checkForChecks && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 2) && isKingSafeAfterMove(pieceRow, pieceColumn, pieceRow, pieceColumn - 1)) || !checkForChecks)
                    {
                        possibleMoves.Add(new Point(pieceRow, pieceColumn - 2));
                        blackLongCastling = true;
                    }
                }
            }
        }
        List<Point> generateAllPossibleMoves(string pieceColor)
        {
            List<Point> possibleMoves = new List<Point>();
            for (int i = 0; i < CHESSBOARD_SIZE; i++)
            {
                for (int j = 0; j < CHESSBOARD_SIZE; j++)
                {
                    if (!chessBoard.isFieldEmpty(i, j) && chessBoard.getPieceColorFromField(i, j) == pieceColor)
                    {
                        string pieceType = chessBoard.GetPieceTypeFromField(i, j);
                        string type = pieceColor + pieceType;
                        generatePossibleMoves(type, i, j, possibleMoves, false);
                    }
                }
            }
            return possibleMoves;
        }
        public List<Point> checkForKingsDefence(string pieceColor)
        {
            List<Point> possibleMoves = new List<Point>();
            for (int i = 0; i < CHESSBOARD_SIZE; i++)
            {
                for (int j = 0; j < CHESSBOARD_SIZE; j++)
                {
                    if (!chessBoard.isFieldEmpty(i, j) && chessBoard.getPieceColorFromField(i, j) == pieceColor)
                    {
                        string pieceType = chessBoard.GetPieceTypeFromField(i, j);
                        string type = pieceColor + pieceType;
                        generatePossibleMoves(type, i, j, possibleMoves, true);
                    }
                }
            }
            return possibleMoves;
        }
        public bool checkIfAnyKingUnderCheck(string pieceColor)
        {
            if (pieceColor == "White")
                pieceColor = "Black";
            else if (pieceColor == "Black")
                pieceColor = "White"; 

            List<Point> possibleMoves = generateAllPossibleMoves(pieceColor);
            bool check = false;
            foreach (Point possibleMove in possibleMoves)
            {
                int row = (int)possibleMove.X;
                int column = (int)possibleMove.Y;
                if (chessBoard.isFieldAKing(row, column) && pieceColor == "White")
                {
                    blackKingUnderCheck = true;
                    return true;
                }
                else if (chessBoard.isFieldAKing(row, column) && pieceColor == "Black")
                {
                    whiteKingUnderCheck = true;
                    return true;
                }
            }
            if(!check)
            {
                whiteKingUnderCheck = false;
                blackKingUnderCheck = false;
            }
            return check;
        }
        bool isKingSafeAfterMove(int pieceRow, int pieceColumn, int fieldRow, int fieldColumn)
        {
            Piece piece = chessBoard.board[pieceRow, pieceColumn];
            if (piece.type != "Empty")
            {
                Piece field = chessBoard.board[fieldRow, fieldColumn];

                chessBoard.board[fieldRow, fieldColumn] = piece;
                chessBoard.board[pieceRow, pieceColumn] = new Piece("None", "Empty", '.');

                bool kingSafe = !checkIfAnyKingUnderCheck(piece.color);

                chessBoard.board[fieldRow, fieldColumn] = field;
                chessBoard.board[pieceRow, pieceColumn] = piece;

                return kingSafe;
            }
            return true;
        } 
        void highlightKingUnderCheck()
        {
            int blackKingRow = (int)getKingPosition("Black").X;
            int blackKingColumn = (int)getKingPosition("Black").Y;

            int whiteKingRow = (int)getKingPosition("White").X;
            int whiteKingColumn = (int)getKingPosition("White").Y;

            Grid blackKing = dolnaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == blackKingRow && Grid.GetColumn(e) == blackKingColumn) as Grid;

            Grid whiteKing = dolnaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == whiteKingRow && Grid.GetColumn(e) == whiteKingColumn) as Grid;

            if (blackKingUnderCheck)
                blackKing.Background = Brushes.DarkRed;

            if (whiteKingUnderCheck)
                whiteKing.Background = Brushes.DarkRed;

            else if (!whiteKingUnderCheck && !blackKingUnderCheck)
                unhighlightRedFields();
        }
        void unhighlightRedFields()
        {
            for (int i = 0; i < CHESSBOARD_SIZE; i++)
            {
                for (int j = 0; j < CHESSBOARD_SIZE; j++)
                {
                    Grid field = dolnaWarstwa.Children.Cast<UIElement>()
                        .FirstOrDefault(e => Grid.GetRow(e) == i && Grid.GetColumn(e) == j) as Grid;

                    if(field.Background == Brushes.DarkRed)
                    {
                        Grid fieldNeighbour;
                        if (i < CHESSBOARD_MAXIMUM_INDEX)
                            fieldNeighbour = dolnaWarstwa.Children.Cast<UIElement>()
                                .FirstOrDefault(e => Grid.GetRow(e) == i + 1 && Grid.GetColumn(e) == j) as Grid;
                        else
                            fieldNeighbour = dolnaWarstwa.Children.Cast<UIElement>()
                                .FirstOrDefault(e => Grid.GetRow(e) == i - 1 && Grid.GetColumn(e) == j) as Grid;

                        if (fieldNeighbour.Background == Brushes.Wheat)
                        {
                            var converter = new BrushConverter();
                            field.Background = (Brush)converter.ConvertFromString("#4E3524");
                        }
                        else
                            field.Background = Brushes.Wheat;
                    }
                }
            }
        }
        Point getKingPosition(string kingColor)
        {
            for (int i = 0; i < CHESSBOARD_SIZE; i++)
            {
                for (int j = 0; j < CHESSBOARD_SIZE; j++)
                {
                    if (chessBoard.isFieldAKing(i, j) && chessBoard.getPieceColorFromField(i, j) == kingColor)
                        return new Point(i, j);
                }
            }
            return new Point(-1, -1);
        }
        bool checkCastling(int kingRow, int kingColumn, int rookRow, int rookColumn)
        {
            if (chessBoard.firstMoveOfPiece(kingRow, kingColumn) && chessBoard.GetPieceTypeFromField(kingRow, kingColumn) == "King" && chessBoard.firstMoveOfPiece(rookRow, rookColumn) && chessBoard.GetPieceTypeFromField(rookRow, rookColumn) == "Rook")
                return true;
            return false;
        }
        void checkPawnDoubleMove(int pieceRow, int pieceColumn, int fieldRow)
        {
            string pieceType = chessBoard.GetPieceTypeFromField(pieceRow, pieceColumn);
            if(pieceType == "Pawn")
            {
                if (Math.Abs(pieceRow - fieldRow) == 2)
                    chessBoard.board[pieceRow, pieceColumn].pawnDoubleMoveTurn = game.movesCounter;
            }
        }
        void promotePawn(int row, int column)
        {
            string type = chessBoard.GetPieceTypeFromField(row, column);
            string color = chessBoard.getPieceColorFromField(row, column);
            if (type == "Pawn")
            {
                if(color == "White" && row == CHESSBOARD_MINIMUM_INDEX)
                {
                    if (!promotionPieceSelected)
                    {
                        setWhitePromotionButtonsVisibility(Visibility.Visible);
                        changeChessBoardOpacity(0.5);
                    }
                    else
                    {
                        setWhitePromotionButtonsVisibility(Visibility.Hidden);
                        changeChessBoardOpacity(1);
                    }
                }
                if(color == "Black" && row == CHESSBOARD_MAXIMUM_INDEX)
                {
                    if(!promotionPieceSelected)
                    {
                        setBlackPromotionButtonsVisibility(Visibility.Visible);
                        changeChessBoardOpacity(0.5);
                    }
                    else
                    {
                        setBlackPromotionButtonsVisibility(Visibility.Hidden);
                        changeChessBoardOpacity(1);
                    }
                }
            }
        }
        void setWhitePromotionButtonsVisibility(Visibility visibility)
        {
            PromoteWhiteBishop.Visibility = visibility;
            PromoteWhiteKnight.Visibility = visibility;
            PromoteWhiteQueen.Visibility = visibility;
            PromoteWhiteRook.Visibility = visibility;
        }
        void setBlackPromotionButtonsVisibility(Visibility visibility)
        {
            PromoteBlackBishop.Visibility = visibility;
            PromoteBlackKnight.Visibility = visibility;
            PromoteBlackQueen.Visibility = visibility;
            PromoteBlackRook.Visibility = visibility;
        }
    }
}
