using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chess
{
    public class ChessBoard
    {
        public Piece[,] board = new Piece[8,8];

        public MainWindow window;
        public Game game;

        public bool whiteShortCastling = false;
        public bool whiteLongCastling = false;
        public bool blackShortCastling = false;
        public bool blackLongCastling = false;

        public int minimumIndex = 0;
        public int maximumIndex = 7;
        public int size = 8;

        public int LONG_CASTLING_KING_COLUMN = 2;
        public int SHORT_CASTLING_KING_COLUMN = 6;
        public int WHITE_CASTLING_ROOK_ROW = 7;
        public int BLACK_CASTLING_ROOK_ROW = 0;
        public int SHORT_CASTLING_ROOK_COLUMN = 7;
        public int SHORT_CASTLING_EMPTYFIELD_COLUMN = 5;
        public int LONG_CASTLING_ROOK_COLUMN = 0;
        public int LONG_CASTLING_EMPTYFIELD_COLUMN = 3;

        public int whitePawns, blackPawns, whiteKnights, blackKnights, whiteBishops, blackBishops, whiteRooks, blackRooks, whiteQueens, blackQueens;

        public void Clear()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new Empty(window, this, game, PieceColor.NONE);
                }
            }
        }
        public void Initialize()
        {
            Clear();
            board[0, 0] = new Rook(window, this, game, PieceColor.BLACK);
            board[0, 1] = new Knight(window, this, game, PieceColor.BLACK);
            board[0, 2] = new Bishop(window, this, game, PieceColor.BLACK);
            board[0, 3] = new Queen(window, this, game, PieceColor.BLACK);
            board[0, 4] = new King(window, this, game, PieceColor.BLACK);
            board[0, 5] = new Bishop(window, this, game, PieceColor.BLACK);
            board[0, 6] = new Knight(window, this, game, PieceColor.BLACK);
            board[0, 7] = new Rook(window, this, game, PieceColor.BLACK);

            for (int i = 0; i < 8; i++)
                board[1, i] = new Pawn(window, this, game, PieceColor.BLACK);

            board[7, 0] = new Rook(window, this, game, PieceColor.WHITE);
            board[7, 1] = new Knight(window, this, game, PieceColor.WHITE);
            board[7, 2] = new Bishop(window, this, game, PieceColor.WHITE);
            board[7, 3] = new Queen(window, this, game, PieceColor.WHITE);
            board[7, 4] = new King(window, this, game, PieceColor.WHITE);
            board[7, 5] = new Bishop(window, this, game, PieceColor.WHITE);
            board[7, 6] = new Knight(window, this, game, PieceColor.WHITE);
            board[7, 7] = new Rook(window, this, game, PieceColor.WHITE);

            for (int i = 0; i < 8; i++)
                board[6, i] = new Pawn(window, this, game, PieceColor.WHITE);
        }
        public Piece GetPieceFromField(int row, int column)
        {
            return board[row, column];
        }
        public PieceType GetPieceTypeFromField(int row, int column)
        {
            return board[row, column].type;
        }
        public PieceColor GetPieceColorFromField(int row, int column)
        {
            return board[row, column].color;
        }
        public char GetPieceSymbolFromField(int row, int column)
        {
            return board[row, column].symbol;
        }
        public bool IsFieldEmpty(int row, int column)
        {
            if (row < 0 || row > 7 || column < 0 || column > 7)
                return false;
            if (board[row, column].type == PieceType.EMPTY)
                return true;
            return false;
        }
        public bool IsFieldPossibleToCapture(int row, int column, PieceColor pieceColor)
        {
            if (row < 0 || row > 7 || column < 0 || column > 7)
                return false;
            if (board[row, column].type != PieceType.EMPTY && pieceColor != board[row, column].color)
                return true;
            return false;
        }
        public bool IsFieldAKing(int row, int column)
        {
            if (board[row, column].type == PieceType.KING)
                return true;
            return false;
        }
        public bool FirstMoveOfPiece(int pieceRow, int pieceColumn)
        {
            return board[pieceRow, pieceColumn].firstMove;
        }
        public bool ArePointsEqual(Point point, int x, int y)
        {
            if (point.X == x && point.Y == y)
                return true;
            return false;
        }
        public bool CanPieceMoveHere(int fieldRow, int fieldColumn, PieceColor pieceColor, bool checkForChecks, int pieceRow, int pieceColumn)
        {
            if (IsFieldEmpty(fieldRow, fieldColumn) || IsFieldPossibleToCapture(fieldRow, fieldColumn, pieceColor))
            {
                if ((checkForChecks && window.IsKingSafeAfterMove(pieceRow, pieceColumn, fieldRow, fieldColumn)) || !checkForChecks)
                    return true;
                return false;
            }
            return false;
        }
        public Point GetKingPosition(PieceColor kingColor)
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (IsFieldAKing(i, j) && GetPieceColorFromField(i, j) == kingColor)
                        return new Point(i, j);
                }
            }
            return new Point(-1, -1);
        }
        public int CountPieces(PieceType pieceType, PieceColor pieceColor)
        {
            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if(GetPieceTypeFromField(i, j) == pieceType && GetPieceColorFromField(i, j) == pieceColor)
                        counter++;
                }
            }
            return counter;
        }
        public void CountAllPiecesOnBoard()
        {
            whitePawns = CountPieces(PieceType.PAWN, PieceColor.WHITE);
            blackPawns = CountPieces(PieceType.PAWN, PieceColor.BLACK);
            whiteKnights = CountPieces(PieceType.KNIGHT, PieceColor.WHITE);
            blackKnights = CountPieces(PieceType.KNIGHT, PieceColor.BLACK);
            whiteBishops = CountPieces(PieceType.BISHOP, PieceColor.WHITE);
            blackBishops = CountPieces(PieceType.BISHOP, PieceColor.BLACK);
            whiteRooks = CountPieces(PieceType.ROOK, PieceColor.WHITE);
            blackRooks = CountPieces(PieceType.ROOK, PieceColor.BLACK);
            whiteQueens = CountPieces(PieceType.QUEEN, PieceColor.WHITE);
            blackQueens = CountPieces(PieceType.QUEEN, PieceColor.BLACK);
        }
        public void CheckAllCastlings(string pieceType, int fieldColumn)
        {
            if (whiteShortCastling && pieceType == "WhiteKing" && fieldColumn == SHORT_CASTLING_KING_COLUMN)
            {
                window.PerformCastling(WHITE_CASTLING_ROOK_ROW, SHORT_CASTLING_ROOK_COLUMN, WHITE_CASTLING_ROOK_ROW, SHORT_CASTLING_EMPTYFIELD_COLUMN);
                whiteShortCastling = false;
            }
            if (whiteLongCastling && pieceType == "WhiteKing" && fieldColumn == LONG_CASTLING_KING_COLUMN)
            {
                window.PerformCastling(WHITE_CASTLING_ROOK_ROW, LONG_CASTLING_ROOK_COLUMN, WHITE_CASTLING_ROOK_ROW, LONG_CASTLING_EMPTYFIELD_COLUMN);
                whiteLongCastling = false;
            }

            if (blackShortCastling && pieceType == "BlackKing" && fieldColumn == SHORT_CASTLING_KING_COLUMN)
            {
                window.PerformCastling(BLACK_CASTLING_ROOK_ROW, SHORT_CASTLING_ROOK_COLUMN, BLACK_CASTLING_ROOK_ROW, SHORT_CASTLING_EMPTYFIELD_COLUMN);
                blackShortCastling = false;
            }
            if (blackLongCastling && pieceType == "BlackKing" && fieldColumn == LONG_CASTLING_KING_COLUMN)
            {
                window.PerformCastling(BLACK_CASTLING_ROOK_ROW, LONG_CASTLING_ROOK_COLUMN, BLACK_CASTLING_ROOK_ROW, LONG_CASTLING_EMPTYFIELD_COLUMN);
                blackLongCastling = false;
            }
        }
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

                if (!IsFieldAKing((int)possibleMoves[i].X, (int)possibleMoves[i].Y))
                {
                    gridDolnaWarstwa.Opacity = 1;
                    gridGornaWarstwa.IsEnabled = true;
                }
            }
        }
        public void HighlightKingUnderCheck()
        {
            int blackKingRow = (int)GetKingPosition(PieceColor.BLACK).X;
            int blackKingColumn = (int)GetKingPosition(PieceColor.BLACK).Y;

            int whiteKingRow = (int)GetKingPosition(PieceColor.WHITE).X;
            int whiteKingColumn = (int)GetKingPosition(PieceColor.WHITE).Y;

            Grid blackKing = window.dolnaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == blackKingRow && Grid.GetColumn(e) == blackKingColumn) as Grid;

            Grid whiteKing = window.dolnaWarstwa.Children.Cast<UIElement>()
                .FirstOrDefault(e => Grid.GetRow(e) == whiteKingRow && Grid.GetColumn(e) == whiteKingColumn) as Grid;

            if (window.blackKingUnderCheck)
                blackKing.Background = Brushes.DarkRed;

            if (window.whiteKingUnderCheck)
                whiteKing.Background = Brushes.DarkRed;

            else if (!window.whiteKingUnderCheck && !window.blackKingUnderCheck)
                UnhighlightRedFields();
        }
        private void UnhighlightRedFields()
        {
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    Grid field = window.dolnaWarstwa.Children.Cast<UIElement>()
                        .FirstOrDefault(e => Grid.GetRow(e) == i && Grid.GetColumn(e) == j) as Grid;

                    if (field.Background == Brushes.DarkRed)
                    {
                        Grid fieldNeighbour;
                        if (i < maximumIndex)
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
        public void Print()
        {
            for(int i = 0; i < size; i++)
            {
                for(int j = 0; j < size; j++)
                {
                    Trace.Write(board[i, j].symbol + " ");
                }
                Trace.WriteLine("");
            }
            Trace.WriteLine("");
        }
        public string ChessboardToString()
        {
            char[] chessBoardToString = new char[64];

            int counter = 0;
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    chessBoardToString[counter] = GetPieceSymbolFromField(i, j);
                    counter++;
                }
            }
            return new string(chessBoardToString);
        }
    }
}
