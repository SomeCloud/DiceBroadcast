using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Coursework
{
    class GameView: Panel
    {
        delegate bool lam();
        public delegate void OnClickRollEvent();
        public delegate void OnClickStopEvent();
        public event OnClickRollEvent ClickRoll;
        public event OnClickStopEvent ClickStop;
        AList<PlayerNoteView> Playerrows;
        AList<APlayer> Notes;
        Button RollButton;
        Button StopButton;


        public GameView(ARoom room, string localPlayer) : base()
        {
            Width = 780;
            AutoScroll = true;
            Notes = room.Players;
            lam s = () => { return false; };

            Playerrows = new AList<PlayerNoteView>();

            foreach (APlayer player in Notes) 
            {
                s = ()=> { if (player == room.ActivePlayer) return true; else return false; };
                PlayerNoteView pl;
                if (player.Id > 1)
                {
                    pl = new PlayerNoteView(player, s()) { Parent = this, Location = new Point(0, Playerrows.Last().Location.Y + 60) };
                }
                else 
                {
                    pl = new PlayerNoteView(player, s()) { Parent = this, Location = new Point(0, 0) };
                }

                Playerrows.Add(pl);
            }

            s = () => { if (room.ActivePlayer.Name == localPlayer) return true; else return false; };

            RollButton = new Button() { Parent = this, Location = new Point(10, Playerrows.Last().Location.Y + 80), Size = new Size(200, 40), Text = "Roll", Enabled = s() };
            StopButton = new Button() { Parent = this, Location = new Point(220, Playerrows.Last().Location.Y + 80), Size = new Size(200, 40), Text = "Штап", Enabled = s() };

            RollButton.Click += (object sender, EventArgs e) => {
                ClickRoll?.Invoke();
            };

            StopButton.Click += (object sender, EventArgs e) => {
                ClickStop?.Invoke();
            };
        }


        public void Update(ARoom room, string localPlayer) 
        {
            lam s = () => { return false; };
            Playerrows.Clear();
            foreach (APlayer player in Notes)
            {
                s = () => { if (player == room.ActivePlayer) return true; else return false; };
                PlayerNoteView pl;
                if (player.Id > 1)
                {
                    pl = new PlayerNoteView(player, s()) { Parent = this, Location = new Point(0, Playerrows.Last().Location.Y + 60) };
                }
                else
                {
                    pl = new PlayerNoteView(player, s()) { Parent = this, Location = new Point(0, 0) };
                }

                s = () => { if (room.ActivePlayer.Name == localPlayer) return true; else return false; };
                RollButton.Enabled = s();
                StopButton.Enabled = s();

                Playerrows.Add(pl);
            }
        }
    }
}
