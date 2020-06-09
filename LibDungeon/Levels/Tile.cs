using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDungeon.Levels
{
    abstract public class Tile
    {
        public enum SolidityType
        {
            Wall,
            Floor,
            Door
        }
        /// <summary>
        /// Тип тайла (проницаемость/проходимость)
        /// </summary>
        public SolidityType Solidity { get; protected set; }
        /// <summary>
        /// Посещённые тайлы отображаются на экране даже в отсутствие прямой видимости
        /// </summary>
        public bool Visited { get; set; } = false;
    }

    public class Floor : Tile
    {
        public Floor() { Solidity = SolidityType.Floor; }
    }

    public class Wall : Tile
    {
        public Wall() { Solidity = SolidityType.Wall; }
    }

    public class Door : Tile
    {
        public Door() { Solidity = SolidityType.Door; }
    }
}
