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
            //UpdateVisits(); // Не нужно здесь - если игрок не двигается, то и поле зрения не обновляется

            // Даже когда игрок не может двигаться, нужно обновлять его эффекты, статусы и проч.
            // Поэтому, если игрок присутствует в списке существ уровня, его Thiink тоже обновляется,
            // но для PlayerPawn всегда возвращается мысль "ничего не делать"
            while(PlayerPawn.MovePoints++ < PlayerPawn.MaxMovePoints)
            {
                foreach (var actor in CurrentFloor.FloorActors)
                {
                    Hunger(actor);
                    if (actor == PlayerPawn)
                        continue;
                    if (actor.MovePoints < actor.MaxMovePoints && actor != PlayerPawn)
                    {
                        actor.MovePoints++;
                        continue;
                    }
                    // HACK: Обновлять монстров тоьлко на видимых клетках
                    if (!CurrentFloor.Tiles[actor.X, actor.Y].Visible)
                        continue;
                    // В зависимости от принятого решения актёр может...
                    switch (actor.Think(PlayerPawn))
                    {
                        case ThoughtTypeEnum.Dead:
                            // Быть мёртвым и ничего не делать
                            break;
                        case ThoughtTypeEnum.Stand:
                            // Стоять на месте
                            continue;
                        case ThoughtTypeEnum.Wander:
                            // Бродить в случайном направлении
                            MoveActor(actor,
                                    Spawner.Random.Next(0, CurrentFloor.Width),
                                    Spawner.Random.Next(0, CurrentFloor.Height)
                                );
                            break;
                        case ThoughtTypeEnum.AttackPlayer:
                            // Преследовать игрока
                            MoveActor(actor, PlayerPawn.X, PlayerPawn.Y);
                            break;
                        case ThoughtTypeEnum.RunAway:
                            // Испуганный противник убегает в противоположном направлении от игрока; если ему не 
                            // удаётся сдвинуться в этом направлении, то он пытается выбрать сдвинуться наугад
                            if (!MoveActor(actor, PlayerPawn.X, PlayerPawn.Y))
                                goto case ThoughtTypeEnum.Wander;
                            break;
                    }
                    actor.MovePoints = 0;
                }
            }
        }

        /// <summary>
        /// Воздействие голода и регенерации здоровья
        /// </summary>
        /// <param name="actor"></param>
        private void Hunger(Actor actor)
        {
            // Некоторые персонажи не испытывают голода и не должны подвергаться его эффектам
            if (actor.HungerRate == 0 || actor.Health == 0)
                return;
            // Неподвижные актёры медленнее становятся голоднее
            // Подвижные актёры, несущие в экипировке много предметров, быстрее становятся голоднее
            int hungerRate = (actor.Thoughts == ThoughtTypeEnum.Stand) 
                ? Math.Max(1, actor.HungerRate / 2) 
                : actor.HungerRate + (actor.Equipment.Count / (2 * actor.MaxMovePoints) );

            actor.Hunger += hungerRate;

            // Восстановление здоровья: чем голоднее актёр, тем медленнее оно регенерирует
            int healRate = (actor.Hunger/(Actor.maxHunger / 4) + 1)
                * actor.MaxMovePoints       // Скорость восстановления зависит от быстроты действий актёра
                * 5                         // И искусственно замедляется во столько раз
                ;
            if (actor.Hunger == Actor.maxHunger)
            {
                SendClientMessage(null, $"{actor.Name} умер от голода");
                actor.Health = 0;
            }
            else if (actor.Hunger % (healRate) == 0)
                actor.Health += 1;
        }

        public bool MoveActor(Actor actor, int dir_x, int dir_y)
        {
            // Вычислить точку, на которую нужно сдвинуться, передвинуться на одну позицию в её сторону,
            // попутно выполняя возможные действия
            int x = actor.X - Math.Sign(actor.X - dir_x),
                y = actor.Y - Math.Sign(actor.Y - dir_y);
            if (CurrentFloor.Tiles[x, y].Solidity == Tile.SolidityType.Wall)
            {
                // Если это дверь, то открыть её
                if (CurrentFloor.Tiles[x, y] is Door)
                {
                    (CurrentFloor.Tiles[x, y] as Door).IsOpen = true;
                    return true;
                }
                // Если стена, то упереться в неё и не засчитать ход
                return false;
            }
            // Проверка на столкновение с актёром
            var occupant = CurrentFloor.FloorActors.FirstOrDefault(a => a.X == x && a.Y == y && a.Health > 0);
            if (occupant != null)
            {
                // Актёры могут атаковать только игрока
                if (actor != PlayerPawn && occupant != PlayerPawn)
                    return false;
                actor.MeleeAttack(occupant);
                return true;
            }

            actor.ChangePos(x, y);
            return true;
        }

        public bool ApplyAction(Actor actor, PlayerCommand action)
        {
            // TODO: Применяет выбранные действия вне зависимости от действующего лица
            return true;
        }
    }
}