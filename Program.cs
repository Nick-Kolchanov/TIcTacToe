using System;

namespace TicTacToe
{
    class Program
    {
        static void Main()
        {
            var gameObj = new TicTacToeGame();
            gameObj.PlayTicTacToe();

            // TODO: External UI, Restart, Local Multiplayer
        }
    }


    public class TicTacToeGame
    {
        #region Variables
        private int[,] gameBoard;
        private int gameBoardSize;
        private const int DEFAULT_BOARD_SIZE = 3;
        private bool isPlayerTurn = true;

        #endregion

        #region Initializations
        public TicTacToeGame() : this(DEFAULT_BOARD_SIZE)
        {

        }

        public TicTacToeGame(int gameFieldSize_)
        {
            Init(gameFieldSize_);
        }

        private void Init(int gameFieldSize_)
        {
            gameBoardSize = gameFieldSize_;
            gameBoard = GetNewPlayField();
        }

        private int[,] GetNewPlayField()
        {
            var newPlayField = new int[gameBoardSize, gameBoardSize];
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    newPlayField[i, j] = 0;
                }
            }
            return newPlayField;
        }

        public void SetPlayerTurn(bool playerturns)
        {
            isPlayerTurn = playerturns;
        }

        #endregion

        public void PlayTicTacToe()
        {
            while (IsFreeCellsLeft() && !IsWinCondition())
            {
                ShowGameField();
                MakeTurn();
                ChangeTurn();
            }

            ShowGameField();
            ShowEndGameMessage();
        }

        private bool IsFreeCellsLeft()
        {
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    if (gameBoard[i, j] == 0) return true;
                }
            }

            return false;
        }

        #region Checking for win
        private bool IsWinCondition()
        {
            if (!isCleanBoard())
            {
                return CheckRowsForWin() ||
                       CheckColumnsForWin() ||
                       CheckDiagForWin();
            }
            else
            {
                return false;
            }
        }

        private bool CheckRowsForWin()
        {
            bool isWin = true;

            for (int i = 0; i < gameBoardSize; i++)
            {
                isWin = true;

                for (int j = 1; j < gameBoardSize; j++)
                {
                    if (gameBoard[i, j] != gameBoard[i, j - 1] ||
                        gameBoard[i, j] == 0)
                        isWin = false;
                }

                if (isWin) return true;
            }

            return false;
        }

        private bool CheckColumnsForWin()
        {
            bool isWin = true;

            for (int i = 0; i < gameBoardSize; i++)
            {
                isWin = true;

                for (int j = 1; j < gameBoardSize; j++)
                {
                    if (gameBoard[j, i] != gameBoard[j - 1, i] ||
                        gameBoard[j, i] == 0)
                        isWin = false;
                }

                if (isWin) return true;
            }

            return isWin;
        }

        private bool CheckDiagForWin()
        {
            bool isWin = true;

            for (int i = 1; i < gameBoardSize; i++)
            {
                if (gameBoard[i, i] != gameBoard[i - 1, i - 1] ||
                    gameBoard[i, i] == 0)
                    isWin = false;
            }

            if (isWin) return true;

            isWin = true;

            for (int i = 1; i < gameBoardSize; i++)
            {
                if (gameBoard[gameBoardSize - i - 1, i] != gameBoard[gameBoardSize - i, i - 1] ||
                    gameBoard[gameBoardSize - i - 1, i] == 0)
                    isWin = false;
            }

            return isWin;
        }

        private bool isCleanBoard()
        {
            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    if (gameBoard[i, j] != 0) return false;
                }
            }

            return true;
        }

        #endregion

        private void ShowGameField()
        {
            Console.Clear();

            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    Console.Write($"{MakeXAndO(gameBoard[i, j])} ");
                }
                Console.WriteLine();
            }
        }

        private char MakeXAndO(int ind)
        {
            if (ind == 1) return 'X';
            if (ind == -1) return 'O';
            return '-';
        }

        private void MakeTurn()
        {
            if (isPlayerTurn)
            {
                MakePlayerTurn();
            }
            else
            {
                MakeAITurn();
            }
        }

        private void ChangeTurn()
        {
            isPlayerTurn = !isPlayerTurn;
        }

        private void MakePlayerTurn()
        {
            (int x, int y) playerChoicedCell = ChooseCellPlayer();

            gameBoard[playerChoicedCell.x, playerChoicedCell.y] = 1;
        }

        private void MakeAITurn()
        {
            (int x, int y) AIChoicedCell = ChooseCellAI();

            gameBoard[AIChoicedCell.x, AIChoicedCell.y] = -1;
        }

        #region Choosing player cell
        private (int, int) ChooseCellPlayer()
        {
            Console.WriteLine("Выберите клетку!");

            int x = 1, y = 1;
            bool ok = false;
            while (!ok)
            {
                InputInt32("Введите x:", out x, num => num <= gameBoardSize && num > 0);
                InputInt32("Введите y:", out y, num => num <= gameBoardSize && num > 0);

                ok = isAvailablePoint((x - 1, y - 1));

                if (!ok)
                    Console.WriteLine("Клетка занята. Попробуйте заново.");
            }

            return (x - 1, y - 1);
        }

        private static void InputInt32(string str, out Int32 number, Predicate<Int32> func)
        {
            bool ok = false;
            number = -1;

            while (!ok)
            {
                Console.WriteLine(str);
                ok = Int32.TryParse(Console.ReadLine(), out number);
                if (!func(number)) ok = false;

                if (!ok) Console.WriteLine("Ошибка с вводом числа!");
            }
        }

        #endregion

        #region Choosing AI cell
        private (int, int) ChooseCellAI() //TODO: Better (non-random) AI
        {
            var cell = FindUsedCell();

            if (cell != (-1, -1))
                cell = FindFreeCellAround(cell);

            if (cell == (-1, -1))
                cell = FindFreeCell();

            return cell;
        }

        private (int, int) FindUsedCell()
        {
            var r = new Random(DateTime.Now.Millisecond);
            var cell = (-1, -1);

            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    if (gameBoard[i, j] == -1)
                    {
                        cell = (i, j);
                        if (r.Next(0, 2) == 0)
                        {
                            return cell;
                        }
                    }
                }
            }

            return cell;
        }

        public (int, int) FindFreeCellAround((int, int) cell)
        {
            for (int i = -1; i < 2; i++)
            {
                for (int j = -1; j < 2; j++)
                {
                    if (isAvailablePoint((cell.Item1 + i, cell.Item2 + j)))
                        return (cell.Item1 + i, cell.Item2 + j);
                }
            }

            return (-1, -1);
        }

        private (int, int) FindFreeCell()
        {
            var r = new Random(DateTime.Now.Millisecond);
            var cell = (-1, -1);

            for (int i = 0; i < gameBoardSize; i++)
            {
                for (int j = 0; j < gameBoardSize; j++)
                {
                    if (gameBoard[i, j] == 0)
                    {
                        cell = (i, j);

                        if (r.Next(0, 3) == 0)
                        {
                            return cell;
                        }
                    }
                }
            }

            return cell;
        }

        #endregion

        private bool isAvailablePoint((int, int) point)
        {
            if (point.Item1 < gameBoardSize && point.Item1 >= 0 &&
                point.Item2 < gameBoardSize && point.Item2 >= 0)
            {
                if (gameBoard[point.Item1, point.Item2] == 0)
                    return true;
            }

            return false;
        }

        private void ShowEndGameMessage()
        {
            if (IsFreeCellsLeft() || IsWinCondition())
            {
                if (isPlayerTurn)
                {
                    Console.WriteLine("Победил Компьютер!");
                }
                else
                {
                    Console.WriteLine("Вы победили!");
                }
            }
            else
            {
                Console.WriteLine("Ничья!");
            }

            Console.WriteLine("Нажмите любую кнопку, чтобы выйти из игры.");
            Console.ReadLine();
        }
    }
}
