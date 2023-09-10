using System.Diagnostics;
using System.Windows;

namespace Chess
{
    public class ChessBoard
    {
        public Piece[,] board = new Piece[8,8];

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

        public void Clear(MainWindow window, Game game)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    board[i, j] = new Empty(window, this, game, PieceColor.NONE);
                }
            }
        }
        public void Initialize(MainWindow window, Game game)
        {
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
        public void Print()
        {
            for(int i = 0; i < 8; i++)
            {
                for(int j = 0; j < 8; j++)
                {
                    Trace.Write(board[i, j].symbol + " ");
                }
                Trace.WriteLine("");
            }
            Trace.WriteLine("");
        }
    }
}
