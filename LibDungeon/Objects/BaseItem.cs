using System;
using System.Collections.Generic;
using System.Text;

namespace LibDungeon.Objects
{
    /// <summary>
    /// Атрибут, определяющий объекты, автоматически расставляемые на уровнях
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    sealed class SpawnableAttribute : Attribute
    {
        private int minLevel;
        private int maxLevel;

        public SpawnableAttribute(string classname) => Classname = classname;

        /// <summary>
        /// Имя класса (используется для отладки)
        /// </summary>
        public string Classname { get; }

        /// <summary>
        /// Минимальный уровень, на котором разрешено создание объекта
        /// </summary>
        public int MinLevel { get => minLevel; set => minLevel = (value >= 0) ? value : 0; }
        /// <summary>
        /// Максимальный уровень, на котором разрешено создание объекта
        /// </summary>
        public int MaxLevel { get => maxLevel; set => maxLevel = (value >= 0) ? value : 0; }
    }

    public abstract class BaseItem
    {
        public int X { get; set; }
        public int Y { get; set; }

        // Статические свойства, общие для всего класса

        /// <summary>
        /// Название предмета
        /// </summary>
        public abstract string Name { get; }
        /// <summary>
        /// Если true, то при поднятии игроком предмет не попадёт в его инвентарь, а будет удалён сразу после Use()
        /// </summary>
        public abstract bool RemoveOnPickup { get; }

        /// <summary>
        /// Срабатывает после поднятия игроком
        /// </summary>
        /// <param name="user"></param>
        public abstract void Use(Actor user);
        /// <summary>
        /// Срабатывает при удалении из инвентаря игрока
        /// </summary>
        /// <param name="user"></param>
        public abstract void Unuse(Actor user);
    }

    /// <summary>
    /// Эффект создаётся в ходе игры, прикрепляется к актёру и модифицирует его свойства
    /// </summary>
    public abstract class BaseEffect
    {
        public int timeToLive;
        Actor user;

        /// <summary>
        /// Сколько будет действовать эффект до своего уничтожения
        /// </summary>
        public int TimeToLive { get; }

        public abstract string Name { get; }
        public abstract string Description { get; }

        public abstract void Apply(Actor user);
        public abstract void Remove();
    }

    [Spawnable("meme")]
    public class TestItem : BaseItem
    {
        public override string Name => "meme";

        public override bool RemoveOnPickup => false;

        public override void Unuse(Actor user)
        {
            throw new NotImplementedException();
        }

        public override void Use(Actor user)
        {
            throw new NotImplementedException();
        }
    }
}
