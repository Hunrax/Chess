using System.Diagnostics;
using System.Windows.Media;

namespace Chess
{
    public class Game
    {
        public GameState gameState;
        public bool gameOver;
        public MainWindow window;
        public ChessBoard chessBoard;
        public PieceColor turn;
        public int movesCounter;
        public bool testMode;
        public Game(GameState setGameState) 
        {
            gameState = setGameState;
            gameOver = false;
            testMode = false;
            turn = PieceColor.WHITE;
            movesCounter = 0;
        }
        public bool CheckIfGameOver()
        {
            if (gameState != GameState.IN_PROGRESS)
            {
                if (gameState == GameState.STALEMATE)
                    Trace.WriteLine("GAME OVER: STALEMATE!");
                if (gameState == GameState.WHITE_WON)
                    Trace.WriteLine("GAME OVER: WHITE WINS!");
                if (gameState == GameState.BLACK_WON)
                    Trace.WriteLine("GAME OVER: BLACK WINS!");
                return true;
            }
            return false;
        }
        public void CheckGameState()
        {
            if (window.CheckForKingsDefence(PieceColor.BLACK).Count == 0 && window.CheckIfAnyKingUnderCheck(PieceColor.BLACK))
                gameState = GameState.WHITE_WON;
            if (window.CheckForKingsDefence(PieceColor.BLACK).Count == 0 && !window.CheckIfAnyKingUnderCheck(PieceColor.BLACK))
                gameState = GameState.STALEMATE;

            if (window.CheckForKingsDefence(PieceColor.WHITE).Count == 0 && window.CheckIfAnyKingUnderCheck(PieceColor.WHITE))
                gameState = GameState.BLACK_WON;
            if (window.CheckForKingsDefence(PieceColor.WHITE).Count == 0 && !window.CheckIfAnyKingUnderCheck(PieceColor.WHITE))
                gameState = GameState.STALEMATE;
        }
        public void ChangeTurn()
        {
            if (turn == PieceColor.WHITE)
            {
                movesCounter++;
                turn = PieceColor.BLACK;
                window.turnTextBox.Background = Brushes.Black;
                window.turnTextBox.Foreground = Brushes.White;
            }
            else if (turn == PieceColor.BLACK)
            {
                turn = PieceColor.WHITE;
                window.turnTextBox.Background = Brushes.White;
                window.turnTextBox.Foreground = Brushes.Black;
            }
            window.movesCounterTextBox.Text = movesCounter.ToString();
        }
        public Piece GetPieceFromPieceType(string pieceType)
        {
            switch (pieceType)
            {
                case "WhitePawn":
                    return new Pawn(window, chessBoard, this, PieceColor.WHITE);
                case "BlackPawn":
                    return new Pawn(window, chessBoard, this, PieceColor.BLACK);
                case "WhiteRook":
                    return new Rook(window, chessBoard, this, PieceColor.WHITE);
                case "BlackRook":
                    return new Rook(window, chessBoard, this, PieceColor.BLACK);
                case "WhiteKnight":
                    return new Knight(window, chessBoard, this, PieceColor.WHITE);
                case "BlackKnight":
                    return new Knight(window, chessBoard, this, PieceColor.BLACK);
                case "WhiteBishop":
                    return new Bishop(window, chessBoard, this, PieceColor.WHITE);
                case "BlackBishop":
                    return new Bishop(window, chessBoard, this, PieceColor.BLACK);
                case "WhiteQueen":
                    return new Queen(window, chessBoard, this, PieceColor.WHITE);
                case "BlackQueen":
                    return new Queen(window, chessBoard, this, PieceColor.BLACK);
                case "WhiteKing":
                    return new King(window, chessBoard, this, PieceColor.WHITE);
                case "BlackKing":
                    return new King(window, chessBoard, this, PieceColor.BLACK);
                default:
                    return new Empty(window, chessBoard, this, PieceColor.NONE);
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
    }
    public enum GameState
    {
        IN_PROGRESS,
        WHITE_WON,
        BLACK_WON,
        STALEMATE
    }
}
