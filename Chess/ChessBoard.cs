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
        public ChessBoardGUI chessBoardGUI;
        public Game game;

        public bool whiteShortCastling = false;
        public bool whiteLongCastling = false;
        public bool blackShortCastling = false;
        public bool blackLongCastling = false;

        public bool whiteKingUnderCheck = false;
        public bool blackKingUnderCheck = false;

        public readonly int minimumIndex = 0;
        public readonly int maximumIndex = 7;
        public readonly int size = 8;

        private readonly int LONG_CASTLING_KING_COLUMN = 2;
        private readonly int SHORT_CASTLING_KING_COLUMN = 6;
        public readonly int WHITE_CASTLING_ROOK_ROW = 7;
        public readonly int BLACK_CASTLING_ROOK_ROW = 0;
        public readonly int SHORT_CASTLING_ROOK_COLUMN = 7;
        private readonly int SHORT_CASTLING_EMPTYFIELD_COLUMN = 5;
        public readonly int LONG_CASTLING_ROOK_COLUMN = 0;
        private readonly int LONG_CASTLING_EMPTYFIELD_COLUMN = 3;

        public int whitePawns, blackPawns, whiteKnights, blackKnights, whiteBishops, blackBishops, whiteRooks, blackRooks, whiteQueens, blackQueens;

        public void Clear()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new Empty(this, game, PieceColor.NONE);
                }
            }
        }
        public void Initialize()
        {
            Clear();
            board[0, 0] = new Rook(this, game, PieceColor.BLACK);
            board[0, 1] = new Knight(this, game, PieceColor.BLACK);
            board[0, 2] = new Bishop(this, game, PieceColor.BLACK);
            board[0, 3] = new Queen(this, game, PieceColor.BLACK);
            board[0, 4] = new King(this, game, PieceColor.BLACK);
            board[0, 5] = new Bishop(this, game, PieceColor.BLACK);
            board[0, 6] = new Knight(this, game, PieceColor.BLACK);
            board[0, 7] = new Rook(this, game, PieceColor.BLACK);

            for (int i = 0; i < 8; i++)
                board[1, i] = new Pawn(this, game, PieceColor.BLACK);

            board[7, 0] = new Rook(this, game, PieceColor.WHITE);
            board[7, 1] = new Knight(this, game, PieceColor.WHITE);
            board[7, 2] = new Bishop(this, game, PieceColor.WHITE);
            board[7, 3] = new Queen(this, game, PieceColor.WHITE);
            board[7, 4] = new King(this, game, PieceColor.WHITE);
            board[7, 5] = new Bishop(this, game, PieceColor.WHITE);
            board[7, 6] = new Knight(this, game, PieceColor.WHITE);
            board[7, 7] = new Rook(this, game, PieceColor.WHITE);

            for (int i = 0; i < 8; i++)
                board[6, i] = new Pawn(this, game, PieceColor.WHITE);
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
                if ((checkForChecks && IsKingSafeAfterMove(pieceRow, pieceColumn, fieldRow, fieldColumn)) || !checkForChecks)
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
            if(FirstMoveOfPiece((int)GetKingPosition(PieceColor.WHITE).X, (int)GetKingPosition(PieceColor.WHITE).Y))
            {
                if (whiteShortCastling && pieceType == "WhiteKing" && fieldColumn == SHORT_CASTLING_KING_COLUMN)
                {
                    chessBoardGUI.PerformCastling(WHITE_CASTLING_ROOK_ROW, SHORT_CASTLING_ROOK_COLUMN, WHITE_CASTLING_ROOK_ROW, SHORT_CASTLING_EMPTYFIELD_COLUMN);
                    whiteShortCastling = false;
                }
                if (whiteLongCastling && pieceType == "WhiteKing" && fieldColumn == LONG_CASTLING_KING_COLUMN)
                {
                    chessBoardGUI.PerformCastling(WHITE_CASTLING_ROOK_ROW, LONG_CASTLING_ROOK_COLUMN, WHITE_CASTLING_ROOK_ROW, LONG_CASTLING_EMPTYFIELD_COLUMN);
                    whiteLongCastling = false;
                }
            }
            if (FirstMoveOfPiece((int)GetKingPosition(PieceColor.BLACK).X, (int)GetKingPosition(PieceColor.BLACK).Y))
            {
                if (blackShortCastling && pieceType == "BlackKing" && fieldColumn == SHORT_CASTLING_KING_COLUMN)
                {
                    chessBoardGUI.PerformCastling(BLACK_CASTLING_ROOK_ROW, SHORT_CASTLING_ROOK_COLUMN, BLACK_CASTLING_ROOK_ROW, SHORT_CASTLING_EMPTYFIELD_COLUMN);
                    blackShortCastling = false;
                }
                if (blackLongCastling && pieceType == "BlackKing" && fieldColumn == LONG_CASTLING_KING_COLUMN)
                {
                    chessBoardGUI.PerformCastling(BLACK_CASTLING_ROOK_ROW, LONG_CASTLING_ROOK_COLUMN, BLACK_CASTLING_ROOK_ROW, LONG_CASTLING_EMPTYFIELD_COLUMN);
                    blackLongCastling = false;
                }
            }
        }
        public void GeneratePossibleMovesForPiece(int pieceRow, int pieceColumn, List<Point> possibleMoves, bool checkForChecks)
        {
            Piece piece = GetPieceFromField(pieceRow, pieceColumn);
            piece.GeneratePossibleMoves(pieceRow, pieceColumn, possibleMoves, checkForChecks);
        }
        public bool CheckIfKingUnderCheck(PieceColor pieceColor)
        {
            List<Point> possibleMoves = new List<Point>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (!IsFieldEmpty(i, j) && GetPieceColorFromField(i, j) != pieceColor)
                    {
                        GeneratePossibleMovesForPiece(i, j, possibleMoves, false);
                        if (possibleMoves.Contains(GetKingPosition(pieceColor)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
        public void CheckBothKingsForChecks()
        {
            blackKingUnderCheck = CheckIfKingUnderCheck(PieceColor.BLACK);
            whiteKingUnderCheck = CheckIfKingUnderCheck(PieceColor.WHITE);
        }
        public bool IsKingSafeAfterMove(int pieceRow, int pieceColumn, int fieldRow, int fieldColumn)
        {
            Piece piece = board[pieceRow, pieceColumn];
            if (piece.type != PieceType.EMPTY)
            {
                Piece field = board[fieldRow, fieldColumn];

                board[fieldRow, fieldColumn] = piece;
                board[pieceRow, pieceColumn] = new Empty(this, game, PieceColor.NONE);

                bool kingSafe = !CheckIfKingUnderCheck(piece.color);

                board[fieldRow, fieldColumn] = field;
                board[pieceRow, pieceColumn] = piece;

                return kingSafe;
            }
            return true;
        }
        public List<Point> CheckForKingsDefence(PieceColor pieceColor)
        {
            List<Point> possibleMoves = new List<Point>();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    if (!IsFieldEmpty(i, j) && GetPieceColorFromField(i, j) == pieceColor)
                    {
                        GeneratePossibleMovesForPiece(i, j, possibleMoves, true);
                    }
                }
            }
            return possibleMoves;
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
