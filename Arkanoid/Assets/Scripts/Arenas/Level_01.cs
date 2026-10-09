public class Level_01 : Arena
{
    private static readonly int[,] _blockLayout = {
        {9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9, 9},
        {5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5, 5},
        {8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8, 8},
        {6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6, 6},
        {7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7, 7},
        {4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4, 4},
    };

    protected override int[,] BlockTypeMap
        {
            get { return _blockLayout; }
        }
}