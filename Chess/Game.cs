using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;

namespace Chess
{
    class Game
    {
        public GameState gameState;
        public bool gameOver;
        public MainWindow window;
        public string turn;
        public int movesCounter;
        public bool testMode;
        public Game(GameState setGameState) 
        {
            gameState = setGameState;
            gameOver = false;
            testMode = false;
            turn = "White";
            movesCounter = 0;
        }
        public bool checkIfGameOver()
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
        public void checkGameState()
        {
            if (window.checkForKingsDefence("Black").Count == 0 && window.checkIfAnyKingUnderCheck("Black"))
                gameState = GameState.WHITE_WON;
            if (window.checkForKingsDefence("Black").Count == 0 && !window.checkIfAnyKingUnderCheck("Black"))
                gameState = GameState.STALEMATE;

            if (window.checkForKingsDefence("White").Count == 0 && window.checkIfAnyKingUnderCheck("White"))
                gameState = GameState.BLACK_WON;
            if (window.checkForKingsDefence("White").Count == 0 && !window.checkIfAnyKingUnderCheck("White"))
                gameState = GameState.STALEMATE;
        }
        public void changeTurn()
        {
            if (turn == "White")
            {
                movesCounter++;
                turn = "Black";
                window.turnTextBox.Background = Brushes.Black;
                window.turnTextBox.Foreground = Brushes.White;
            }
            else
            {
                turn = "White";
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
