using System.Collections.Generic;
using Tetris;

namespace Cell
{
    public abstract class Block
    {
        protected abstract Position[][] Tiles { get; }
        protected abstract Position StartOffset { get; }
        public abstract int Id { get; }

        private int rotationState;
        private Position offset;

        public Block()
        {
            offset = new Position(StartOffset.Rows, StartOffset.Columns);
        }

        public IEnumerable<Position> TilePositions()
        {
            foreach (Position p in Tiles[rotationState])
            {
                yield return new Position(p.Rows + offset.Rows, p.Columns + offset.Columns);
            }
        }

        public void RotateCW()
        {
            rotationState = (rotationState + 1) % Tiles.Length;
        }

        public void RotateCCW()
        {
            if (rotationState == 0)
            {
                rotationState = Tiles.Length - 1;
            }
            else
            {
                rotationState--;
            }
        }

        public void Move(int rows, int columns)
        {
            offset.Rows += rows;
            offset.Columns += columns;
        }

        public void Reset()
        {
            rotationState = 0;
            offset.Rows = StartOffset.Rows;
            offset.Columns = StartOffset.Columns;
        }
    }
}
