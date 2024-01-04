//IMPORTING THE LIBRARIES
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using static Project.Program;
using static Project.Program.Product;

namespace Project
{


    internal class Program
    {
        //MAIN CLASS ORDER USED TO MANAGE THE PAYMENT
        // Strategy pattern - used for payment methods (Credit Card and PayPal).
        // This allows changing the payment method without affecting the ShoppingCart or Order classes.
        public class Order
        {
            private IPaymentStrategy paymentStrategy;

            public void SetPaymentStrategy(IPaymentStrategy paymentStrategy)
            {
                this.paymentStrategy = paymentStrategy;
            }

            public void ProcessPayment(double amount)
            {
                if (paymentStrategy == null)
                {
                    Console.WriteLine("Please select a payment method.");
                    return;
                }
                paymentStrategy.Pay(amount);
            }
        }

        // Strategy Interface
        // IPaymentStrategy: Interface for payment strategies(Credit Card and PayPal).
        public interface IPaymentStrategy
        {
            void Pay(double amount);
        }

        // Concrete Strategies
        public class CreditCardPayment : IPaymentStrategy
        {
            public void Pay(double amount)
            {
                Console.WriteLine($"Paying ${amount} using Credit Card.");
                
            }
        }

        public class PayPalPayment : IPaymentStrategy
        {
            public void Pay(double amount)
            {
                Console.WriteLine($"Paying ${amount} using PayPal.");
                
            }
        }

        // Singleton pattern - used for the Logger class
        // This ensures only one instance of the Logger class is available throughout the application.
        // - Logger: Logs exceptions to a file using a Singleton pattern.

        public class Logger
        {
            private static Logger instance;
            private static readonly object padlock = new object();

            private Logger() { }

            public static Logger Instance
            {
                get
                {
                    lock (padlock)
                    {
                        if (instance == null)
                        {
                            instance = new Logger();
                        }
                        return instance;
                    }
                }
            }

            public void LogException(Exception ex)
            {
                string logMessage = $"Exception occurred: {ex.Message}\nStackTrace: {ex.StackTrace}\n";

                // Write log message to a log file
                WriteLogToFile(logMessage);
            }

            private void WriteLogToFile(string logMessage)
            {
                // Write log message to a file
                // Prepare the file name or use a timestamped file name
                string fileName = $"Log_{DateTime.Now:yyyyMMdd}.txt";

                // Path where the file will be stored.
                string filePath = Path.Combine(Environment.CurrentDirectory, fileName);
                using (StreamWriter writer = File.AppendText(filePath))
                {
                    writer.WriteLine(logMessage);
                }
            }
        }

        // Singleton pattern - used for the Logger class
        // This ensures only one instance of the ShoppingCart class is available throughout the application.
        public class ShoppingCart
        {
            private static ShoppingCart instance;

            private List<(string prodName, int quantity, double netPrice, double tax, double grossPrice)> products;

            private ShoppingCart()
            {
                this.products = new List<(string, int, double, double, double)>();
            }

            public static ShoppingCart GetInstance()
            {
                if (instance == null)
                {
                    instance = new ShoppingCart();
                }

                return instance;
            }

            // Method to add a product to the shopping cart
            public void AddProduct(string productName, int quantity, double netPrice, double tax, double grossPrice)
            {
                Console.WriteLine("Added");
                this.products.Add((productName, quantity, netPrice, tax, grossPrice));
            }

            // Method to remove a product from the shopping cart
            public void RemoveProduct(string productName)
            {

                var productToRemove = products.FindIndex(p => p.prodName == productName);
                if (productToRemove != -1)
                {
                    products.RemoveAt(productToRemove);
                }
            }

            // Method to change the quantity of a product in the shopping cart
            public void ChangeQuantity(string productName, int newQuantity)
            {
                var productToUpdate = products.Find(p => p.prodName == productName);
                if (!Equals(productToUpdate, default((string, int, double, double, double))))
                {
                    productToUpdate.quantity = newQuantity;
                }
            }

            public void doBuying(Assortment product1, Assortment product2, Assortment product3, Assortment product4, Assortment product5)
            {

                List<int> indexesToRemove = new List<int>();
                Dictionary<string, int> invoices = new Dictionary<string, int>();

                foreach (var product in products)
                {
                    string productName = product.prodName;
                    int productCount = product.quantity;
                    Boolean Check = false;
                    int index = products.FindIndex(p => p.prodName == productName);
                    int Quantity = 0;
                    for (int i = 0; i < productCount; i++)
                    {
                        Dictionary<string, int> items1 = product1.GetItems();
                        if (items1.ContainsKey(productName))
                        {
                            items1.Remove(productName);
                            Check = true;
                            Quantity++;
                        }
                        Dictionary<string, int> items2 = product2.GetItems();
                        if (items2.ContainsKey(productName))
                        {
                            items2.Remove(productName);
                            Check = true;
                            Quantity++;

                        }
                        Dictionary<string, int> items3 = product3.GetItems();
                        if (items3.ContainsKey(productName))
                        {
                            items3.Remove(productName);
                            Check = true;
                            Quantity++;

                        }
                        Dictionary<string, int> items4 = product4.GetItems();
                        if (items4.ContainsKey(productName))
                        {
                            items4.Remove(productName);
                            Check = true;
                            Quantity++;

                        }
                        Dictionary<string, int> items5 = product5.GetItems();
                        if (items5.ContainsKey(productName))
                        {
                            items5.Remove(productName);
                            Check = true;
                            Quantity++;

                        }

                    }

                    if (Check == true)
                    {
                        indexesToRemove.Add(index);
                        invoices.Add(productName,Quantity);
                        
                    }
                }

                for (int i = indexesToRemove.Count - 1; i >= 0; i--)
                {
                    int indexToRemove = indexesToRemove[i];
                    products.RemoveAt(indexToRemove);
                    
                }

                prepareInvoice(invoices);

            }

            public void prepareInvoice(Dictionary<string, int> invoices)
            {
                string filePath = "Invoices.txt"; // Change the file path as needed

                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var kvp in invoices)
                    {
                        writer.WriteLine($"Item sold: {kvp.Key}, Quantity {kvp.Value}");
                    }
                }

                Console.WriteLine("File created and added the invoices");
            }

            // Method to display the contents of the shopping cart
            public void DisplayCart()
            {
                Console.WriteLine("prodotti: ");
                foreach (var product in products)
                {
                    Console.WriteLine($"Product: {product.prodName}, Quantity: {product.quantity}, " +
                                      $"Net Price: {product.netPrice}, Tax: {product.tax}, " +
                                      $"Gross Price: {product.grossPrice}");
                }
            }
        }

        // Assortment interface, here we define the methods
        public interface Assortment
        {
            void addItem(String item, int price);
            void delItem(String item);
            void displayItems();
            void TakeDelivery(String productName, int Quantity, int priceOf);
            Dictionary<string, int> GetItems();
        }

        // Factory pattern - used to create product instances (Product and Assortment).
        // Provides an interface for creating instances of classes related by a common theme, without specifying the concrete class.
        public class Product : Assortment
        {
            public String productName;
            public Dictionary<String,int> items;

            public Product(String productName)
            {
                this.productName = productName;
                this.items = new Dictionary<String,int>();
            }

            public void addItem(String item, int price)
            {
                try
                {
                    items.Add(item,price);
                    
                    Console.WriteLine("Item inserted" + item);
                }
                catch
                {
                    Console.WriteLine("element not inserted" + item);
                }
            }

            public void delItem(String item)
            {
                if (items.Remove(item))
                {
                    Console.WriteLine("Item " + item + " Removed");
                }
                else
                {
                    Console.WriteLine("Item" + item + " Not removed");
                }
            }

            public void displayItems()
            {
                if (items.Count == 0)
                {
                    Console.WriteLine("There are no items in this product");
                }
                else
                {
                    Console.WriteLine("Items in " + productName + " assortment:");
                    foreach (var item in items)
                    {
                        Console.WriteLine($"Item: {item.Key}, Price: {item.Value}");
                    }
                }
            }

            public Dictionary<string, int> GetItems()
            {
                return items; // Returns the list of items stored in the product
            }

            public void TakeDelivery( String productName, int Quantity, int priceOf)
            {
                string quantity= Quantity.ToString();
                //Adding the delivery to the assortement

                for (int i = 0;i < Quantity; i++)
                {
                    items.Add(productName, priceOf);
                }

                //Creating the file to add the Taken Delivery

                // Prepare the file name or use a timestamped file name
                string fileName = $"Deliveries_{DateTime.Now:yyyyMMdd}.txt";

                // Path where the file will be stored, change it according to your requirements
                string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

                try
                {
                    // Write delivery information to the file
                    File.WriteAllText(filePath, productName + " " + quantity);
                    Console.WriteLine($"Delivery information saved to file: {fileName}");
                }

                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred while creating the delivery file: {ex.Message}");
                }

            }

            //Factory for the Assortment
            public class productFactory
            {
                public Assortment createProduct(String productName)
                {
                    return new Product(productName);
                }

            }

        }

        static void Main(string[] args)
        {
            Logger logger = Logger.Instance;
            //INITIALIZATION OF THE ASSORTMENT AND SHOPPING CARD
            try { 
                productFactory factory = new productFactory();

                Assortment product1 = factory.createProduct("Shoes");
                Assortment product2 = factory.createProduct("T-shirt");
                Assortment product3 = factory.createProduct("Pants");
                Assortment product4 = factory.createProduct("Pjama");
                Assortment product5 = factory.createProduct("Accessories");

                populateRandomProducts(product1, 5, 0);
                populateRandomProducts(product2, 5, 1);
                populateRandomProducts(product3, 5, 2);
                populateRandomProducts(product4, 5, 3);
                populateRandomProducts(product5, 5, 4);

                //INITIALIZATION OF THE SHOPPING CART
                ShoppingCart cart = ShoppingCart.GetInstance();

                // Add/remove/change products in the shopping cart
                cart.AddProduct("Sample", 2, 20.0, 5.0, 25.0);
            

                Boolean exitRequested = false;

                while (!exitRequested)
                {
                    Console.WriteLine("Select an operation:");
                    Console.WriteLine("1. Display items avaible in the Assortment");
                    Console.WriteLine("2. Displaying the Shopping Cart");
                    Console.WriteLine("3. Insert new Delivery");
                    Console.WriteLine("4. To place an order");
                    Console.WriteLine("5  Shopping Card");
                    Console.WriteLine("6  Generate Stock File");
                    Console.WriteLine("0. Exit");

                    Console.Write("Enter your choice: ");
                    string userInput = Console.ReadLine();

                    switch (userInput)
                    {

                        case "1":
 
                             product1.displayItems();
                             product2.displayItems();
                             product3.displayItems();
                             product4.displayItems();
                             product5.displayItems();
                             break;
                         case "2":

                             //Displaying of the shopping cart.
                            cart.DisplayCart();
                            break;
                        case "3":

                            Console.Write("Enter product name: ");
                            string productName = Console.ReadLine();

                            Console.Write("Enter quantity received: ");
                            int quantityReceived = Int32.Parse(Console.ReadLine());

                            Console.Write("Enter the price: ");
                            int priceOf = Int32.Parse(Console.ReadLine());

                            //Displaying of the shopping cart.
                            product1.TakeDelivery(productName, quantityReceived, priceOf);
                            break;

                        case "4":
                            Console.Write("Enter the stock treshold: ");
                            int number = Int32.Parse(Console.ReadLine());
                            toPlaceAnOrder(product1, product2, product3, product4, product5, number);
                            break;
                        case "5":
                            Console.WriteLine("1. Insert new product in the Shopping Cart");
                            Console.WriteLine("2. Displaying the Shopping Cart");
                            Console.WriteLine("3. Change product in the shopping card");
                            Console.WriteLine("4. Buy products in the shopping card");

                            Console.Write("Enter your choice for the Shopping Card: ");
                            string userInput2 = Console.ReadLine();
                            switch (userInput2)
                            {
                                case "1":
                                    Console.Write("Insert the name of the product: ");
                                    string nameProduct = Console.ReadLine();
                                    Console.Write("Insert the quantity of the product: ");
                                    int quantityProduct = Int32.Parse(Console.ReadLine());
                                    cart.AddProduct(nameProduct, quantityProduct, 20.0, 5.0, 25.0); // TODO: Here this quantity are still fixed, i need to add in the class
                                    break;
                                case "2":
                                    //Displaying of the shopping cart.
                                    cart.DisplayCart();
                                    break;
                                case "3":
                                    Console.Write("Insert the name of the product: ");
                                    nameProduct = Console.ReadLine();
                                    Console.Write("Insert the quantity of the product: ");
                                    quantityProduct = Int32.Parse(Console.ReadLine());
                                    cart.ChangeQuantity(nameProduct, quantityProduct);
                                    break;
                                case "4":
                                    Console.Write("Do you wanna finalize your buying?: ");
                                    string decision = Console.ReadLine();

                                    if (decision == "yes")
                                    {
                                        Order order = new Order();
                                        Console.Write("1. To Pay with CrediCard: ");
                                        Console.Write("2. To Pay with Paypal: ");
                                        string userInput3 = Console.ReadLine();
                                        switch(userInput3)
                                        {
                                            case "1":
                                                Console.Write("Insert the total price amount : ");
                                                int totalPrice = Int32.Parse(Console.ReadLine());

                                                order.SetPaymentStrategy(new CreditCardPayment());
                                                order.ProcessPayment(totalPrice); // Pays using Credit Card
                                                break;
                                            case "2":
                                                Console.Write("Insert the total price amount : ");
                                                int totalPrice2 = Int32.Parse(Console.ReadLine());

                                                order.SetPaymentStrategy(new PayPalPayment());
                                                order.ProcessPayment(totalPrice2); // Pays using Paypal
                                                break;
                                            default:
                                                Console.WriteLine("Invalid choice. Please try again.");
                                                break;
                                        }
                                        cart.doBuying(product1, product2, product3, product4, product5);

                                    }

                                    break;

                                default:
                                    Console.WriteLine("Invalid choice. Please try again.");
                                    break;
                            }

                            break;

                        case "6":
                            Dictionary<string, int> stock = new Dictionary<string, int>();
                            stock = MergeProductsStock(product1, product2, product3, product4, product5);
                            GenerateStockFile(stock);
                            break;

                        case "0":

                            exitRequested = true;
                            break;

                        default:

                            Console.WriteLine("Invalid choice. Please try again.");
                            break;

                    }

                    // Keep the console open until Enter is pressed
                    Console.WriteLine("Press Enter to exit...");
                    Console.ReadLine();
                }
                
            }
            catch(Exception ex) {
                Logger.Instance.LogException(ex);
            }
            }


            public static void toPlaceAnOrder(Assortment product1, Assortment product2, Assortment product3, Assortment product4, Assortment product5, int quantity)
            {
            Dictionary<string, int> itemsCount = new Dictionary<string, int>();

            itemsCount = countOccurences(product1, itemsCount);
            itemsCount = countOccurences(product2, itemsCount);
            itemsCount = countOccurences(product3, itemsCount);
            itemsCount = countOccurences(product4, itemsCount);
            itemsCount = countOccurences(product5, itemsCount);

            foreach (var item in itemsCount.Keys.ToList())
            {
                if (itemsCount[item] < quantity)
                {
                    itemsCount.Remove(item);
                }
            }

            string filePath = "PlaceAnOrder.txt"; // Change the file path as needed

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var kvp in itemsCount)
                {
                    writer.WriteLine($"Item: {kvp.Key}, Count: {kvp.Value}");
                }
            }

            Console.WriteLine("Items and counts saved to file.");

        }

        public static Dictionary<string, int> countOccurences(Assortment product, Dictionary<string, int> itemsCount)
        {
            Dictionary<string, int> items = product.GetItems();

            foreach (var item in items)
            {
                if (itemsCount.ContainsKey(item.Key))
                {
                    itemsCount[item.Key]++;
                }
                else
                {
                    itemsCount.Add(item.Key, 1);
                }
            }
            return itemsCount;
        }

        public static void populateRandomProducts(Assortment product, int numberOfProducts, int typecloth)
            {
            Random random = new Random();



            string[] brands = { "NIKE", "ADIDAS", "UNDER ARMOUR","PIERONE", "GUCCI", "GUESS" };
            string[] types = { "BAGGY", "SHORT", "SHOES", "JACKET", "SHIRT", "PANTS" };
            string[] model = { "M1", "M2", "M3", "M4", "M5", "M6" };
            string[] type = { "SHOES", "T-SHIRT", "PANTS", "PJAMA", "ACCESSORIES" };
            int[] price = { 1, 3, 5, 7, 13, 15, 18, 21 };

            HashSet<string> generatedProducts = new HashSet<string>();

            for (int i = 0; i < numberOfProducts; i++)
            {
                string randomBrand = brands[random.Next(brands.Length)];
                string randomType = types[random.Next(types.Length)];
                string randomModel = model[random.Next(model.Length)];
                string productType = "";

                if (typecloth == 0)
                {
                    productType = "SHOES";
                }

                if (typecloth == 1)
                {
                    productType = "T-SHIRT";
                }

                if (typecloth == 2)
                {
                    productType = "PANTS";
                }

                if (typecloth == 3)
                {
                    productType = "PJAMA";
                }

                if (typecloth == 4)
                {
                    productType = "ACCESSORIES";
                }

                string productName = $"{randomBrand} {randomType} {randomModel} {productType}";
                
                // Ensure the generated product name is unique
                while (generatedProducts.Contains(productName))
                {
                    randomBrand = brands[random.Next(brands.Length)];
                    randomType = types[random.Next(types.Length)];
                    randomModel = model[random.Next(model.Length)];
                    productName = $"{randomBrand} {randomType} {randomModel} {productType}";
                }

                // Add the unique product name to the set
                generatedProducts.Add(productName);

                int productPrice = price[random.Next(price.Length)];

                // Add the unique product to the assortment
                product.addItem(productName, productPrice);

            }
            }
        public static void GenerateStockFile(Dictionary<string, int> stock)
        {
            // Prepare the file name or use a timestamped file name
            string fileName = $"StockOnHand_{DateTime.Now:yyyyMMdd}.txt";

            // Path where the file will be stored.
            string filePath = Path.Combine(Environment.CurrentDirectory, fileName);

            try
            {
                // Write stock information to the file
                using (StreamWriter writer = new StreamWriter(filePath))
                {
                    foreach (var item in stock)
                    {
                        writer.WriteLine($"Product: {item.Key}, Price: {item.Value}");
                    }
                }

                Console.WriteLine($"Stock information saved to file: {fileName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred while creating the stock file: {ex.Message}");
            }
        }
        
        public static Dictionary<string, int> MergeProductsStock(Assortment product1, Assortment product2, Assortment product3, Assortment product4, Assortment product5)
        {
            Dictionary<string, int> stock = new Dictionary<string, int>();
            stock=AggregateProductStock(product1, stock);
            stock = AggregateProductStock(product2, stock);
            stock = AggregateProductStock(product3, stock);
            stock = AggregateProductStock(product4, stock);
            stock = AggregateProductStock(product5, stock);

            return stock;
        }



        private static Dictionary<string, int> AggregateProductStock(Assortment product, Dictionary<string, int> aggregatedStock)
        {
            Dictionary<string, int> productStock = product.GetItems();
            
            foreach (var item in productStock)
            {
               
                if (aggregatedStock.ContainsKey(item.Key))
                {
                    string concat = item.Key + "_";
                    
                }
                else
                {
                    aggregatedStock.Add(item.Key, item.Value);
                }
            }
            return aggregatedStock;
        }
    }
    } 
