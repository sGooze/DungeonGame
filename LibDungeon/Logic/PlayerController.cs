using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibDungeon
{
    using LibDungeon.Levels;
    using Objects;

    public partial class Dungeon
    {
        public enum PlayerCommands
        {
            // Перемещение по миру + взаимодействие + атака
            // Интервал 45 градусов
            Move0,
            Move45,
            Move90,
            Move135,
            Move180,
            Move225,
            Move270,
            Move315,
        }

        public class RemoteMsg
        {
            PlayerCommands MsgType { get; set; }
            Dictionary<string, string> KeyValues { get; set; } = new Dictionary<string, string>();
        }

        public readonly Dictionary<PlayerCommands, Func<RemoteMsg, RemoteMsg>> RemoteMsgHandlers
            = new Dictionary<PlayerCommands, Func<RemoteMsg, RemoteMsg>>();

        /// <summary>
        /// Управляемый игроком персонаж
        /// </summary>
        public Actor PlayerPawn { get; set; }

        // TODO: Всё временно, убрать!!!!
        /// <summary>
        /// Обновление списка посещённых (хотя бы единожды увиденных) тайлов
        /// </summary>
        [Placeholder]
        internal void UpdateVisits()
        {
            var tile = CurrentLevel.Tiles[PlayerPawn.X, PlayerPawn.Y];
            if (tile.GetType() != typeof(Floor))
                return;

            List<(int, int)> border = new List<(int, int)>() 
                { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
            CurrentLevel.Tiles[PlayerPawn.X, PlayerPawn.Y].Visited = true;
            Queue<(int, int)> visq = new Queue<(int, int)>(); 
            visq.Enqueue((PlayerPawn.X, PlayerPawn.Y));
            while (visq.Count > 0)
            {
                var node = visq.Dequeue();
                foreach (var neigh in border.Select(x => (x.Item1 + node.Item1, x.Item2 + node.Item2)))
                    if (CurrentLevel.Tiles[neigh.Item1, neigh.Item2].Solidity == Tile.SolidityType.Floor 
                        && !CurrentLevel.Tiles[neigh.Item1, neigh.Item2].Visited
                        && (neigh.Item1 - PlayerPawn.X)* (neigh.Item1 - PlayerPawn.X) + (neigh.Item2 - PlayerPawn.Y)* (neigh.Item2 - PlayerPawn.Y) < 64)
                    {
                        CurrentLevel.Tiles[neigh.Item1, neigh.Item2].Visited = true;
                        visq.Enqueue(neigh);
                    }
            }
        }
    }
}
