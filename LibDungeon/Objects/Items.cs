using System;
using System.Collections.Generic;
using System.Text;

namespace LibDungeon.Objects
{
    //[Spawnable("meme")]
    public class TestItem : BaseItem
    {
        int rand;

        public override string Name => $"Шутка за {rand}";

        public override bool RemoveOnPickup => false;

        public override bool OneTimeUse => throw new NotImplementedException();

        public override void Unuse(Actor user)
        {
            throw new NotImplementedException();
        }

        public override void Use(Actor user)
        {
            throw new NotImplementedException();
        }

        public TestItem() => rand = Spawner.Random.Next(0, 301);
    }

    [Spawnable("food")]
    public class Food : BaseItem
    {
        public override string Name => "Пищевой паёк";

        public override bool RemoveOnPickup => false;

        public override bool OneTimeUse => true;

        public override void Unuse(Actor user) { }

        public override void Use(Actor user)
        {
            user.Hunger -= 250;
            user.Health += 5;
        }
    }


    [Spawnable("apple")]
    public class Apple : BaseItem
    {
        public override string Name => "Яблоко";

        public override bool RemoveOnPickup => false;

        public override bool OneTimeUse => true;

        public override void Unuse(Actor user) { }

        public override void Use(Actor user)
        {
            user.Hunger -= 100;
            user.Health += 5;
        }
    }

    [Spawnable("sword")]
    public class Sword : BaseItem
    {
        public override string Name => "Тяжёлый меч";

        public override bool RemoveOnPickup => false;

        public override bool OneTimeUse => false;

        public override void Unuse(Actor user)
        {
            user.MeleeDamage -= 5;
            user.HungerRate -= 1;
        }

        public override void Use(Actor user)
        {
            user.MeleeDamage += 5;
            user.HungerRate += 1;
        }
    }

    [Spawnable("shield")]
    public class Shield : BaseItem
    {
        public override string Name => "Щит";

        public override bool RemoveOnPickup => false;

        public override bool OneTimeUse => false;

        public override void Unuse(Actor user)
        {
            user.MaxHealth -= 5;
            user.Health = Math.Min(1, user.Health - 5);
            user.HungerRate -= 1;
        }

        public override void Use(Actor user)
        {
            user.MaxHealth += 5;
            user.Health += 5;
            user.HungerRate += 1;
        }
    }

    [Spawnable("armor")]
    public class Armor : BaseItem
    {
        public override string Name => "Броня";

        public override bool RemoveOnPickup => false;

        public override bool OneTimeUse => false;

        public override void Unuse(Actor user)
        {
            user.MaxHealth -= 10;
            user.Health = Math.Max(1, user.Health - 10);
            user.HungerRate -= 1;
        }

        public override void Use(Actor user)
        {
            user.MaxHealth += 10;
            user.Health += 10;
            user.HungerRate += 1;
        }
    }
}
