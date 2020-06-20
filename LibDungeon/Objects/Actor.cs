using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LibDungeon.Objects
{
    /// <summary>
    /// Базовый класс для актёров
    /// </summary>
    public abstract partial class Actor
    {
        private int movePoints;
        private int health;
        private int hunger = 0;
        private int hungerRate = 0;
        public const int maxHunger = 2000;

        public int Score { get; set; } = 1;

        public int X { get; set; }
        public int Y { get; set; }
        // Статы
        public int Health { get => health; set => health = Spawner.Clamp(0, value, MaxHealth); }
        public int MaxHealth { get; set; }
        public int MeleeDamage { get; set; }

        // Голод
        /// <summary>
        /// Уровень голода (от 0 до 100)
        /// </summary>
        public int Hunger { get => hunger; set => hunger = Spawner.Clamp(0, value, maxHunger); }

        /// <summary>
        /// Насколько голод увеличивается с каждым ходом (для голодающих актёров (игрока))
        /// </summary>
        public int HungerRate { get => hungerRate; set => hungerRate = Spawner.Clamp(0, value, 100); }
        // Скиллы

        /// <summary>
        /// Определяет вероятность того, что атака будет успешной
        /// </summary>
        public int MeleeSkill { get; set; }


        // Состояние
        /// <summary>
        /// Вид перемещения актёра
        /// </summary>
        public MoveTypeEnum MoveType { get; set; }
        /// <summary>
        /// Последняя мысль актёра
        /// </summary>
        public ThoughtTypeEnum Thoughts { get; set; }


        /// <summary>
        /// Очки передвижения. Каждый вызов Think() даёт актёру одно очко. Только когда число очков достигает
        /// максимума, актёр можт потратить их все и выполнить какое-либо действие.
        /// Другими словами, чем меньше очков, тем чаще актёр может действовать.
        /// </summary>
        public int MovePoints { get => movePoints; set => movePoints = Math.Min(value, MaxMovePoints); }
        public int MaxMovePoints { get; set; }

        public abstract string Name { get; }

        /// <summary>
        /// Определяет дальнейшие действия
        /// </summary>
        protected abstract ThoughtTypeEnum _Think(Actor hostile);

        public ThoughtTypeEnum Think(Actor hostile)
        {
            Thoughts = _Think(hostile);
            return Thoughts;
        }


        public virtual void MeleeAttack(Actor target)
        {
            Dungeon.SendClientMessage(this, $"{Name} атакует {target.Name}");
            if (Spawner.DistanceSqr(this, target) > 2)
            {
                Dungeon.SendClientMessage(this, $"{Name} не может атаковать {target.Name}");
                return;
            }
            int chance = Spawner.Random.Next(0, 10);
            if (chance > MeleeSkill)
            {
                Dungeon.SendClientMessage(this, $"{Name} промахивается!");
                return;
            }
            int damage = Spawner.Random.Next(MeleeSkill / 2, MeleeSkill * 2);
            if (chance == 0)
            {
                damage = (int)(MeleeSkill * 2.5);
                Dungeon.SendClientMessage(this, $"{Name} наносит сокрушительный урон {MeleeSkill * 2.5}!");
            }
            else
            {
                Dungeon.SendClientMessage(this, $"{Name} наносит урон {damage}");
            }
            target.Health -= damage;
            if (target.Health == 0)
            {
                Dungeon.SendClientMessage(this, $"{target.Name} умирает");
                Score += target.Score;
                //Dungeon.SendClientMessage(this, $"{Name} получает {target.Score} очков");
            }

            return;
        }

        public void ChangePos(int x, int y)
        {
            X = x;
            Y = y;
        }
    }

    /// <summary>
    /// Тип передвижения
    /// </summary>
    public enum MoveTypeEnum
    {
        Stationary,
        Walking,
        Jumping,
        Flying
    }

    public enum ThoughtTypeEnum
    {
        Dead,
        Stand,
        Wander,
        AttackPlayer,
        RunAway
    }

    /// <summary>
    /// Персонаж, доступный для выбора в качестве играбельного в начале игры
    /// </summary>
    [System.AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public sealed class PlayerClassAttribute : Attribute
    {
        public PlayerClassAttribute(string name) { Name = name; }
        public string Name { get; private set; }
        public string Description { get; set; }
        public string StartingInventory { get; set; }
    }


    /// <summary>
    /// Тестовый персонаж
    /// </summary>
    [Spawnable("char")]
    public class Char : Actor
    {
        string[] names = new string[]
        {
            "Старый бандит",
            "Разбойник",
            "Одноглазый бандит",
            "Наёмник",
            "Расхититель гробниц",
            "Бандит"
        };

        public string CharName { get; set; } = "Бандит";

        public override string Name => CharName;

        bool enraged = false;
        protected override ThoughtTypeEnum _Think(Actor hostile)
        {
            if (Health == 0)
                return ThoughtTypeEnum.Dead;
            if (hostile == this)
                return ThoughtTypeEnum.Stand;

            if (enraged)
            {
                if (Health < MaxHealth / 3)
                {
                    if (Spawner.Random.Next(0, 3) == 0)
                        Dungeon.SendClientMessage(this, $"{Name}: Нет! Пощадите меня!");
                    return ThoughtTypeEnum.RunAway;
                }
                return ThoughtTypeEnum.AttackPlayer;
            }
            if (Spawner.Distance(this, hostile) <= 6 && !enraged)
            {
                Dungeon.SendClientMessage(this, $"{Name}: Попался! Пощады не жди!");
                enraged = true;
            }
            return ThoughtTypeEnum.Stand;
        }

        public Char()
        {
            MaxHealth = 10;
            Health = 10;
            MeleeDamage = 2;
            MeleeSkill = 5;
            MoveType = MoveTypeEnum.Walking;
            Thoughts = ThoughtTypeEnum.Stand;
            MaxMovePoints = 3;
            MovePoints = MaxMovePoints;
            Score = 50;

            CharName = names[Spawner.Random.Next(0, names.Length)];
        }

    }

    [Spawnable("spider")]
    public class Spider : Actor
    {
        public string CharName { get; set; } = "Гигантский паук";

        public override string Name => CharName;

        bool enraged = false;
        protected override ThoughtTypeEnum _Think(Actor hostile)
        {
            if (Health == 0)
                return ThoughtTypeEnum.Dead;
            if (hostile == this)
                return ThoughtTypeEnum.Stand;

            if (enraged)
            {
                return ThoughtTypeEnum.AttackPlayer;
            }
            if (Spawner.Distance(this, hostile) <= 6 && !enraged)
            {
                Dungeon.SendClientMessage(this, $"{Name} страшно шипит");
                enraged = true;
            }
            return ThoughtTypeEnum.Stand;
        }


        public Spider()
        {
            MaxHealth = 5;
            Health = 5;
            MeleeDamage = 3;
            MeleeSkill = 5;
            MoveType = MoveTypeEnum.Walking;
            Thoughts = ThoughtTypeEnum.Stand;
            MaxMovePoints = 4;
            MovePoints = MaxMovePoints;
            Score = 25;
        }
    }

    [Spawnable("zombie")]
    public class Zombie : Actor
    {
        public override string Name => "Безмозглый зомби";

        protected override ThoughtTypeEnum _Think(Actor hostile)
        {
            if (Health == 0)
                return ThoughtTypeEnum.Dead;
            if (hostile == this)
                return ThoughtTypeEnum.Stand;

            if (Spawner.Distance(this, hostile) <= 6 && Spawner.Random.Next(0, 2) == 0)
                Dungeon.SendClientMessage(this, $"{Name} тихо стонет");

            return (Spawner.Random.Next(0,2) == 0) ? ThoughtTypeEnum.Stand : ThoughtTypeEnum.Wander;
        }

        public Zombie()
        {
            MaxHealth = 5;
            Health = 5;
            MeleeDamage = 3;
            MeleeSkill = 5;
            MoveType = MoveTypeEnum.Walking;
            Thoughts = ThoughtTypeEnum.Wander;
            MaxMovePoints = 4;
            MovePoints = MaxMovePoints;
            Score = 10;
        }
    }





    [PlayerClass(
        "Рыцарь", Description = "Сбалансированный класс для начинающих", StartingInventory = "food;food;sword"
    )]
    [PlayerClass(
        "Бедный рыцарь", Description = "Несбалансированный класс для неначинающих"
    )]
    public class KnightClass : Actor
    {
        public string CName { get; set; }
        public override string Name => CName;

        protected override ThoughtTypeEnum _Think(Actor hostile)
        {
            return (Health > 0) ? ThoughtTypeEnum.Dead : ThoughtTypeEnum.Wander;
        }

        public KnightClass()
        {
            CName = "Мистер Прикол";
            HungerRate = 1;

            MaxHealth = 25;
            Health = 25;
            MeleeDamage = 5;
            MeleeSkill = 7;
            MoveType = MoveTypeEnum.Walking;
            Thoughts = ThoughtTypeEnum.Stand;
            MaxMovePoints = 2;
            MovePoints = MaxMovePoints;
        }

        public KnightClass(string name) : this()
        {
            CName = name;
        }
    }
}