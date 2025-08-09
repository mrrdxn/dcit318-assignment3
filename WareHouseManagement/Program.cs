using System;
using System.Collections.Generic;

class InventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
}

class Program
{
    static List<InventoryItem> inventory = new List<InventoryItem>();

    static void Main()
    {
        while (true)
        {
            Console.WriteLine("\nWarehouse Management Console");
            Console.WriteLine("1. Add Item");
            Console.WriteLine("2. Remove Item");
            Console.WriteLine("3. Update Item");
            Console.WriteLine("4. View Inventory");
            Console.WriteLine("5. Exit");
            Console.Write("Select an option: ");

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1": AddItem(); break;
                case "2": RemoveItem(); break;
                case "3": UpdateItem(); break;
                case "4": ViewInventory(); break;
                case "5": return;
                default: Console.WriteLine("Invalid option."); break;
            }
        }
    }

    static void AddItem()
    {
        InventoryItem item = new InventoryItem();
        Console.Write("Enter item ID: ");
        item.Id = int.Parse(Console.ReadLine());
        Console.Write("Enter item name: ");
        item.Name = Console.ReadLine();
        Console.Write("Enter quantity: ");
        item.Quantity = int.Parse(Console.ReadLine());
        inventory.Add(item);
        Console.WriteLine("Item added.");
    }

    static void RemoveItem()
    {
        Console.Write("Enter ID to remove: ");
        int id = int.Parse(Console.ReadLine());
        var item = inventory.Find(i => i.Id == id);
        if (item != null)
        {
            inventory.Remove(item);
            Console.WriteLine("Item removed.");
        }
        else
        {
            Console.WriteLine("Item not found.");
        }
    }

    static void UpdateItem()
    {
        Console.Write("Enter ID to update: ");
        int id = int.Parse(Console.ReadLine());
        var item = inventory.Find(i => i.Id == id);
        if (item != null)
        {
            Console.Write("Enter new name: ");
            item.Name = Console.ReadLine();
            Console.Write("Enter new quantity: ");
            item.Quantity = int.Parse(Console.ReadLine());
            Console.WriteLine("Item updated.");
        }
        else
        {
            Console.WriteLine("Item not found.");
        }
    }

    static void ViewInventory()
    {
        Console.WriteLine("\nInventory:");
        foreach (var item in inventory)
        {
            Console.WriteLine($"ID: {item.Id}, Name: {item.Name}, Quantity: {item.Quantity}");
        }
    }
}
