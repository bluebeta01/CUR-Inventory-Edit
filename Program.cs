using System.Net.Security;

namespace CurInventoryEdit;

class Program
{
    private static string _currentFilePath = "";
    private static string _saveGameName = "";
    private static bool _fileLoaded;
    private static MemoryStream _fileMemory = new();
    private static BinaryReader _fileReader = new(_fileMemory);
    private static BinaryWriter _fileWriter = new(_fileMemory);
    private static byte[] _curFileMagic = [0x4E, 0x61, 0x6E, 0x63, 0x79, 0x20, 0x44, 0x72, 0x65, 0x77, 0x20, 0x4D, 0x79, 0x73, 0x74, 0x65, 0x72, 0x79, 0x20, 0x53, 0x74, 0x6F, 0x72, 0x69, 0x65, 0x73, 0x3A, 0x00, 0x00, 0x00, 0x54, 0x68, 0x65, 0x20, 0x43, 0x75, 0x72, 0x73, 0x65, 0x20, 0x6F, 0x66, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x42, 0x6C, 0x61, 0x63, 0x6B, 0x6D, 0x6F, 0x6F, 0x72, 0x20, 0x4D, 0x61, 0x6E, 0x6F, 0x72];
    private static List<ushort> _currentInventory = new();
    private static Dictionary<ushort, string> _curItems = new()
    {
        [1] = "LouLou Cake",
        [2] = "Coat of Arms (Gargoyle Hint)",
        [3] = "Cricket Ball",
        [4] = "Full Glow Stick",
        [5] = "Empty Glow Stick",
        [6] = "Lightning Bolt Column Key",
        [7] = "East Hall Key",
        [8] = "Helmet Column Key",
        [9] = "Wand Column Key",
        [10] = "Key Mould",
        [11] = "Moon Column Key",
        [12] = "Telescope Lens",
        [13] = "Glowstone",
        [14] = "Uncle Roger Note",
        [15] = "Final Key",
        [16] = "Clock Column Key",
        [17] = "Jane's Door Puzzle Key",
        [19] = "Arrowhead Column Key",
        [20] = "Crank",
        [21] = "Goggles",
        [22] = "Easter Egg",
        [23] = "Strange Key",
        [25] = "Jonny Rutter",
        [26] = "Food",
        [27] = "Sealed Letter",
        [28] = "Moving Rooms Map"
    };

    private static bool ValidateFile()
    {
        _fileMemory.Seek(0, SeekOrigin.Begin);
        var buffer = new byte[_curFileMagic.Length];
        var bytesRead = _fileMemory.Read(buffer, 0, buffer.Length);
        return bytesRead == _curFileMagic.Length && buffer.SequenceEqual(_curFileMagic);
    }

    private static bool LoadFile(string path)
    {
        try
        {
            if (!File.Exists(path))
            {
                Console.WriteLine("File does not exist.");
                return false;
            }
            using var fileStream = new FileStream(path, FileMode.Open);
            _fileMemory.Seek(0, SeekOrigin.Begin);
            fileStream.CopyTo(_fileMemory);
            
            var fileValid = ValidateFile();
            if (!fileValid)
            {
                Console.WriteLine("File is not a valid CUR save file. This tool is only compatible with Curse of Blackmoor Manor save files.");
                return false;
            }
            _fileMemory.Seek(0, SeekOrigin.Begin);
            _fileLoaded = true;
            _currentFilePath = path;
            _saveGameName = Path.GetFileNameWithoutExtension(path);
            
            Console.WriteLine("Loading inventory...");
            _currentInventory.Clear();
            _fileMemory.Seek(0x110, SeekOrigin.Begin);
            uint invSize = _fileReader.ReadUInt32();
            ushort unknown = _fileReader.ReadUInt16();
            for (int i = 0; i < invSize; i++)
            {
                ushort itemId = _fileReader.ReadUInt16();
                _currentInventory.Add(itemId);
            }
            
            Console.WriteLine("File loaded successfully.");
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }

    private static void PrintHelp()
    {
        Console.WriteLine("help - Displays this help message.");
        Console.WriteLine("exit - Exits the program.");
        Console.WriteLine("inv - Lists the items in your inventory.");
        Console.WriteLine("list - Lists all items in the game, along with their IDs.");
        Console.WriteLine("add <item ID> - Adds an item to your inventory.");
        Console.WriteLine("remove <item ID> - Removes an item from your inventory.");
        Console.WriteLine("save - Saves your inventory to the save file.");
    }

    private static void ListAllItems()
    {
        foreach (var item in _curItems)
        {
            Console.WriteLine($"{item.Key}: {item.Value}");
        }
    }

    private static void ListInventory()
    {
        foreach (var item in _currentInventory)
        {
            var itemName = _curItems.GetValueOrDefault(item, "Unknown item");
            Console.WriteLine($"{item}: {itemName}");
        }
        if (_currentInventory.Count == 0)
        {
            Console.WriteLine("Inventory is empty.");
        }
    }

    private static void AddItem(string[] commandParts)
    {
        if (commandParts.Length != 2)
        {
            Console.WriteLine("Invalid command. Usage: add <item ID>");
            return;
        }
        ushort itemId = ushort.Parse(commandParts[1]);
        if (!_curItems.ContainsKey(itemId))
        {
            Console.WriteLine("Invalid item ID.");
            return;
        }

        if (_currentInventory.Contains(itemId))
        {
            Console.WriteLine("Item is already in inventory.");
            return;
        }
        _currentInventory.Add(itemId);
    }

    private static void RemoveItem(string[] commandParts)
    {
        if (commandParts.Length != 2)
        {
            Console.WriteLine("Invalid command. Usage: remove <item ID>");
            return;
        }
        ushort itemId = ushort.Parse(commandParts[1]);
        if (!_currentInventory.Contains(itemId))
        {
            Console.WriteLine("Item is not in inventory.");
            return;
        }
        _currentInventory.Remove(itemId);
    }

    private static void SaveFile()
    {
        try
        {
            _fileMemory.Seek(0x110, SeekOrigin.Begin);
            _fileWriter.Write((uint)_currentInventory.Count);
            _fileMemory.Seek(2, SeekOrigin.Current); // Skip unknown value
            foreach (var item in _currentInventory)
            {
                _fileWriter.Write(item);
            }
            _fileMemory.Seek(0, SeekOrigin.Begin);
            using var fileStream = new FileStream(_currentFilePath, FileMode.Truncate);
            fileStream.SetLength(0);
            _fileMemory.Seek(0, SeekOrigin.Begin);
            _fileMemory.CopyTo(fileStream);
            fileStream.Flush();
            Console.WriteLine("Inventory saved successfully.");
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }
    }
    
    public static void Main()
    {
        Console.WriteLine("~~~Klumpp's CUR Inventory Editor~~~");
        Console.WriteLine("This tool is designed to edit player inventory for Nancy Drew: The Curse of Blackmoor Manor.");
        Console.WriteLine("This tool is not compatible with other games.");
        Console.WriteLine("Your save file will not be modified unless you run the 'save' command.");
        Console.WriteLine("!!!BACK UP YOUR SAVE FILE BEFORE RUNNING THIS TOOL!!!\n");
        
        while (!_fileLoaded)
        {
            Console.Write("Enter the path to your Curse of Blackmoor Manor save file (.SAV): ");
            var path = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(path))
                continue;
            path = path.Trim('"');
            var result = LoadFile(path);
            if (!result)
                continue;
            break;
        }
        
        Console.WriteLine("Enter a command or type 'help' for a list of commands.");

        while (true)
        {
            Console.Write(_saveGameName);
            Console.Write("> ");
            var command = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(command))
                continue;
            var commandParts = command.Split(' ');
            if (commandParts.Length == 0)
                continue;
            var commandName = commandParts[0].ToLower();
            
            if (commandName == "exit")
            {
                break;
            }
            
            if (commandName == "help")
            {
                PrintHelp();
                continue;
            }
            
            if (commandName == "list")
            {
                ListAllItems();
                continue;
            }

            if (commandName == "inv")
            {
                ListInventory();
                continue;
            }

            if (commandName == "add")
            {
                AddItem(commandParts);
                continue;
            }

            if (commandName == "remove")
            {
                RemoveItem(commandParts);
                continue;
            }
            
            if (commandName == "save")
            {
                SaveFile();
                continue;
            }
            
            Console.WriteLine("Invalid command.");
        }
        
    }
}