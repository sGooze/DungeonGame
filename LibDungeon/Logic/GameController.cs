using System;
using System.Collections.Generic;
using System.Linq;

namespace LibDungeon
{
    using Levels;
    using Objects;

    public partial class Dungeon
    {
        /// <summary>
        /// Симулирует один шаг игрового мира
        /// </summary>
        public void Think()
        {
            // Перед Think должен происходить сброс очков движения игрока
            UpdateVisits();

            // Даже когда игрок не может двигаться, нужно обновлять его эффекты, статусы и проч.
            // Поэтому, если игрок присутствует в списке существ уровня, его Thiink тоже обновляется,
            // но для PlayerPawn всегда возвращается мысль "ничего не делать"
            while(PlayerPawn.MovePoints++ < PlayerPawn.MaxMovePoints)
            {
                foreach (var actor in CurrentLevel.FloorActors)
                {
                    /*if (actor == PlayerPawn)
                        continue;*/
                    if (actor.MovePoints < actor.MaxMovePoints && actor != PlayerPawn)
                    {
                        actor.MovePoints++;
                        continue;
                    }
                    // HACK: Обновлять монстров тоьлко на видимых клетках
                    if (!CurrentLevel.Tiles[actor.X, actor.Y].Visible)
                        continue;
                    switch (actor.Think(PlayerPawn))
                    {
                        case ThoughtTypeEnum.Dead:
                            break;
                        case ThoughtTypeEnum.Stand:
                            continue;
                        case ThoughtTypeEnum.AttackPlayer:
                            // TODO: Переписать MoveAction, чтобы можно было ходить на клетку, а не в направлении
                            if ((actor.X - PlayerPawn.X) * (actor.X - PlayerPawn.X) + (actor.Y - PlayerPawn.Y) * (actor.Y - PlayerPawn.Y) <= 2)
                                ;//ATTACK
                            else
                            {
                                int x = actor.X - Math.Sign(actor.X - PlayerPawn.X),
                                    y = actor.Y - Math.Sign(actor.Y - PlayerPawn.Y);
                                // TODO: Проверять наличие свободного пола и актёров в одной процедуре?
                                if (CurrentLevel.Tiles[x, y].Solidity == Tile.SolidityType.Floor)
                                    actor.ChangePos(x, y);
                            }
                            break;
                        case ThoughtTypeEnum.RunAway:
                            break;
                    }
                    actor.MovePoints = 0;
                }
            }
        }

        public bool ApplyAction(Actor actor, PlayerCommand action)
        {
            // TODO: Применяет выбранные действия вне зависимости от действующего лица
            return true;
        }
    }
}