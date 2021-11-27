using System;

Room[,] rooms = GameController.InitializeRooms(4,4);
Room.PopulateRooms(rooms);
Player player = new Player();
Maelstrom maelstrom = new Maelstrom(rooms);

while (!GameController.GameWon && !GameController.PlayerDead)
{
    GameController.DisplayStatus(player, rooms, maelstrom);
    player.Move(rooms);
    GameController.UpdatePlayerRoom(player, rooms, maelstrom);
    Console.WriteLine($"\n");
}

GameController.DisplayStatus(player, rooms, maelstrom);

public class Room
{
    public int[] Coordinates { get; set; }
    public RoomContents RoomContents { get; set; }
    public bool NorthBorder { get; set; } = false;
    public bool SouthBorder { get; set; } = false;
    public bool EastBorder { get; set; } = false;
    public bool WestBorder { get; set; } = false;
    public bool ContainsPlayer { get; set; } = false;

    public Room(int[] coordinates, RoomContents roomContents)
    {
        Coordinates = coordinates;
        RoomContents = roomContents;
    }

    public override string ToString()
    {
        return $"This room's coordinates are ({Coordinates[0]}, {Coordinates[1]}).";
    }

    public static void PopulateRooms(Room[,] rooms)
    {
        for (int row = 0; row < rooms.GetLength(0); row++)
        {
            for (int column = 0; column < rooms.GetLength(1); column++)
            {
                if (row == 0 && column == 0)
                {
                    rooms[row, column].RoomContents = RoomContents.Entrance;
                    rooms[row, column].ContainsPlayer = true;
                }
                else if (row == 0 && column == 2) rooms[row, column].RoomContents = RoomContents.Fountain;
                else if (row == 2 && column == 2) rooms[row, column].RoomContents = RoomContents.Pit;
                //else if (row == 1 && column == 3) rooms[row, column].RoomContents = RoomContents.Maelstrom;
                else rooms[row, column].RoomContents = RoomContents.Empty;

                if (row == 3) rooms[row, column].SouthBorder = true; else rooms[row, column].SouthBorder = false;
                if (row == 0) rooms[row, column].NorthBorder = true; else rooms[row, column].NorthBorder = false;
                if (column == 3) rooms[row, column].EastBorder = true; else rooms[row, column].EastBorder = false;
                if (column == 0) rooms[row, column].WestBorder = true; else rooms[row, column].WestBorder = false;
            }
        }
    }

    public void RoomMessage(Player player, Room[,] rooms, Maelstrom maelstrom)
    {
        if (rooms[player.Position[0], player.Position[1]].RoomContents == RoomContents.Entrance && GameController.FountainActivated)
        {
            Console.WriteLine("You have made it back to the entrance of the cave!");
        }

        if (rooms[player.Position[0], player.Position[1]].RoomContents == RoomContents.Entrance)
        {
            Console.WriteLine("You see light coming from outside. You are at the mouth of the cave.");
        }

        if (rooms[player.Position[0], player.Position[1]].RoomContents == RoomContents.Fountain)
        {
            Console.WriteLine("You can hear a steady trickling sound. The Fountain must be in this room.");
        }

        if (rooms[player.Position[0], player.Position[1]].RoomContents == RoomContents.FountainActive)
        {
            Console.WriteLine("You hear the rapturous flow of the Fountain. It has been activated!");
        }

        if ( GameController.AdjecencyCheck(rooms[player.Position[0], player.Position[1]], rooms[2, 2]) )
        {
            Console.WriteLine("You feel a light breeze; there must be a pit nearby...");
        }

        
        if (GameController.AdjecencyCheck(rooms[player.Position[0], player.Position[1]], rooms[maelstrom.Position[0], maelstrom.Position[1]]))
        {
            Console.WriteLine("You hear the groaning of a nearby Mealstrom.");
        }
    }
}

public class GameController
{
    public static bool FountainActivated { get; set; } = false;
    public static bool GameWon { get; set; } = false;
    public static bool PlayerDead { get; set; } = false;
    public static DateTime GameStart { get; set; } = DateTime.Now;
    public static DateTime GameEnd { get; set; }

    public static Room[,] InitializeRooms(int rowCount, int columnCount)
    {
        Room[,] output = new Room[rowCount, columnCount];
        for (int row = 0; row < rowCount; row++)
        {
            for (int column = 0; column < columnCount; column++)
            {
                int[] coordinates = new int[2] { row, column };
                //coordinates[0] = row;
                //coordinates[1] = column;
                output[row, column] = new Room(coordinates, RoomContents.Empty);
            }
        }
        return output;
    }

    public static void DisplayStatus(Player player, Room[,] rooms, Maelstrom maelstrom)
    {
        if (GameWon)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("You have escaped successful! Congratulations!");
            Console.ForegroundColor = ConsoleColor.White;
            GameEnd = DateTime.Now;
            TimeSpan timeElapsed = (GameEnd - GameStart);
            Console.WriteLine($"Time spent in the cave: {timeElapsed.Minutes} minutes and {timeElapsed.Seconds} seconds.");
            //GameWon = true;
        }
        else if (PlayerDead)
        {
            //Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("You have died! Game over.");
            Console.ForegroundColor = ConsoleColor.White;
        }
        else
        {
            Console.WriteLine($"{rooms[player.Position[0], player.Position[1]]}");
            rooms[player.Position[0], player.Position[1]].RoomMessage(player, rooms, maelstrom);
            Console.WriteLine("----------------------------------------");
        }

    }

    public static void DisplayVictory()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("You have escaped successful! Congratulations!");
        Console.ForegroundColor = ConsoleColor.White;
        GameWon = true;
    }

    public static void ActivateFountain(Player player, Room[,] rooms)
    {
        rooms[player.Position[0], player.Position[1]].RoomContents = RoomContents.FountainActive;
        FountainActivated = true;
    }

    public static void UpdatePlayerRoom(Player player, Room[,] rooms, Maelstrom maelstrom)
    {
        int[] playerPosition = player.Position;

        for (int row = 0; row < rooms.GetLength(0); row++)
        {
            for (int column = 0; column < rooms.GetLength(1); column++)
            {
                if (row == playerPosition[0] && column == playerPosition[1]) rooms[row, column].ContainsPlayer = true;
                else rooms[row, column].ContainsPlayer = false;
            }
        }

        if (rooms[player.Position[0], player.Position[1]].RoomContents == RoomContents.Entrance && GameController.FountainActivated)
        {
            GameWon = true;
        }

        if (rooms[player.Position[0], player.Position[1]].RoomContents == RoomContents.Pit)
        {
            Console.Clear();
            PlayerDead = true;
            Console.WriteLine("You stumble into a giant pit and fall for a ludicrously long time.");
        }

        if (rooms[player.Position[0], player.Position[1]].RoomContents == RoomContents.Maelstrom)
        {
            maelstrom.MovePlayer(player, rooms);
        }

    }

    public static bool AdjecencyCheck(Room roomOne, Room roomTwo)
    {
        //Adjacent horizontally
        if (roomOne.Coordinates[0] == roomTwo.Coordinates[0] && roomOne.Coordinates[1] == (roomTwo.Coordinates[1] - 1)) return true;
        else if (roomOne.Coordinates[0] == roomTwo.Coordinates[0] && roomOne.Coordinates[1] == roomTwo.Coordinates[1]) return true;
        else if (roomOne.Coordinates[0] == roomTwo.Coordinates[0] && roomOne.Coordinates[1] == (roomTwo.Coordinates[1] + 1)) return true;

        //Adjacent vertically
        else if (roomOne.Coordinates[1] == roomTwo.Coordinates[1] && roomOne.Coordinates[0] == (roomTwo.Coordinates[0] - 1)) return true;
        else if (roomOne.Coordinates[1] == roomTwo.Coordinates[1] && roomOne.Coordinates[0] == roomTwo.Coordinates[0]) return true;
        else if (roomOne.Coordinates[1] == roomTwo.Coordinates[1] && roomOne.Coordinates[0] == (roomTwo.Coordinates[0] + 1)) return true;

        //Adjacent diagaonlly
        else if (roomOne.Coordinates[0] == (roomTwo.Coordinates[0] + 1) && roomOne.Coordinates[1] == (roomTwo.Coordinates[1] + 1)) return true;
        else if (roomOne.Coordinates[0] == (roomTwo.Coordinates[0] - 1) && roomOne.Coordinates[1] == (roomTwo.Coordinates[1] - 1)) return true;
        else if (roomOne.Coordinates[0] == (roomTwo.Coordinates[0] + 1) && roomOne.Coordinates[1] == (roomTwo.Coordinates[1] - 1)) return true;
        else if (roomOne.Coordinates[0] == (roomTwo.Coordinates[0] - 1) && roomOne.Coordinates[1] == (roomTwo.Coordinates[1] + 1)) return true;

        else return false;

    }

}

public class Player
{
    public int[] Position { get; set; } = { 0, 0 };
    public Player() { }

    public void Move(Room[,] rooms)
    {
        if (rooms[Position[0], Position[1]].RoomContents == RoomContents.Fountain)
        {
            Console.Write($"Press any key to activate the Fountain. ");
            Console.ReadKey();
            GameController.ActivateFountain(this, rooms);
            Console.Clear();
        }
        else
        {
            Console.Write("Would you like to move [n]orth, [s]outh, [e]ast, or [w]est? ");
            string input = Console.ReadLine();

            switch (input.ToLower())
            {
                case "n":
                    if (rooms[Position[0], Position[1]].NorthBorder)
                    {
                        Console.Clear();
                        Console.WriteLine("\nCannot move north.");
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Position[0]--;
                        break;
                    }

                case "s":
                    if (rooms[Position[0], Position[1]].SouthBorder)
                    {
                        Console.Clear();
                        Console.WriteLine("\nCannot move south.");
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Position[0]++;
                        break;
                    }

                case "e":
                    if (rooms[Position[0], Position[1]].EastBorder)
                    {
                        Console.Clear();
                        Console.WriteLine("\nCannot move east.");
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Position[1]++;
                        break;
                    }

                case "w":
                    if (rooms[Position[0], Position[1]].WestBorder)
                    {
                        Console.Clear();
                        Console.WriteLine("\nCannot move west.");
                        break;
                    }
                    else
                    {
                        Console.Clear();
                        Position[1]--;
                        break;
                    }

                default:
                    Console.Clear();
                    Console.WriteLine("\nInvalid entry.");
                    break;
            }
        }
    }

}

public class Maelstrom
{
    public int[] Position { get; set; } = { 1, 3 };
    public Maelstrom(Room[,] rooms)
    {
        rooms[Position[0], Position[1]].RoomContents = RoomContents.Maelstrom;
    }

    public void MovePlayer(Player player, Room[,] rooms)
    {
        Console.Clear();
        Console.WriteLine("You have been blown to another room by a Maelstrom!\nThe Maelstrom is blown as well.");
        rooms[Position[0], Position[1]].RoomContents = RoomContents.Empty;

        if (!rooms[player.Position[0], player.Position[1]].NorthBorder) player.Position[0]--;
        if (!rooms[player.Position[0], player.Position[1]].EastBorder) player.Position[1]++;
        if (!rooms[player.Position[0], player.Position[1]].EastBorder) player.Position[1]++;

        if (!rooms[Position[0], Position[1]].SouthBorder) Position[0]++;
        if (!rooms[Position[0], Position[1]].WestBorder) Position[1]--;
        if (!rooms[Position[0], Position[1]].WestBorder) Position[1]--;

        rooms[Position[0], Position[1]].RoomContents = RoomContents.Maelstrom;
    }
}

    public enum RoomContents { Empty, Entrance, Fountain, FountainActive, Pit, Maelstrom }