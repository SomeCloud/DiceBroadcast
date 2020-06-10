using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Coursework
{
    class AClientForm: Form
    {

        public AServer Server;
        public AClient Client;

        public AClient LobbyClient;

        public AClientForm(string adress, int sendport, int receiveport, int lobbyport) : base()
        {

            Text = "Local Client";
            ClientSize = new Size(800, 600);

            Server = new AServer(adress, sendport); // 8000
            Client = new AClient(adress, receiveport); // 8001
            LobbyClient = new AClient(adress, lobbyport);

            Client.StartReceive("ClientReceiver");
            LobbyClient.StartReceive("LobbyReceiver");

            var e = 0;

            InitLobby();

        }

        private void InitLobby()
        {
            Controls.Clear();

            Text = "Local Client. Lobby";
            ClientSize = new Size(800, 600);

            Button CreateGame = new Button() { Parent = this, Location = new Point(10, 10), Size = new Size(200, 40), Text = "Создать сервер" };

            // список игр в локальной сети
            AList<ARoom> Notes = new AList<ARoom>();
            // Форма для отображения игр действующих в локальной сети
            NotesView Lobbys = new NotesView(Notes) { Parent = this, Location = new Point(10, 60), Height = 530 };

            LobbyClient.Receive += (frame) =>
            {
                if (InvokeRequired) Invoke(new Action<AFrame>((s) =>
                {
                    AList<ARoom> rooms = (AList<ARoom>)frame.Data;
                    if (Notes.Count > 0)
                    {
                        foreach (ARoom room in rooms)
                        {
                            ARoom temp;
                            if (FindById(room.Id, Notes, out temp) == false)
                            {
                                Notes.Add(room);
                            }
                            else
                            {
                                temp = room;
                            }
                        }
                    }
                    else
                    {
                        foreach (ARoom room in rooms)
                        {

                            Notes.Add(room);
                        }
                    }
                }
                ), frame);
            };

            CreateGame.Click += (object sender, EventArgs e) => {
                InitCreateRoomForm();
            };

        }

        private void InitCreateRoomForm()
        {
            Controls.Clear();

            Text = "Local Client. Create a new room...";
            ClientSize = new Size(800, 600);

            Label RoomTitle = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 195), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 24), Text = "Настройки создания комнаты" };

            Label RoomNameInputLabel = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 145), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Введетие название для комнаты" };
            TextBox RoomNameInput = new TextBox() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 95), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12) };

            Label PlayersCountInputLabel = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 45), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Количество игроков в игре: 2" };
            TrackBar PlayersCountInput = new TrackBar() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 5), Size = new Size(500, 40), Minimum = 2, Maximum = 5 };

            Label PlayerNameInputLabel = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 55), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Введетие ваш ник" };
            TextBox PlayerNameInput = new TextBox() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 105), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12) };

            Button Done = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 155), Size = new Size(200, 40), Font = new Font(Font.FontFamily, 12), Text = "Готово" };
            Button Back = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 + 50, ClientSize.Height / 2 + 155), Size = new Size(200, 40), Font = new Font(Font.FontFamily, 12), Text = "Вернуться в лобби" };

            PlayersCountInput.ValueChanged += (object sender, EventArgs e) => {
                PlayersCountInputLabel.Text = "Количество игроков в игре: " + PlayersCountInput.Value;
            };

            Done.Click += (object sender, EventArgs e) => {
                CRoom room = new CRoom(0, RoomNameInput.Text, PlayerNameInput.Text, PlayersCountInput.Value);
                Server.StartSending(new AFrame(room.Id, room, AMessageType.CreateGame), true, "ClientSender");
                Client.Receive += (frame) => {
                    if (InvokeRequired) Invoke(new Action<AFrame>((s) =>
                    {
                        switch (frame.MessageType)
                        {
                            case AMessageType.Connect:
                                InitWaitingRoomForm((ARoom)frame.Data, PlayerNameInput.Text);
                                break;
                            case AMessageType.Send:
                                break;
                            case AMessageType.PlayerDisconnect:
                                break;
                            case AMessageType.GameOver:
                                break;
                        }
                    }
                ), frame);
                };
            };

            Back.Click += (object sender, EventArgs e) => {
                InitLobby();
            };

        }

        private void InitWaitingRoomForm(ARoom room, string player)
        {
            Controls.Clear();

            Text = "Local Client. Waiting Room...";
            ClientSize = new Size(800, 600); 

            Label RoomTitle = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 195), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 24), Text = room.Name };

            Label PlyersCount = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 125), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Игроки: " + room.Players.Count + "/" + room.MaxPlayers };
            Label PlayerName = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 85), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Ваш ник: " + player };
            Label GameStatus = new Label() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 - 45), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Ожидаем подключения игроков" };

            Button Back = new Button() { Parent = this, Location = new Point(ClientSize.Width / 2 - 250, ClientSize.Height / 2 + 155), Size = new Size(500, 40), Font = new Font(Font.FontFamily, 12), Text = "Вернуться в лобби" };

            Back.Click += (object sender, EventArgs e) => {
                Server.StartSending(new AFrame(room.Id, new CRoom(room.Id, room.Name, player, room.MaxPlayers), AMessageType.PlayerDisconnect), true, "ClientSender");
                InitLobby();
            };

        }

        private bool FindById(int id, AList<ARoom> RoomsList, out ARoom Room)
        {
            foreach (ARoom room in RoomsList)
            {
                if (room.Id == id)
                {
                    Room = room;
                    return true;
                }
            }
            Room = null;
            return false;
        }

    }
}
