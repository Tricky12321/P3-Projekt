using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace P3_Projekt.Classes.Utilities
{
    public class StorageController
    {
        BoerglumAbbeyStorageandSale _boerglumAbbeyStorageandSale;

        private int _idGroupCounter = 0;

        public Dictionary<int, Product> ProductDictionary = new Dictionary<int, Product>();
        public Dictionary<int, Group> GroupDictionary = new Dictionary<int, Group>();
        public Dictionary<int, StorageRoom> StorageRoomDictionary = new Dictionary<int, StorageRoom>();

        public StorageController(BoerglumAbbeyStorageandSale boerglumAbbeyStorageandSale)
        {
            _boerglumAbbeyStorageandSale = boerglumAbbeyStorageandSale;

        }

        public void DeleteProduct(int ProductID)
        {
            ProductDictionary.Remove(ProductID);
        }

        public void DeleteGroup(int GroupID)
        {
            GroupDictionary.Remove(GroupID);
        }

        public void CreateProduct(string name, string brand, decimal purchasePrice, string group, bool discount, decimal discountPrice, Image image, params KeyValuePair<StorageRoom, int>[] storageRoomStockInput)
        {
            Product newProduct = new Product(name, brand, purchasePrice, group, discount, discountPrice, image);
            
            foreach(StorageRoom roomDictionary in StorageRoomDictionary.Values)
            {
                newProduct.StorageWithAmount.Add(roomDictionary, 0);
            }
            
            foreach(KeyValuePair<StorageRoom, int> roomInput in storageRoomStockInput)
            {
                newProduct.StorageWithAmount[roomInput.Key] = roomInput.Value;
            }

            ProductDictionary.Add(newProduct.ID, newProduct);
        }

        public void CreateStorageRoom(string name, string description)
        {
            StorageRoom newRoom = new StorageRoom(name, description);
            StorageRoomDictionary.Add(newRoom.ID, newRoom);

            foreach (Product product in ProductDictionary.Values)
            {
                product.StorageWithAmount.Add(newRoom, 0);
            }
        }
    }
}
