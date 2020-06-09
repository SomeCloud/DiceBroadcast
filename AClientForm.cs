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

            Server = new AServer(adress, sendport);
            Client = new AClient(adress, receiveport);
            LobbyClient = new AClient(adress, lobbyport);

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
            };

            Back.Click += (object sender, EventArgs e) => {
                BackToLobbyEvent?.Invoke();
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
