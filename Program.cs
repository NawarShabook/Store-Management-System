using System;
using System.Collections.Generic;

/// <summary>
/// Store Management System
/// 
/// Purpose: A program that calculates the final invoice for products with discounts and taxes
/// 
/// Subject: FPR501 - Store Management System
/// University: Syrian Virtual University

/// </summary>

class StoreManagementSystem
{
    // Program constants
    // NOTE: Tax rate is a configurable default value (15%).
    // Change this single constant to adjust the tax rate for the entire program.
    private const decimal TAX_RATE = 0.15m;
    private const string SEPARATOR = "========================================================";

    // Delivery method constants
    // The user selects a delivery METHOD (not a free-typed fee).
    // Each method has a fixed cost defined here as a single source of truth.
    private const decimal STANDARD_DELIVERY_FEE = 10.00m;   // Standard delivery cost
    private const decimal EXPRESS_DELIVERY_FEE = 25.00m;    // Express delivery cost

    // Free delivery condition (business rule / constant from requirements)
    // If the subtotal reaches or exceeds this threshold, delivery becomes
    // free automatically, regardless of which method the user picked.
    private const decimal FREE_DELIVERY_THRESHOLD = 500.00m;

    // Program variables
    private int numberOfProducts = 0;
    private List<Product> products = new List<Product>();
    private decimal subtotal = 0m;
    private decimal discountAmount = 0m;
    private decimal taxAmount = 0m;
    private int deliveryMethodChoice = 0;      // 1 = Standard, 2 = Express
    private decimal shippingFee = 0m;          // Calculated based on method + free-delivery rule
    private decimal finalTotal = 0m;

    /// <summary>
    /// Product class - represents one product in the store
    /// </summary>
    private class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal DiscountPercentage { get; set; }
        public decimal Total { get; set; }
    }

    /// <summary>
    /// Program entry point
    /// </summary>
    static void Main()
    {
        StoreManagementSystem system = new StoreManagementSystem();
        system.Run();
    }

    /// <summary>
    /// Run the main program
    /// </summary>
    private void Run()
    {
        DisplayWelcome();
        GetNumberOfProducts();
        InputProductsData();
        CalculateSubtotalAndDiscount();  // Needed first: free delivery depends on subtotal
        GetDeliveryMethod();             // User selects a delivery METHOD, not a free-typed fee
        CalculateFinalTotals();
        DisplayInvoice();
        SaveInvoiceOption();
    }

    /// <summary>
    /// Display welcome message
    /// </summary>
    private void DisplayWelcome()
    {
        Console.Clear();
        Console.WriteLine(SEPARATOR);
        Console.WriteLine("           STORE MANAGEMENT SYSTEM");
        Console.WriteLine(SEPARATOR);
        Console.WriteLine();
    }

    /// <summary>
    /// Get number of products with validation
    /// </summary>
    private void GetNumberOfProducts()
    {
        bool isValid = false;
        while (!isValid)
        {
            try
            {
                Console.Write("Enter number of products: ");
                numberOfProducts = int.Parse(Console.ReadLine() ?? "0");

                if (numberOfProducts <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: Number must be positive (greater than 0)");
                    Console.ResetColor();
                }
                else if (numberOfProducts > 100)
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("WARNING: Number of products is too large (max 100)");
                    Console.ResetColor();
                }
                else
                {
                    isValid = true;
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Must enter a valid number");
                Console.ResetColor();
            }
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Input product data
    /// </summary>
    private void InputProductsData()
    {
        Console.WriteLine($"Enter data for {numberOfProducts} products:");
        Console.WriteLine();

        for (int i = 0; i < numberOfProducts; i++)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Product {i + 1}");
            Console.WriteLine("----------------------------------------");

            string productName = GetProductName();
            decimal price = GetPrice();
            int quantity = GetQuantity();
            decimal discount = GetDiscount();

            // Calculate product price
            decimal productTotal = price * quantity;
            decimal discountValue = (discount / 100) * productTotal;
            productTotal -= discountValue;

            // Add product to list
            products.Add(new Product
            {
                Name = productName,
                Price = price,
                Quantity = quantity,
                DiscountPercentage = discount,
                Total = productTotal
            });

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"SUCCESS: Product added: {productName}");
            Console.ResetColor();
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Get product name with validation
    /// </summary>
    private string GetProductName()
    {
        string name = "";
        bool isValid = false;

        while (!isValid)
        {
            Console.Write("Product name: ");
            name = Console.ReadLine() ?? "";

            if (string.IsNullOrWhiteSpace(name))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Product name is required");
                Console.ResetColor();
            }
            else
            {
                isValid = true;
            }
        }
        return name;
    }

    /// <summary>
    /// Get price with validation
    /// </summary>
    private decimal GetPrice()
    {
        decimal price = 0m;
        bool isValid = false;

        while (!isValid)
        {
            try
            {
                Console.Write("Unit price: ");
                price = decimal.Parse(Console.ReadLine() ?? "0");

                if (price <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: Price must be positive");
                    Console.ResetColor();
                }
                else
                {
                    isValid = true;
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Must enter a valid number");
                Console.ResetColor();
            }
        }
        return price;
    }

    /// <summary>
    /// Get quantity with validation
    /// </summary>
    private int GetQuantity()
    {
        int quantity = 0;
        bool isValid = false;

        while (!isValid)
        {
            try
            {
                Console.Write("Quantity: ");
                quantity = int.Parse(Console.ReadLine() ?? "0");

                if (quantity <= 0)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: Quantity must be positive");
                    Console.ResetColor();
                }
                else
                {
                    isValid = true;
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Must enter a valid number");
                Console.ResetColor();
            }
        }
        return quantity;
    }

    /// <summary>
    /// Get discount percentage with validation
    /// </summary>
    private decimal GetDiscount()
    {
        decimal discount = 0m;
        bool isValid = false;

        while (!isValid)
        {
            try
            {
                Console.Write("Discount % (0 if none): ");
                discount = decimal.Parse(Console.ReadLine() ?? "0");

                if (discount < 0 || discount > 100)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: Discount must be between 0 and 100");
                    Console.ResetColor();
                }
                else
                {
                    isValid = true;
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Must enter a valid number");
                Console.ResetColor();
            }
        }
        return discount;
    }

    /// <summary>
    /// Calculate subtotal and total discount from the entered products.
    /// This MUST run before delivery method selection, because the
    /// free-delivery rule depends on the subtotal amount.
    /// </summary>
    private void CalculateSubtotalAndDiscount()
    {
        // Calculate subtotal
        subtotal = 0m;
        foreach (var product in products)
        {
            subtotal += product.Total;
        }

        // Calculate total discount
        discountAmount = 0m;
        foreach (var product in products)
        {
            decimal originalPrice = product.Price * product.Quantity;
            decimal discountValue = (product.DiscountPercentage / 100) * originalPrice;
            discountAmount += discountValue;
        }
    }

    /// <summary>
    /// Get delivery method from the user.
    /// The user picks a METHOD from a menu (Standard / Express) - not a
    /// free-typed fee. If the subtotal already reaches the free delivery
    /// threshold (a fixed business constant), delivery is free automatically
    /// and the choice of method does not add any cost.
    /// </summary>
    private void GetDeliveryMethod()
    {
        Console.WriteLine();
        Console.WriteLine("----------------------------------------");
        Console.WriteLine("Delivery Method");
        Console.WriteLine("----------------------------------------");

        // Business rule: free delivery if subtotal meets the threshold
        if (subtotal >= FREE_DELIVERY_THRESHOLD)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Your order qualifies for FREE delivery (subtotal >= {FREE_DELIVERY_THRESHOLD:F2}).");
            Console.ResetColor();
            shippingFee = 0m;
            deliveryMethodChoice = 0; // 0 = Free (threshold met)
            Console.WriteLine();
            return;
        }

        bool isValid = false;
        while (!isValid)
        {
            try
            {
                Console.WriteLine("Choose a delivery method:");
                Console.WriteLine($"  1. Standard Delivery ({STANDARD_DELIVERY_FEE:F2})");
                Console.WriteLine($"  2. Express Delivery  ({EXPRESS_DELIVERY_FEE:F2})");
                Console.Write("Enter choice (1 or 2): ");
                deliveryMethodChoice = int.Parse(Console.ReadLine() ?? "0");

                if (deliveryMethodChoice == 1)
                {
                    shippingFee = STANDARD_DELIVERY_FEE;
                    isValid = true;
                }
                else if (deliveryMethodChoice == 2)
                {
                    shippingFee = EXPRESS_DELIVERY_FEE;
                    isValid = true;
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("ERROR: Please enter 1 or 2");
                    Console.ResetColor();
                }
            }
            catch (FormatException)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR: Must enter a valid number");
                Console.ResetColor();
            }
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Calculate taxes and the final total.
    /// Runs after the delivery method has been resolved (shippingFee is set).
    /// </summary>
    private void CalculateFinalTotals()
    {
        taxAmount = subtotal * TAX_RATE;
        finalTotal = subtotal + taxAmount + shippingFee;
    }

    /// <summary>
    /// Display complete invoice
    /// </summary>
    private void DisplayInvoice()
    {
        //Console.Clear();
        Console.WriteLine(SEPARATOR);
        Console.WriteLine("                 FINAL INVOICE");
        Console.WriteLine(SEPARATOR);
        Console.WriteLine();

        // Display product details
        Console.WriteLine("Product Details:");
        Console.WriteLine("========================================================");
        Console.WriteLine(
            String.Format("{0,-20} {1,10} {2,10} {3,10} {4,10}",
            "Product", "Price", "Quantity", "Discount%", "Total")
        );
        Console.WriteLine("========================================================");

        foreach (var product in products)
        {
            Console.WriteLine(
                String.Format("{0,-20} {1,10:F2} {2,10} {3,10:F2} {4,10:F2}",
                product.Name,
                product.Price,
                product.Quantity,
                product.DiscountPercentage,
                product.Total)
            );
        }

        Console.WriteLine("========================================================");
        Console.WriteLine();

        // Display calculations
        Console.WriteLine("Calculations:");
        Console.WriteLine("========================================================");
        Console.WriteLine($"Subtotal (before taxes and shipping):     {subtotal,12:F2}");
        Console.WriteLine($"Total Discount:                          {discountAmount,12:F2}");
        Console.WriteLine($"Taxes (15%):                             {taxAmount,12:F2}");
        Console.WriteLine($"Delivery Method:                         {GetDeliveryMethodName(),12}");
        Console.WriteLine($"Shipping Fee:                            {shippingFee,12:F2}");
        Console.WriteLine("========================================================");

        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"FINAL TOTAL:                             {finalTotal,12:F2}");
        Console.ResetColor();

        Console.WriteLine(SEPARATOR);
        Console.WriteLine();
    }

    /// <summary>
    /// Returns a display-friendly name for the currently selected delivery method.
    /// </summary>
    private string GetDeliveryMethodName()
    {
        if (deliveryMethodChoice == 0)
            return "Free (threshold)";
        if (deliveryMethodChoice == 1)
            return "Standard";
        if (deliveryMethodChoice == 2)
            return "Express";
        return "Unknown";
    }

    /// <summary>
    /// Option to save invoice to file
    /// </summary>
    private void SaveInvoiceOption()
    {
        Console.WriteLine("Do you want to save the invoice?");
        Console.Write("(Type: yes to confirm, anything else to exit): ");
        string choice = Console.ReadLine() ?? "";

        if (choice.ToLower() == "yes")
        {
            SaveInvoiceToFile();
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("Thank you for using Store Management System!");
        Console.ResetColor();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Save invoice to text file
    /// </summary>
    private void SaveInvoiceToFile()
    {
        try
        {
            string fileName = $"Invoice_{DateTime.Now:yyyy-MM-dd_HH-mm-ss}.txt";
            string filePath = fileName;

            using (System.IO.StreamWriter writer = new System.IO.StreamWriter(filePath))
            {
                writer.WriteLine(SEPARATOR);
                writer.WriteLine("                 FINAL INVOICE");
                writer.WriteLine($"Date and Time: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
                writer.WriteLine(SEPARATOR);
                writer.WriteLine();

                // Product details
                writer.WriteLine("Product Details:");
                writer.WriteLine("========================================================");
                writer.WriteLine(
                    String.Format("{0,-20} {1,10} {2,10} {3,10} {4,10}",
                    "Product", "Price", "Quantity", "Discount%", "Total")
                );
                writer.WriteLine("========================================================");

                foreach (var product in products)
                {
                    writer.WriteLine(
                        String.Format("{0,-20} {1,10:F2} {2,10} {3,10:F2} {4,10:F2}",
                        product.Name,
                        product.Price,
                        product.Quantity,
                        product.DiscountPercentage,
                        product.Total)
                    );
                }

                writer.WriteLine("========================================================");
                writer.WriteLine();
                writer.WriteLine("Calculations:");
                writer.WriteLine("========================================================");
                writer.WriteLine($"Subtotal:              {subtotal,15:F2}");
                writer.WriteLine($"Total Discount:        {discountAmount,15:F2}");
                writer.WriteLine($"Taxes (15%):           {taxAmount,15:F2}");
                writer.WriteLine($"Delivery Method:       {GetDeliveryMethodName(),15}");
                writer.WriteLine($"Shipping Fee:          {shippingFee,15:F2}");
                writer.WriteLine("========================================================");
                writer.WriteLine($"FINAL TOTAL:           {finalTotal,15:F2}");
                writer.WriteLine(SEPARATOR);
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"SUCCESS: Invoice saved to file: {fileName}");
            Console.ResetColor();
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"ERROR: Failed to save invoice: {ex.Message}");
            Console.ResetColor();
        }
    }
}