using System;
using System.Collections.Generic;
using System.Linq;

// Marker interface
interface IInventoryItem { }

// Custom exceptions
class DuplicateItemException : Exception { public DuplicateItemException(string message) : base(message) { } }
class ItemNotFoundException : Exception { public ItemNotFoundException(string message) : base(message) { } }
class InvalidQuantityException : Exception { public InvalidQuantityException(string message) : base(message) { } }

// Product base
abstract class Product : IInventoryItem
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Quantity { get; set; }
}

class Grocery : Product { public DateTime ExpiryDate { get; set; } }
class Electronic : Product { public int WarrantyMonths { get; set; } }

// Generic repository
class InventoryRepository<T> where T : IInventoryItem
{
    private List<T> items = new List<T>();
    public void Add(T item)
    {
        if (items.Contains(item)) throw new DuplicateItemException("Item already exists.");
        items.Add(item);
    }
    public void Remove(T item)
    {
        if (!items.Contains(item)) throw new ItemNotFoundException("Item not found.");
        items.Remove(item);
    }
    public List<T> GetAll() => items;
}

class WarehouseManager
{
    private InventoryRepository<Product> repository = new InventoryRepository<Product>();
    public void AddProduct(Product product)
    {
        if (product.Quantity < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        repository.Add(product);
    }
    public void RemoveProduct(int id)
    {
        var product = repository.GetAll().FirstOrDefault(p => p.Id == id);
        if (product == null) throw new ItemNotFoundException("Product not found.");
        repository.Remove(product);
    }
    public void UpdateQuantity(int id, int qty)
    {
        if (qty < 0) throw new InvalidQuantityException("Quantity cannot be negative.");
        var product = repository.GetAll().FirstOrDefault(p => p.Id == id);
        if (product == null) throw new ItemNotFoundException("Product not found.");
        product.Quantity = qty;
    }
    public void ViewInventory()
    {
        var all = repository.GetAll();
        if (!all.Any()) { Console.WriteLine("Inventory is empty."); return; }
        foreach (var p in all)
        {
            Console.WriteLine($"ID: {p.Id}, Name: {p.Name}, Quantity: {p.Quantity}");
        }
    }
}

class Program
{
    static void Main()
    {
        var manager = new WarehouseManager();
        bool running = true;
        while (running)
        {
            Console.WriteLine("\nWarehouse Management Menu:");
            Console.WriteLine("1. Add Product");
            Console.WriteLine("2. Remove Product");
            Console.WriteLine("3. Update Quantity");
            Console.WriteLine("4. View Inventory");
            Console.WriteLine("5. Exit");
            Console.Write("Choose an option: ");
            string choice = Console.ReadLine();

            try
            {
                switch (choice)
                {
                    case "1":
                        Console.Write("Enter Product ID: ");
                        int id = int.Parse(Console.ReadLine());
                        Console.Write("Enter Product Name: ");
                        string name = Console.ReadLine();
                        Console.Write("Enter Quantity: ");
                        int qty = int.Parse(Console.ReadLine());
                        Console.Write("Is it Grocery (G) or Electronic (E)?: ");
                        string type = Console.ReadLine().ToUpper();
                        if (type == "G")
                        {
                            Console.Write("Enter Expiry Date (yyyy-mm-dd): ");
                            DateTime expiry = DateTime.Parse(Console.ReadLine());
                            manager.AddProduct(new Grocery { Id = id, Name = name, Quantity = qty, ExpiryDate = expiry });
                        }
                        else
                        {
                            Console.Write("Enter Warranty Months: ");
                            int warranty = int.Parse(Console.ReadLine());
                            manager.AddProduct(new Electronic { Id = id, Name = name, Quantity = qty, WarrantyMonths = warranty });
                        }
                        Console.WriteLine("Product added successfully.");
                        break;

                    case "2":
                        Console.Write("Enter Product ID to remove: ");
                        int removeId = int.Parse(Console.ReadLine());
                        manager.RemoveProduct(removeId);
                        Console.WriteLine("Product removed successfully.");
                        break;

                    case "3":
                        Console.Write("Enter Product ID to update: ");
                        int updateId = int.Parse(Console.ReadLine());
                        Console.Write("Enter new Quantity: ");
                        int newQty = int.Parse(Console.ReadLine());
                        manager.UpdateQuantity(updateId, newQty);
                        Console.WriteLine("Quantity updated successfully.");
                        break;

                    case "4":
                        manager.ViewInventory();
                        break;

                    case "5":
                        running = false;
                        break;

                    default:
                        Console.WriteLine("Invalid choice, try again.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }
}
