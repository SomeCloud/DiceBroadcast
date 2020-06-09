using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Coursework
{
    class AServerForm : Form
    {

        public AServer Server;
        public AClient Client;

        public AServer LobbyServer;

        AList<ARoom> Rooms;

        public AServerForm(string adress, int sendport, int receiveport, int lobbyport) : base()
        {

            Text = "Local Server";
            ClientSize = new Size(800, 600);

            Server = new AServer(adress, sendport);
            Client = new AClient(adress, receiveport);
            LobbyServer = new AServer(adress, lobbyport);

            Rooms = new AList<ARoom>();

            ComboBox Lobbys = new ComboBox() { Parent = this, Location = new Point(10, 10), Size = new Size(500, 30), Text = "Не выбрано" };
            RichTextBox Chrono = new RichTextBox() { Parent = this, Location = new Point(10, 40), Size = new Size(500, 490) };

            Client.StartReceive();

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
                                Server.StartSending(new AFrame(room.Id, room, AMessageType.Connect), true);
                            }
                            break;
                        case AMessageType.Connect:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == true)
                            {
                                Server.StartSending(new AFrame(room.Id, room, AMessageType.Connect), true);
                            }
                            break;
                        case AMessageType.PlayerDisconnect:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == true)
                            {
                                if (FindPlayerByName(croom.PlayerName, room, out player) == true)
                                {
                                    room.Players.Remove(player);
                                }
                                Server.StartSending(new AFrame(room.Id, room, AMessageType.PlayerDisconnect), true);
                            }
                            break;
                        case AMessageType.Send:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == true)
                            {
                                if (Process(room) == true)
                                {
                                    Server.StartSending(new AFrame(room.Id, room, AMessageType.Send), true);
                                }
                                else
                                {
                                    Server.StartSending(new AFrame(room.Id, room, AMessageType.GameOver), true);
                                }
                            }
                            break;
                        case AMessageType.Wait:
                            croom = (CRoom)frame.Data;
                            if (FindRoomByName(croom.RoomName, out room) == true)
                            {
                                room.NextPlayer();
                            }
                            Server.StartSending(new AFrame(room.Id, room, AMessageType.Send), true);
                            break;
                    }
                }
                ), frame);

            };

        }

        private bool Process(ARoom room)
        {
            int score = new Random(Convert.ToInt32((int)DateTime.Now.Ticks)).Next(1, 6);
            if (score > 1)
            {
                room.ActivePlayer.AddScore(score);
                if (room.ActivePlayer.Score >= 100)
                {
                    return false;
                }
            }
            else
            {
                room.ActivePlayer.SumScore(true);
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
