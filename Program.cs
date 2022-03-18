using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
public struct Vector2
{
   public int a;
   public int b;
   public Vector2(int a,int b)
    {
        this.a = a;
        this.b = b;
    }
}
public struct Cell
{
    public Vector2 pos;
    public bool isSettle;
    public int finall;
    public List<int> list;
}
public class myProgram
{
    static void Main(string[] args)
    {
        Console.ForegroundColor = ConsoleColor.White;
        Console.WriteLine("input cell size");
        var size = 0;
        Solution s = new Solution();
        //while (size <= 0)
        //{
        //    string str = Console.ReadLine();
        //    int.TryParse(str, out size);
        //}
        size = 9;
        while (true)
        {
            Cell[,] cellMapTemp = new Cell[size, size];
            Console.WriteLine("input Sidoku");
            for (int i = 0; i < size; i++)
            {
                string str = Console.ReadLine();
                for (int j = 0; j < size; j++)
                {
                    var c = new Cell();
                    c.pos = new Vector2(i, j);
                    if (j < str.Length && int.TryParse(str[j].ToString(), out int nub) && nub != 0)
                    {
                        c.isSettle = true;
                        c.finall = nub;
                        c.list = new List<int>();
                    }
                    else
                    {
                        c.list = new List<int>() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                    }
                    cellMapTemp[i, j] = c;
                }
            }
            s.Init(cellMapTemp);
        }
    }
}
public class Solution
{

    Random rnd = new Random();
    private Cell[,] cellMap;
    Queue<Vector2> cellQueue;


    public void Init(Cell[,] cells)
    {
        cellMap = cells;
        cellQueue = new Queue<Vector2>();
        foreach (var item in cellMap)
        {
            if(item.isSettle)
            {
                BordCast2OtherCell(item.pos, CallBackAddorRemove(false, 0, true, item.finall));
            }
        }
        ProcessCellMap();

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(OutPutMap());
        Console.ForegroundColor = ConsoleColor.White;
    }
    public string OutPutMap()
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < cellMap.GetLength(0); i++)
        {
            for (int j = 0; j < cellMap.GetLength(0); j++)
            {
                var ce = cellMap[i, j];
                if (ce.isSettle == false)
                {
                    sb.Append('*');
                }
                else
                {
                    sb.Append(ce.finall);
                    sb.Append(" |");
                }
            }
            sb.Append("\n");
            sb.Append("===========================");
            sb.Append("\n");
        }
        return sb.ToString();
    }
    public void ProcessCellMap()
    {
        while(IsComplete()==false)
        {
            var pos = GetEntropySmallistPos();
            var success = Collapse(pos);
            if(success)
            {
                cellQueue.Enqueue(pos);
            }
            else
            {
                var lastPos = cellQueue.Dequeue();
                UnCollapse(lastPos);
            }
        }
    }

    public Vector2 GetEntropySmallistPos()
    {
        Vector2 pos = new Vector2();
        bool findOne = false;
        foreach (var item in cellMap)
        {
            if (item.isSettle)
                continue;
            if (!findOne)
            {

                findOne = true;
                pos = item.pos;

            }
            else
            {
                if (CompareEntropy(cellMap[pos.a, pos.b], item) > 0)
                {
                    pos = item.pos;
                }
            }
        }
        return pos;
    }


    public bool IsComplete()
    {
        foreach (var item in cellMap)
        {
            if (item.isSettle == false)
                return false;
        }
        return true;
    }
    private int CompareEntropy(Cell a, Cell b)
    {
        return a.list.Count.CompareTo(b.list.Count);
    }
    private bool Collapse(Vector2 pos)
    {
        if (pos.a >= cellMap.GetLength(0) || pos.b >= cellMap.GetLength(0))
        {
            return false;
        }
        var ce = cellMap[pos.a, pos.b];
        if (ce.isSettle || ce.list.Count == 0)
        {
            return false;
        }

        var index = rnd.Next(0, ce.list.Count - 1);
        ce.finall = ce.list[index];
        ce.isSettle = true;
        cellMap[pos.a, pos.b] = ce;
        BordCast2OtherCell(ce.pos, CallBackAddorRemove(false, 0, true, ce.finall));

        return true;
    }
    private Action<Vector2> CallBackAddorRemove(bool isAdd, int addValue, bool isRemove, int removeValue)
    {
        return (itemPos) =>
        {
            var _addValue = addValue;
            var _removeValue = removeValue;
            var _isAdd = isAdd;
            var _isRemove = isRemove;
            var c = cellMap[itemPos.a, itemPos.b];
            if (c.isSettle == false)
            {
                if (_isAdd)
                    c.list.Add(_addValue);
                if (_isRemove)
                    c.list.Remove(_removeValue);
                cellMap[itemPos.a, itemPos.b] = c;
            }
        };
    }
    private void UnCollapse(Vector2 pos)
    {
        if (pos.a >= cellMap.GetLength(0) || pos.b >= cellMap.GetLength(0))
        {
            return;
        }
        var ce = cellMap[pos.a, pos.b];
        if (!ce.isSettle)
        {
            return;
        }
        ce.list.Remove(ce.finall);
        ce.isSettle = false;
        cellMap[pos.a, pos.b] = ce;
        BordCast2OtherCell(ce.pos, CallBackAddorRemove(true, ce.finall, false, 0)); ;
    }

    private void BordCast2OtherCell(Vector2 pos, Action<Vector2> callback)
    {
        var cell = cellMap[pos.a, pos.b];
        List<Vector2> list = new List<Vector2>();
        Vector2 center = new Vector2() { a = (pos.a / 3) * 3 + 1, b = (pos.b / 3) * 3 + 1 };
        list.Add(new Vector2(center.a + 1, center.b + 1));
        list.Add(new Vector2(center.a + 1, center.b));
        list.Add(new Vector2(center.a + 1, center.b - 1));
        list.Add(new Vector2(center.a, center.b + 1));
        list.Add(new Vector2(center.a, center.b));
        list.Add(new Vector2(center.a, center.b - 1));
        list.Add(new Vector2(center.a - 1, center.b + 1));
        list.Add(new Vector2(center.a - 1, center.b));
        list.Add(new Vector2(center.a - 1, center.b - 1));
        for (int i = 0; i < cellMap.GetLength(0); i++)
        {
            list.Add(new Vector2(pos.a, i));
            list.Add(new Vector2(i, pos.b));
        }
        list = list.Distinct().ToList();
        list.Remove(pos);
        foreach (var itemPos in list)
        {
            callback?.Invoke(itemPos);
        }
    }

}