using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using minesweeper.Core.Boards;

namespace minesweeper.Core
{
    public class Cell
    {
        public int XLoc { get; set; }                               // Номер ячейки по X
        public int YLoc { get; set; }                               // Номер ячейки по Y
        public int XPos { get; set; }                               // Позиция на поле по X
        public int YPos { get; set; }                               // Позиция на поле по Y
        public Point CenterPos { get; set; }                        // Центр ячейки
        public Point TopLeftPos { get; set; }                       // Верхняя левая точка
        public Point BottomLeftPos { get; set; }                    // Нижняя левая точка

        public int CellSize { get; set; }                           // Размер ячейки

        public CellState CellState { get; set; }                    // Состояние ячейки
        public CellType CellType { get; set; }                      // Тип ячейки

        public int NumMines { get; set; }                           // Количество мин

        public Board Board { get; set; }                            // Текущее поле

        public Rectangle Bounds { get; private set; }               // Границы ячейки

        private List<Cell> Surrounding { get; set; }                // Список соседних клеток



        // Список помеченных соседей
        public List<Cell> SurroundingFlagged => GetNeighborCells().Where(cell => cell.Flagged).ToList();

        // Количество оставшихся мин
        public int MinesRemaining => Opened && NumMines > 0 ? (NumMines - SurroundingFlagged.Count) : 0;

        // Помечена ли клетка флажком
        public bool Flagged => CellType == CellType.Flagged || CellType == CellType.FlaggedMine;

        // Закрыта ли клетка
        public bool Closed => CellState == CellState.Closed;

        // Открыта ли клетка
        public bool Opened => CellState == CellState.Opened;

        // Есть ли мина в этой клетке
        public bool IsMine => CellType == CellType.Mine || CellType == CellType.FlaggedMine;



        // Конструктор ячейки
        public Cell(int x, int y, Board board)
        {
            XLoc = x;
            YLoc = y;

            CellSize = Board.CellSize;
            CellState = CellState.Closed;
            CellType = CellType.Regular;
            
            Board = board;

            XPos = XLoc * CellSize;
            YPos = YLoc * CellSize;
            Bounds = new Rectangle(XPos, YPos, CellSize, CellSize);

            CenterPos = new Point(XPos + (CellSize / 2 - 10), YPos + (CellSize / 2 - 10));
            TopLeftPos = new Point(XPos, YPos);
            BottomLeftPos = new Point(XPos, YPos + (CellSize - 10));
        }

        // Пометка клетки флажком
        public void OnFlag()
        {
            CellType = CellType switch
            {
                CellType.Regular => CellType.Flagged,
                CellType.Mine => CellType.FlaggedMine,
                CellType.Flagged => CellType.Regular,
                CellType.FlaggedMine => CellType.Mine,
                _ => throw new Exception($"Error type {CellType}")
            };

            Board.Minesweeper.UpdateMinesRemaining();
            Board.Minesweeper.Invalidate();
        }

        // Нажатие на клетку
        public void OnClick(bool recursiveCall = false)
        {
            if (recursiveCall)
            {
                // Рекурсивное открытие остановится если тип клетки не обычный или состояние клетки не закрытое
                if (CellType != CellType.Regular || CellState != CellState.Closed)
                {
                    return;
                }
            }

            // Если в клетке мина
            if (CellType == CellType.Mine)
            {
                CellState = CellState.Opened;
                Board.RevealMines();
                return;
            }

            // Если клетка обычная
            if (CellType == CellType.Regular)
            {
                CellState = CellState.Opened;
            }

            // Рекурсивное откытие клеток
            if (NumMines == 0 || MinesRemaining == 0)
            {
                foreach (var n in GetNeighborCells())
                {
                     n.OnClick(true);
                }
            }
        }

        // Получение списка соседей
        public List<Cell> GetNeighborCells()
        {
            if (Surrounding == null)
            {
                Surrounding = new List<Cell>();

                for (int x = -1; x <= 1; x++)
                {
                    for (int y = -1; y <= 1; y++)
                    {
                        // Не проверяем текущую ячейку
                        if (x == 0 && y == 0)
                        {
                            continue;
                        }

                        // Если ячейка выходит за границы поля
                        if (XLoc + x < 0 || XLoc + x >= Board.Width || YLoc + y < 0 || YLoc + y >= Board.Height)
                        {
                            continue;
                        }

                        Surrounding.Add(Board.Cells[XLoc + x, YLoc + y]);
                    }
                }
            }

            return Surrounding;
        }
    }
}