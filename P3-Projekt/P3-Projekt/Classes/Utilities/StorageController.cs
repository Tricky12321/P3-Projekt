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
            
        //Move products from deleted group to new group?
        //Group with ID 0 for products with no group
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

        public void CreateProduct(string name, string brand, decimal purchasePrice, Group group, bool discount, decimal discountPrice, decimal salePrice, Image image, params KeyValuePair<StorageRoom, int>[] storageRoomStockInput)
        {
            Product newProduct = new Product(name, brand, purchasePrice, group, discount, salePrice, discountPrice, image);
            
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

        public void EditProduct(bool isAdmin, Product editProduct, string name, string brand, decimal purchasePrice, decimal salePrice, Group group, bool discount, decimal discountPrice, Image image)
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


        /* User has already found the matching product ID.
         * First line findes the store storage
         * Second line subtracts the amound sold from storage*/
        public void MergeTempProduct(SaleTransaction tempProductTransaction, int matchedProductID)
        {
            var StoreStorage = ProductDictionary[matchedProductID].StorageWithAmount.Where(x => x.Key.ID == 0).First();

            ProductDictionary[matchedProductID].StorageWithAmount[StoreStorage.Key] -= tempProductTransaction.Amount;
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

        public void EditStorageRoom(int id, string name, string description)
        {
            StorageRoomDictionary[id].Name = name;
            StorageRoomDictionary[id].Description = description;
        }

        public void DeleteStorageRoom(int id)
        {
            foreach(Product product in ProductDictionary.Values)
            {
                product.StorageWithAmount.Remove(StorageRoomDictionary[id]);
            }
            StorageRoomDictionary.Remove(id);
        }
    }
}
