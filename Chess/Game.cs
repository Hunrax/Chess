using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Controls;

namespace Chess
{
    public class Game
    {
        public GameState gameState;
        public PieceColor turn;
        public MainWindow window;
        public ChessBoard chessBoard;
        public int movesCounter;
        public bool testMode;
        public List<string> gameHistory;
        public string gameOverMessage;
        public Game(GameState setGameState, bool setTestMode) 
        {
            gameState = setGameState;
            testMode = setTestMode;
            turn = PieceColor.WHITE;
            movesCounter = 0;
            gameHistory = new List<string>();
        }
        public bool CheckIfGameOver()
        {
            if (gameState != GameState.IN_PROGRESS)
            {
                if (gameState == GameState.STALEMATE)
                    gameOverMessage = "STALEMATE!";
                if (gameState == GameState.WHITE_WON)
                    gameOverMessage = "WHITE WINS!";
                if (gameState == GameState.BLACK_WON)
                    gameOverMessage = "BLACK WINS!";
                if (gameState == GameState.DRAW_INSUFFICIENT_MATERIAL)
                    gameOverMessage = "DRAW - INSUFFICIENT MATERIAL!";
                if (gameState == GameState.DRAW_THREEFOLD_REPETITION)
                    gameOverMessage = "DRAW - THREEFOLD REPETITION!";
                Trace.WriteLine(gameOverMessage);
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
        public void DisablePieces() // Move it to Window? It operates on Buttons, Game should operate on Pieces only
        {
            if(gameState == GameState.IN_PROGRESS)
            {
                if (!testMode)
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
        }
        public void ChangeTurn()
        {
            if (turn == PieceColor.WHITE)
            {
                movesCounter++;
                turn = PieceColor.BLACK;
            }
            else if (turn == PieceColor.BLACK)
            {
                turn = PieceColor.WHITE;
            }
            window.movesCounterTextBox.Text = movesCounter.ToString(); //This is tempting but game should not know anything about windows and textBoxes
        }
        private bool CheckForInsufficientMaterial() // Since this function uses only chessBoard fields it probably belongs to chessBoard class
        {
            chessBoard.CountAllPiecesOnBoard();
            if (chessBoard.whitePawns == 0 && chessBoard.blackPawns == 0) // 0 pawns
            {
                if (chessBoard.whiteQueens == 0 && chessBoard.blackQueens == 0) // 0 queens
                {
                    if (chessBoard.whiteRooks == 0 && chessBoard.blackRooks == 0) // 0 rooks
                    {
                        if (chessBoard.whiteBishops == 2 || chessBoard.blackBishops == 2) // 2 bishops
                            return false;
                        else if ((chessBoard.whiteBishops == 1 && chessBoard.whiteKnights == 1) || (chessBoard.blackBishops == 1 && chessBoard.blackKnights == 1)) // bishop and knight
                            return false;
                        else if ((chessBoard.whiteKnights == 2 && (chessBoard.blackBishops > 0 || chessBoard.blackKnights > 0))) // 2 white knights and black has something
                            return false;
                        else if ((chessBoard.blackKnights == 2 && (chessBoard.whiteBishops > 0 || chessBoard.whiteKnights > 0))) // 2 black knights and white has something
                            return false;
                        else // insufficient material
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
                    return new Pawn(chessBoard, PieceColor.WHITE);
                case "BlackPawn":
                    return new Pawn(chessBoard, PieceColor.BLACK);
                case "WhiteRook":
                    return new Rook(chessBoard, PieceColor.WHITE);
                case "BlackRook":
                    return new Rook(chessBoard, PieceColor.BLACK);
                case "WhiteKnight":
                    return new Knight(chessBoard, PieceColor.WHITE);
                case "BlackKnight":
                    return new Knight(chessBoard, PieceColor.BLACK);
                case "WhiteBishop":
                    return new Bishop(chessBoard, PieceColor.WHITE);
                case "BlackBishop":
                    return new Bishop(chessBoard, PieceColor.BLACK);
                case "WhiteQueen":
                    return new Queen(chessBoard, PieceColor.WHITE);
                case "BlackQueen":
                    return new Queen(chessBoard, PieceColor.BLACK);
                case "WhiteKing":
                    return new King(chessBoard, PieceColor.WHITE);
                case "BlackKing":
                    return new King(chessBoard, PieceColor.BLACK);
                default:
                    return new Empty(chessBoard, PieceColor.NONE);
            }
        }
        public PieceColor GetPieceColorFromPieceType(string pieceType)  // Not used
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
