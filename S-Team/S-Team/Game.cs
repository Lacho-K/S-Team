using System;
using System.Collections.Generic;
using System.Text;

namespace S_Team
{
    class Game
    {
        public string Name { get; private set; }
        public string Genre { get; private set; }
        public string RD { get; private set; }

        public int ReleaseDate
        {
            get 
            {
                string[] RDData = RD.Split();
                Dictionary<string, int> months = new Dictionary<string, int>()
{
                { "january", 1},
                { "february", 2},
                { "march", 3},
                { "april", 4},
                { "may", 5},
                { "june", 6},
                { "july", 7},
                { "august", 8},
                { "september", 9},
                { "october", 10},
                { "november", 11},
                { "december", 12},
                };

                int result = 0;

                if (months.ContainsKey(RDData[2].ToLower()))
                {
                    result = months[RDData[2].ToLower()] * 100 + int.Parse(RDData[3]) + int.Parse(RDData[4]) * 10000;
                }

                else if(RDData.Length < 5)
                {
                    result =  int.Parse(RDData[2]) * 10000;
                }

                return result;
            }
        }

        public string GeneralInfo { get; private set; }

        public string Type { get; private set; }

        public bool IsPlayable { get; private set; }

        public string Location { get; private set; }

        public Game(string name, string genre, string RD, string info, string type, bool isPlayable)
        {
            this.Name = name;
            this.Genre = genre;
            this.RD = RD;
            this.GeneralInfo = info;
            this.Type = type;
            this.IsPlayable = isPlayable;
        }

        public Game(string name, string genre, string RD, string info, string type, bool isPlayable, string location)
            :this(name, genre, RD, info, type, isPlayable)
        {
            this.Location = location;
        }

        public void print()
        {
            Console.WriteLine("{0}" + new string('\n', 2) + "{1}" + new string('\n', 2) + "{2}" + new string('\n', 2) + "{3}" + "\n", this.Name, this.Genre, this.RD, this.Type);
            string[] wordsGeneralInfo = this.GeneralInfo.Split();
            int wordCounter = 0;
            foreach (string word in wordsGeneralInfo)
            {
                wordCounter++;
                if(wordCounter == 25)
                {
                    Console.Write(new string('\n',2) + "    ");
                    wordCounter = 0;
                }
                Console.Write(word + " ");
            }
            Console.WriteLine("\n");
            Console.WriteLine(new string('-', Console.LargestWindowWidth) + "\n");
        }
    }
}
