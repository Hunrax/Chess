using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    class Game
    {
        public GameState gameState;
        public Game(GameState setGameState) 
        {
            gameState = setGameState;
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
