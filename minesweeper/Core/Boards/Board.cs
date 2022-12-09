using System;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace minesweeper.Core.Boards
{
    public class Board
    {
        public Minesweeper Minesweeper { get; set; }

        public BoardPainter Painter { get; set; }

        public int Width { get; set; }                                  // Ширина
        public int Height { get; set; }                                 // Высота

        public int NumMines { get; set; }                               // Количество мин на поле

        public GameMode Mode { get; set; }
        public int NumMinesRemaining => NumMines - FlagsPlaced();       // Количество оставшихся мин

        public Cell[,] Cells { get; set; }                              // Ячейки

        public bool GameOver { get; set; }                              // Окончание игры

        public int Time { get; set; }                                   // Время игры

        public bool IsFirstStep { get; set; }                           // Первый ли ход?



        public const int CellSize = 32;                                 // Размер ячейки



        // Конструктор поля
        public Board(Minesweeper minesweeper, int width, int height, int mines, GameMode mode)
        {
            Minesweeper = minesweeper;
            Width = width;
            Height = height;
            NumMines = mines;
            Mode = mode;
            Cells = new Cell[width, height];
            Painter = new BoardPainter { Board = this };
        }

        // Подготовка поля
        public void SetupBoard()
        {
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cells[x, y] = new Cell(x, y, this);
                }
            }
            Time = 0;
            IsFirstStep = true;
            GameOver = false;
        }

        // Расстановка мин
        public void PlaceMines(int xcur = -1, int ycur = -1)
        {
            var MinesPlaced = 0;
            var random = new Random();

            while (MinesPlaced < NumMines)
            {
                int x = random.Next(0, Width);
                int y = random.Next(0, Height);

                if (!Cells[x, y].IsMine && x != xcur && y != ycur)
                {
                    Cells[x, y].CellType = CellType.Mine;
                    MinesPlaced++;
                }
            }

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var c = Cells[x, y];
                    c.NumMines = c.GetNeighborCells().Where(n => n.IsMine).Count();
                }
            }

            Minesweeper.Invalidate();
        }

        // Проигрыш. Показать мины и перезапустить игру
        public void RevealMines()
        {
            
            // Показать мины
            GameOver = true;
            Minesweeper.Invalidate();

            // Запрос сыграть снова
            HandleGameOver(gameWon: false);
        }

        // Окончание игры и ее перезапуск
        private void HandleGameOver(bool gameWon)
        {
            var message = gameWon ? "Поздравляю! Вы победили!" : "К сожалению, вы проиграли...";
            message += "\nХотите сыграть еще?";

            var response = MessageBox.Show(message, "Игра окончена", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (response == DialogResult.Yes)
            {
                SetupBoard();
                //PlaceMines();
            }
        }

        // Количество ячеек, помеченных флажками
        public int FlagsPlaced()
        {
            int totalFlagged = 0;

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    if (Cells[x, y].Flagged)
                    {
                        totalFlagged++;
                    }
                }
            }

            return totalFlagged;
        }

        // Проверка победы
        public void CheckForWin()
        {
            var correctMines = 0;           // Правильно найденные мины
            var incorrectMines = 0;         // Неправильно найденные мины
            var remainingCells = 0;         // Неоткрытые ячейки

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    var c = Cells[x, y];

                    // Если на клетке без мины стоит флажок
                    if (c.CellType == CellType.Flagged)
                    {
                        incorrectMines++;
                    }

                    // Если на клетке с миной стоит флажок
                    if (c.CellType == CellType.FlaggedMine)
                    {
                        correctMines++;
                    }

                    // Если клетка закрыта
                    if (c.Closed)
                    {
                        remainingCells++;
                    }
                }
            }

            // Все мины помечены
            bool flaggedAllMines = correctMines == NumMines && incorrectMines == 0;
            // Все пустые клетки - мины
            bool onlyCellsLeftAreMines = remainingCells == NumMines;

            // Победа
            if (flaggedAllMines || onlyCellsLeftAreMines)
            {
                GameOver = true;
                Input input_name = new Input();
                input_name.ShowDialog();
                string name = input_name.NAME;

                string filename = "../../../res/Records_";
                if (Mode == GameMode.Beginner) filename += "beginner.bin";
                else if (Mode == GameMode.Intermediate) filename += "intermediate.bin";
                else if (Mode == GameMode.Expert) filename += "expert.bin";

                using (BinaryWriter bw = new BinaryWriter(File.Open(filename, FileMode.Append)))
                {
                    
                    bw.Write(name);
                    bw.Write(Time);
                }
                HandleGameOver(gameWon: true);
            }
        }
    }
}