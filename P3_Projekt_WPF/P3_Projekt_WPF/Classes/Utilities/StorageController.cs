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
                ProductDictionary[productID].StorageWithAmount.TryAdd(storageRoom, amount);

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

        //Creates product with storage and stocka as keyvalue, then add the product to the list
        public void CreateProduct(int id, string name, string brand, decimal purchasePrice, int groupID, bool discount, decimal discountPrice, decimal salePrice, ConcurrentDictionary<int,int> storageWithAmount, bool UploadToDatabase = true)
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
            Product newProduct = new Product(id, name, brand, purchasePrice, groupID, discount, salePrice, discountPrice);
            newProduct.StorageWithAmount = storageWithAmount;
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
                product.StorageWithAmount.TryAdd(newRoom.ID, 0);
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
                int h = 0;
                product.StorageWithAmount.TryRemove(id, out h);
                product.UpdateInDatabase();
            }
            StorageRoom outVal = null;
            StorageRoomDictionary.TryRemove(id, out outVal);
            string deleteQuery = $"DELETE FROM `storagerooms` WHERE `id` = '{id}'";
            Mysql.RunQuery(deleteQuery);
        }
    }
}
