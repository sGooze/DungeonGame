using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LibDungeon
{
    using LibDungeon.Levels;
    using Objects;
    using System.IO;
    using System.Security.Cryptography.X509Certificates;

    public partial class Dungeon
    {
        Random random = new Random();
        public enum PlayerCommand
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

            Wait,

            LadderDown,
            LadderUp,

            PickupItem,

            OpenDoor,
            CloseDoor
        }

        
        

        /// <summary>
        /// Выполнить игровое действие
        /// </summary>
        /// <param name="command">Выбранное действие</param>
        /// <returns>
        /// <code>true</code>, если действие принято и ход засчитан, <code>false</code> в обратном случае.
        /// </returns>
        public bool PlayerMove(PlayerCommand command) {
            bool move = _PlayerMove(command);
            if (move)
            {
                UpdateVisits();
                PlayerPawn.MovePoints = 0;
                Think();
            }
            return move;
        }

        /// <summary>
        /// Переместить игрока в указанном направлении
        /// </summary>
        /// <param name="dir_x"></param>
        /// <param name="dir_y"></param>
        /// <returns></returns>
        public bool PlayerMove(int dir_x, int dir_y) {
            bool move = _PlayerMove(dir_x, dir_y);
            if (move)
            {
                UpdateVisits();
                PlayerPawn.MovePoints = 0;
                Think();
            }
            return move;
        }

        private bool _PlayerMove(int dir_x, int dir_y)
        {
            bool move = MoveActor(PlayerPawn, dir_x, dir_y);
            if (move)
            {
                /*PlayerPawn.MovePoints = 0;
                Think();*/
                // Если мы не в бою, то попробуем подобрать что-нибудь
                PlayerMove(PlayerCommand.PickupItem);
            }
            return move;
        }

        private bool _PlayerMove(PlayerCommand command)
        {
            // Исходя из команды, выполнить то или иное действие.
            // Если команда пройдёт успешно, то исчерпать запасы стамины и гонять Think, пока она не восстановится
            // Перемещение: 
            if ((int)command < (int)PlayerCommand.Wait)
            {
                int x = PlayerPawn.X, y = PlayerPawn.Y;
                switch (command)
                {
                    case PlayerCommand.Move0:
                        x++;
                        break;
                    case PlayerCommand.Move45:
                        x++; y--;
                        break;
                    case PlayerCommand.Move90:
                        y--;
                        break;
                    case PlayerCommand.Move135:
                        x--; y--;
                        break;
                    case PlayerCommand.Move180:
                        x--;
                        break;
                    case PlayerCommand.Move225:
                        x--; y++;
                        break;
                    case PlayerCommand.Move270:
                        y++;
                        break;
                    case PlayerCommand.Move315:
                        x++; y++;
                        break;
                }

                return _PlayerMove(x, y);
            }


            if (command == PlayerCommand.Wait)
            {
                // Во время отдыха игрок должен быстрее регенерировать здоровье
                //  и медленнее становиться голоднее
                return true;
            }

            // Лестница
            if (command == PlayerCommand.LadderDown || command == PlayerCommand.LadderUp)
            {
                if (!(CurrentFloor.Tiles[PlayerPawn.X, PlayerPawn.Y] is Ladder))
                    return false;

                Ladder ourpos = CurrentFloor.Tiles[PlayerPawn.X, PlayerPawn.Y] as Ladder;
                int nextlevel = currentLevel + ((ourpos.Direction == Ladder.LadderDirection.Up) ? -1 : 1);
                if (nextlevel < 0)
                    return false;
                // Если следующий уровень ещё не сгенерирован, то сгенерировать и связать текущую лестницу со случайной              
                if (nextlevel > floors.Count - 1)// Уравнял счетчик с индексами
                {
                    ourpos.LadderId = random.Next(1, Int32.MaxValue);
                    var newfloor = new DungeonFloor(
                        Spawner.Random.Next(currentLevel+2*10, 81), Spawner.Random.Next(currentLevel + 2 * 10, 81)
                    ); 
                    floors.Add(newfloor); newfloor.FloorActors.AddLast(PlayerPawn);
                    int lx = 0;
                    int ly = 0;
                    for (int i = 0; i < newfloor.Width; i++)
                    {                    
                        for (int j = 0; j < newfloor.Height; j++)
                        {
                        
                            if (newfloor.Tiles[i,j] is Ladder 
                                && (newfloor.Tiles[i, j] as Ladder).Direction == Ladder.LadderDirection.Up)
                            {   
                                lx = i;
                                ly = j;
                                PlayerPawn.ChangePos(lx, ly);                                
                            }
                        }
                    }
                    (newfloor.Tiles[lx, ly] as Ladder).LadderId = ourpos.LadderId;
                    SendClientMessage(null, $"Вы спускаетесь на уровень {nextlevel}");
                }
                // Если он уже существует, то попытаться найти свободную лестницу, если не выйдет, то скинуть в случ.
                // позицию.
                else
                {
                    //Этаж на который переходим
                    var floor = floors[nextlevel];
                    
                    if (ourpos.LadderId == 0)
                    {
                        int lx = 0;
                        int ly = 0;
                        //Задаем id для новой лестницы
                        ourpos.LadderId = random.Next(1, Int32.MaxValue);
                        for (int i = 0; i < floor.Width; i++)
                        {
                            for (int j = 0; j < floor.Height; j++)
                            {
                                //Если 2 лестницы не связаны, то ищем свободную
                                if (floor.Tiles[i, j] is Ladder                               
                                    && (floor.Tiles[i, j] as Ladder).LadderId == 0
                                    && (floor.Tiles[i, j] as Ladder).Direction != ourpos.Direction)
                                {
                                    lx = i;
                                    ly = j;
                                    PlayerPawn.ChangePos(lx, ly);                                       
                                }
                            }
                        }
                        (floor.Tiles[lx, ly] as Ladder).LadderId = ourpos.LadderId;
                    }

                    else 
                    {
                        
                        for (int i = 0; i < floor.Width; i++)
                        {
                            for (int j = 0; j < floor.Height; j++)
                            {
                                //Если 2 лестницы связаны то перемещаемся по ним
                                if (floor.Tiles[i, j] is Ladder                               
                                    && (floor.Tiles[i, j] as Ladder).LadderId == ourpos.LadderId)
                                {
                                    PlayerPawn.ChangePos(i, j);                                    
                                }
                            }
                        } 
                    }
                    SendClientMessage(null, $"Вы возвращаетесь на уровень {nextlevel}");
                }

                currentLevel = nextlevel;
                return true;
            }

            // Поднятие предмета
            if (command == PlayerCommand.PickupItem)
            {
                var item = CurrentFloor.FloorItems.FirstOrDefault(x => x.X == PlayerPawn.X && x.Y == PlayerPawn.Y);
                if (item == null)
                    return false;

                CurrentFloor.FloorItems.Remove(item);
                if (item.RemoveOnPickup)
                    item.Use(PlayerPawn);
                else
                    Inventory.AddLast(item);
                return true;
            }

            // Открытие / закрытие дверей
            if (command == PlayerCommand.OpenDoor || command == PlayerCommand.CloseDoor)
            {
                for (int i = PlayerPawn.X - 1; i <= PlayerPawn.X + 1; i++)
                    for (int j = PlayerPawn.Y - 1; j <= PlayerPawn.Y + 1; j++)
                        if (CurrentFloor.Tiles[i, j] is Door)
                        {
                            (CurrentFloor.Tiles[i, j] as Door).IsOpen =
                                (command == PlayerCommand.OpenDoor) ? true : false;
                            return true;
                        }
                return false;
            }

            return false;
        }






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
            foreach (var t in CurrentFloor.Tiles)
                t.Visible = false;

            var tile = CurrentFloor.Tiles[PlayerPawn.X, PlayerPawn.Y];
            if (tile.Solidity != Tile.SolidityType.Floor)
                return;

            List<(int, int)> border = new List<(int, int)>() 
                { (-1, -1), (-1, 0), (-1, 1), (0, -1), (0, 1), (1, -1), (1, 0), (1, 1) };
            CurrentFloor.Tiles[PlayerPawn.X, PlayerPawn.Y].Visited = true;
            Queue<(int, int)> visq = new Queue<(int, int)>(); 
            visq.Enqueue((PlayerPawn.X, PlayerPawn.Y));
            while (visq.Count > 0)
            {
                var node = visq.Dequeue();
                foreach (var neigh in border.Select(x => (x.Item1 + node.Item1, x.Item2 + node.Item2)))
                    if (!CurrentFloor.Tiles[neigh.Item1, neigh.Item2].Visible
                        && (neigh.Item1 - PlayerPawn.X)* (neigh.Item1 - PlayerPawn.X) + (neigh.Item2 - PlayerPawn.Y)* (neigh.Item2 - PlayerPawn.Y) < 64)
                    {
                        CurrentFloor.Tiles[neigh.Item1, neigh.Item2].Visible = true;
                        if (CurrentFloor.Tiles[neigh.Item1, neigh.Item2].Solidity == Tile.SolidityType.Floor)
                            visq.Enqueue(neigh);
                    }
            }
        }


        public LinkedList<BaseItem> Inventory { get; set; } = new LinkedList<BaseItem>();

    }
}
