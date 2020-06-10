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
        public event OnClickRollEvent ClickRoll;
        AList<PlayerNoteView> Playerrows;
        AList<APlayer> Notes;
        Button RollButton;
        

        public GameView(ARoom room) : base()
        {
            Width = 780;
            AutoScroll = true;
            Notes = room.Players;
            lam s = () => { return false; };


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

            RollButton = new Button() { Parent = this, Location = new Point(10, Playerrows.Last().Location.Y + 80), Size = new Size(200, 40), Text = "Roll", Enabled = s() };

            RollButton.Click += (object sender, EventArgs e) => {
                ClickRoll?.Invoke();
            };
        }


        public void Update(ARoom room) 
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

                Playerrows.Add(pl);
            }
        }
    }
}
