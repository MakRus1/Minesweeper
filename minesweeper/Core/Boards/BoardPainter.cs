using System.Collections.Generic;
using System.Drawing;

namespace minesweeper.Core.Boards
{
    public class BoardPainter
    {
        public Board Board { get; set; }                                                // Поле

        private Dictionary<int, SolidBrush> _cellColours;                               // Цвета

        private readonly Font _textFont = new Font("Veranda", 16f, FontStyle.Bold);     // Основной шрифт



        // Конструктор отрисовки поля
        public BoardPainter()
        {
            SetupColours();
        }

        // Подгрузка цветов
        private void SetupColours()
        {
            if (_cellColours == null)
            {
                _cellColours = new Dictionary<int, SolidBrush>
                {
                    { 0, new SolidBrush(ColorTranslator.FromHtml("0xffffff")) },        // Белый
                    { 1, new SolidBrush(ColorTranslator.FromHtml("0x0000FE")) },        // Синий
                    { 2, new SolidBrush(ColorTranslator.FromHtml("0x186900")) },        // Зеленый
                    { 3, new SolidBrush(ColorTranslator.FromHtml("0xAE0107")) },        // Красный
                    { 4, new SolidBrush(ColorTranslator.FromHtml("0x000177")) },        // Темно-синий
                    { 5, new SolidBrush(ColorTranslator.FromHtml("0x8D0107")) },        // Темно-красный
                    { 6, new SolidBrush(ColorTranslator.FromHtml("0x007A7C")) },        // Кораловый
                    { 7, new SolidBrush(ColorTranslator.FromHtml("0x902E90")) },        // Фиолетовый
                    { 8, new SolidBrush(ColorTranslator.FromHtml("0x000000")) },        // Черный
                };
            }
        }

        // Отрисовка поля
        public void Paint(Graphics graphics)
        {
            graphics.Clear(Color.White);
            graphics.TranslateTransform(Board.CellSize / 2, Board.CellSize * 2);

            for (int x = 0; x < Board.Width; x++)
            {
                for (int y = 0; y < Board.Height; y++)
                {
                    var cell = Board.Cells[x, y];
                    DrawInsideCell(cell, graphics);
                    graphics.DrawRectangle(Pens.DimGray, cell.Bounds);
                }
            }
        }

        // Получение нужного цвета
        private Brush GetBackgroundBrush(Cell cell)
        {
            if (cell.Opened)
                return Brushes.LightGray;

            return Brushes.DarkGray;
        }

        // Отрисовка одной ячейки на поле
        private void DrawInsideCell(Cell cell, Graphics graphics)
        {
            // Закрытая ячейка
            if (cell.Closed)
            {
                graphics.FillRectangle(GetBackgroundBrush(cell), cell.Bounds);
            }

            // Открытая ячейка
            if (cell.Opened)
            {
                graphics.FillRectangle(GetBackgroundBrush(cell), cell.Bounds);

                if (cell.NumMines > 0)
                {
                    graphics.DrawString(cell.NumMines.ToString(), _textFont, GetCellColour(cell), cell.CenterPos);
                }
            }

            // Ячейка с флажком
            if (cell.Flagged)
            {
                graphics.DrawString("?", _textFont, Brushes.Black, cell.CenterPos);
            }

            // Ячейка с миной
            if (cell.IsMine && Board.GameOver)
            {
                // Проигрышная клетка
                if (cell.Opened)
                {
                    graphics.FillRectangle(Brushes.Red, cell.Bounds);
                }

                // Мины, которые не были помечены
                if (!cell.Flagged)
                {
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };
                    graphics.DrawString("М", _textFont, Brushes.Black, cell.Bounds, format);
                }
            }
        }

        // Цвет, соответствующий количеству мин
        private SolidBrush GetCellColour(Cell cell)
        {
            return _cellColours[cell.NumMines];
        }
    }
}