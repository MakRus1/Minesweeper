namespace minesweeper.Core.Boards
{
    public class Board
    {
        public Minesweeper Minesweeper { get; set; }

        public int Width { get; set; }                                  // Ширина
        public int Height { get; set; }                                 // Высота

        public int NumMines { get; set; }                               // Количество мин на поле
        public int NumMinesRemaining => NumMines - FlagsPlaced();       // Количество оставшихся мин

        public Cell[,] Cells { get; set; }                              // Ячейки

        public bool GameOver { get; set; }                              // Окончание игры



        public const int CellSize = 32;                                 // Размер ячейки



        // Конструктор поля
        public Board(Minesweeper minesweeper, int width, int height, int mines)     
        {
            Minesweeper = minesweeper;
            Width = width;
            Height = height;
            NumMines = mines;
            Cells = new Cell[width, height];
        }

        // 
    }
}