using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibDungeon.Levels
{
    abstract public class Tile
    {
        private bool visible = false;

        public enum SolidityType
        {
            Wall,
            Floor,
            Door
        }
        /// <summary>
        /// Тип тайла (проницаемость/проходимость)
        /// </summary>
        public abstract SolidityType Solidity { get; }

        public bool Visible { get => visible; set { if (value) Visited = true; visible = value; } }
        /// <summary>
        /// Посещённые тайлы отображаются на экране даже в отсутствие прямой видимости
        /// </summary>
        public bool Visited { get; set; } = false;
    }

    public class Floor : Tile
    {
        public override SolidityType Solidity => SolidityType.Floor;
    }

    public class Wall : Tile
    {
        public override SolidityType Solidity => SolidityType.Wall;
    }

    public class Door : Tile
    {
        public bool IsOpen { get; set; } = false;
        public override SolidityType Solidity => (IsOpen) ? SolidityType.Floor : SolidityType.Wall;
    }

    public class Ladder : Tile
    {
        public enum LadderDirection
        {
            Up,
            Down
        }

        /// <summary>
        /// Используется для связывания лестниц на разных этажах
        /// </summary>
        public int LadderId { get; set; }
        /// <summary>
        /// Направление, в которое ведёт лестница
        /// </summary>
        public LadderDirection Direction { get; set; } = LadderDirection.Down;
        public override SolidityType Solidity => SolidityType.Floor;
    }
}
