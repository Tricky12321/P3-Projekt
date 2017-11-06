using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using P3_Projekt_WPF.Classes.Database;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;
namespace P3_Projekt_WPF.Classes.Utilities
{
    public class StorageController
    {
        public Dictionary<int, Product> ProductDictionary = new Dictionary<int, Product>();
        public Dictionary<int, Group> GroupDictionary = new Dictionary<int, Group>();
        public Dictionary<int, StorageRoom> StorageRoomDictionary = new Dictionary<int, StorageRoom>();
        public Dictionary<int, SaleTransaction> SaleTransactionsDictionary = new Dictionary<int, SaleTransaction>();
        public Dictionary<int, Receipt> ReceiptDictionary = new Dictionary<int, Receipt>();
        public List<TempProduct> TempProductList = new List<TempProduct>();
        public List<Thread> Threads = new List<Thread>();
        public ConcurrentBag<Thread> ProductThreads = new ConcurrentBag<Thread>();
        public object ThreadLock = new object();

        public bool ThreadDone()
        {
            lock (ThreadLock)
            {
                foreach (var item in Threads)
                {
                    if (item.IsAlive)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public StorageController()
        {
            //GetAllProductsFromDatabase();
            //GetAllReceiptsFromDatabase();
        }

        private void CreateProduct_Thread(object rowData)
        {
            Row Data = (rowData as Row);
            Product NewProduct = new Product(Data);
            if (ProductDictionary.ContainsKey(NewProduct.ID) == false)
            {
                ProductDictionary.Add(NewProduct.ID, NewProduct);
            }
        }

        private void CreateGroups_Thread(object row_data)
        {
            Row Data = (row_data as Row);
            Group NewGroup = new Group(Data);
            GroupDictionary.Add(NewGroup.ID, NewGroup);
        }

        private void CreateStorageRoom_Thread(object row_data)
        {
            Row Data = (row_data as Row);
            StorageRoom NewStorageRoom = new StorageRoom(Data);
            StorageRoomDictionary.Add(NewStorageRoom.ID, NewStorageRoom);
        }

        private void CreateTempProduct_Thread(object row_data)
        {
            Row Data = (row_data as Row);
            TempProduct NewTempProduct = new TempProduct(Data);
            lock (TempProductList)
            {
                TempProductList.Add(NewTempProduct);
            }

        }

        private void CreateReceipt_Thread(object row_data)
        {
            Row Data = (row_data as Row);
            Receipt NewReceipt = new Receipt(Data);
            ReceiptDictionary.Add(NewReceipt.ID, NewReceipt);
            foreach (var saleTransaction in NewReceipt.Transactions)
            {
                if (saleTransaction is SaleTransaction)
                {
                    SaleTransactionsDictionary.Add(saleTransaction.GetID(), saleTransaction);
                }
            }
        }

        public void GetAllProductsFromDatabase()
        {
            string sql = "SELECT * FROM `products`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            foreach (var row in Results.RowData)
            {
                Thread NewThread = new Thread(new ParameterizedThreadStart(CreateProduct_Thread));
                lock (ThreadLock)
                {
                    Threads.Add(NewThread);
                }
                NewThread.Start(row);
            }
        }

        public void GetAllGroupsFromDatabase()
        {
            string sql = "SELECT * FROM `groups`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            foreach (var row in Results.RowData)
            {
                CreateGroups_Thread(row);
            }
        }

        public void GetAllStorageRooms()
        {
            string sql = "SELECT * FROM `storagerooms`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            foreach (var row in Results.RowData)
            {
                CreateStorageRoom_Thread(row);
                /*
                Thread NewThread = new Thread(new ParameterizedThreadStart(CreateStorageRoom_Thread));
                lock (ThreadLock)
                {
                    Threads.Add(NewThread);
                }
                NewThread.Start(row);
                */
            }
        }

        public void GetAllTempProductsFromDatabase()
        {
            string sql = "SELECT * FROM `temp_products`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            foreach (var row in Results.RowData)
            {
                CreateTempProduct_Thread(row);
                /*
                Thread NewThread = new Thread(new ParameterizedThreadStart(CreateTempProduct_Thread));
                lock (ThreadLock)
                {
                    Threads.Add(NewThread);
                }
                NewThread.Start(row);
                */
            }
        }

        public void GetAllReceiptsFromDatabase()
        {
            string sql = "SELECT * FROM `receipt`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            foreach (var row in Results.RowData)
            {
                CreateReceipt_Thread(row);
                /*
                Thread NewThread = new Thread(new ParameterizedThreadStart(CreateReceipt_Thread));
                lock (ThreadLock)
                {
                    Threads.Add(NewThread);
                }
                NewThread.Start(row);
                */
            }
        }

        public void GetAll()
        {
            // Multithreading the different mysql calls, so it goes much faster
            Thread GetAllProductsThread = new Thread(new ThreadStart(GetAllProductsFromDatabase));
            Thread GetAllGroupsThread = new Thread(new ThreadStart(GetAllGroupsFromDatabase));
            //Thread GetAllReceiptsThread = new Thread(new ThreadStart(GetAllReceiptsFromDatabase));
            Thread GetAllTempProductsThread = new Thread(new ThreadStart(GetAllProductsFromDatabase));
            lock (ThreadLock)
            {
                Threads.Add(GetAllProductsThread);
                Threads.Add(GetAllGroupsThread);
                //Threads.Add(GetAllReceiptsThread);
                Threads.Add(GetAllTempProductsThread);
            }
            GetAllProductsThread.Start();
            GetAllGroupsThread.Start();
            //GetAllReceiptsThread.Start();
            GetAllTempProductsThread.Start();
        }

        public void DeleteProduct(int ProductID)
        {
            ProductDictionary.Remove(ProductID);
        }

        public void CreateGroup(string name, string description)
        {
            Group newGroup = new Group(name, description);
            GroupDictionary.Add(newGroup.ID, newGroup);
        }

        public void EditGroup(int id, string name, string description)
        {
            GroupDictionary[id].Name = name;
            GroupDictionary[id].Description = description;
        }

        //Assign group 0 to products left with no group
        //Removes group from dictionary
        public void DeleteGroup(int GroupID)
        {
            //Mulighed for at flytte alle produkter til en bestem gruppe???
            foreach (Product product in ProductDictionary.Values.Where(x => x.ProductGroup == GroupDictionary[GroupID]))
            {
                product.ProductGroup = GroupDictionary[0];
            }
            GroupDictionary.Remove(GroupID);
        }

        //Assign new group to products left with no group
        //Removes group from dictionary
        public void DeleteGroupAndMove(int removeID, int moveID)
        {
            foreach (Product product in ProductDictionary.Values.Where(x => x.ProductGroup == GroupDictionary[removeID]))
            {
                product.ProductGroup = GroupDictionary[moveID];
            }
            GroupDictionary.Remove(removeID);
        }

        /////////--------------------SEARCH---------------------------------
        public List<Product> SearchForProduct(string searchedString)
        {
            bool wordIsMatched = false;
            List<string> produtNames = new List<string>();
            List<Product> productsToReturn = new List<Product>();

            int isNumber;
            if (Int32.TryParse(searchedString, out isNumber))
            {
                productsToReturn.Add(ProductDictionary[isNumber]);
                return productsToReturn;
            }
            else
            {
                //checks if the searched string is
                //matching with a product name.
                foreach (Product p in ProductDictionary.Values)
                {
                    if (p.Name == searchedString)
                    {
                        productsToReturn.Add(p);
                    }
                    produtNames.Add(p.Name);
                }

                //if af matching name is not found the string will undergo different searching methods.
                if (!wordIsMatched)
                {
                    foreach (Product p in ProductDictionary.Values)
                    {
                        //levenshteins will try to autocorrect the string and suggest items with similar names to the string
                        LevenshteinsProductSearch(searchedString, p, ref productsToReturn);
                    }
                    //will add all the matching brands to the productlist
                    BrandSearch(searchedString, ref productsToReturn);
                    //will add all th matching groups to the produclist
                    GroupSearch(searchedString, ref productsToReturn);

                    //removes duplicates from the list. 
                    productsToReturn = productsToReturn.Distinct().ToList();
                    return productsToReturn;
                }
                else
                {
                    //will be called if a matching word is found
                    return productsToReturn;
                }
            }
        }

        //----Levensthein---------------------
        public void LevenshteinsProductSearch(string searchedString, Product productCheck, ref List<Product> productsToReturn)//tested
        {//setup for levenshteins
            //getting the chardifference between the searchedstring and the productname
            int charDifference = ComputeLevenshteinsDistance(searchedString, productCheck.Name);
            //Evaluate if the chardifference is in between the changelimit of the string
            if (EvaluateStringLimit(searchedString, charDifference))
            {
                if (!productsToReturn.Contains(productCheck))
                {
                    productsToReturn.Add(productCheck);
                }

            }
        }

        public bool LevenshteinsGroupSearch(string[] searchedString, Group groupCheck)//tested
        {//setup for levenshteins
            foreach (string s in searchedString)
            {
                //getting the chardifference between the searchedstring and the productname
                int charDifference = ComputeLevenshteinsDistance(s, groupCheck.Name);
                //Evaluate if the chardifference is in between the changelimit of the string
                if (EvaluateStringLimit(s, charDifference))
                {
                    //only returns true if it is matching
                    return true;
                }
            }

            return false;

        }

        public bool LevenshteinsBrandSearch(string[] searchedString, string productBrandName)//tested
        {//setup for levenshteins
            foreach (string s in searchedString)
            {
                //getting the chardifference between the searchedstring and the productname
                int charDifference = ComputeLevenshteinsDistance(s, productBrandName);
                //Evaluate if the chardifference is in between the changelimit of the string
                if (EvaluateStringLimit(s, charDifference))
                {
                    //only returns true of it is matching
                    return true;
                }
            }
            return false;
        }

        public int ComputeLevenshteinsDistance(string searchedString, string productToCompare)//tested
        {
            //searchString Length
            int searchStringLength = searchedString.Length;
            //productname Length       
            int productNameLength = productToCompare.Length;
            int cost;
            int minimum1, minimum2, minimum3;
            //size of int array
            int[,] d = new int[searchStringLength + 1, productNameLength + 1];
            //--------------
            //Checks if the strings are empty
            if (string.IsNullOrEmpty(searchedString))
            {
                if (!string.IsNullOrEmpty(productToCompare))
                {
                    return searchStringLength;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(productToCompare))
            {
                if (!string.IsNullOrEmpty(searchedString))
                {
                    return productNameLength;
                }
                return 0;
            }
            //-------------
            //GetUpperBound gets the index of the last element in the array.
            for (int i = 0; i <= d.GetUpperBound(0); ++i)
            {
                d[i, 0] = i;
            }
            //(0) and (1) is to differentiate between the first and second element in the array.
            for (int i = 0; i <= d.GetUpperBound(1); ++i)
            {
                d[0, i] = i;
            }

            for (int i = 1; i <= d.GetUpperBound(0); ++i)
            {
                for (int j = 1; j <= d.GetUpperBound(1); ++j)
                {
                    //will convert a boolean to int, depending if a char is different different between the two strings.
                    cost = Convert.ToInt32(!(searchedString[i - 1] == productToCompare[j - 1]));


                    minimum1 = d[i - 1, j] + 1;          //takes the element in the previous row i
                    minimum2 = d[i, j - 1] + 1;          //takes the element in the previous column j
                    minimum3 = d[i - 1, j - 1] + cost;   //takes the element in the previous column j and previos row i, and adds the cost of changing a char, +1 og or +0
                    //the minmum of the 3 will be put into the 2-dimensial array at row i column j
                    d[i, j] = Math.Min(Math.Min(minimum1, minimum2), minimum3);

                    //for a array example, see step 1-7 https://people.cs.pitt.edu/~kirk/cs1501/Pruhs/Spring2006/assignments/editdistance/Levenshtein%20Distance.htm
                }
            }
            //returns the value of the last coloumn and last row of the array, which is the amount that is needed to change between the words.
            return d[d.GetUpperBound(0), d.GetUpperBound(1)];


        }

        public bool EvaluateStringLimit(string searchedString, int charDiff)//tested
        {
            int limitOfChanges;
            int searchedStringLength = searchedString.Length;

            // Determines how many changes is allowed, depending on how long the string is
            if (searchedStringLength < 5)
                limitOfChanges = 1;
            else if (searchedStringLength < 10 && searchedStringLength >= 5)
                limitOfChanges = 3;
            else if (searchedStringLength >= 10 && searchedStringLength < 20)
                limitOfChanges = 6;
            else
                limitOfChanges = 9;

            return limitOfChanges >= charDiff;
        }
        //----LevenSthein-END-----------------------

        public void GroupSearch(string searchedString, ref List<Product> productListToReturn)//tested
        {
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchedString.Split(' ');
            //checking all groups to to match with the searched string elements
            foreach (Group g in GroupDictionary.Values)
            {
                //matching on each element in the string
                bool groupMatched = LevenshteinsGroupSearch(dividedString, g);
                //if the string contains a name of a group, or the string is matched, each product with the same group 
                //is added to the list of products to show.
                if (dividedString.Contains(g.Name) || groupMatched)
                {
                    foreach (Product p in ProductDictionary.Values.Where(x => x.ProductGroup == g))
                    {
                        productListToReturn.Add(p);
                    }
                }
            }
        }

        public void BrandSearch(string searchedString, ref List<Product> productListToReturn)//tested
        {
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchedString.Split(' ');
            //checking all products to to match the brands with the searched string elements
            foreach (Product p in ProductDictionary.Values)
            {
                //matching on each element in the string
                bool brandMatched = LevenshteinsBrandSearch(dividedString, p.Brand);
                //if the string contains a product brand, or the string is matched, each product with the same brand
                //is added to the list of products to show.
                if (dividedString.Contains(p.Brand) || brandMatched)
                {
                    if (!productListToReturn.Contains(p))
                    {
                        productListToReturn.Add(p);
                    }

                }
            }
        }
        //----SEARCH-END---------------------

        //Creates product with storage and stocka as keyvalue, then add the product to the list
        public void CreateProduct(string name, string brand, decimal purchasePrice, Group group, bool discount, decimal discountPrice, decimal salePrice, Image image, params KeyValuePair<int, int>[] storageRoomStockInput)
        {
            Product newProduct = new Product(name, brand, purchasePrice, group, discount, salePrice, discountPrice, image);

            foreach (KeyValuePair<int, int> roomInput in storageRoomStockInput)
            {
                newProduct.StorageWithAmount[roomInput.Key] = roomInput.Value;
            }

            ProductDictionary.Add(newProduct.ID, newProduct);
        }

        //edit product, calles two different methods depending if its run by an admin
        public void EditProduct(bool isAdmin, Product editProduct, string name, string brand, decimal purchasePrice, Group group, bool discount, decimal salePrice, decimal discountPrice, Image image)
        {
            if (isAdmin)
            {
                editProduct.AdminEdit(name, brand, purchasePrice, salePrice, group, discount, discountPrice, image);
            }
            else
            {
                editProduct.Edit(name, brand, group, image);
            }
        }

        public void CreateTempProduct(string description, decimal salePrice)
        {
            TempProduct newTempProduct = new TempProduct(description, salePrice);
            TempProductList.Add(newTempProduct);
        }

        /* User has already found the matching product ID.
         * First line findes the store storage
         * Second line subtracts the amound sold from storage*/
        public void MergeTempProduct(TempProduct tempProductToMerge, int matchedProductID)
        {
            SaleTransaction tempProductsTransaction = tempProductToMerge.GetTempProductsSaleTransaction();

            ProductDictionary[matchedProductID].StorageWithAmount[0] -= tempProductsTransaction.Amount;
            tempProductsTransaction.EditSaleTransactionFromTempProduct(ProductDictionary[matchedProductID]);
            tempProductToMerge.Resolve();
            TempProductList.Remove(tempProductToMerge);
        }

        public void EditTempProduct(TempProduct tempProductToEdit, string description, decimal salePrice)
        {
            tempProductToEdit.Edit(description, salePrice);
        }

        //Adds new storage room to dictionary, and to all products
        public void CreateStorageRoom(string name, string description)
        {
            StorageRoom newRoom = new StorageRoom(name, description);
            StorageRoomDictionary.Add(newRoom.ID, newRoom);

            foreach (Product product in ProductDictionary.Values)
            {
                product.StorageWithAmount.Add(newRoom.ID, 0);
            }
        }

        public void EditStorageRoom(int id, string name, string description)
        {
            StorageRoomDictionary[id].Name = name;
            StorageRoomDictionary[id].Description = description;
        }

        //Removes storage room from dictionary, and all products
        public void DeleteStorageRoom(int id)
        {
            foreach (Product product in ProductDictionary.Values)
            {
                product.StorageWithAmount.Remove(id);
                product.UpdateInDatabase();
            }
            StorageRoomDictionary.Remove(id);
            string deleteQuery = $"DELETE FROM `storagerooms` WHERE `id` = '{id}'";
            Mysql.RunQuery(deleteQuery);
        }
    }
}
