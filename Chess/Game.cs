using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
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
        public List<string> gameHistory;
        public Game(GameState setGameState) 
        {
            gameState = setGameState;
            gameOver = false;
            testMode = false;
            turn = PieceColor.WHITE;
            movesCounter = 0;
            gameHistory = new List<string>();
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
                if (gameState == GameState.DRAW_INSUFFICIENT_MATERIAL)
                    Trace.WriteLine("GAME OVER: DRAW - INSUFFICIENT MATERIAL!");
                if (gameState == GameState.DRAW_THREEFOLD_REPETITION)
                    Trace.WriteLine("GAME OVER: DRAW - THREEFOLD REPETITION!");
                return true;
            }
            return false;
        }
        public void CheckGameState(string currentState)
        {
            if (chessBoard.CheckForKingsDefence(PieceColor.BLACK).Count == 0 && chessBoard.CheckIfKingUnderCheck(PieceColor.BLACK))
                gameState = GameState.WHITE_WON;
            if (chessBoard.CheckForKingsDefence(PieceColor.BLACK).Count == 0 && !chessBoard.CheckIfKingUnderCheck(PieceColor.BLACK))
                gameState = GameState.STALEMATE;

            if (chessBoard.CheckForKingsDefence(PieceColor.WHITE).Count == 0 && chessBoard.CheckIfKingUnderCheck(PieceColor.WHITE))
                gameState = GameState.BLACK_WON;
            if (chessBoard.CheckForKingsDefence(PieceColor.WHITE).Count == 0 && !chessBoard.CheckIfKingUnderCheck(PieceColor.WHITE))
                gameState = GameState.STALEMATE;

            if (CheckForInsufficientMaterial())
                gameState = GameState.DRAW_INSUFFICIENT_MATERIAL;
            if (CheckThreefoldRepetition(currentState))
                gameState = GameState.DRAW_THREEFOLD_REPETITION;
        }
        public void DisablePieces()
        {
            if(!testMode)
            {
                foreach (Button field in window.gornaWarstwa.Children)
                {
                    if (chessBoard.GetPieceColorFromField(Grid.GetRow(field), Grid.GetColumn(field)) == turn)
                        field.IsEnabled = true;
                    else 
                        field.IsEnabled = false;
                }
            }
            else
            {
                foreach (Button field in window.gornaWarstwa.Children)
                {
                    if (chessBoard.GetPieceColorFromField(Grid.GetRow(field), Grid.GetColumn(field)) != PieceColor.NONE)
                        field.IsEnabled = true;
                    else
                        field.IsEnabled = false;
                }
            }
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
        private bool CheckForInsufficientMaterial()
        {
            chessBoard.CountAllPiecesOnBoard();
            if (chessBoard.whitePawns == 0 && chessBoard.blackPawns == 0)
            {
                if (chessBoard.whiteQueens == 0 && chessBoard.blackQueens == 0)
                {
                    if (chessBoard.whiteRooks == 0 && chessBoard.blackRooks == 0)
                    {
                        if (chessBoard.whiteBishops == 2 || chessBoard.blackBishops == 2)
                            return false;
                        else if ((chessBoard.whiteBishops == 1 && chessBoard.whiteKnights == 1) || (chessBoard.blackBishops == 1 && chessBoard.blackKnights == 1))
                            return false;
                        else if ((chessBoard.whiteKnights == 2 && (chessBoard.blackBishops > 0 || chessBoard.blackKnights > 0)))
                            return false;
                        else if ((chessBoard.blackKnights == 2 && (chessBoard.whiteBishops > 0 || chessBoard.whiteKnights > 0)))
                            return false;
                        else
                            return true;
                    }
                }
            }
            return false;
        }
        private bool CheckThreefoldRepetition(string currentState)
        {
            int repetitionCounter = 0;
            foreach (string chessBoardState in gameHistory)
            {
                if(currentState == chessBoardState)
                    repetitionCounter++;

                if (repetitionCounter == 3)
                    return true;
            }
            return false;
        }
        public Piece GetPieceFromPieceType(string pieceType)
        {
            switch (pieceType)
            {
                case "WhitePawn":
                    return new Pawn(chessBoard, this, PieceColor.WHITE);
                case "BlackPawn":
                    return new Pawn(chessBoard, this, PieceColor.BLACK);
                case "WhiteRook":
                    return new Rook(chessBoard, this, PieceColor.WHITE);
                case "BlackRook":
                    return new Rook(chessBoard, this, PieceColor.BLACK);
                case "WhiteKnight":
                    return new Knight(chessBoard, this, PieceColor.WHITE);
                case "BlackKnight":
                    return new Knight(chessBoard, this, PieceColor.BLACK);
                case "WhiteBishop":
                    return new Bishop(chessBoard, this, PieceColor.WHITE);
                case "BlackBishop":
                    return new Bishop(chessBoard, this, PieceColor.BLACK);
                case "WhiteQueen":
                    return new Queen(chessBoard, this, PieceColor.WHITE);
                case "BlackQueen":
                    return new Queen(chessBoard, this, PieceColor.BLACK);
                case "WhiteKing":
                    return new King(chessBoard, this, PieceColor.WHITE);
                case "BlackKing":
                    return new King(chessBoard, this, PieceColor.BLACK);
                default:
                    return new Empty(chessBoard, this, PieceColor.NONE);
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
        STALEMATE,
        DRAW_INSUFFICIENT_MATERIAL,
        DRAW_THREEFOLD_REPETITION
    }
}
