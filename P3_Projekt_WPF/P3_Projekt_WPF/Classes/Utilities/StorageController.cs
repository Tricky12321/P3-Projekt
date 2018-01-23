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
using System.Windows;
namespace P3_Projekt_WPF.Classes.Utilities
{
    public class StorageController
    {
        public ConcurrentDictionary<int, Product> ProductDictionary = new ConcurrentDictionary<int, Product>();
        public ConcurrentDictionary<int, ServiceProduct> ServiceProductDictionary = new ConcurrentDictionary<int, ServiceProduct>();
        public ConcurrentDictionary<int, Group> GroupDictionary = new ConcurrentDictionary<int, Group>();
        public ConcurrentDictionary<int, StorageRoom> StorageRoomDictionary = new ConcurrentDictionary<int, StorageRoom>();
        public ConcurrentDictionary<int, TempProduct> TempProductDictionary = new ConcurrentDictionary<int, TempProduct>();
        public ConcurrentDictionary<int, StorageTransaction> StorageTransactionDictionary = new ConcurrentDictionary<int, StorageTransaction>();
        public ConcurrentDictionary<int, OrderTransaction> OrderTransactionDictionary = new ConcurrentDictionary<int, OrderTransaction>();
        public List<string[]> InformationGridData = new List<string[]>();
        private object _informationGridLock = new object();
        private ConcurrentQueue<Product> _productResults = new ConcurrentQueue<Product>();
        public ConcurrentDictionary<int, BaseProduct> AllProductsDictionary = new ConcurrentDictionary<int, BaseProduct>();
        public ConcurrentDictionary<int, Product> DisabledProducts = new ConcurrentDictionary<int, Product>();
        public ConcurrentDictionary<int, ServiceProduct> DisabledServiceProducts = new ConcurrentDictionary<int, ServiceProduct>();
        public List<TempProduct> TempTempProductList = new List<TempProduct>();

        public StorageController()
        {
            AddInformation("Core Antal", Utils.NumberOfCores);
        }

        public void ClearDictionaries()
        {
            if (_queueThreads != null)
            {
                _queueThreads.Clear();
            }
            AllProductsDictionary.Clear();
            TempProductDictionary.Clear();
            ServiceProductDictionary.Clear();
            DisabledProducts.Clear();
            DisabledServiceProducts.Clear();
            GroupDictionary.Clear();
            StorageRoomDictionary.Clear();
            StorageTransactionDictionary.Clear();
            ProductDictionary.Clear();
            StorageTransactionDictionary.Clear();
            OrderTransactionDictionary.Clear();
            ResetCounters();
        }

        public void ReloadAllDictionaries()
        {
            ClearDictionaries();
            GetAll();
            while (!ThreadsDone)
            {
                Thread.Sleep(1);
            }
        }

        public void LoadAllProductsDictionary()
        {
            AllProductsDictionary.Clear();
            foreach (KeyValuePair<int, Product> productWithID in ProductDictionary.Where(x => x.Value.Active == true))
            {
                AllProductsDictionary.TryAdd(productWithID.Key, productWithID.Value);

            }

            foreach (KeyValuePair<int, ServiceProduct> serviceProductWithID in ServiceProductDictionary)
            {
                if (serviceProductWithID.Value.GetName() != "Is")
                {
                    AllProductsDictionary.TryAdd(serviceProductWithID.Key, serviceProductWithID.Value);
                }
            }
        }

        #region Multithreading

        public List<Thread> Threads = new List<Thread>();
        public object ThreadLock = new object();
        // For at holde garbage collector fra at dræbe tråde
        public bool ThreadsDone
        {
            get
            {
                if (_queueThreads == null)
                {
                    return false;
                }
                return (_queueThreads.Where(x => x.ThreadState == System.Threading.ThreadState.Running || x.ThreadState == System.Threading.ThreadState.Unstarted).Count() == 0);
            }
        }

        public void AddInformation(string name, object value)
        {
            lock (_informationGridLock)
            {
                string[] result = { name, value.ToString() };
                InformationGridData.Add(result);
            }
        }


        private void UpdateStorageStatus()
        {
            string sql = "SELECT * FROM `storage_status`";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _storageStatusCount = Result.RowCounter;
            AddInformation("Storage Status Antal", Result.RowCounter.ToString());
            _storageStatusQueue = Result.RowData;
            _storageStatusLoaded = true;
        }

        public void GetAllProductsFromDatabase()
        {
            string sql = "SELECT * FROM `products` WHERE `id` > '0' AND `active` = '1'";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _productsCount = Result.RowCounter;
            AddInformation("Active Products Antal", Result.RowCounter.ToString());
            _productQueue = Result.RowData;
            _productsLoaded = true;
        }

        public void GetAllDisabledProductsFromDatabase()
        {
            string sql = "SELECT * FROM `products` WHERE `id` > '0' AND `active` = '0'";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _disabledProductsCount = Result.RowCounter;
            AddInformation("Disabled Products Antal", Result.RowCounter.ToString());
            _disabledProductsQueue = Result.RowData;
            _disabledProductsLoaded = true;
        }

        public void GetAllGroupsFromDatabase()
        {
            string sql = "SELECT * FROM `groups`";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _groupsCount = Result.RowCounter;
            AddInformation("Group Antal", Result.RowCounter.ToString());
            _groupsQueue = Result.RowData;
            _groupsLoaded = true;
        }

        public void GetAllStorageRoomsFromDatabase()
        {
            string sql = "SELECT * FROM `storagerooms` WHERE `deactivated` = '0'";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _storageRoomsCount = Result.RowCounter;
            AddInformation("StorageRoom Antal", Result.RowCounter.ToString());
            _storageRoomQueue = Result.RowData;
            _storageRoomLoaded = true;
        }

        public void GetAllTempProductsFromDatabase()
        {
            string sql = "SELECT * FROM `temp_products`";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _tempProductCount = Result.RowCounter;
            AddInformation("TempProduct Antal", Result.RowCounter.ToString());
            _tempProductQueue = Result.RowData;
            _tempProductLoaded = true;
        }

        public void GetAllServiceProductsFromDatabase()
        {
            string sql = "SELECT * FROM `service_products` WHERE `active` = '1'";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _serviceProductCount = Result.RowCounter;
            AddInformation("Active ServiceProduct Antal", Result.RowCounter.ToString());
            _serviceProductQueue = Result.RowData;
            _serviceProductLoaded = true;
        }

        public void GetAllDisabledServiceProductsFromDatabase()
        {
            string sql = "SELECT * FROM `service_products` WHERE `active` = '0'";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _disabledServiceProductsCount = Result.RowCounter;
            AddInformation("Disabled ServiceProduct Antal", Result.RowCounter.ToString());
            _disabledServiceProductsQueue = Result.RowData;
            _disabledServiceProductsLoaded = true;
        }

        public void GetAllStorageTransactions()
        {
            string sql = $"SELECT * FROM `storage_transaction`";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _storageTransactionCount = Result.RowCounter;
            AddInformation("Storage Transaction Antal", Result.RowCounter.ToString());
            _storageTransactionsQueue = Result.RowData;
            _storageTransactionsLoaded = true;
        }

        public void GetAllOrderTransactions()
        {
            string sql = $"SELECT * FROM `order_transactions`";
            TableDecodeQueue Result = Mysql.RunQueryWithReturnQueue(sql);
            _orderTransactionCount = Result.RowCounter;
            AddInformation("Order Transaction Antal", Result.RowCounter.ToString());
            _orderTransactionsQueue = Result.RowData;
            _orderTransactionsLoaded = true;
        }

        private void ResetCounters()
        {
            _doneProductsCount = 0;
            _doneServiceProductCount = 0;
            _doneGroupsCount = 0;
            _doneStorageRoomsCount = 0;
            _doneTempProductCount = 0;
            _doneDisabledProductsCount = 0;
            _doneDisabledServiceProductsCount = 0;
            _doneOrderTransactionCount = 0;
            _doneStorageTransactionCount = 0;
            _doneStorageStatusCount = 0;
            _serviceProductLoaded = false;
            _tempProductLoaded = false;
            _storageRoomLoaded = false;
            _productsLoaded = false;
            _storageStatusLoaded = false;
            _groupsLoaded = false;
            _disabledProductsLoaded = false;
            _disabledServiceProductsLoaded = false;
            _storageTransactionsLoaded = false;
            _orderTransactionsLoaded = false;
        }

        private int _productsCount;
        private int _serviceProductCount;
        private int _groupsCount;
        private int _storageRoomsCount;
        private int _tempProductCount;
        private int _disabledProductsCount;
        private int _disabledServiceProductsCount;
        private int _orderTransactionCount;
        private int _storageTransactionCount;
        private int _storageStatusCount;

        private int _doneProductsCount = 0;
        private int _doneServiceProductCount = 0;
        private int _doneGroupsCount = 0;
        private int _doneStorageRoomsCount = 0;
        private int _doneTempProductCount = 0;
        private int _doneDisabledProductsCount = 0;
        private int _doneDisabledServiceProductsCount = 0;
        private int _doneOrderTransactionCount = 0;
        private int _doneStorageTransactionCount = 0;
        private int _doneStorageStatusCount = 0;


        private bool _serviceProductLoaded = false;
        private bool _tempProductLoaded = false;
        private bool _storageRoomLoaded = false;
        private bool _productsLoaded = false;
        private bool _storageStatusLoaded = false;
        private bool _groupsLoaded = false;
        private bool _disabledProductsLoaded = false;
        private bool _disabledServiceProductsLoaded = false;
        private bool _storageTransactionsLoaded = false;
        private bool _orderTransactionsLoaded = false;
        private ConcurrentQueue<Row> _serviceProductQueue;
        private ConcurrentQueue<Row> _tempProductQueue;
        private ConcurrentQueue<Row> _storageRoomQueue;
        private ConcurrentQueue<Row> _groupsQueue;
        private ConcurrentQueue<Row> _productQueue;
        private ConcurrentQueue<Row> _storageStatusQueue;
        private ConcurrentQueue<Row> _disabledProductsQueue;
        private ConcurrentQueue<Row> _disabledServiceProductsQueue;
        private ConcurrentQueue<Row> _storageTransactionsQueue;
        private ConcurrentQueue<Row> _orderTransactionsQueue;

        private List<Thread> _queueThreads;
        private int _queueThreadsCount = Utils.NumberOfCores;

        private bool SecondPoolDone()
        {
            if (_doneStorageTransactionCount != _storageTransactionCount)
            {
                return false;
            }
            if (_doneOrderTransactionCount != _orderTransactionCount)
            {
                return false;
            }
            if (_doneStorageStatusCount != _storageStatusCount)
            {
                return false;
            }
            return true;
        }

        private bool FirstPoolDone()
        {
            if (_doneProductsCount != _productsCount)
            {
                return false;
            }
            if (_doneStorageRoomsCount != _storageRoomsCount)
            {
                return false;
            }
            if (_doneDisabledProductsCount != _disabledProductsCount)
            {
                return false;
            }
            if (_doneServiceProductCount != _serviceProductCount)
            {
                return false;
            }
            if (_doneDisabledServiceProductsCount != _disabledServiceProductsCount)
            {
                return false;
            }
            if (_doneTempProductCount != _tempProductCount)
            {
                return false;
            }
            if (_doneGroupsCount != _groupsCount)
            {
                return false;
            }
            return true;
        }

        private void HandleSecondPool()
        {
            Row Data;
            if (FirstPoolDone())
            {
                Debug.WriteLine("First Queue is [DONE] going to second queue");
            }
            while (!SecondPoolDone())
            {
                while (_storageStatusQueue.TryDequeue(out Data))
                {
                    int id = Convert.ToInt32(Data.Values[0]);
                    if (ProductDictionary.ContainsKey(id))
                    {
                        ProductDictionary[id].StorageWithAmount.TryAdd(Convert.ToInt32(Data.Values[1]), Convert.ToInt32(Data.Values[2]));
                    }
                    else
                    {
                        DisabledProducts[id].StorageWithAmount.TryAdd(Convert.ToInt32(Data.Values[1]), Convert.ToInt32(Data.Values[2]));
                    }
                    Interlocked.Increment(ref _doneStorageStatusCount);
                }
                while (_storageTransactionsQueue.TryDequeue(out Data))
                {
                    int ProductID = Convert.ToInt32(Data.Values[1]);
                    int SourceID = Convert.ToInt32(Data.Values[4]);
                    int DestinationID = Convert.ToInt32(Data.Values[5]);
                    if (StorageRoomDictionary.ContainsKey(SourceID) && StorageRoomDictionary.ContainsKey(DestinationID))
                    {
                        StorageTransaction StorageTrans = new StorageTransaction(Data, true);
                        StorageRoom Source = StorageRoomDictionary[SourceID];
                        StorageRoom Destination = StorageRoomDictionary[DestinationID];
                        BaseProduct prod;
                        if (ProductDictionary.ContainsKey(ProductID))
                        {
                            prod = ProductDictionary[ProductID];
                        }
                        else
                        {
                            prod = DisabledProducts[ProductID];
                        }
                        StorageTrans.SetInformation(Source, Destination, prod);
                        StorageTransactionDictionary.TryAdd(StorageTrans.ID, StorageTrans);
                    }
                    Interlocked.Increment(ref _doneStorageTransactionCount);


                }
                while (_orderTransactionsQueue.TryDequeue(out Data))
                {
                    OrderTransaction OrderTrans = new OrderTransaction(Data, true);
                    int ProductID = Convert.ToInt32(Data.Values[1]);
                    if (ProductDictionary.ContainsKey(ProductID))
                    {
                        OrderTrans.SetInformation(ProductDictionary[ProductID]);
                    }
                    else
                    {
                        OrderTrans.SetInformation(DisabledProducts[ProductID]);
                    }
                    OrderTransactionDictionary.TryAdd(OrderTrans.ID, OrderTrans);
                    Interlocked.Increment(ref _doneOrderTransactionCount);

                }
            }

        }

        private void HandlePools()
        {
            HandleFirstPool();
            HandleSecondPool();
        }

        private void HandleFirstPool()
        {
            Row Data;
            while (!FirstPoolDone())
            {
                while (_serviceProductQueue.TryDequeue(out Data))
                {
                    ServiceProduct NewServiceProduct = new ServiceProduct(Data);
                    ServiceProductDictionary.TryAdd(NewServiceProduct.ID, NewServiceProduct);
                    Interlocked.Increment(ref _doneServiceProductCount);
                }
                while (_tempProductQueue.TryDequeue(out Data))
                {
                    TempProduct NewTempProduct = new TempProduct(Data);
                    TempProductDictionary.TryAdd(NewTempProduct.ID, NewTempProduct);
                    Interlocked.Increment(ref _doneTempProductCount);

                }
                while (_storageRoomQueue.TryDequeue(out Data))
                {
                    StorageRoom NewStorageRoom = new StorageRoom(Data);
                    StorageRoomDictionary.TryAdd(NewStorageRoom.ID, NewStorageRoom);
                    Interlocked.Increment(ref _doneStorageRoomsCount);

                }
                while (_groupsQueue.TryDequeue(out Data))
                {
                    Group NewGroup = new Group(Data);
                    GroupDictionary.TryAdd(NewGroup.ID, NewGroup);
                    Interlocked.Increment(ref _doneGroupsCount);

                }
                while (_productQueue.TryDequeue(out Data))
                {
                    Product NewProduct = new Product(Data);
                    ProductDictionary.TryAdd(NewProduct.ID, NewProduct);
                    Interlocked.Increment(ref _doneProductsCount);

                }

                while (_disabledProductsQueue.TryDequeue(out Data))
                {
                    Product NewProduct = new Product(Data);
                    DisabledProducts.TryAdd(NewProduct.ID, NewProduct);
                    Interlocked.Increment(ref _doneDisabledProductsCount);

                }
                while (_disabledServiceProductsQueue.TryDequeue(out Data))
                {
                    ServiceProduct NewProduct = new ServiceProduct(Data);
                    DisabledServiceProducts.TryAdd(NewProduct.ID, NewProduct);
                    Interlocked.Increment(ref _doneDisabledServiceProductsCount);
                }
            }
        }

        public void GetAll()
        {
            bool local = Properties.Settings.Default.local_or_remote;
            _queueThreads = new List<Thread>();
            for (int i = 0; i < _queueThreadsCount - 1; i++)
            {
                _queueThreads.Add(new Thread(new ThreadStart(HandlePools)));
            }

            if (!local)
            {
                StartSQLThreads();
                while (!_productsLoaded || !_storageRoomLoaded || !_groupsLoaded || !_tempProductLoaded || !_storageStatusLoaded || !_serviceProductLoaded || !_disabledProductsLoaded || !_disabledServiceProductsLoaded || !_orderTransactionsLoaded || !_storageTransactionsLoaded)
                {
                    Thread.Sleep(1);
                }
            }
            else
            {
                RunGetSQL();
            }
            foreach (var thread in _queueThreads)
            {
                thread.Start();
            }
            HandlePools();
        }

        public void RunGetSQL()
        {
            UpdateStorageStatus();
            GetAllProductsFromDatabase();
            GetAllServiceProductsFromDatabase();
            GetAllTempProductsFromDatabase();
            GetAllStorageRoomsFromDatabase();
            GetAllGroupsFromDatabase();
            GetAllDisabledProductsFromDatabase();
            GetAllDisabledServiceProductsFromDatabase();
            GetAllStorageTransactions();
            GetAllOrderTransactions();
        }

        public void StartSQLThreads()
        {
            // Multithreading the different mysql calls, so it goes much faster
            Thread GetAllProductsThread = new Thread(new ThreadStart(GetAllProductsFromDatabase));
            Thread GetAllGroupsThread = new Thread(new ThreadStart(GetAllGroupsFromDatabase));
            Thread GetAllTempProductsThread = new Thread(new ThreadStart(GetAllTempProductsFromDatabase));
            Thread GetAllStorageRoomsThread = new Thread(new ThreadStart(GetAllStorageRoomsFromDatabase));
            Thread GetAllServiceProductsThread = new Thread(new ThreadStart(GetAllServiceProductsFromDatabase));
            Thread GetAllStorageStatusThread = new Thread(new ThreadStart(UpdateStorageStatus));
            Thread GetAllDisabledProductsThread = new Thread(new ThreadStart(GetAllDisabledProductsFromDatabase));
            Thread GetAllDisabledServiceProductsThread = new Thread(new ThreadStart(GetAllDisabledServiceProductsFromDatabase));
            Thread GetAllStorageTransactionsThread = new Thread(new ThreadStart(GetAllStorageTransactions));
            Thread GetAllOrderTransactionsThread = new Thread(new ThreadStart(GetAllOrderTransactions));
            GetAllProductsThread.Name = "GetAllProductsThread";
            GetAllGroupsThread.Name = "GetAllGroupsThread";
            GetAllTempProductsThread.Name = "GetAllTempProductsThread";
            GetAllStorageRoomsThread.Name = "GetAllStorageRoomsThread";
            GetAllServiceProductsThread.Name = "GetAllServiceProductsThread";
            GetAllDisabledProductsThread.Name = "GetAllDisabledProductsThread";
            GetAllStorageStatusThread.Name = "GetAllStorageStatusThread";
            GetAllDisabledServiceProductsThread.Name = "GetAllDisabledServiceProductsThread";
            GetAllStorageTransactionsThread.Name = "GetAllStorageTransactionsThread";
            GetAllOrderTransactionsThread.Name = "GetAllOrderTransactionsThread";
            GetAllProductsThread.Start();
            GetAllGroupsThread.Start();
            GetAllStorageRoomsThread.Start();
            GetAllTempProductsThread.Start();
            GetAllServiceProductsThread.Start();
            GetAllDisabledProductsThread.Start();
            GetAllDisabledServiceProductsThread.Start();
            GetAllStorageStatusThread.Start();
            GetAllStorageTransactionsThread.Start();
            GetAllOrderTransactionsThread.Start();
        }

        #endregion 

        public void DeleteProduct(int ProductID)
        {
            Product outVal1 = null;
            BaseProduct outVal2 = null;
            ProductDictionary.TryRemove(ProductID, out outVal1);
            AllProductsDictionary.TryRemove(ProductID, out outVal2);
        }

        public void CreateGroup(string name, string description)
        {
            Group newGroup = new Group(name, description);
            newGroup.ID = Group.GetNextID();
            newGroup.UploadToDatabase();
            GroupDictionary.TryAdd(newGroup.ID, newGroup);
        }

        public void EditGroup(int id, string name, string description)
        {
            GroupDictionary[id].Name = name;
            GroupDictionary[id].Description = description;
            GroupDictionary[id].UpdateInDatabase();
        }

        //Assign group 0 to products left with no group
        //Removes group from dictionary
        public void DeleteGroup(int GroupID)
        {
            //Mulighed for at flytte alle produkter til en bestem gruppe???
            var Products = ProductDictionary.Values.Where(x => x.ProductGroupID == GroupID);
            foreach (Product product in Products)
            {
                product.ProductGroupID = GroupDictionary[0].ID;
                string updateProductsSQL = $"UPDATE `products` SET `groups` = '{GroupDictionary[0].ID}' WHERE `groups` = '{GroupID}'";
                Mysql.RunQuery(updateProductsSQL);
                string updateServiceProductsSQL = $"UPDATE `service_products` SET `groups` = '{GroupDictionary[0].ID}' WHERE `groups` = '{GroupID}'";
                Mysql.RunQuery(updateServiceProductsSQL);
            }
            Group outVal = null;
            string sql = $"DELETE FROM `groups` WHERE `id` = '{GroupID}' ";
            Mysql.RunQuery(sql);
            GroupDictionary.TryRemove(GroupID, out outVal);
        }

        //Assign new group to products left with no group
        //Removes group from dictionary
        public IEnumerable<string> GetProductBrands()
        {
            return ProductDictionary.Values.Select(x => x.Brand).Distinct();
        }

        //Creates product with storage and stocka as keyvalue, then add the product to the list
        public void CreateProduct(int id, string name, string brand, decimal purchasePrice, int groupID, bool discount, decimal discountPrice, decimal salePrice, ConcurrentDictionary<int, int> storageWithAmount, bool UploadToDatabase = true)
        {
            Product newProduct = new Product(id, name, brand, purchasePrice, groupID, discount, salePrice, discountPrice);
            newProduct.StorageWithAmount = storageWithAmount;
            if (!ProductDictionary.TryAdd(newProduct.ID, newProduct) || !AllProductsDictionary.TryAdd(newProduct.ID, newProduct))
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
            discount = discountPrice > 0;
            Product newProduct = new Product(id, name, brand, purchasePrice, groupID, discount, salePrice, discountPrice);
            newProduct.StorageWithAmount = new ConcurrentDictionary<int, int>(storageWithAmount.Where(x => x.Value != 0));
            ProductDictionary[newProduct.ID] = newProduct;
            AllProductsDictionary[newProduct.ID] = newProduct;
            newProduct.UpdateInDatabase();
        }

        public void UpdateDeactivatedProduct(int id, string name, string brand, decimal purchasePrice, int groupID, bool discount, decimal discountPrice, decimal salePrice, ConcurrentDictionary<int, int> storageWithAmount, bool UploadToDatabase = true)
        {
            discount = discountPrice > 0;
            Product newProduct = new Product(id, name, brand, purchasePrice, groupID, discount, salePrice, discountPrice);
            newProduct.DeactivateProduct();
            newProduct.StorageWithAmount = new ConcurrentDictionary<int, int>(storageWithAmount.Where(x => x.Value != 0));
            DisabledProducts[newProduct.ID] = newProduct;
            newProduct.UpdateInDatabase();
        }

        public void CreateServiceProduct(int id, decimal salePrice, decimal groupPrice, int groupLimit, string name, int serviceProductGroupID, bool UploadToDatabase = true)
        {
            ServiceProduct newServiceProduct = new ServiceProduct(id, salePrice, groupPrice, groupLimit, name, serviceProductGroupID);
            if (!ServiceProductDictionary.TryAdd(newServiceProduct.ID, newServiceProduct) || !AllProductsDictionary.TryAdd(newServiceProduct.ID, newServiceProduct))
            {
                throw new Exception("This key already exists");
            }
            if (UploadToDatabase)
            {
                newServiceProduct.UploadToDatabase();
            }
        }

        public string ActivateProduct(int ID)
        {
            if (DisabledProducts.ContainsKey(ID))
            {
                Product ProductToActivate;
                DisabledProducts.TryRemove(ID, out ProductToActivate);
                ProductToActivate.ActivateProduct();
                AllProductsDictionary.TryAdd(ID, ProductToActivate);
                ProductDictionary.TryAdd(ID, ProductToActivate);
                return ProductToActivate.ToString();
            }
            else
            {
                ServiceProduct ServiceProductToActivate;
                DisabledServiceProducts.TryRemove(ID, out ServiceProductToActivate);
                ServiceProductToActivate.ActivateProduct();
                AllProductsDictionary.TryAdd(ID, ServiceProductToActivate);
                ServiceProductDictionary.TryAdd(ID, ServiceProductToActivate);
                return ServiceProductToActivate.ToString();
            }
        }

        public void UpdateDeactivatedServiceProduct(int id, decimal salePrice, decimal groupPrice, int groupLimit, string name, int serviceProductGroupID, bool UpdateInDatabase = true)
        {
            ServiceProduct newServiceProduct = new ServiceProduct(id, salePrice, groupPrice, groupLimit, name, serviceProductGroupID);
            if (UpdateInDatabase)
            {
                newServiceProduct.UpdateInDatabase();
            }
            BaseProduct noProd;
            ServiceProduct noProd2;
            AllProductsDictionary.TryRemove(id, out noProd);
            ServiceProductDictionary.TryRemove(id, out noProd2);
            DisabledServiceProducts.TryAdd(id, newServiceProduct);
        }

        public void UpdateServiceProduct(int id, decimal salePrice, decimal groupPrice, int groupLimit, string name, int serviceProductGroupID, bool UpdateInDatabase = true)
        {
            ServiceProduct newServiceProduct = new ServiceProduct(id, salePrice, groupPrice, groupLimit, name, serviceProductGroupID);
            if (UpdateInDatabase)
            {
                newServiceProduct.UpdateInDatabase();
            }
            ServiceProductDictionary[newServiceProduct.ID] = newServiceProduct;
            AllProductsDictionary[newServiceProduct.ID] = newServiceProduct;
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

        public TempProduct CreateTempProduct(string description, decimal salePrice, int ID)
        {
            TempProduct newTempProduct = new TempProduct(description, salePrice);
            newTempProduct.ID = ID;
            TempProductDictionary.TryAdd(ID, newTempProduct);
            return newTempProduct;
        }

        public TempProduct CreateTempTempProduct(string description, decimal salePrice, int ID)
        {
            TempProduct newTempProduct = new TempProduct(description, salePrice);
            newTempProduct.ID = ID;
            TempTempProductList.Add(newTempProduct);
            return newTempProduct;
        }

        /* User has already found the matching product ID.
         * First line findes the store storage
         * Second line subtracts the amount sold from Shop storage*/
        public void MatchTempProduct(TempProduct tempProductToMatch, int matchedProductID)
        {
            Product MatchedProduct = ProductDictionary[matchedProductID];
            tempProductToMatch.Resolve(MatchedProduct);
            SaleTransaction tempProductsTransaction = tempProductToMatch.GetTempProductsSaleTransaction();
            //Gets the Shop storage room, which has = 1, but if it doesn't exist, gets the next one
            if (MatchedProduct.StorageWithAmount.Count == 0)
            {
                MatchedProduct.StorageWithAmount.TryAdd(1, 0);
            }
            KeyValuePair<int, int> StorageRoomStatus = MatchedProduct.StorageWithAmount.First();
            MatchedProduct.StorageWithAmount[StorageRoomStatus.Key] = StorageRoomStatus.Value - tempProductsTransaction.Amount;
            MatchedProduct.UpdateInDatabase();
        }

        public void RematchTempProduct(TempProduct tempProductToMatch, int matchedProductID)
        {
            Product PreviouslyMatchdProduct = ProductDictionary[tempProductToMatch.ResolvedProductID];
            KeyValuePair<int, int> StorageRoomStatus = PreviouslyMatchdProduct.StorageWithAmount.Where(x => x.Key != 0).First();

            PreviouslyMatchdProduct.StorageWithAmount[StorageRoomStatus.Key] = StorageRoomStatus.Value + tempProductToMatch.GetTempProductsSaleTransaction().Amount;
            PreviouslyMatchdProduct.UpdateInDatabase();

            MatchTempProduct(tempProductToMatch, matchedProductID);
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
            if (id != 1)
            {
                string CheckProductsStorageAmount = $"SELECT * FROM `storage_status` WHERE `amount` != '0' AND `storageroom` = '{id}'";
                TableDecode Results = Mysql.RunQueryWithReturn(CheckProductsStorageAmount);
                if (Results.RowCounter != 0)
                {
                    MessageBox.Show("Du kan ikke slette lager rum som stadig har varer på lager!");
                }
                else
                {
                    if (StorageRoomDictionary.ContainsKey(id))
                    {
                        StorageRoomDictionary[id].DeactivateStorageRoom();
                        foreach (Product product in ProductDictionary.Values)
                        {
                            int h = 0;
                            product.StorageWithAmount.TryRemove(id, out h);
                        }
                        StorageRoom outVal = null;
                        StorageRoomDictionary.TryRemove(id, out outVal);
                        string deleteQuery = $"DELETE FROM `storage_status` WHERE `storageroom` = '{id}'";
                        Mysql.RunQuery(deleteQuery);
                    }
                    else
                    {
                        MessageBox.Show("Det lager du forsøger at slette, eksistere ikke...");
                    }
                }
            }
            else
            {
                MessageBox.Show("Du kan ikke slette butikken!");
            }
        }

        /// <summary>
        /// Creates Icecream as a serviceproduct, then uploads it to the database, if not already in the system.
        /// </summary>
        /// <returns></returns>
        public void MakeSureIcecreamExists()
        {
            int IcecreamProductID;
            if (!ServiceProductDictionary.Values.Any(x => x.Name == "Is"))
            {
                IcecreamProductID = ServiceProduct.GetNextID();
                var IcecreamProduct = new ServiceProduct(IcecreamProductID, 0, 0, 1000, "Is", Properties.Settings.Default.IcecreamID);
                ServiceProductDictionary.TryAdd(IcecreamProductID, IcecreamProduct);
                AllProductsDictionary.TryAdd(IcecreamProductID, IcecreamProduct);
                IcecreamProduct.UploadToDatabase();
            }
            else
            {
                IcecreamProductID = ServiceProductDictionary.Values.Where(x => x.Name == "Is").First().ID;
            }

            Properties.Settings.Default.IcecreamProductID = IcecreamProductID;
            Properties.Settings.Default.Save();
        }

        public void TempProductToDictionary()
        {
            foreach (TempProduct tempproduct in TempTempProductList)
            {
                TempProductDictionary.TryAdd(tempproduct.ID, tempproduct);
            }
            TempTempProductList.Clear();
        }

        public List<string> GetSuppliers()
        {
            List<string> suppliersList = OrderTransactionDictionary.Values.Select(x => x._supplier.ToLower()).ToList();
            if (suppliersList.Count != suppliersList.Distinct().Count())
            {
                return suppliersList.Distinct().ToList();
            }
            return suppliersList;
        }

        #region SearchAlgorithm

        public List<SearchProduct> weigthedSearchList;
        public ConcurrentDictionary<int, SearchProduct> SearchForProduct(string searchString)
        {
            weigthedSearchList = new List<SearchProduct>();
            ConcurrentDictionary<int, SearchProduct> productsToReturn = new ConcurrentDictionary<int, SearchProduct>();
            int isNumber;
            string searchStringLower = searchString.ToLower();
            // skal alle produkter igennem = n (WorstCase) | n/2 (AverageCase)

            if (int.TryParse(searchStringLower, out isNumber))
            {
                if (AllProductsDictionary.Keys.Contains(isNumber))
                {
                    SearchProduct matchedProduct = new SearchProduct(AllProductsDictionary[isNumber]);
                    matchedProduct.NameMatch = 1000;
                    productsToReturn.TryAdd(isNumber, matchedProduct);
                }
            }
            // skal alle ord igennem, indtil der ikke er flere mellemrum, men kun i "søge strengen" 
            // n = antallet af mellemrum
            while (searchStringLower.Contains("  "))
            {
                searchStringLower = searchStringLower.Replace("  ", " ");
            }
            // Levensteins Søge Algoritme 
            foreach (BaseProduct product in AllProductsDictionary.Values)
            {
                NameSearch(searchStringLower, product);
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

        private void NameSearch(string searchStringElement, BaseProduct productToConvert)
        {
            SearchProduct productToAdd = new SearchProduct(productToConvert);

            List<string> searchSplit = searchStringElement.Split(' ').ToList();
            WordPairer(ref searchSplit);
            List<string> nameSplit = productToConvert.GetName().ToLower().Split(' ').ToList();
            WordPairer(ref nameSplit);
            foreach (string searchWord in searchSplit)
            {
                foreach (string nameWord in nameSplit)
                {
                    if (searchWord == nameWord)
                    {
                        productToAdd.NameMatch += 5;
                    }
                    else if (WithinLevenstheinLimit(searchWord, nameWord))
                    {
                        productToAdd.NameMatch += 1;
                    }
                }
            }
            weigthedSearchList.Add(productToAdd);
        }

        // Complexity = (n*s)/2
        private void WordPairer(ref List<string> listOfWords)
        {
            int searchAmount = listOfWords.Count();
            if (searchAmount > 1)
            {
                for (int i = 1; i <= searchAmount - 1; i++)
                {
                    listOfWords.Add(listOfWords[i] + listOfWords[i - 1]);
                    listOfWords.Add(listOfWords[i - 1] + listOfWords[i]);
                }
            }
        }

        private void GroupSearch(string searchString, BaseProduct product)
        {
            //divides all the elements in the string, to evaluate each element
            List<string> searchSplit = searchString.Split(' ').ToList();
            WordPairer(ref searchSplit);
            //matching on each element in the string
            //if the string contains a name of a group, or the string is matched, each product with the same group 
            //is added to the list of products to show.
            SearchProduct currentProduct = weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First();
            int groupID = (product is Product) ? (product as Product).ProductGroupID : (product as ServiceProduct).ServiceProductGroupID;
            List<string> groupSplit = GroupDictionary[groupID].Name.Split(' ').ToList();
            WordPairer(ref groupSplit);

            foreach (string searchWord in searchSplit)
            {
                foreach (string groupWord in groupSplit)
                {
                    if (searchWord == groupWord)
                    {
                        currentProduct.GroupMatch += 5;
                    }
                    else if (WithinLevenstheinLimit(searchWord, groupWord))
                    {
                        currentProduct.GroupMatch += 1;
                    }
                }
            }
        }

        private void BrandSearch(string searchString, BaseProduct product)
        {
            //divides all the elements in the string, to evaluate each element
            List<string> searchSplit = searchString.Split(' ').ToList();
            WordPairer(ref searchSplit);
            //checking all products to to match the brands with the searched string elements
            //matching on each element in the string
            //if the string contains a product brand, or the string is matched, each product with the same brand
            //is added to the list of products to show.
            SearchProduct currentProduct = weigthedSearchList.Where(x => x.CurrentProduct.ID == product.ID).First();
            if (product is Product)
            {
                List<string> brandSplit = (product as Product).Brand.Split(' ').ToList();
                WordPairer(ref brandSplit);
                foreach (string searchWord in searchSplit)
                {
                    foreach (string brandWord in brandSplit)
                    {
                        if (searchWord == brandWord)
                        {
                            currentProduct.BrandMatch += 5;
                        }
                        else if (WithinLevenstheinLimit(searchWord, brandWord))
                        {
                            currentProduct.BrandMatch += 1;
                        }
                    }
                }
            }
        }

        private bool WithinLevenstheinLimit(string searchEle, string ProductEle)
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