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

        public Level CurrentLevel { get => floors[currentLevel]; }

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

            floors.Add(new DungeonFloor());
            while (true)
            {
                // Добавить игрока на первый уровень
                PlayerPawn = new Char() { X = 40, Y = 40 };
                break;
            }
            UpdateVisits();
        }

        public void Think()
        {
            UpdateVisits();
        }
    }


    [System.AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class PlaceholderAttribute : Attribute
    {
        public PlaceholderAttribute() { }
    }
}
