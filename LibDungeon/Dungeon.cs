using System;
using System.Collections.Generic;
using System.Linq;

namespace LibDungeon
{
    using Levels;
    using Objects;
    using System.Diagnostics;
    using System.Reflection;

    internal static class Spawner
    {
        internal static Dictionary<string, Type> Items { get; set; }
        internal static Dictionary<string, Type> Actors { get; set; }

        static Random random = new Random();
        public static Random Random { get => random; }
        internal static BaseItem SpawnRandomItem(int x, int y) => SpawnRandomItem(x, y, int.MinValue, int.MaxValue);
        internal static BaseItem SpawnRandomItem(int coord_x, int coord_y, int minlevel, int maxlevel)
        {
            var items = Items.Select(x => x.Value).ToArray();
            var sitem = Activator.CreateInstance(items[random.Next(items.Length)]) as BaseItem;
            sitem.X = coord_x; sitem.Y = coord_y;
            return sitem;
        }


        internal static Actor SpawnRandomActor(int x, int y) => SpawnRandomActor(x, y, int.MinValue, int.MaxValue);
        internal static Actor SpawnRandomActor(int coord_x, int coord_y, int minlevel, int maxlevel)
        {
            var actors = Actors.Select(x => x.Value).ToArray();
            var sactor = Activator.CreateInstance(actors[random.Next(actors.Length)]) as Actor;
            sactor.X = coord_x; sactor.Y = coord_y;
            return sactor;
        }

        internal static double Distance(int x1, int y1, int x2, int y2) 
            => Math.Sqrt(Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));

        internal static double Distance(Actor a, Actor b) => Distance(a.X, a.Y, b.X, b.Y);

        internal static double DistanceSqr(int x1, int y1, int x2, int y2)
            => (Math.Pow(x1 - x2, 2) + Math.Pow(y1 - y2, 2));

        internal static double DistanceSqr(Actor a, Actor b) => DistanceSqr(a.X, a.Y, b.X, b.Y);

        internal static T Clamp<T>(T left, T value, T right) where T : IComparable<T>
        {
            if (value.CompareTo(left) < 0) return left;
            else if (value.CompareTo(right) > 0) return right;
            else return value;
        }
    }

    /// <summary>
    /// ♂ Dungeon ♂
    /// </summary>
    public partial class Dungeon
    {
        /// <summary>
        /// Уровни подземелья
        /// </summary>
        List<Level> floors = new List<Level>();
        int currentLevel = 0;

        public int CurrentLevel => currentLevel;
        public Level CurrentFloor { get => floors[currentLevel]; }

        public Dungeon()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

#if DEBUG
            foreach (var type in types)
            {
                if (type.IsDefined(typeof(PlaceholderAttribute)))
                    Debug.WriteLine($"Placeholder type: {type.Name}");
                foreach (var method in type.GetRuntimeMethods())
                    if (method.IsDefined(typeof(PlaceholderAttribute)))
                        Debug.WriteLine($"Placeholder method in type {type.Name}: {method.Name}");
            }
#endif

            // Создание списков типов
            var spawnable = types.Where(x => x.IsDefined(typeof(SpawnableAttribute)));

            Spawner.Items = spawnable.Where(x => x.IsSubclassOf(typeof(BaseItem)))
                .ToDictionary(x => (x.GetCustomAttribute<SpawnableAttribute>().Classname));

            Spawner.Actors = spawnable.Where(x => x.IsSubclassOf(typeof(Actor)))
                .ToDictionary(x => (x.GetCustomAttribute<SpawnableAttribute>().Classname));

            floors.Add(new DungeonFloor(35, 35)); // Первый уровень поменьше остальных
            PlayerPawn = new KnightClass();
            while (CurrentFloor.Tiles[PlayerPawn.X, PlayerPawn.Y].Solidity != Tile.SolidityType.Floor)
            {
                // Добавить игрока на первый уровень
                PlayerPawn.X = Spawner.Random.Next(0, CurrentFloor.Width);
                PlayerPawn.Y = Spawner.Random.Next(0, CurrentFloor.Height);
            }
            CurrentFloor.FloorActors.AddLast(PlayerPawn);
            UpdateVisits();
        }

        public static event EventHandler<string> ClientMessageSent;

        internal static void SendClientMessage(object sender, string msg) => ClientMessageSent?.Invoke(sender, msg);
    }


    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class PlaceholderAttribute : Attribute
    {
        public PlaceholderAttribute() { }
    }
}
