using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LibDungeon.Objects
{
    /// <summary>
    /// Базовый класс для актёров
    /// </summary>
    public abstract class Actor
    {
        private int movePoints;
        private int health;

        public int X { get; set; }
        public int Y { get; set; }
        // Статы
        public int Health { get => health; set => health = Math.Max(Math.Min(value, MaxHealth),0); }
        public int MaxHealth { get; set; }
        public int MeleeDamage { get; set; }

        // Скиллы

        /// <summary>
        /// Определяет вероятность того, что атака будет успешной
        /// </summary>
        public int MeleeSkill { get; set; }


        // Состояние
        public MoveTypeEnum MoveType { get; set; }
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
        public abstract ThoughtTypeEnum Think(Actor hostile);




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
                Dungeon.SendClientMessage(this, $"{target.Name} умирает");

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
    /// Тестовый персонаж
    /// </summary>
    [Spawnable("char")]
    public class Char : Actor
    {
        public string CharName { get; set; } = "Персонаж";

        public override string Name => CharName;

        bool enraged = false;
        public override ThoughtTypeEnum Think(Actor hostile)
        {
            if (Health == 0)
                return ThoughtTypeEnum.Dead;
            if (hostile == this)
                return ThoughtTypeEnum.Stand;

            if (enraged) return ThoughtTypeEnum.AttackPlayer;
            if (Spawner.Distance(this, hostile) <= 6 && !enraged)
            {
                Dungeon.SendClientMessage(this, $"{Name}: Я вижу тебя! Ты на {hostile.X},{hostile.Y}");
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
            MaxMovePoints = 4;
            MovePoints = MaxMovePoints;
        }
    }

    public class KnightClass : Actor
    {
        public string CName { get; set; }
        public override string Name => CName;

        public override ThoughtTypeEnum Think(Actor hostile)
        {
            return (Health > 0) ? ThoughtTypeEnum.Dead : ThoughtTypeEnum.Wander;
        }

        public KnightClass()
        {
            CName = "Мистер Прикол";

            MaxHealth = 25;
            Health = 25;
            MeleeDamage = 5;
            MeleeSkill = 7;
            MoveType = MoveTypeEnum.Walking;
            Thoughts = ThoughtTypeEnum.Stand;
            MaxMovePoints = 2;
            MovePoints = MaxMovePoints;
        }
    }
}
