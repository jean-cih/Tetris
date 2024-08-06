namespace Tetris
{
    public class Position
    {
        public int Rows {  get; set; }
        public int Columns {  get; set; }

        public Position(int rows, int columns)
        {
            Rows = rows;
            Columns = columns;
        }
    }
}
