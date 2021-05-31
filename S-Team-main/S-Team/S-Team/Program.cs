using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;

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
            while (true)
            {
                Console.WriteLine();
                Console.WriteLine("What do you want to do?");
                Console.WriteLine("Current options: Play \"Game to play\", Print All/Library, Add To Library \"Game to Add\", Remove from Library \"Game to remove\"(or index of game you wanna remove), Find \"Game to find\", Sort \"All/Library\" \"Descending/Ascending\" \"Name\", \"Genre\", \"Release date\" Then by \"Descending/Ascending\" \"Name\", \"Genre\", \"Release date\"(optional, can be input multiple times), Filter All\\Library \"Release date\" >\\<\\= \"number\", \"Name\" first letter >\\<\\= \\leter\\, filter \"Genre\" by giving genre,  \"playable\\not\" or Exit");
                string userInput = Console.ReadLine();                
                userInputData = userInput.Split().Select(w => w.ToLower()).ToArray();                
                if(userInputData[0] == "exit")
                {
                    Console.WriteLine("Goodbye!");
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
                        Console.WriteLine("Invalid option!");
                    }
                }
                else if(userInputData.Length > 3 && userInputData[0] == "add" && userInputData[1] == "to" && userInputData[2] == "library")
                {
                    string desiredGame;

                    if (userInputData.Length > 4)
                    {
                        desiredGame = GetMultipleWords(3, userInputData);
                    }
                    else
                    {
                        desiredGame = userInputData[3];
                    }

                    Game gameForLibrary = FindDesiredGame(desiredGame);
                    int gameNum = library.Find(gameForLibrary);
                    if (gameForLibrary != null && gameNum == -1)
                    {
                        library.Add(gameForLibrary);
                    }
                    else if(gameNum != -1)
                    {
                        Console.WriteLine("Game already in library!");
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
                        CustomList<Game> sortedGames = new CustomList<Game>();
                        CustomList<Game> GamesForSorting = DetermineGamesForManipulating();                       
                        for (int i = 1; i < userInputData.Length; i+=4)
                        {
                            string targetProp = FixFormating(userInputData[i + 2]);
                            keyWord = "sort";
                            if (i != 1)
                            {
                                keyWord = "then";
                            }
                            if (targetProp != "Property doesn't exist!" && userInputData[i - 1] == keyWord)
                            {
                                if(i != 1 && userInputData[i] != "by")
                                {
                                    Console.WriteLine("Invalid Input!");
                                    sortedGames = null;
                                    break;
                                }
                                if (userInputData[i + 1] == "ascending")
                                {
                                    sortedGames = SortAscending(targetProp, GamesForSorting);                                   
                                }
                                else if (userInputData[i + 1] == "descending")
                                {
                                    sortedGames = SortDescending(targetProp, GamesForSorting);
                                }
                                else
                                {
                                    Console.WriteLine("Invalid input!");
                                    break;
                                }
                                GamesForSorting = sortedGames;
                            }
                            else
                            {
                                Console.WriteLine("Invalid input!");
                                sortedGames = null;
                                break;
                            }
                        }
                        if (sortedGames != null)
                        {
                            PrintGames(sortedGames);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Invalid input!");
                    }                   
                }
                else if(userInputData.Length > 3 && userInputData[0] == "remove" && userInputData[1] == "from" && userInputData[2] == "library")
                {
                    Console.WriteLine();
                    Console.WriteLine(new string('-', Console.LargestWindowWidth));
                    Console.WriteLine();
                    int index;
                    bool isIndex = int.TryParse(userInputData[3], out index);
                    if (isIndex)
                    {
                        library.RemoveAt(index);                                          
                    }
                    else
                    {
                        string gameToRemoveName;
                        if (userInputData.Length > 4)
                        {
                            gameToRemoveName = GetMultipleWords(3, userInputData);
                        }
                        else
                        {
                            gameToRemoveName = userInputData[3];
                        }
                        Game GameToRemove = FindDesiredGame(gameToRemoveName);
                        library.Remove(GameToRemove);
                    }
                }
                else if(userInputData[0] == "filter" && userInputData.Length > 2)
                {
                    CustomList<Game> gamesForFilter = DetermineGamesForManipulating();
                    FilterGamesAccordingly();
                }
                else
                {
                    Console.WriteLine("Invalid Input!");
                }
            }
            
        }

        private static void FilterGamesAccordingly()
        {
            // TODO add filter method
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
                Console.WriteLine("You only have one or zero games in your library!");
                return null;
            }
        }

        private static string FixFormating(string input)
        {
            switch (input)
            {
                case "name":
                    return "Name";
                case "genre":
                    return "Genre";
                case "releasedate":
                    return "ReleaseDate";
                default:
                    return "Property doesn't exist!";
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
            Console.WriteLine("Your game was not found!");
            return null;
        }

        private static void Play(string userInput)
        {
            Game gameToplay = FindDesiredGame(SkipFirstWord(userInput));
            if (gameToplay != null)
            {
                if (gameToplay.IsPlayable)
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
    }
}
