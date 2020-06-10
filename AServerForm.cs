using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

namespace Coursework
{
    class AServerForm : Form
    {

        public delegate void OnRoomListChangeEvent();
        public event OnRoomListChangeEvent RoomListChangeEvent;

        public AServer Server;
        public AClient Client;

        public AServer LobbyServer;

        RichTextBox Chrono;
        AList<ARoom> Rooms;

        public AServerForm(string adress, int sendport, int receiveport, int lobbyport) : base()
        {

            Text = "Local Server";
            ClientSize = new Size(800, 600);

            Server = new AServer(adress, sendport); // 8001
            Client = new AClient(adress, receiveport); // 8000
            //LobbyServer = new AServer(adress, lobbyport);

            Thread LobbyThread;

            Rooms = new AList<ARoom>();

            Chrono = new RichTextBox() { Parent = this, Location = new Point(10, 10), Size = new Size(500, 580) };

            RoomListChangeEvent += () => {
                LobbyThread = new Thread(new ParameterizedThreadStart((object obj) => {
                    AServer LobbyServer = new AServer(adress, lobbyport);
                    while (true)
                    {
                        LobbyServer.StartSending(new AFrame(0, Rooms.Clone(), AMessageType.Undefined), true, "LobbyServer"); Thread.Sleep(500);
                    }
                }))
                { Name = "LobbyThread", IsBackground = true };
                LobbyThread.Start();
            };

            Client.StartReceive("ServerReceiver");

            Client.Receive += (frame) => {
                if (InvokeRequired) Invoke(new Action<AFrame>((s) =>
                {
                    ARoom room;
                    APlayer player;
                    CRoom croom;
                    switch (frame.MessageType)
                    {
                        case AMessageType.CreateGame:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == false)
                            {
                                room = new ARoom(croom, Rooms.Count + 1);
                                Rooms.Add(room);
                                Server.StartSending(new AFrame(room.Id, room, AMessageType.Connect), true, "ServerSender");
                                Chrono.AppendText("[System] : Создана комната - " + room.Name + "\n");
                                RoomListChangeEvent?.Invoke();
                            }
                            break;
                        case AMessageType.Connect:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == true)
                            {
                                APlayer newPlayer = new APlayer(croom.PlayerName, room.Players.Count + 1);
                                if (room.AddPlayer(newPlayer) == true)
                                {
                                    Server.StartSending(new AFrame(room.Id, room, AMessageType.Connect), true, "ServerSender");
                                    Chrono.AppendText("[" + room.Name + "] : Игрок " + croom.PlayerName + " подключился\n");
                                    if (room.GameStatus.Equals(AGameStatus.Game) == true)
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.Send), true, "ServerSender");
                                        Chrono.AppendText("[" + room.Name + "] : Игра началась\n");
                                    }
                                }
                            }
                            break;
                        case AMessageType.PlayerDisconnect:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == true)
                            {
                                if (FindPlayerByName(croom.PlayerName, room, out player) == true)
                                {
                                    room.Players.Remove(player);
                                    Chrono.AppendText("[" + room.Name + "] : Игрок " + croom.PlayerName + " отключился\n");
                                }
                                if (room.GameStatus.Equals(AGameStatus.Game) == true)
                                {
                                    if (room.Players.Count > 1)
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.PlayerDisconnect), true, "ServerSender");
                                    }
                                    else
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.GameOver), true, "ServerSender");
                                        Chrono.AppendText("[System] : комната " + room.Name + " расформирована\n");
                                        Rooms.Remove(room);
                                    }
                                }
                                else
                                {
                                    if (room.Players.Count >= 1)
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.PlayerDisconnect), true, "ServerSender");
                                    }
                                    else
                                    {
                                        Server.StartSending(new AFrame(room.Id, room, AMessageType.GameOver), true, "ServerSender");
                                        Chrono.AppendText("[System] : комната " + room.Name + " расформирована\n");
                                        Rooms.Remove(room);
                                    }
                                }
                                RoomListChangeEvent?.Invoke();
                            }
                            break;
                        case AMessageType.Send:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == true)
                            {
                                if (Process(room) == true)
                                {
                                    Server.StartSending(new AFrame(room.Id, room, AMessageType.Send), true, "ServerSender");
                                    Chrono.AppendText("[" + room.Name + "] : Игрок " + croom.PlayerName + " сделал ход (" + room.ActivePlayer.LastRound.Last() + ")\n");
                                }
                                else
                                {
                                    Server.StartSending(new AFrame(room.Id, room, AMessageType.GameOver), true, "ServerSender");
                                    Chrono.AppendText("[" + room.Name + "] : Игрок " + croom.PlayerName + " победил в игре со счетом " + room.ActivePlayer.Score + "\n");
                                    Rooms.Remove(room);
                                    RoomListChangeEvent?.Invoke();
                                }
                            }
                            break;
                        case AMessageType.Wait:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == true)
                            {
                                room.NextPlayer();
                            }
                            Server.StartSending(new AFrame(room.Id, room, AMessageType.Send), true, "ServerSender");
                            Chrono.AppendText("[" + room.Name + "] : Игрок " + croom.PlayerName + " завершает свой раунд со счетом:  " + room.ActivePlayer.LastRound.Sum() + "\n");
                            break;
                    }
                }
                ), frame);

            };

        }

        private bool Process(ARoom room)
        {
            int score = new Random().Next(1, 6);
            if (score > 1)
            {
                room.ActivePlayer.AddScore(score);
                if (room.ActivePlayer.Score + room.ActivePlayer.LastRound.Sum() >= 100)
                {
                    return false;
                }
            }
            else
            {
                room.ActivePlayer.SumScore(true);
                Chrono.AppendText("[" + room.Name + "] : Игрок " + room.ActivePlayer.Name + " завершает свой раунд со счетом:  " + room.ActivePlayer.LastRound.Sum() + "\n");
                room.NextPlayer();
            }
            return true;
        }

        private bool FindRoomByName(string name, out ARoom Room)
        {
            foreach (ARoom room in Rooms)
            {
                if (room.Name == name)
                {
                    Room = room;
                    return true;
                }
            }
            Room = null;
            return false;
        }

        private bool FindPlayerByName(string name, ARoom Room, out APlayer Player)
        {
            foreach (APlayer player in Room.Players)
            {
                if (player.Name == name)
                {
                    Player = player;
                    return true;
                }
            }
            Player = null;
            return false;
        }

    }
}
