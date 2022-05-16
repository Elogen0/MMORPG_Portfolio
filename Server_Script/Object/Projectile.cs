using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer.Game
{
    public class Projectile : GameObject
    {
        public Data.SkillData SkillData { get; set; }

        public Projectile()
        {
            ObjectType = GameObjectType.Projectile;
        }

    }
}
