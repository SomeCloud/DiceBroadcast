﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    [Serializable]
    public enum AGameStatus
    {
        // ожидаем игроков
        Wait,
        // игра идет
        Game,
        // игра окончена
        Over
    }

    [Serializable]
    public class ARoom
    {
        public delegate void OnCloseRoomEvent();
        public event OnCloseRoomEvent CloseRoomEvent;

        public delegate void OnChangeRoomStatusEvent(AGameStatus status);
        public event OnChangeRoomStatusEvent ChangeRoomStatusEvent;

        // идентификатор комнаты
        public int Id;
        // название комнаты
        public string Name;
        // список игроков
        public AList<APlayer> Players;
        // статус игры
        public AGameStatus GameStatus;

        public AGameStatus SetGameStatus
        {
            set
            {
                GameStatus = value;
                ChangeRoomStatusEvent?.Invoke(value);
            }
            get => GameStatus;
        }

        // ограничение по количеству игроков
        public int MaxPlayers;
        // игрок, который делает сейчас ход
        public APlayer ActivePlayer;

        public ARoom(int id, string name, APlayer player, int max)
        {
            Id = id;
            Name = name;
            Players = new AList<APlayer>();
            Players.Add(player);
            MaxPlayers = max;
            SetGameStatus = AGameStatus.Wait;
            ActivePlayer = player;

            Players.BeforeAddEvent += (item) => { 
                if (Players.Count == MaxPlayers - 1)
                {
                    SetGameStatus = AGameStatus.Game;
                }
            };

            Players.AfterRemoveEvent += (item) => { 
                if (Players.Count == 0)
                {
                    CloseRoomEvent?.Invoke();
                }
            };
        }

        public ARoom(CRoom room, int id)
        {
            Id = id;
            Name = room.RoomName;
            Players = new AList<APlayer>() { new APlayer(room.PlayerName, 1) };
            MaxPlayers = room.MaxPlayers;
            SetGameStatus = AGameStatus.Wait;
            ActivePlayer = Players.First();

            Players.BeforeAddEvent += (item) => {
                if (Players.Count == MaxPlayers - 1)
                {
                    SetGameStatus = AGameStatus.Game;
                }
            };

            Players.AfterRemoveEvent += (item) => {
                if (Players.Count == 0)
                {
                    CloseRoomEvent?.Invoke();
                }
            };
        }

        public void NextPlayer()
        {
            for (int i = 0; i < Players.Count; i++)
            {

                if (Players[i] == ActivePlayer)
                {
                    if (i + 1 < Players.Count)
                    {
                        ActivePlayer = Players[i + 1];
                        break;
                    }
                    else
                    {
                        ActivePlayer = Players[0];
                        break;
                    }
                }
            }
        }

        public APlayer GetPlayerById(int id)
        {
            foreach (APlayer player in Players)
            {
                if (player.Id == id)
                {
                    return player;
                }
            }
            return null;
        }

        public bool DisconnectPlayer(APlayer player)
        {
            foreach (APlayer pl in Players)
            {
                if (player.Id == pl.Id)
                {
                    Players.Remove(pl);
                    return true;
                }
            }
            return false;
        }

        public bool AddPlayer(APlayer player)
        {
            if (Players.Count < MaxPlayers) {
                Players.Add(player);
                return true; 
            }
            else return false;
            
        }

    }

    [Serializable]
    public class CRoom
    {
        public int Id;
        public string RoomName;
        public string PlayerName;
        public int MaxPlayers;

        public CRoom(int id, string room, string player, int max) {
            Id = id;
            RoomName = room;
            PlayerName = player;
            MaxPlayers = max;
        }

    }

}
