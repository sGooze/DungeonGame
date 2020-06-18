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

        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        // Статы


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
        AttackPlayer,
        RunAway
    }




    /// <summary>
    /// Тестовый персонаж
    /// </summary>
    [Spawnable("char")]
    public class Char : Actor
    {
        public override string Name => "Персонаж";

        bool enraged = false;
        public override ThoughtTypeEnum Think(Actor hostile)
        {
            if (Health == 0)
                return ThoughtTypeEnum.Dead;
            if (hostile == this)
                return ThoughtTypeEnum.Stand;

            if (enraged) return ThoughtTypeEnum.AttackPlayer;
            if (Math.Pow(X - hostile.X, 2) + Math.Pow(Y - hostile.Y, 2) <= 36 && !enraged)
            {
                Dungeon.SendClientMessage(this, $"{Name}: Я вижу тебя! Ты на {hostile.X},{hostile.Y}");
                enraged = true;
            }
            return ThoughtTypeEnum.Stand;
        }

        public Char()
        {
            Health = 10;
            Attack = 2;
            MoveType = MoveTypeEnum.Walking;
            Thoughts = ThoughtTypeEnum.Stand;
            MaxMovePoints = 4;
            MovePoints = MaxMovePoints;
        }
    }
}
