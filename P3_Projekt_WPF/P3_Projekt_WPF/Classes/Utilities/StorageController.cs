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
using P3_Projekt_WPF.Classes.Exceptions;
namespace P3_Projekt_WPF.Classes.Utilities
{
    public class StorageController
    {
        public ConcurrentDictionary<int, Product> ProductDictionary = new ConcurrentDictionary<int, Product>();
        public ConcurrentDictionary<int, ServiceProduct> ServiceProductDictionary = new ConcurrentDictionary<int, ServiceProduct>();
        public ConcurrentDictionary<int, Group> GroupDictionary = new ConcurrentDictionary<int, Group>();
        public ConcurrentDictionary<int, StorageRoom> StorageRoomDictionary = new ConcurrentDictionary<int, StorageRoom>();
        public ConcurrentDictionary<int, SaleTransaction> SaleTransactionsDictionary = new ConcurrentDictionary<int, SaleTransaction>();
        public ConcurrentDictionary<int, Receipt> ReceiptDictionary = new ConcurrentDictionary<int, Receipt>();
        public List<TempProduct> TempProductList = new List<TempProduct>();
        public List<string[]> InformationGridData = new List<string[]>();
        private object InformationGridLock = new object();
        private ConcurrentQueue<Product> _productResults = new ConcurrentQueue<Product>();
        public StorageController()
        {
            //GetAllProductsFromDatabase();
            //GetAllReceiptsFromDatabase();
        }

        #region Multithreading

        public List<Thread> Threads = new List<Thread>();
        public object ThreadLock = new object();
        private int _productThreadsCount = 4;
        private ConcurrentQueue<Row> _productInformation = new ConcurrentQueue<Row>();
        // For at holde garbage collector fra at dræbe tråde
        private List<Thread> _productThreads = new List<Thread>();
        // Til at tjekke om de forskellige tråde er færdige med at hente data.
        private int _productsLoadedFromDatabase = 0;
        private int _productsCreateByThreads = -1;
        private bool _productQueueDone = false;
        private bool _tempProductQueueDone = false;
        private bool _groupQueueDone = false;
        private bool _storageRoomQueueDone = false;
        private bool _serviceProductQueueDone = false;
        private bool _stoargeStatusQueueDone = false;
        private int _storageStatusThreadCount = 4;
        private List<Thread> _storageStatusThreads = new List<Thread>();
        private ConcurrentQueue<Row> _storageStatusInformation = new ConcurrentQueue<Row>();
        public bool ThreadsDone
        {
            get
            {
                CheckThreads();
                return _productQueueDone && _groupQueueDone && _tempProductQueueDone && _storageRoomQueueDone && _serviceProductQueueDone && _stoargeStatusQueueDone;
            }
        }

        public void AddInformation(string name, object value)
        {
            lock (InformationGridLock)
            {
                string[] result = { name, value.ToString() };
                InformationGridData.Add(result);
            }
        }

        /// <summary>
        /// Checks if the ProductQueue and StorageStatus is done
        /// </summary>
        private void CheckThreads()
        {
            if (!_productQueueDone && (_productsCreateByThreads == _productsLoadedFromDatabase))
            {
                Debug.WriteLine("ProductQue: Done");
                _productQueueDone = true;
                UpdateStorageStatus();
                AddInformation("Product count", _productsCreateByThreads);
            } else
            {
                if (_storageStatusInformation.IsEmpty)
                {
                    _stoargeStatusQueueDone = true;
                }
            }
        }

        // Opretter Product tråde
        private void CreateProductThreads()
        {
            for (int i = 0; i < _productThreadsCount; i++)
            {
                Thread NewThread = new Thread(new ThreadStart(HandleCreateProductQue));
                NewThread.Name = "Create Product Thread";
                NewThread.Start();
                _productThreads.Add(NewThread);
            }
            Debug.WriteLine("Created all product threads [" + _productThreads.Count + "]");
        }

        // Når trådene skal få opgaver
        private void HandleCreateProductQue()
        {
            while (!_productQueueDone)
            {
                Row Information = null;
                if (_productInformation.TryDequeue(out Information) == true)
                {
                    CreateProduct_Thread(Information);
                    Interlocked.Increment(ref _productsCreateByThreads);
                }
                else
                {
                    Thread.Sleep(1);
                }
            }
        }

        private void UpdateStorageStatus_Thread()
        {
            Row Data = null;
            while (_storageStatusInformation.TryDequeue(out Data))
            {
                int productID = Convert.ToInt32(Data.Values[1]);
                int storageRoom = Convert.ToInt32(Data.Values[2]);
                int amount = Convert.ToInt32(Data.Values[3]);
                ProductDictionary[productID].StorageWithAmount[storageRoom] = amount;

            }
        }


        private void UpdateStorageStatus()
        {
            string sql = "SELECT * FROM `storage_status`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            _storageStatusInformation = new ConcurrentQueue<Row>(Results.RowData);
            CreateStorageStatusThreads();
        }

        private void CreateStorageStatusThreads()
        {
            for (int i = 0; i < _storageStatusThreadCount; i++)
            {
                Thread NewThread = new Thread(new ThreadStart(UpdateStorageStatus_Thread));
                NewThread.Start();
                _storageStatusThreads.Add(NewThread);
            }
        }

        // Når tråde skal oprette objecter
        private void CreateProduct_Thread(object rowData)
        {
            Row Data = (rowData as Row);
            Product NewProduct = new Product(Data);
            if (ProductDictionary.ContainsKey(NewProduct.ID) == false)
            {
                ProductDictionary.TryAdd(NewProduct.ID, NewProduct);
            }
        }

        private void CreateGroups_Thread(object row_data)
        {
            Row Data = (row_data as Row);
            Group NewGroup = new Group(Data);
            GroupDictionary.TryAdd(NewGroup.ID, NewGroup);
        }

        private void CreateStorageRoom_Thread(object row_data)
        {
            Row Data = (row_data as Row);
            StorageRoom NewStorageRoom = new StorageRoom(Data);
            StorageRoomDictionary.TryAdd(NewStorageRoom.ID, NewStorageRoom);
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
            ReceiptDictionary.TryAdd(NewReceipt.ID, NewReceipt);
            foreach (var saleTransaction in NewReceipt.Transactions)
            {
                if (saleTransaction is SaleTransaction)
                {
                    SaleTransactionsDictionary.TryAdd(saleTransaction.GetID(), saleTransaction);
                }
            }
        }

        private void CreateServiceProduct_Thread(object row_data)
        {
            Row Data = (row_data as Row);
            ServiceProduct NewServiceProduct = new ServiceProduct(Data);
            ServiceProductDictionary.TryAdd(NewServiceProduct.ID, NewServiceProduct);
        }

        public void GetAllProductsFromDatabase()
        {
            string sql = "SELECT * FROM `products` WHERE `id` > '0'";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            _productsLoadedFromDatabase = Results.RowData.Count;
            _productsCreateByThreads = 0;
            CreateProductThreads(); // Opretter tråde til at behandle data
            foreach (var row in Results.RowData)
            {
                _productInformation.Enqueue(row);
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
            Debug.WriteLine("GroupQue: Done!");
            _groupQueueDone = true;
            AddInformation("Groups count", GroupDictionary.Count);

        }

        public void GetAllStorageRoomsFromDatabase()
        {
            string sql = "SELECT * FROM `storagerooms`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            foreach (var row in Results.RowData)
            {
                CreateStorageRoom_Thread(row);
            }
            Debug.WriteLine("StorageRoomQue: Done!");
            _storageRoomQueueDone = true;
            AddInformation("Storageroom count", StorageRoomDictionary.Count);
        }

        public void GetAllTempProductsFromDatabase()
        {
            string sql = "SELECT * FROM `temp_products`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            foreach (var row in Results.RowData)
            {
                CreateTempProduct_Thread(row);
            }
            Debug.WriteLine("TempProductQue: Done!");
            _tempProductQueueDone = true;
            AddInformation("TempProduct count", TempProductList.Count);
        }

        public void GetAllServiceProductsFromDatabase()
        {
            string sql = "SELECT * FROM `service_products`";
            try
            {
                TableDecode Results = Mysql.RunQueryWithReturn(sql);
                foreach (var row in Results.RowData)
                {
                    CreateServiceProduct_Thread(row);
                }
            }
            catch (EmptyTableException)
            {

            }
            Debug.WriteLine("ServiceProductQueue: Done!");
            _serviceProductQueueDone = true;
            AddInformation("ServiceProduct count", ServiceProductDictionary.Count);
        }

        public void GetAllReceiptsFromDatabase()
        {
            string sql = "SELECT * FROM `receipt`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            foreach (var row in Results.RowData)
            {
                CreateReceipt_Thread(row);
            }
        }

        public void GetAll()
        {
            _productsLoadedFromDatabase = 0;
            _productsCreateByThreads = -1;
            _productQueueDone = false;
            _tempProductQueueDone = false;
            _groupQueueDone = false;
            _storageRoomQueueDone = false;
            _serviceProductQueueDone = false;
            // Multithreading the different mysql calls, so it goes much faster
            Thread GetAllProductsThread = new Thread(new ThreadStart(GetAllProductsFromDatabase));
            Thread GetAllGroupsThread = new Thread(new ThreadStart(GetAllGroupsFromDatabase));
            Thread GetAllTempProductsThread = new Thread(new ThreadStart(GetAllTempProductsFromDatabase));
            Thread GetAllStorageRoomsThread = new Thread(new ThreadStart(GetAllStorageRoomsFromDatabase));
            Thread GetAllServiceProductsThread = new Thread(new ThreadStart(GetAllServiceProductsFromDatabase));
            GetAllProductsThread.Name = "GetAllProductsThread";
            GetAllGroupsThread.Name = "GetAllGroupsThread";
            GetAllTempProductsThread.Name = "GetAllTempProductsThread";
            GetAllStorageRoomsThread.Name = "GetAllStorageRoomsThread";
            GetAllServiceProductsThread.Name = "GetAllServiceProductsThread";
            lock (ThreadLock)
            {
                Threads.Add(GetAllProductsThread);
                Threads.Add(GetAllGroupsThread);
                Threads.Add(GetAllTempProductsThread);
                Threads.Add(GetAllStorageRoomsThread);
                Threads.Add(GetAllServiceProductsThread);
            }
            GetAllProductsThread.Start();
            GetAllGroupsThread.Start();
            GetAllStorageRoomsThread.Start();
            GetAllTempProductsThread.Start();
            GetAllServiceProductsThread.Start();
        }

        #endregion

        public void DeleteProduct(int ProductID)
        {
            Product outVal = null;
            ProductDictionary.TryRemove(ProductID, out outVal);
        }

        public void CreateGroup(int id, string name, string description)
        {
            Group newGroup = new Group(name, description);
            newGroup.ID = id;
            GroupDictionary.TryAdd(newGroup.ID, newGroup);
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
            foreach (Product product in ProductDictionary.Values.Where(x => x.ProductGroupID == GroupID))
            {
                product.ProductGroupID = GroupDictionary[0].ID;
            }
            Group outVal = null;
            GroupDictionary.TryRemove(GroupID, out outVal);
        }

        //Assign new group to products left with no group
        //Removes group from dictionary
        public void DeleteGroupAndMove(int removeID, int moveID)
        {
            foreach (Product product in ProductDictionary.Values.Where(x => x.ProductGroupID == removeID))
            {
                product.ProductGroupID = GroupDictionary[moveID].ID;
            }
            Group outVal = null;
            GroupDictionary.TryRemove(removeID, out outVal);
        }

        public IEnumerable<string> GetProductBrands()
        {
            return ProductDictionary.Values.Select(x => x.Brand).Distinct();
        }
        /*
        // Levenstein multithreading
        private bool _productSearchDone = false;
        private bool _groupSearchDone = false;
        private bool _brandSearchDone = false;
        private int _productThreadCount = 4;
        private ConcurrentQueue<Product> _productsToSearch = null;
        //private ConcurrentQueue<SearchedProduct> _productsFound = null;
        //private ConcurrentQueue<SearchedProduct> _productsToSort = new ConcurrentQueue<SearchedProduct>();
        private List<Thread> _productSearchThreads = new List<Thread>();
        private string _searchedString = "";

        private void LevenshteinSearch_Thread()
        {
            while (!_productSearchDone)
            {
                Product p = null;
                if (_productsToSearch.TryDequeue(out p) && p != null)
                {
                    LevenshteinsProductSearch(_searchedString, p);
                }
            }
        }

        private void StartLevenshteinSearchThreads()
        {
            for (int i = 0; i < _productThreadCount; i++)
            {
                Thread NewThread = new Thread(new ThreadStart(LevenshteinSearch_Thread));
                NewThread.Name = "LevenshteinSearchThread";
                _productSearchThreads.Add(NewThread);
                NewThread.Start();
            }
        }

        private void BrandGroupSearchThreaded()
        {
            _brandSearchDone = false;
            _groupSearchDone = false;
            Thread BrandSearchThread = new Thread(new ParameterizedThreadStart(BrandSearch));
            BrandSearchThread.Name = "BrandSearchThread";
            Thread GroupSearchThread = new Thread(new ParameterizedThreadStart(GroupSearch));
            GroupSearchThread.Name = "GroupSearchThread";

            BrandSearchThread.Start(_searchedString);
            GroupSearchThread.Start(_searchedString);
            while (!_brandSearchDone && !_groupSearchDone)
            {
                Thread.Sleep(1);
            }
        }

        private void ProductSearchThreaded()
        {
            StartLevenshteinSearchThreads();
            _productSearchDone = false;
            while (_productsToSearch.IsEmpty == false)
            {
                Thread.Sleep(1);
            }
            _productSearchDone = true;
        }

        private void SearchProductsBrandsAndGroups()
        {
            ProductSearchThreaded();
            BrandGroupSearchThreaded();
        }

        /////////--------------------SEARCH---------------------------------
        public ConcurrentQueue<SearchedProduct> SearchForProduct(string searchedString)
        {
            ConcurrentQueue<SearchedProduct> productsToReturn = new ConcurrentQueue<SearchedProduct>();
            IEnumerable<SearchedProduct> returnableProducts;
            int isNumber;
            //returns 1 product if it is a matching ID number
            if (Int32.TryParse(searchedString, out isNumber))
            {
                SearchedProduct NewSearch = new SearchedProduct(ProductDictionary[isNumber]);
                productsToReturn.Enqueue(NewSearch);
                return productsToReturn;
            }
            else
            {
                foreach (var p in ProductDictionary.Where(x => x.Value.Name == searchedString))
                {
                    productsToReturn.Enqueue(new SearchedProduct(p.Value));
                }
                _productsToSearch = new ConcurrentQueue<Product>(ProductDictionary.Values);
                _productsFound = productsToReturn;
                _productsToSort = new ConcurrentQueue<SearchedProduct>();
                // Starter multithreading 
                _searchedString = searchedString;
                StartLevenshteinSearchThreads();
                _productSearchDone = false;
                while (_productsToSearch.IsEmpty == false)
                {
                    Thread.Sleep(1);
                }
                _productSearchDone = true;
                

               

                //will add all the matching brands to the productlist
                _brandSearchDone = false;
                _groupSearchDone = false;
                Thread BrandSearchThread = new Thread(new ParameterizedThreadStart(BrandSearch));
                BrandSearchThread.Name = "BrandSearchThread";
                Thread GroupSearchThread = new Thread(new ParameterizedThreadStart(GroupSearch));
                GroupSearchThread.Name = "GroupSearchThread";
                BrandSearchThread.Start(searchedString);
                GroupSearchThread.Start(searchedString);
                while (!_brandSearchDone && !_groupSearchDone)
                {
                    Thread.Sleep(1);
                }
                
                SearchProductsBrandsAndGroups();
                //productsToReturn = ok as ConcurrentQueue<Product>;
                return productsToReturn;
            }
        }

        //----Levensthein---------------------
        /*public void LevenshteinsProductSearch(string searchedString, Product productCheck, ref ConcurrentQueue<Product> productsToReturn)//tested
        {//setup for levenshteins
            //getting the chardifference between the searchedstring and the productname
            int charDifference = ComputeLevenshteinsDistance(searchedString, productCheck.Name);
            //Evaluate if the chardifference is in between the changelimit of the string
            if (EvaluateStringLimit(searchedString, charDifference))
            {
                if (!productsToReturn.Contains(productCheck))
                {
                    productsToReturn.Enqueue(productCheck);
                }

            }
        }

        public void LevenshteinsProductSearch(string searchStringElement, Product productToConvert)
        {
            ConcurrentQueue<SearchedProduct> sortQueue = _productsToSort;
            SearchedProduct productToAdd = new SearchedProduct(productToConvert);
            string[] searchSplit = searchStringElement.Split(' ');
            string[] productSplit = productToConvert.Name.Split(' ');

            foreach(string s in searchSplit)
            {
                foreach(string t in productSplit)
                {
                    if(LevenstheinProductSearch(s, t))
                    {
                        productToAdd.NameMatch += 1;
                    }
                }
            }
            sortQueue.Enqueue(productToAdd);

        }


        public void GroupSearch(object searchString)//tested
        {
            string searchedString = searchString as string;
            ConcurrentQueue<SearchedProduct> productListToReturn = _productsFound;
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchedString.Split(' ');
            //checking all groups to to match with the searched string elements
            foreach (Group g in GroupDictionary.Values)
            {
                //matching on each element in the string
                int MatchedValue;
                bool groupMatched = LevenshteinsGroupSearch(dividedString, g, out MatchedValue);
                //if the string contains a name of a group, or the string is matched, each product with the same group 
                //is added to the list of products to show.
                if (dividedString.Contains(g.Name) || groupMatched)
                {
                    foreach (Product p in ProductDictionary.Values.Where(x => x.ProductGroupID == g.ID))
                    {
                        if (productListToReturn.Where(x => x.CurrentProduct.ID == p.ID).Count() > 0)
                        {
                            if (productListToReturn.Where(x => x.CurrentProduct.ID == p.ID).First() != null)
                            {
                                SearchedProduct NewProduct = new SearchedProduct(p);
                                NewProduct.SetGroupMatch(MatchedValue);
                                productListToReturn.Enqueue(NewProduct);
                            }
                        }
                    }
                }
            }
            _groupSearchDone = true;
        }

        public void BrandSearch(object searchString)//tested
        {
            string searchedString = searchString as string;
            ConcurrentQueue<SearchedProduct> productListToReturn = _productsFound;
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchedString.Split(' ');
            //checking all products to to match the brands with the searched string elements
            foreach (Product p in ProductDictionary.Values)
            {
                int MatchedValues;
                //matching on each element in the string
                bool brandMatched = LevenshteinsBrandSearch(dividedString, p.Brand, out MatchedValues);
                //if the string contains a product brand, or the string is matched, each product with the same brand
                //is added to the list of products to show.
                if (dividedString.Contains(p.Brand) || brandMatched)
                {
                    if (productListToReturn.Where(x => x.CurrentProduct.ID == p.ID).First() == null) ;
                    {
                        SearchedProduct NewProduct = new SearchedProduct(p);
                        NewProduct.SetBrandMatch(MatchedValues);
                        productListToReturn.Enqueue(NewProduct);
                    }

                }
            }
            _brandSearchDone = true;
        }

        public bool LevenstheinProductSearch(string searchEle, string ProductEle)
        {
            int charDifference = ComputeLevenshteinsDistance(searchEle, ProductEle);

            if(EvaluateStringLimit(searchEle, charDifference))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool LevenshteinsGroupSearch(string[] searchedString, Group groupCheck, out int charDifference)//tested
        {//setup for levenshteins
            foreach (string s in searchedString)
            {
                //getting the chardifference between the searchedstring and the productname
                charDifference = ComputeLevenshteinsDistance(s, groupCheck.Name);
                //Evaluate if the chardifference is in between the changelimit of the string
                if (EvaluateStringLimit(s, charDifference))
                {
                    //only returns true if it is matching
                    return true;
                }
            }
            charDifference = -1;
            return false;

        }

        public bool LevenshteinsBrandSearch(string[] searchedString, string productBrandName, out int charDifference)//tested
        {//setup for levenshteins
            foreach (string s in searchedString)
            {
                //getting the chardifference between the searchedstring and the productname
                charDifference = ComputeLevenshteinsDistance(s, productBrandName);
                //Evaluate if the chardifference is in between the changelimit of the string
                if (EvaluateStringLimit(s, charDifference))
                {
                    //only returns true of it is matching
                    return true;
                }
            }
            charDifference = -1;
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

        //----SEARCH-END---------------------

            */

        //Creates product with storage and stocka as keyvalue, then add the product to the list
        public void CreateProduct(int id, string name, string brand, decimal purchasePrice, int groupID, bool discount, decimal discountPrice, decimal salePrice, Dictionary<int,int> storageWithAmount, bool UploadToDatabase = true)
        {
            Product newProduct = new Product(id, name, brand, purchasePrice, groupID, discount, salePrice, discountPrice);
            newProduct.StorageWithAmount = storageWithAmount;
            if (!ProductDictionary.TryAdd(newProduct.ID, newProduct))
            {
                throw new Exception("This key already exists");
            }
            if (UploadToDatabase)
            {
                newProduct.UploadToDatabase();
            }
        }

        public void CreateServiceProduct(int id, decimal salePrice, decimal groupPrice, int groupLimit, string name, int serviceProductGroupID, bool UploadToDatabase = true)
        {
            ServiceProduct newServiceProduct = new ServiceProduct(id, salePrice, groupPrice, groupLimit, name, serviceProductGroupID);
            if (!ServiceProductDictionary.TryAdd(newServiceProduct.ID, newServiceProduct))
            {
                throw new Exception("This key already exists");
            }
            if (UploadToDatabase)
            {
                newServiceProduct.UploadToDatabase();
            }
        }

        //edit product, calles two different methods depending if its run by an admin
        public void EditProduct(bool isAdmin, Product editProduct, string name, string brand, decimal purchasePrice, int groupID, bool discount, decimal salePrice, decimal discountPrice, string imagePath)
        {
            if (isAdmin)
            {
                editProduct.AdminEdit(name, brand, purchasePrice, salePrice, groupID, discount, discountPrice);
            }
            else
            {
                editProduct.Edit(name, brand, groupID);
            }
        }

        public TempProduct CreateTempProduct(string description, decimal salePrice)
        {
            TempProduct newTempProduct = new TempProduct(description, salePrice);
            TempProductList.Add(newTempProduct);
            newTempProduct.UploadToDatabase();
            return newTempProduct;
        }

        /* User has already found the matching product ID.
         * First line findes the store storage
         * Second line subtracts the amound sold from storage*/
        public void MergeTempProduct(TempProduct tempProductToMerge, int matchedProductID)
        {
            SaleTransaction tempProductsTransaction = tempProductToMerge.GetTempProductsSaleTransaction();
            ProductDictionary[matchedProductID].StorageWithAmount[0] -= tempProductsTransaction.Amount;
            tempProductsTransaction.EditSaleTransactionFromTempProduct(ProductDictionary[matchedProductID]);
            Product MergedProduct = ProductDictionary[matchedProductID];
            tempProductToMerge.Resolve(MergedProduct);
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
            StorageRoomDictionary.TryAdd(newRoom.ID, newRoom);

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
            StorageRoom outVal = null;
            StorageRoomDictionary.TryRemove(id, out outVal);
            string deleteQuery = $"DELETE FROM `storagerooms` WHERE `id` = '{id}'";
            Mysql.RunQuery(deleteQuery);
        }
    }
}
