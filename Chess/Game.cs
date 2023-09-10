using System.Diagnostics;
using System.Windows.Media;

namespace Chess
{
    public class Game
    {
        public GameState gameState;
        public bool gameOver;
        public MainWindow window;
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
    }
    public enum GameState
    {
        IN_PROGRESS,
        WHITE_WON,
        BLACK_WON,
        STALEMATE
    }
}
