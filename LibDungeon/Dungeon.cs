using System;
using System.Collections.Generic;
using System.Linq;

namespace LibDungeon
{
    using Levels;
    using Objects;
    using System.Reflection;

    internal static class Spawner
    {
        internal static Dictionary<string, Type> Items { get; set; }
        internal static Dictionary<string, Type> Actors { get; set; }

        static Random random = new Random();
        internal static BaseItem SpawnRandomItem(int minlevel = int.MinValue, int maxlevel = int.MaxValue)
        {
            var items = Items.Select(x => x.Value).ToArray();
            return Activator.CreateInstance(items[random.Next(items.Length)]) as BaseItem;
        }
    }

    /// <summary>
    /// ♂ Dungeon ♂
    /// </summary>
    public class Dungeon
    {
        /// <summary>
        /// Уровни подземелья
        /// </summary>
        List<ILevel> floors;

        public Dungeon()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();

            // Созданеи списка типов
            var spawnable = types.Where(x => x.IsDefined(typeof(SpawnableAttribute)));
            //.ToDictionary(x => (x.GetCustomAttribute<SpawnableAttribute>().Classname));

            Spawner.Items = spawnable.Where(x => x.IsSubclassOf(typeof(BaseItem)))
                .ToDictionary(x => (x.GetCustomAttribute<SpawnableAttribute>().Classname));

            Spawner.Actors = spawnable.Where(x => x.IsSubclassOf(typeof(Actor)))
                .ToDictionary(x => (x.GetCustomAttribute<SpawnableAttribute>().Classname));
        }
    }
}
