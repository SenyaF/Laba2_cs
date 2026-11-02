using System;
using System.IO;

namespace Laba2_cs
{
    public enum State
    {
        Winner,
        Looser,
        Playing,
        NotInGame
    }

    public enum GameState
    {
        Start,
        End
    }

    public class Player
    {
        public string name;
        public int location;
        public State state = State.NotInGame;
        public int distanceTraveled = 0;

        public Player(string name)
        {
            this.name = name;
            this.location = -1;
        }

        public void Move(int steps)
        {
            if (state == State.NotInGame)
            {
                location = steps;
                state = State.Playing;
                return;
            }

            if (state != State.Playing) return;

            distanceTraveled += Math.Abs(steps);

            int size = Game.BoardSize;
            int newLocation = (location - 1 + steps) % size;
            if (newLocation < 0)
            {
                newLocation += size;
            }
            location = newLocation + 1;
        }
    }

    public class Game
    {
        public static string InputFile = "1.ChaseData.txt";
        public static string OutFile = "1.PursuitLog.txt";
        public static int BoardSize;

        public int size;
        public Player cat;
        public Player mouse;
        public GameState state;

        public Game(int size)
        {
            this.size = size;
            BoardSize = size;
            cat = new Player("Cat");
            mouse = new Player("Mouse");
            state = GameState.Start;
        }

        public void Run()
        {
            if (!File.Exists(InputFile))
            {
                Console.WriteLine($"Ошибка: Входной файл {InputFile} не найден.");
                return;
            }

            string[] lines = File.ReadAllLines(InputFile);
            if (lines.Length == 0) return;

            using (StreamWriter sw = new StreamWriter(OutFile, false))
            {
                sw.WriteLine("Cat and Mouse");
                sw.WriteLine();
                sw.WriteLine("Cat\tMouse\tDistance");
                sw.WriteLine("-----------------------");
            }

            for (int i = 1; i < lines.Length; i++)
            {
                if (state == GameState.End) break;

                string line = lines[i].Trim();
                if (string.IsNullOrEmpty(line)) continue;

                string[] parts = line.Split(new char[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length == 0) continue;

                char command = parts[0][0];

                int steps = 0;
                if (parts.Length > 1)
                {
                    int.TryParse(parts[1], out steps);
                }

                DoMoveCommand(command, steps);

                
                DoPrintCommand();

                CheckGameStatus();
            }

            state = GameState.End;
            AppendFinalResults();
        }

        private void DoMoveCommand(char command, int steps)
        {
            switch (command)
            {
                case 'M':
                    mouse.Move(steps);
                    break;
                case 'C':
                    cat.Move(steps);
                    break;
                case 'P':
                    DoPrintCommand();
                    break;
            }
        }

        private void CheckGameStatus()
        {
            if (cat.state == State.Playing && mouse.state == State.Playing)
            {
                if (cat.location == mouse.location)
                {
                    cat.state = State.Winner;
                    mouse.state = State.Looser;
                    state = GameState.End;
                }
            }
        }

        private void DoPrintCommand()
        {
            using (StreamWriter sw = new StreamWriter(OutFile, true))
            {
                string catLoc = cat.state == State.NotInGame ? "??" : cat.location.ToString();
                string mouseLoc = mouse.state == State.NotInGame ? "??" : mouse.location.ToString();
                string distStr = (cat.state == State.NotInGame || mouse.state == State.NotInGame)
                    ? "??"
                    : GetDistance().ToString();

                sw.WriteLine($"{catLoc}\t{mouseLoc}\t{distStr}");
            }
        }

        private int GetDistance()
        {
            return Math.Abs(cat.location - mouse.location);
        }

        private void AppendFinalResults()
        {
            using (StreamWriter sw = new StreamWriter(OutFile, true))
            {
                sw.WriteLine("-----------------------");
                sw.WriteLine();
                sw.WriteLine();
                sw.WriteLine($"Distance traveled:\tMouse\tCat");
                sw.WriteLine($"\t\t\t{mouse.distanceTraveled}\t{cat.distanceTraveled}");
                sw.WriteLine();

                if (mouse.state == State.Looser)
                {
                    sw.WriteLine($"Mouse caught at: {cat.location}");
                }
                else
                {
                    sw.WriteLine("Mouse evaded Cat");
                }
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Game.InputFile = "1.ChaseData.txt";
            Game.OutFile = "1.PursuitLog.txt";

            int boardSize = 16;
            if (File.Exists(Game.InputFile))
            {
                string[] lines = File.ReadAllLines(Game.InputFile);
                if (lines.Length > 0 && int.TryParse(lines[0].Trim(), out int parsedSize))
                {
                    boardSize = parsedSize;
                }
            }

            Game game = new Game(boardSize);
            game.Run();

            Console.WriteLine("Расчёт окончен. Результаты записаны в файл 1.PursuitLog.txt");
        }
    }
}