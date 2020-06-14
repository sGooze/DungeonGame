using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// http://journal.stuffwithstuff.com/2014/12/21/rooms-and-mazes/
// http://pcg.wikidot.com/pcg-algorithm:dungeon-generation
// http://donjon.bin.sh/d20/dungeon/

namespace LibDungeon.Levels
{
    using Objects;

    /// <summary>
    /// Составляющие подземелье уровни
    /// </summary>
    public abstract class Level 
    {
        public abstract Tile[,] Tiles { get; }
    }

    /// <summary>
    /// Уровень, состоящий из маленьких комнат, соединённых извилистыми узкими коридорами.
    /// </summary>
    public class DungeonFloor : Level
    {
        public const int width = 80, height = 80;

        // Объявим ♂ донжон ♂ как двумерный массив тайлов
        Tile[,] tiles;

        public LinkedList<BaseItem> FloorItems { get; set; } = new LinkedList<BaseItem>();
        public LinkedList<Actor> Actors { get; set; } = new LinkedList<Actor>();

        public override Tile[,] Tiles { get => tiles; }

        /// <summary>
        /// Создание нового уровня
        /// </summary>
        public DungeonFloor()
        {
            // Изначально уровень заполнен блоками стен
            tiles = new Tile[width, height];
            for (int i = 0; i < width; i++)
                for (int j = 0; j < height; j++)
                    tiles[i, j] = new Wall();

            Random rand = new Random();
            int[,] regionmap = new int[width, height]; // Карта регионов - используется для генерации проходов

            // Стороны света
            List<(int, int)> cardinal = new List<(int, int)>() { (1, 0), (0, 1), (-1, 0), (0, -1) };
            List<(int, int)> border = new List<(int, int)>() { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
            // Создание коридоров путём генерации случайного лабиринта
            //if (false)
                int sx, sy;
            {
                Queue<(int, int)> tileList = new Queue<(int, int)>();

                // Начав со случайной позиции, добавить в список все окружающие её тайлы
                //int sx, sy;
                sx = rand.Next(1, width - 2); sy = rand.Next(1, height - 2);
                tiles[sx, sy] = new Floor(); regionmap[sx, sy] = 1;
                foreach (var c in cardinal.Select(x => (x.Item1 + sx, x.Item2 + sy))) tileList.Enqueue(c);

                while (tileList.Count > 0)
                {
                    var tile = tileList.Dequeue();
                    if (tile.Item1 < 1 || tile.Item2 < 1 || tile.Item1 > width - 2 || tile.Item2 > height - 2)
                        continue;

                    if (tiles[tile.Item1, tile.Item2].Solidity == Tile.SolidityType.Floor)
                        continue;

                    int touching = 0;
                    
                    // Подсчитать количество уже вырытых тайлов вокруг текущего
                    touching = cardinal.Select(card => new { x = tile.Item1 + card.Item1, y = tile.Item2 + card.Item2 })
                        .Where(c => c.x > 0 && c.y > 0 && c.x < width - 1 && c.y < height - 1 && tiles[c.x, c.y].Solidity == Tile.SolidityType.Floor).Count();

                    // Если таких тайлов не больше одного, то текущий тайл также выкапывается, и все его соседи добавляются в список
                    if (touching < 2)
                    {
                        tiles[tile.Item1, tile.Item2] = new Floor(); regionmap[tile.Item1, tile.Item2] = 1;
                        foreach (var c in cardinal.Select(x => (x.Item1 + tile.Item1, x.Item2 + tile.Item2))) 
                            if (!tileList.Contains(c)) tileList.Enqueue(c);
                    }
                    else
                        continue;

                    // mix the tile set
                    var temp = new LinkedList<(int, int)>();
                    for (int i = 0; i < rand.Next(0, tileList.Count); i++)
                        temp.AddLast(tileList.Dequeue());
                    foreach (var c in temp.OrderBy(x => rand.Next()))
                        tileList.Enqueue(c);
                }

                int sparse = (width * height) / 5;
                while (sparse-- > 0)
                {
                    int x, y;
                    do
                    {
                        x = rand.Next(1, width - 1); y = rand.Next(1, height - 1);
                        if (tiles[x, y].Solidity == Tile.SolidityType.Wall)
                            continue;

                        int touching = 0;
                        foreach (var card in cardinal)
                        {
                            int dx = x + card.Item1;
                            int dy = y + card.Item2;
                            if (dx >= 0 && dy >= 0 && dx <= width - 1 && dy <= height - 1 && tiles[dx, dy].Solidity == Tile.SolidityType.Wall)
                                touching++;
                        }
                        if (touching != 3)
                            continue;
                        break;
                    } while (true);
                    tiles[x, y] = new Wall(); regionmap[x, y] = 0;
                }
            }

            //if (false)
            {
                // Generate a list of non-intersecting room bounding boxes
                Queue<Rectangle> rooms = new Queue<Rectangle>(10);
                for (int i = 0; i < 20; i++)
                {
                    int rw = rand.Next(4, 20), rh = rand.Next(4, 20);
                    int rx = rand.Next(1, width - rw - 2), ry = rand.Next(1, height - rh - 2);

                    bool validRoom = true;
                    Rectangle room = new Rectangle(rx, ry, rw, rh);
                    foreach (var prev in rooms)
                        if (prev.IntersectsWith(room))
                        {
                            validRoom = false;
                            break;
                        }
                    if (validRoom)
                        rooms.Enqueue(room);
                    else i--;
                }

                // Carve rooms inside the floor's tile list
                int myregion = 2;
                foreach (var room in rooms)
                {
                    for (int i = room.X; i < room.X + room.Width; i++)
                        for (int j = room.Y; j < room.Y + room.Height; j++)
                        {
                            if (i == room.X || i == room.X + room.Width - 1 || j == room.Y || j == room.Y + room.Height - 1)
                            { tiles[i, j] = new Wall(); regionmap[i, j] = 0; }
                            else
                            { tiles[i, j] = new Floor(); regionmap[i, j] = myregion; }
                        }
                    myregion++;
                }

                // Flood fill each individual corridor region
                for (int i = 1; i < width - 1; i++)
                    for (int j = 1; j < height - 1; j++)
                    {
                        if (regionmap[i, j] != 1)
                            continue;
                        regionmap[i, j] = myregion;
                        Queue<(int, int)> flood = new Queue<(int, int)>(); flood.Enqueue((i, j));
                        while (flood.Count > 0)
                        {
                            var node = flood.Dequeue();
                            foreach (var neigh in cardinal.Select(x => (x.Item1 + node.Item1, x.Item2 + node.Item2)))
                                if (regionmap[neigh.Item1, neigh.Item2] == 1)
                                {
                                    regionmap[neigh.Item1, neigh.Item2] = myregion;
                                    flood.Enqueue(neigh);
                                }
                        }
                        myregion++;
                    }

                /*LinkedList<(int, int)> connectors = new LinkedList<(int, int)>();
                for (int i = 1; i < width - 1; i++)
                    for (int j = 1; j < height - 1; j++)
                        if (regionmap[i, j] == 0)
                        {
                            int c = cardinal.Select(x => regionmap[i + x.Item1, j + x.Item2]).Distinct().Count();
                            if (c > 2)
                                connectors.AddLast((i, j));
                        }
                SortedSet<int> connectedRegions = new SortedSet<int>() { 0 }; // Множество объединённых регионов
                foreach (var con in connectors)
                {
                    var regs = cardinal.Select(x => regionmap[con.Item1 + x.Item1, con.Item2 + x.Item2]).Distinct();
                    var isect = connectedRegions.Intersect(regs);

                    // Если тайл соседствует с больше чем одним неприсоединённым регионом, то добавить их в множество и превратить тайл в дверь
                    if (isect.Count() <= 1 || rand.Next(0, 25) == 0)
                    {
                        connectedRegions.UnionWith(regs);
                        tiles[con.Item1, con.Item2] = new Door();
                    } else if (rand.Next(0, 50) == 0)
                    {
                        tiles[con.Item1, con.Item2] = new Door();
                    }
                }*/
                LinkedList<(int, int, int, int)> connectors = new LinkedList<(int, int, int, int)>();
                for (int i = 1; i < width - 1; i++)
                    for (int j = 1; j < height - 1; j++)
                        if (regionmap[i, j] == 0)
                        {
                            var c = cardinal.Select(x => regionmap[i + x.Item1, j + x.Item2]).Where(x => x != 0).Distinct();
                            if (c.Count() > 1)
                                connectors.AddLast((i, j, c.Min(), c.Max()));
                        }

                HashSet<(int, int)> connectedRegions = new HashSet<(int, int)>();
                foreach (var con in connectors)
                {
                    var regs = (con.Item3, con.Item4);
                    if (!connectedRegions.Contains(regs))
                    {
                        connectedRegions.Add(regs);
                        int chance = rand.Next(0, 100);
                        if (chance <= 45)
                            tiles[con.Item1, con.Item2] = new Door();
                        else if (chance <= 90)
                            tiles[con.Item1, con.Item2] = new Floor();
                        else
                            tiles[con.Item1, con.Item2] = new Wall();
                    }
                }
            }

            // Для проверки доступности элементов подземелья залить все связаные с начальной точкой клетки
            //if (false)
            {
                tiles[sx, sy].Visited = true;
                Queue<(int, int)> visq = new Queue<(int, int)>(); visq.Enqueue((sx, sy));
                while(visq.Count > 0)
                {
                    var node = visq.Dequeue();
                    foreach(var neigh in border.Select(x => (x.Item1 + node.Item1, x.Item2 + node.Item2)))
                        if (tiles[neigh.Item1, neigh.Item2].Solidity != Tile.SolidityType.Wall && !tiles[neigh.Item1, neigh.Item2].Visited)
                        {
                            tiles[neigh.Item1, neigh.Item2].Visited = true;
                            visq.Enqueue(neigh);
                        }
                }
            }

            // Добавляет на уровень лестницы на другие уровни
            // TODO: Связать их с лестницами на соседних этажах!
            int ladder_quota = rand.Next(2, 10);
            while (ladder_quota != 0)
            {
                // Поиск подходящей клетки
                int x = rand.Next(0, width), y = rand.Next(0, height);
                if (!tiles[x, y].Visited)
                    continue;
                tiles[x, y] = new Ladder() 
                    { Direction = (ladder_quota % 2 == 0) ? Ladder.LadderDirection.Down : Ladder.LadderDirection.Up };
                ladder_quota--;
            }

            // Добавляет на уровень предметы
            int item_quota = 15, enemy_quota = 10;
            while (item_quota != 0)
            {
                // Поиск подходящей клетки
                int x = rand.Next(0, width), y = rand.Next(0, height);
                if (!tiles[x, y].Visited)
                    continue;
                FloorItems.AddLast(Spawner.SpawnRandomItem(x, y));
                item_quota--;
            }

            // Очищаем поле Visited; теперь оно будет использоваться для определения видимости
            foreach (var tile in tiles)
                tile.Visited = false;
        }
    }
}
