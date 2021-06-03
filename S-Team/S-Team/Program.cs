using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace S_Team
{
    class Program
    {
        private static CustomList<Game> games = new CustomList<Game>();
        private static CustomList<Game> library = new CustomList<Game>();
        private static string[] userInputData;
        static void Main()
        {
            PutGamesInList();
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(new string('-', Console.LargestWindowWidth));
            Console.WriteLine($"{new string(' ',Console.LargestWindowWidth/2)}Welcome to S-Team!");
            Console.WriteLine(new string('-', Console.LargestWindowWidth));

            while (true)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("\nWhat do you want to do?\n");
                Console.WriteLine("Current options:\nPlay \"Game to play\"\nPrint All/Library\nAdd To Library \"Game(s) to Add\"(add comma ofter each game)\nRemove from Library(press enter)\nFind \"Game to find\"\nSort \"All/Library\" \"Descending/Ascending\" \"Name\", \"Genre\", \"RD\" Then by \"Descending/Ascending\" \"Name\", \"Genre\", \"RD\"(optional, can be input multiple times)\nFilter All\\Library \"RD\" >\\<\\=\\<=\\>= \"number\", \"Name\" first letter >\\<\\=\\<=\\>= \"leter\", \"Genre\" \"genre to filter\",  \"playable\\not playable\"\nExit");
                string userInput = Console.ReadLine();                
                userInputData = userInput.Split().Select(w => w.ToLower()).ToArray();                
                if(userInputData[0] == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Goodbye!");
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
                }
                else if (userInputData[0] == "play" && userInputData.Length > 1)
                {
                    Play(userInput);
                }
                else if(userInputData[0] == "print" && userInputData.Length > 1)
                {
                    if(userInputData[1] == "all")
                    {
                        PrintGames(games);
                    }
                    else if(userInputData[1] == "library")
                    {
                        PrintGames(library);
                    } 
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid option!");
                    }
                }
                else if(userInputData.Length > 3 && userInputData[0] == "add" && userInputData[1] == "to" && userInputData[2] == "library")
                {
                    string[] inputGames = userInput.Substring(15).Split(',');
                    Console.WriteLine();
                    for (int i = 0; i < inputGames.Length; i++)
                    {
                        string gameToAdd = inputGames[i].ToLower();
                        Game gameForLibrary = FindDesiredGame(gameToAdd);
                        int gameNum = library.Find(gameForLibrary);
                        if (gameForLibrary != null && gameNum == -1)
                        {
                            library.Add(gameForLibrary);
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine($"{gameToAdd.Trim()} added to library");
                        }
                        else if (gameNum != -1)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine($"{gameToAdd.Trim()} already in library!");
                        }
                    }
                }
                else if(userInputData[0] == "find" && userInputData.Length > 1)
                {
                    Console.WriteLine();
                    Console.WriteLine(new string('-', Console.LargestWindowWidth));
                    Console.WriteLine();
                    string gameForSearch;
                    if (userInputData.Length > 2)
                    {
                        gameForSearch = GetMultipleWords(1,userInputData);
                    }
                    else
                    {
                        gameForSearch = userInputData[1];
                    }
                    Game searchedGame = FindDesiredGame(gameForSearch);
                    if (searchedGame != null)
                    {
                        searchedGame.print();
                    }
                }
                else if(userInputData[0] == "sort")
                {
                    Console.WriteLine();
                    Console.WriteLine(new string('-', Console.LargestWindowWidth));
                    Console.WriteLine();
                    
                    if (userInputData.Length % 4 == 0)
                    {
                        string keyWord;
                        CustomList<Game> gamesForSorting = DetermineGamesForManipulating();                   
                        for (int i = 1; i < userInputData.Length; i+=4)
                        {
                            string targetProp = FixFormatingProp(userInputData[i + 2]);
                            keyWord = "sort";
                            if (i != 1)
                            {
                                keyWord = "then";
                            }
                            if (targetProp != "Property doesn't exist!" && userInputData[i - 1] == keyWord)
                            {
                                if(i != 1 && userInputData[i] != "by")
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Invalid Input!");
                                    gamesForSorting = null;
                                    break;
                                }
                                if (userInputData[i + 1] == "ascending")
                                {
                                    gamesForSorting = SortAscending(targetProp, gamesForSorting);                                   
                                }
                                else if (userInputData[i + 1] == "descending")
                                {
                                    gamesForSorting = SortDescending(targetProp, gamesForSorting);
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Invalid input!");
                                    break;
                                }
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Invalid input!");
                                gamesForSorting = null;
                                break;
                            }
                        }
                        if (gamesForSorting != null)
                        {
                            PrintGames(gamesForSorting);
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Invalid input!");
                    }                   
                }
                else if(userInputData[0] == "remove" && userInputData[1] == "from" && userInputData[2] == "library" && userInputData.Length == 3)
                {
                    Console.WriteLine();
                    Console.WriteLine(new string('-', Console.LargestWindowWidth));
                    Console.WriteLine();
                    if (library.Count > 0)
                    {
                        Console.WriteLine("Type in the name of the game you wanna remove, it's index or type \"All\" to remove all games from your library.\nYou can also type in \"first\" to remove the first game in your library or \"last\" to remove the last game in your library. Type in \"stop\" to stop removing.\n");
                        string input;
                        ShowIndexes();
                        while ((input = Console.ReadLine()) != "stop")
                        {
                            if (input.ToLower() == "all")
                            {
                                library.Clear();
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.WriteLine("All games have been removed from your library.\n");
                                break;
                            }
                            else
                            {
                                bool isIndex = int.TryParse(input, out int index);
                                if (isIndex)
                                {
                                    library.RemoveAt(index);
                                }
                                else if (FirstOrLast(input) != -1)
                                {
                                    int desiredIndex = FirstOrLast(input);
                                    library.RemoveAt(desiredIndex);
                                }
                                else
                                {
                                    Game gameToRemove = FindDesiredGame(input);
                                    library.Remove(gameToRemove);
                                }
                                if (library.Count > 0)
                                {
                                    ShowIndexes();
                                }
                                else
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("No games left in library!");
                                    break;
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("No games in library!");
                    }
                }
                else if(userInputData[0] == "filter" && userInputData.Length > 2)
                {
                    CustomList<Game> gamesForFilter = DetermineGamesForManipulating();
                    if(gamesForFilter == null)
                    {
                        continue;
                    }
                    FilterGamesAccordingly(gamesForFilter);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid Input!");
                }
            }
            
        }

        private static void ShowIndexes()
        {
            Console.WriteLine("Current indexes are:" + "\n");
            for (int i = 0; i < library.Count; i++)
            {
                Console.WriteLine($"{i} -> {SkipFirstWord(library[i].Name)}");
            }
        }

        private static void FilterGamesAccordingly(CustomList<Game> gamesForFilter)
        {
            CustomList<Game> filteredGames = new CustomList<Game>();
            if(userInputData[2] == "name" && userInputData.Length == 5 && userInputData[4].Length == 1 && Char.IsLetter(userInputData[4][0]))
            { 
                if (userInputData[3] == ">" && userInputData[4].Length == 1) 
                {
                    gamesForFilter.Where(g => (int)SkipFirstWord(g.Name)[0]  + 32 > (int)userInputData[4][0]).ToList().ForEach(g => filteredGames.Add(g));
                }
                else if (userInputData[3] == "<" && userInputData[4].Length == 1)
                {
                    gamesForFilter.Where(g => (int)SkipFirstWord(g.Name)[0] + 32 < (int)userInputData[4][0]).ToList().ForEach(g => filteredGames.Add(g));
                }
                else if (userInputData[3] == "=" && userInputData[4].Length == 1)
                {
                    gamesForFilter.Where(g => (int)SkipFirstWord(g.Name)[0] + 32 == (int)userInputData[4][0]).ToList().ForEach(g => filteredGames.Add(g));
                }
                else if (userInputData[3] == ">=" && userInputData[4].Length == 1)
                {
                    gamesForFilter.Where(g => (int)SkipFirstWord(g.Name)[0] + 32 >= (int)userInputData[4][0]).ToList().ForEach(g => filteredGames.Add(g));
                }
                else if (userInputData[3] == "<=" && userInputData[4].Length == 1)
                {
                    gamesForFilter.Where(g => (int)SkipFirstWord(g.Name)[0] + 32 <= (int)userInputData[4][0]).ToList().ForEach(g => filteredGames.Add(g));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("invalid input!");
                    return;
                }
            }
            else if(userInputData[2] == "rd" && userInputData.Length == 5)
            {
                bool isValid = int.TryParse(userInputData[4], out int num);
                if (!isValid)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input!");
                    return;
                }
                if(userInputData[3] == ">")
                {
                     gamesForFilter.Where(g => int.Parse(GetLastWord(g.RD)) > num).ToList().ForEach(g => filteredGames.Add(g));
                }
                else  if (userInputData[3] == "<")
                {
                    gamesForFilter.Where(g => int.Parse(GetLastWord(g.RD)) < num).ToList().ForEach(g => filteredGames.Add(g));
                }
                else if (userInputData[3] == "=")
                {
                    gamesForFilter.Where(g => int.Parse(GetLastWord(g.RD)) == num).ToList().ForEach(g => filteredGames.Add(g));
                }
                else if (userInputData[3] == "<=")
                {
                    gamesForFilter.Where(g => int.Parse(GetLastWord(g.RD)) <= num).ToList().ForEach(g => filteredGames.Add(g));
                }
                else if (userInputData[3] == ">=")
                {
                    gamesForFilter.Where(g => int.Parse(GetLastWord(g.RD)) >= num).ToList().ForEach(g => filteredGames.Add(g));
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid input!");
                }
            }
            else if(userInputData[2] == "playable" && userInputData.Length == 3)
            {
                gamesForFilter.Where(g => g.IsPlayable).ToList().ForEach(g => filteredGames.Add(g));
            }
            else if(userInputData[2] == "not" && userInputData[3] == "playable" && userInputData.Length == 4)
            {
                gamesForFilter.Where(g => !g.IsPlayable).ToList().ForEach(g => filteredGames.Add(g));
            }
            else if(userInputData[2] == "genre" && userInputData[3].Length > 0 && userInputData.Length == 4)
            {
                gamesForFilter.Where(g => g.Genre.ToLower().Contains(userInputData[3])).ToList().ForEach(g => filteredGames.Add(g));
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid input!");
                return;
            }
            PrintGames(filteredGames);
        }

        private static CustomList<Game> DetermineGamesForManipulating()
        {
            if (userInputData[1] == "all")
            {
                return games;
            }
            else if (userInputData[1] == "library")
            {
                return library;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(userInputData[1] + " doesn't exist!");
                return null;
            }
        }

        private static CustomList<Game> SortAscending(string targetProp, CustomList<Game> gamesForSort)
        {
            if (gamesForSort.Count > 1)
            {
                CustomList<Game> sortedCustomList = new CustomList<Game>();
                gamesForSort.OrderBy(g => g.GetType().GetProperty(targetProp).GetValue(g)).ToList().ForEach(g => sortedCustomList.Add(g));
                return sortedCustomList;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You only have one or zero games in your library!");
                return null;
            }
        }

        private static CustomList<Game> SortDescending(string targetProp, CustomList<Game> gamesForSort)
        {
            if (gamesForSort.Count > 1)
            {
                CustomList<Game> sortedCustomList = new CustomList<Game>();
                gamesForSort.OrderByDescending(g => g.GetType().GetProperty(targetProp).GetValue(g)).ToList().ForEach(g => sortedCustomList.Add(g));
                return sortedCustomList;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("You only have one or zero games in your library!");
                return null;
            }
        }

        private static string FixFormatingProp(string input)
        {
            switch (input)
            {
                case "name":
                    return "Name";
                case "genre":
                    return "Genre";
                case "rd":
                    return "ReleaseDate";
                default:
                    return "Property doesn't exist!";
            }
        }

        private static int FirstOrLast(string input)
        {
            switch (input.ToLower())
            {
                case "first":
                    return 0;
                case "last":
                    return library.Count - 1;
                default:
                    return -1;
            }
        }

        private static void PutGamesInList()
        {
            using (StreamReader reader = new StreamReader("gamesList.txt"))
            {
                string line = reader.ReadLine();
                CustomList<string> gamesInfo = new CustomList<string>();
                bool isPlayable = false;
                while (line != null)
                {
                    gamesInfo.Add(line);
                    string[] currentGameInfo = line.Split();
                    if (currentGameInfo[0] == "Type:" && currentGameInfo.Length == 3)
                    {
                        isPlayable = false;
                    }
                    else if (currentGameInfo[0] == "Type:" && currentGameInfo.Length == 2)
                    {
                        isPlayable = true;
                    }
                    if (currentGameInfo[0] == "Info:" && !isPlayable)
                    {
                        games.Add(new Game(gamesInfo[0], gamesInfo[1], gamesInfo[2], gamesInfo[4], gamesInfo[3], isPlayable));
                        gamesInfo.Clear();
                    }
                    else if (currentGameInfo[0] == "Location:" && isPlayable)
                    {
                        games.Add(new Game(gamesInfo[0], gamesInfo[1], gamesInfo[2], gamesInfo[4], gamesInfo[3], isPlayable, SkipFirstWord(gamesInfo[5])));
                        gamesInfo.Clear();
                    }
                    line = reader.ReadLine();
                }
            }
        }

        private static void PrintGames(CustomList<Game> gamesToPrint)
        {
            if (gamesToPrint != null && gamesToPrint.Count > 0)
            {
                foreach (Game game in gamesToPrint)
                {
                    game.print();
                }              
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No games to print!");
            }
        }

        private static string SkipFirstWord(string word)
        {
            if (word.Length > 0)
            {
                int i = word.IndexOf(" ") + 1;
                string str = word.Substring(i);
                return str;
            }
            return "There is only one or zero words in here!";           
        }

        private static Game FindDesiredGame(string userInput)
        {
            foreach (Game game in games)
            {
                if(SkipFirstWord(game.Name.ToLower().Trim()) == userInput.Trim())
                {
                    return game;
                }
            }
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"{userInput.Trim()} not found!");
            return null;
        }

        private static void Play(string userInput)
        {
            Game gameToplay = FindDesiredGame(SkipFirstWord(userInput));
            if (gameToplay != null)
            {
                if (gameToplay.IsPlayable)
                {
                    if (File.Exists(gameToplay.Location))
                    {
                        using (Process CurrentGame = new Process())
                        {
                            CurrentGame.StartInfo.FileName = gameToplay.Location;
                            CurrentGame.StartInfo.UseShellExecute = true;
                            CurrentGame.Start();
                        }
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("File not found!");
                    }
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Selected game is not playable!");
                }
            }
        }

        private static string GetMultipleWords(int startIndex, string[] words)
        {
            string result = "";
            for (int i = startIndex; i < words.Length; i++)
            {
                result += words[i];
                if (i < words.Length - 1)
                {
                    result += " ";
                }
            }
            return result;
        }

        private static string GetLastWord(string input)
        {
            if (input.Contains(' '))
            {
                string workingInput = input.Trim();
                string last = workingInput.Substring(workingInput.LastIndexOf(' ')).Trim();
                return last;
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("No separated words!");
                return null;                
            }
        }
    }
}
