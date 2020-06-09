using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Coursework
{
    [Serializable]
    public class APlayer
    {
        // идентификатор игрока
        public int Id { get; }
        // имя игрока
        public string Name { get; }
        // список очков за отдельные раунды
        public AList<int> Rounds;
        // список очков за отдельные раунды
        public AList<int> LastRound;
        // статус игрока (ходит/ожидает ход)
        public bool IsTurned;

        public APlayer(string name, int id)
        {
            Id = id;
            Name = name;
            Rounds = new AList<int>();
            LastRound = new AList<int>();
            IsTurned = false;
        }

        public void SumScore(bool isOne)
        {
            int temp = 0;
            if (isOne == false)
            {
                foreach (int e in LastRound)
                {
                    temp += e;
                }
                Rounds.Add(temp);
            }
            else
            {
                Rounds.Add(0);
            }
        }

        public void AddScore(int score)
        {
            LastRound.Add(score);
        }

        public int Score {
            get {
                int score = 0;
                foreach (int round in Rounds) score += round;
                return score;
            }
        }

    }

}
