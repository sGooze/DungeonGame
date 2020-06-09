using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace LibDungeon.Objects
{
    public abstract class Actor
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Health { get; set; }
        // Статы


        /// <summary>
        /// Очки передвижения. Пока они не исчерпаны, актёр может продолжать думать
        /// Перезаряжаются в конце раунда
        /// </summary>
        public int MovePoints { get; set; }

        public string Name { get; }
        public abstract int MaxMovePoints { get; }

        public abstract void Think();
    }

    /// <summary>
    /// Тип передвижения
    /// </summary>
    public enum MoveType
    {
        Stationary,
        Walking,
        Jumping,
        Flying
    }

    public enum ThoughtType
    {
        Dead,
        Stand,
        AttackPlayer,
        RunAway
    }
}
