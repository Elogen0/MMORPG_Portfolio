using Google.Protobuf.Protocol;
using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer.Game
{
    public class Zone
    {
        public int IndexY { get; private set; }
        public int IndexX { get; private set; }

        public HashSet<Player> Players = new HashSet<Player>();
        public HashSet<Monster> Monsters = new HashSet<Monster>();
        public HashSet<Projectile> Projectiles = new HashSet<Projectile>();

        public Zone(int x, int y)
        {
            IndexX = x;
            IndexY = y;
        }

        public void Remove(GameObject gameObject)
        {
            GameObjectType type = ObjectManager.GetObjectTypeById(gameObject.Id);

            switch (type)
            {
                case GameObjectType.Player:
                    Players.Remove((Player)gameObject);
                    break;
                case GameObjectType.Monster:
                    Monsters.Remove((Monster)gameObject);
                    break;
                case GameObjectType.Projectile:
                    Projectiles.Remove((Projectile)gameObject);
                    break;
            }
        }

        public Player FindOnePlayer(Func<Player, bool> condition)
        {
            foreach (var player in Players)
            {
                if (condition.Invoke(player))
                    return player;
            }
            return null;
        }

        public List<Player> FindAllPlayer(Func<Player, bool> condition)
        {
            List<Player> findList = new List<Player>();
            foreach (var player in Players)
            {
                if (condition.Invoke(player))
                    findList.Add(player);
            }
            return findList;
        }
    }
}
