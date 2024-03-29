﻿using InflearnServer.Game.Job;
using System;
using System.Collections.Generic;
using System.Text;

namespace InflearnServer.Game
{
    public class GameLogic : JobSerializer
    {
        public static GameLogic Instance { get; } = new GameLogic();
        Dictionary<int, GameRoom>  _rooms = new Dictionary<int, GameRoom>();
        int _roomId = 1;

        public void Update()
        {
            Flush();

            foreach (GameRoom room in _rooms.Values)
            {
                room.Update();
            }
        }

        public GameRoom Add(int mapId)
        {
            GameRoom gameRoom = new GameRoom();
            gameRoom.Push(gameRoom.Init, mapId, 10);

            gameRoom.RoomId = mapId;
            _rooms.Add(mapId, gameRoom);

            return gameRoom;
        }


        public bool Remove(int roomId)
        {
            return _rooms.Remove(roomId);
        }

        public GameRoom Find(int roomId)
        {
            GameRoom room = null;
            if (_rooms.TryGetValue(roomId, out room))
            {
                return room;
            }
            return null;
        }
    }
}
