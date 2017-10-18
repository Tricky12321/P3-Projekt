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

        public List<Product> SearchForProduct(string searchedString)
        {
            bool wordIsMatched = false;
            List<string> produtNames = new List<string>();
            List<Product> productsToReturn = new List<Product>();
            

            foreach(Product p in ProductDictionary.Values)
            {
                if(p.Name == searchedString)
                {
                    productsToReturn.Add(p);
                }
                produtNames.Add(p.Name);
            }

            if (wordIsMatched == false)
            {

                return productsToReturn;
            }
            else
            {
                return productsToReturn;
            }
        }

        public int computeLevenshteinsDistance(string searchedString, Product productToCompare)
        {
            int searchStringLength = searchedString.Length;
            int productNameLength = productToCompare.Name.Length;
            int cost;
            int minimum1, minimum2, minimum3;
            int[,] d = new int[searchStringLength + 1, productNameLength + 1];

            if(string.IsNullOrEmpty(searchedString))
            {
                if (!string.IsNullOrEmpty(productToCompare.Name))
                {
                    return searchStringLength;
                }
                return 0;
            }
            
            if(string.IsNullOrEmpty(productToCompare.Name))
            {
                if (!string.IsNullOrEmpty(searchedString))
                {
                    return productNameLength;
                }
                return 0;
            }
            
            for(int i = 0; i <= d.GetUpperBound(0); ++i)
            {
                d[i, 0] = i;
            }

            for (int i = 0; i <= d.GetUpperBound(0); ++i)
            {
                d[0, i] = i;
            }

            for (int i = 0; i <= d.GetUpperBound(0); ++i)
            {
                for (int j = 0; j <= d.GetUpperBound(0); ++j)
                {
                    cost = Convert.ToInt32(!(searchedString[i - 1] == productToCompare.Name[j - 1]));

                    minimum1 = d[i - 1, j] + 1;
                    minimum2 = d[i, j - 1] + 1;
                    minimum3 = d[i - 1, j - 1] + cost;
                    d[i, j] = Math.Min(Math.Min(minimum1, minimum2), minimum3);
                }
            }

            return d[d.GetUpperBound(0), d.GetUpperBound(1)];

            
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

        public void EditStorageRoom(int ID, string name, string description)
        {
            StorageRoomDictionary[ID].Name = name;
            StorageRoomDictionary[ID].Description = description;
        }

        public void DeleteStorageRoom(int ID)
        {
            foreach(Product product in ProductDictionary.Values)
            {
                product.StorageWithAmount.Remove(StorageRoomDictionary[ID]);
            }
            StorageRoomDictionary.Remove(ID);
        }
    }
}
