using System.Collections.Generic;
using System.Linq;

public enum Mark
{
    None,
    Flag,
    Question,
}

public class FieldModel
{
    private int width { get; set; }
    private int height { get; set; }
    private List<bool> mines { get; set; }
    private List<bool> opens { get; set; }
    private List<Mark> marks { get; set; }

    public FieldModel(int width, int height, int count, int ignoredX, int ignoredY)
    {
        this.width = width;
        this.height = height;
        mines = CreateRandomMines(count, ignoredX, ignoredY);
        opens = Enumerable.Repeat(false, width * height).ToList();
        marks = Enumerable.Repeat(Mark.None, width * height).ToList();
    }

    public bool IsOpenedAny()
    {
        return opens.Any(isOpened => isOpened);
    }

    public bool IsCompleted()
    {
        return Enumerable.Range(0, width * height).All(i => mines[i] || opens[i]);
    }

    public int NoFlagMineCount()
    {
        return mines.Count(isMine => isMine) - marks.Count(mark => mark == Mark.Flag);
    }

    public bool IsMine(int x, int y)
    {
        var index = Index(x, y);
        return index >= 0 && mines[index];
    }

    public bool IsOpened(int x, int y)
    {
        var index = Index(x, y);
        return index >= 0 && opens[index];
    }

    public bool IsMarked(int x, int y)
    {
        var index = Index(x, y);
        return index >= 0 && marks[index] != Mark.None;
    }

    public Mark GetMark(int x, int y)
    {
        var index = Index(x, y);
        return index >= 0 ? marks[index] : Mark.None;
    }

    public int NearByMineCount(int x, int y)
    {
        return NearByXYs(x, y).Count(xy => IsMine(xy.Item1, xy.Item2));
    }

    public void Open(int x, int y)
    {
        var index = Index(x, y);
        if (index < 0)
        {
            return;
        }

        if (IsOpened(x, y) || IsMarked(x, y))
        {
            return;
        }

        opens[index] = true;

        if (NearByMineCount(x, y) == 0)
        {
            NearByXYs(x, y).ForEach(xy => Open(xy.Item1, xy.Item2));
        }
    }

    public Mark RotateMark(int x, int y)
    {
        var index = Index(x, y);
        if (index < 0)
        {
            return Mark.None;
        }

        return marks[index] = marks[index] switch
        {
            Mark.None => Mark.Flag,
            Mark.Flag => Mark.Question,
            Mark.Question => Mark.None,
            _ => Mark.Flag,
        };
    }

    public void FlagAllMines()
    {
        foreach (var (isMine, index) in mines.WithIndex())
        {
            marks[index] = isMine ? Mark.Flag : Mark.None;
        }
    }

    private List<bool> CreateRandomMines(int count, int ignoredX, int ignoredY)
    {
        // 初期マスと周囲は爆弾なしにすることで、初手は必ず複数マス開くようにする
        var ignoredIndexes = NearByXYs(ignoredX, ignoredY).Select(xy => Index(xy.Item1, xy.Item2)).ToList();
        ignoredIndexes.Add(Index(ignoredX, ignoredY));
        ignoredIndexes = ignoredIndexes.Where(i => i >= 0).ToList();

        var randomMineList = Enumerable.Repeat(true, count)
                                       .Concat(Enumerable.Repeat(false, width * height - count - ignoredIndexes.Count))
                                       .Shuffle().ToList();

        return Enumerable.Range(0, width * height).Select(i => !ignoredIndexes.Contains(i) && randomMineList.Pop()).ToList();
    }

    private int Index(int x, int y)
    {
        if (x < 0 || x >= width)
        {
            return -1;
        }

        if (y < 0 || y >= height)
        {
            return -1;
        }

        return width * y + x;
    }

    private List<(int, int)> NearByXYs(int x, int y)
    {
        return new List<(int, int)> {
            (x - 1, y - 1), (x, y - 1), (x + 1, y - 1),
            (x - 1, y),                 (x + 1, y),
            (x - 1, y + 1), (x, y + 1), (x + 1, y + 1),
        }.Where(xy => Index(xy.Item1, xy.Item2) >= 0).ToList();
    }
}
