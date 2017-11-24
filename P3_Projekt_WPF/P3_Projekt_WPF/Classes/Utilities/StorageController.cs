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
        public ConcurrentDictionary<int, TempProduct> TempProductList = new ConcurrentDictionary<int, TempProduct>();
        public List<string[]> InformationGridData = new List<string[]>();
        private object InformationGridLock = new object();
        private ConcurrentQueue<Product> _productResults = new ConcurrentQueue<Product>();
        public ConcurrentDictionary<int, BaseProduct> AllProductsDictionary = new ConcurrentDictionary<int, BaseProduct>();

        public StorageController()
        {
            //GetAllProductsFromDatabase();
            //GetAllReceiptsFromDatabase();
        }


        public void LoadAllProductsDictionary()
        {
            AllProductsDictionary.Clear();
            foreach (KeyValuePair<int, Product> productWithID in ProductDictionary)
            {
                AllProductsDictionary.TryAdd(productWithID.Key, productWithID.Value as BaseProduct);
            }

            foreach (KeyValuePair<int, ServiceProduct> serviceProductWithID in ServiceProductDictionary)
            {
                AllProductsDictionary.TryAdd(serviceProductWithID.Key, serviceProductWithID.Value as BaseProduct);
            }
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
        private bool _productQueueLoaded = false;
        private int _storageStatusThreadCount = 4;
        private List<Thread> _storageStatusThreads = new List<Thread>();
        private ConcurrentQueue<Row> _storageStatusInformation = new ConcurrentQueue<Row>();
        public ConcurrentQueue<string> TimerStrings = new ConcurrentQueue<string>();
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
        private Stopwatch _storageStatusTimer = new Stopwatch();
        private Stopwatch _productStatusTimer = new Stopwatch();
        /// <summary>
        /// Checks if the ProductQueue and StorageStatus is done
        /// </summary>
        private void CheckThreads()
        {
            if (_productQueueLoaded && !_productQueueDone && _productInformation.IsEmpty)
            {
                //Debug.WriteLine("ProductQue: Done");
                _productQueueDone = true;
                _storageStatusTimer.Start();
                AddInformation("Product count", _productsCreateByThreads);
                _productStatusTimer.Stop();
                TimerStrings.Enqueue("[P3] Det tog " + _productStatusTimer.ElapsedMilliseconds + "ms at oprette produkter");
            }
            else if (!_stoargeStatusQueueDone && _productQueueDone && _storageStatusInformation.IsEmpty)
            {
                _stoargeStatusQueueDone = true;
                _storageStatusTimer.Stop();
                TimerStrings.Enqueue("[P3] Det tog " + _storageStatusTimer.ElapsedMilliseconds + "ms at oprette storage status");
            }
        }

        // Opretter Product tråde
        private void CreateProductThreads()
        {
            for (int i = 0; i < _productThreadsCount - 1; i++)
            {
                Thread NewThread = new Thread(new ThreadStart(HandleCreateProductQue));
                NewThread.Name = "Create Product Thread";
                NewThread.Start();
                _productThreads.Add(NewThread);
            }
            Debug.WriteLine("Created all product threads [" + _productThreadsCount + "]");
            HandleCreateProductQue();
        }

        // Når trådene skal få opgaver
        private void HandleCreateProductQue()
        {
            Row Information = null;
            while (_productInformation.TryDequeue(out Information))
            {
                Product NewProduct = new Product(Information);
                ProductDictionary.TryAdd(NewProduct.ID, NewProduct);
            }
        }

        private void UpdateStorageStatus_Thread()
        {
            while (!_productQueueDone)
            {
                Thread.Sleep(5);
            }
            Row Data = null;
            while (_storageStatusInformation.TryDequeue(out Data))
            {
                ProductDictionary[Convert.ToInt32(Data.Values[1])].StorageWithAmount.TryAdd(Convert.ToInt32(Data.Values[2]), Convert.ToInt32(Data.Values[3]));
            }
        }


        private void UpdateStorageStatus()
        {
            string sql = "SELECT * FROM `storage_status`";
            TableDecode Results = Mysql.RunQueryWithReturn(sql);
            _storageStatusInformation = Mysql.RunQueryWithReturnQueue(sql).RowData;
            CreateStorageStatusThreads();
        }

        private void CreateStorageStatusThreads()
        {
            for (int i = 0; i < _storageStatusThreadCount - 1; i++)
            {
                Thread NewThread = new Thread(new ThreadStart(UpdateStorageStatus_Thread));
                NewThread.Start();
                _storageStatusThreads.Add(NewThread);
            }
            Debug.WriteLine("Created all product threads [" + _storageStatusThreadCount + "]");
            UpdateStorageStatus_Thread();

        }

        public void GetAllProductsFromDatabase()
        {
            string sql = "SELECT * FROM `products` WHERE `id` > '0'";
            _productInformation = Mysql.RunQueryWithReturnQueue(sql).RowData;
            _productQueueLoaded = true;
            _productStatusTimer.Start();
            CreateProductThreads(); // Opretter tråde til at behandle data

        }

        public void GetAllGroupsFromDatabase()
        {
            string sql = "SELECT * FROM `groups`";
            Stopwatch GroupTimer = new Stopwatch();
            Stopwatch DatabaseTimer = new Stopwatch();
            DatabaseTimer.Start();
            GroupTimer.Start();
            ConcurrentQueue<Row> GroupsQueue = Mysql.RunQueryWithReturnQueue(sql).RowData;
            DatabaseTimer.Stop();
            Row Data = null;
            while (GroupsQueue.TryDequeue(out Data))
            {
                Group NewGroup = new Group(Data);
                GroupDictionary.TryAdd(NewGroup.ID, NewGroup);
            }
            GroupTimer.Stop();
            TimerStrings.Enqueue("[P3] Det tog " + GroupTimer.ElapsedMilliseconds + "ms at oprette grupper [" + DatabaseTimer.ElapsedMilliseconds + "ms]");
            Debug.WriteLine("GroupQue: Done!");
            _groupQueueDone = true;
            AddInformation("Groups count", GroupDictionary.Count);

        }

        public void GetAllStorageRoomsFromDatabase()
        {
            string sql = "SELECT * FROM `storagerooms`";
            Stopwatch StorageRoomTimer = new Stopwatch();
            Stopwatch DatabaseTimer = new Stopwatch();
            DatabaseTimer.Start();
            StorageRoomTimer.Start();
            ConcurrentQueue<Row> StorageRoomsQueue = Mysql.RunQueryWithReturnQueue(sql).RowData;
            DatabaseTimer.Stop();
            Row Data = null;
            while (StorageRoomsQueue.TryDequeue(out Data))
            {
                StorageRoom NewStorageRoom = new StorageRoom(Data);
                StorageRoomDictionary.TryAdd(NewStorageRoom.ID, NewStorageRoom);
            }
            StorageRoomTimer.Stop();
            TimerStrings.Enqueue("[P3] Det tog " + StorageRoomTimer.ElapsedMilliseconds + "ms at oprette Storage rooms [" + DatabaseTimer.ElapsedMilliseconds + "ms]");
            Debug.WriteLine("StorageRoomQue: Done!");
            _storageRoomQueueDone = true;
            AddInformation("Storageroom count", StorageRoomDictionary.Count);
        }

        public void GetAllTempProductsFromDatabase()
        {
            string sql = "SELECT * FROM `temp_products`";
            Stopwatch TempProductTimer = new Stopwatch();
            Stopwatch DatabaseTimer = new Stopwatch();
            DatabaseTimer.Start();
            TempProductTimer.Start();
            ConcurrentQueue<Row> TempProductQueue = Mysql.RunQueryWithReturnQueue(sql).RowData;
            DatabaseTimer.Stop();
            Row Data = null;
            while (TempProductQueue.TryDequeue(out Data))
            {
                TempProduct NewTempProduct = new TempProduct(Data);
                TempProductList.TryAdd(NewTempProduct.ID, NewTempProduct);
            }
            TempProductTimer.Stop();
            TimerStrings.Enqueue("[P3] Det tog " + TempProductTimer.ElapsedMilliseconds + "ms at oprette Temp produkter [" + DatabaseTimer.ElapsedMilliseconds + "ms]");

            Debug.WriteLine("TempProductQue: Done!");
            _tempProductQueueDone = true;
            AddInformation("TempProduct count", TempProductList.Count);
        }

        public void GetAllServiceProductsFromDatabase()
        {
            string sql = "SELECT * FROM `service_products`";
            Stopwatch ServiceProductTimer = new Stopwatch();
            Stopwatch DatabaseTimer = new Stopwatch();
            DatabaseTimer.Start();
            ServiceProductTimer.Start();
            ConcurrentQueue<Row> ServiceProductQueue = Mysql.RunQueryWithReturnQueue(sql).RowData;
            DatabaseTimer.Stop();
            Row Data = null;
            while (ServiceProductQueue.TryDequeue(out Data))
            {
                ServiceProduct NewServiceProduct = new ServiceProduct(Data);
                ServiceProductDictionary.TryAdd(NewServiceProduct.ID, NewServiceProduct);
            }
            ServiceProductTimer.Stop();
            TimerStrings.Enqueue("[P3] Det tog " + ServiceProductTimer.ElapsedMilliseconds + "ms at oprette Service Produkter [" + DatabaseTimer.ElapsedMilliseconds + "ms]");
            Debug.WriteLine("ServiceProductQueue: Done!");
            _serviceProductQueueDone = true;
            AddInformation("ServiceProduct count", ServiceProductDictionary.Count);
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
            Thread GetAllStorageStatusThread = new Thread(new ThreadStart(UpdateStorageStatus));
            GetAllProductsThread.Name = "GetAllProductsThread";
            GetAllGroupsThread.Name = "GetAllGroupsThread";
            GetAllTempProductsThread.Name = "GetAllTempProductsThread";
            GetAllStorageRoomsThread.Name = "GetAllStorageRoomsThread";
            GetAllServiceProductsThread.Name = "GetAllServiceProductsThread";
            GetAllStorageStatusThread.Name = "GetAllStorageStatusThread";
            lock (ThreadLock)
            {
                Threads.Add(GetAllProductsThread);
                Threads.Add(GetAllGroupsThread);
                Threads.Add(GetAllTempProductsThread);
                Threads.Add(GetAllStorageRoomsThread);
                Threads.Add(GetAllServiceProductsThread);
                Threads.Add(GetAllStorageStatusThread);
            }
            GetAllProductsThread.Start();
            GetAllGroupsThread.Start();
            GetAllStorageRoomsThread.Start();
            GetAllTempProductsThread.Start();
            GetAllServiceProductsThread.Start();
            GetAllStorageStatusThread.Start();
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
            var groups = ProductDictionary.Values.Where(x => x.ProductGroupID == GroupID);
            foreach (Product product in groups)
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
            var groups = ProductDictionary.Values.Where(x => x.ProductGroupID == removeID);
            foreach (Product product in groups)
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

        //Creates product with storage and stocka as keyvalue, then add the product to the list
        public void CreateProduct(int id, string name, string brand, decimal purchasePrice, int groupID, bool discount, decimal discountPrice, decimal salePrice, ConcurrentDictionary<int, int> storageWithAmount, bool UploadToDatabase = true)
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

        public void UpdateProduct(int id, string name, string brand, decimal purchasePrice, int groupID, bool discount, decimal discountPrice, decimal salePrice, ConcurrentDictionary<int, int> storageWithAmount, bool UploadToDatabase = true)
        {
            if (discountPrice > 0)
            {
                discount = true;
            }
            else
            {
                discount = false;
            }
            Product newProduct = new Product(id, name, brand, purchasePrice, groupID, discount, salePrice, discountPrice);
            newProduct.StorageWithAmount = new ConcurrentDictionary<int, int>(storageWithAmount.Where(x => x.Value != 0));
            ProductDictionary[newProduct.ID] = newProduct;
            newProduct.UpdateInDatabase();
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

        public void UpdateServiceProduct(int id, decimal salePrice, decimal groupPrice, int groupLimit, string name, int serviceProductGroupID, bool UpdateInDatabase = true)
        {
            ServiceProduct newServiceProduct = new ServiceProduct(id, salePrice, groupPrice, groupLimit, name, serviceProductGroupID);
            if (UpdateInDatabase)
            {
                newServiceProduct.UpdateInDatabase();
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
            TempProductList.TryAdd(newTempProduct.ID, newTempProduct);
            return newTempProduct;
        }

        /* User has already found the matching product ID.
         * First line findes the store storage
         * Second line subtracts the amound sold from storage*/
        public void MergeTempProduct(TempProduct tempProductToMerge, int matchedProductID)
        {
            Product MergedProduct = ProductDictionary[matchedProductID];
            tempProductToMerge.Resolve(MergedProduct);
            SaleTransaction tempProductsTransaction = tempProductToMerge.GetTempProductsSaleTransaction();
            var StorageRoomStatus = ProductDictionary[matchedProductID].StorageWithAmount.Where(x => x.Value >= tempProductsTransaction.Amount).OrderBy(x => x.Key).First();
            int StorageRoomKey = StorageRoomStatus.Key;
            int StorageRoomAmount = StorageRoomStatus.Value;
            MergedProduct.StorageWithAmount[StorageRoomKey] = StorageRoomAmount - tempProductsTransaction.Amount;
            MergedProduct.UpdateInDatabase();
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
                product.StorageWithAmount.TryAdd(newRoom.ID, 0);
            }
            newRoom.UploadToDatabase();
        }

        public void EditStorageRoom(int id, string name, string description)
        {
            StorageRoom roomToEdit = StorageRoomDictionary[id];
            roomToEdit.Name = name;
            roomToEdit.Description = description;
            roomToEdit.UpdateInDatabase();
        }

        //Removes storage room from dictionary, and all products
        public void DeleteStorageRoom(int id)
        {
            foreach (Product product in ProductDictionary.Values)
            {
                int h = 0;
                product.StorageWithAmount.TryRemove(id, out h);
            }
            string sql = $"DELETE FROM `storage_status` where `storageroom` == '{id}'";
            Mysql.RunQuery(sql);
            StorageRoom outVal = null;
            StorageRoomDictionary.TryRemove(id, out outVal);
            string deleteQuery = $"DELETE FROM `storagerooms` WHERE `id` = '{id}'";
            Mysql.RunQuery(deleteQuery);
        }


        #region SearchAlgorithm

        static List<SearchProduct> weigthedSearchList;
        public ConcurrentDictionary<int, SearchProduct> SearchForProduct(string searchString)
        {
            weigthedSearchList = new List<SearchProduct>();
            ConcurrentDictionary<int, SearchProduct> productsToReturn = new ConcurrentDictionary<int, SearchProduct>();
            int isNumber;
            string searchStringLower = searchString.ToLower();

            if (int.TryParse(searchStringLower, out isNumber))
            {
                if (ProductDictionary.Keys.Contains(isNumber))
                {
                    SearchProduct matchedProduct = new SearchProduct(AllProductsDictionary[isNumber]);
                    matchedProduct.NameMatch = 1000;
                    productsToReturn.TryAdd(isNumber, matchedProduct);
                }
            }

            foreach (BaseProduct product in AllProductsDictionary.Values)
            {
                ProductSearch(searchStringLower, product);
                GroupSearch(searchStringLower, product);
                BrandSearch(searchStringLower, product);
            }
            TakeProductsToReturnFromWeightedList(ref productsToReturn);

            return productsToReturn;
        }

        private void TakeProductsToReturnFromWeightedList(ref ConcurrentDictionary<int, SearchProduct> productsToReturn)
        {
            var searchList = weigthedSearchList.Where(x => (x.BrandMatch + x.GroupMatch + x.NameMatch) > 0);
            foreach (SearchProduct searchproduct in searchList)
            {
                productsToReturn.TryAdd(searchproduct.CurrentProduct.ID, searchproduct);
            }
        }

        private void ProductSearch(string searchStringElement, BaseProduct productToConvert)
        {
            SearchProduct productToAdd = new SearchProduct(productToConvert);

            string[] searchSplit = searchStringElement.Split(' ');

            if (productToConvert is Product)
            {
                string[] productSplit = (productToConvert as Product).Name.ToLower().Split(' ');

                foreach (string s in searchSplit)
                {
                    foreach (string t in productSplit)
                    {
                        if (s == t)
                        {
                            productToAdd.NameMatch += 100;
                        }
                        else if (LevenstheinProductSearch(s, t))
                        {
                            productToAdd.NameMatch += 1;
                        }
                    }
                }
            }
            else if (productToConvert is ServiceProduct)
            {
                string[] productSplit = (productToConvert as ServiceProduct).Name.ToLower().Split(' ');

                foreach (string s in searchSplit)
                {
                    foreach (string t in productSplit)
                    {
                        if (s == t)
                        {
                            productToAdd.NameMatch += 100;
                        }
                        else if (LevenstheinProductSearch(s, t))
                        {
                            productToAdd.NameMatch += 1;
                        }
                    }
                }
            } else
            {
                throw new WrongProductTypeException("Produktet der søges efter er af forkert type");
            }

            weigthedSearchList.Add(productToAdd);
        }

        private bool LevenshteinsGroupAndProductSearch(string[] searchedString, string stringToCompare, out int charDifference)
        {//setup for levenshteins
            string[] compareSplit = stringToCompare.ToLower().Split(' ');

            foreach (string sString in searchedString)
            {
                foreach (string compareString in compareSplit)
                {

                    //getting the chardifference between the searchedstring and the productname
                    charDifference = ComputeLevenshteinsDistance(sString, compareString);
                    //Evaluate if the chardifference is in between the changelimit of the string
                    if (EvaluateStringLimit(sString.Length, charDifference))
                    {
                        //only returns true of it is matching
                        return true;
                    }
                }
            }
            charDifference = -1;
            return false;
        }


        private void GroupSearch(string searchString, BaseProduct product)
        {
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchString.Split(' ');
            //matching on each element in the string
            int MatchedValue;
            //if the string contains a name of a group, or the string is matched, each product with the same group 
            //is added to the list of products to show.

            if (product is Product)
            {
                foreach (string searchedString in dividedString)
                {
                    if (GroupDictionary[(product as Product).ProductGroupID].Name.Contains(searchedString))
                    {
                        weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().GroupMatch += 100;
                    }
                }
                if (LevenshteinsGroupAndProductSearch(dividedString, GroupDictionary[(product as Product).ProductGroupID].Name, out MatchedValue))
                {
                    if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).Count() > 0)
                    {
                        if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First() != null)
                        {
                            weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().GroupMatch += 1;
                        }
                    }
                }
            } else if (product is ServiceProduct)
            {
                foreach (string searchedString in dividedString)
                {
                    if (GroupDictionary[(product as ServiceProduct).ServiceProductGroupID].Name.Contains(searchedString))
                    {
                        weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().GroupMatch += 100;
                    }
                }
                if (LevenshteinsGroupAndProductSearch(dividedString, GroupDictionary[(product as ServiceProduct).ServiceProductGroupID].Name, out MatchedValue))
                {
                    if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).Count() > 0)
                    {
                        if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First() != null)
                        {
                            weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().GroupMatch += 1;
                        }
                    }
                }
            }
        }

        private void BrandSearch(string searchString, BaseProduct product)
        {
            //divides all the elements in the string, to evaluate each element
            string[] dividedString = searchString.Split(' ');
            //checking all products to to match the brands with the searched string elements
            int MatchedValues;
            //matching on each element in the string
            //if the string contains a product brand, or the string is matched, each product with the same brand
            //is added to the list of products to show.
            if (product is Product)
            {
                foreach (string searchedString in dividedString)
                {
                    if ((product as Product).Brand.Contains(searchedString))
                    {
                        weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().BrandMatch += 100;
                    }
                }
                if (LevenshteinsGroupAndProductSearch(dividedString, (product as Product).Brand, out MatchedValues))
                {
                    if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).Count() > 0)
                    {
                        if (weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First() != null)
                        {
                            weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First().BrandMatch += 1;
                        }
                    }
                }
            }
        }

        private bool LevenstheinProductSearch(string searchEle, string ProductEle)
        {
            int charDifference = ComputeLevenshteinsDistance(searchEle, ProductEle);
            return EvaluateStringLimit(searchEle.Length, charDifference);
        }

        private bool EvaluateStringLimit(int searchedStringLength, int charDiff)
        {
            int limitOfChanges;

            // Determines how many changes is allowed, depending on how long the string is
            if (searchedStringLength < 3)
                limitOfChanges = 0;
            else if (searchedStringLength < 5)
                limitOfChanges = 1;
            else if (searchedStringLength < 8)
                limitOfChanges = 2;
            else if (searchedStringLength < 12)
                limitOfChanges = 3;
            else
            {
                limitOfChanges = 4;
            }

            return limitOfChanges >= charDiff;
        }

        private int ComputeLevenshteinsDistance(string searchedString, string productToCompare)
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
            var test = d[d.GetUpperBound(0), d.GetUpperBound(1)];

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];

            #endregion
        }
    }
}