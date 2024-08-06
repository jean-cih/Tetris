namespace Tetris
{
    public class GameGrid
    {
        private readonly int[,] grid;
        public int Rows { get; }
        public int Columns { get; }

        public int this[int r, int c]
        {
            get => grid[r, c];
            set => grid[r, c] = value;
        }

        public GameGrid(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
            grid = new int[rows, columns];
        }

        public bool isInside(int r, int c)
        {
            return r >= 0 && r < Rows && c >= 0 && c < Columns;
        }

        public bool isEmpty(int r, int c)
        {
            return isInside(r, c) && grid[r, c] == 0;
        }

        public bool isRowFull(int r)
        {
            for (int c = 0; c < Columns; c++)
                if (grid[r, c] == 0) return false;

            return true;
        }

        public bool isRowEmpty(int r)
        {
            for (int c = 0; c < Columns; c++)
                if (grid[r, c] != 0) return false;

            return true;
        }

        private void ClearRow(int r)
        {
            for (int c = 0; c < Columns; c++)
                grid[r, c] = 0;
        }

        private void MoveRowDown(int r, int numRows)
        {
            for (int c = 0; c < Columns; c++)
            {
                grid[r + numRows, c] = grid[r, c];
                grid[r, c] = 0;
            }
        }

        public int ClearFullRows()
        {
            int cleared = 0;

            for (int r = Rows - 1; r >= 0; r--)
            {
                if (isRowFull(r))
                {
                    ClearRow(r);
                    cleared++;
                }
                else if (cleared > 0)
                {
                    MoveRowDown(r, cleared);
                }
            }

            return cleared;
        }
    }
}
