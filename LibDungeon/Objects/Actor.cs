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
        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
        public int Attack { get; set; }
        // Статы


        // Состояние
        public MoveTypeEnum MoveType { get; set; }
        public ThoughtTypeEnum Thoughts { get; set; }


        /// <summary>
        /// Очки передвижения. Пока они не исчерпаны, актёр может продолжать думать
        /// Перезаряжаются в конце раунда
        /// </summary>
        public int MovePoints { get; set; }

        public abstract string Name { get; }
        public abstract int MaxMovePoints { get; }

        /// <summary>
        /// Определяет дальнейшие действия
        /// </summary>
        public abstract void Think();

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

        public override int MaxMovePoints => 2;

        public override void Think()
        {
            throw new NotImplementedException();
        }

        public Char()
        {
            Health = 10;
            Attack = 2;
            MoveType = MoveTypeEnum.Walking;
            Thoughts = ThoughtTypeEnum.Stand;
            MovePoints = MaxMovePoints;
        }
    }
}
