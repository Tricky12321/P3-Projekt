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
        public List<TempProduct> TempProductList = new List<TempProduct>();

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

        /////////--------------------SEARCH---------------------------------
        public List<Product> SearchForProduct(string searchedString)
        {
            bool wordIsMatched = false;
            List<string> produtNames = new List<string>();
            List<Product> productsToReturn = new List<Product>();

            //checks if the searched string is
            //matching with a product name.
            foreach (Product p in ProductDictionary.Values)                     
            {                                                                   
                if (p.Name == searchedString)
                {
                    productsToReturn.Add(p);
                    wordIsMatched = true;
                }
                produtNames.Add(p.Name);
            }

            //if af matching name is not found the string will undergo different searching methods.
            if (!wordIsMatched)                                                 
            {                                                                   
                foreach (Product p in ProductDictionary.Values)                 
                {
                    //levenshteins will try to autocorrect the string and suggest items with similar names to the string
                    LevenshteinsSearch(searchedString, p, ref productsToReturn);    
                                                                                   
                    groupSearch(p);
 
                }
                return productsToReturn;
            }
            else
            {
                //will be called if a matching word is found
                return productsToReturn;       
            }
        }

        //----Levensthein---------------------
        public void LevenshteinsSearch(string searchedString, Product productCheck, ref List<Product> productsToReturn)
        {//setup for levenshteins
            //getting the chardifference between the searchedstring and the productname
            int charDifference = ComputeLevenshteinsDistance(searchedString, productCheck);
            //Evaluate if the chardifference is in between the changelimit of the string
            if (EvaluateStringLimit(searchedString, charDifference)) 
            {
                productsToReturn.Add(productCheck);
            }
        }

        public int ComputeLevenshteinsDistance(string searchedString, Product productToCompare)//tested
        {
            //searchString Length
            int searchStringLength = searchedString.Length;
            //productname Length       
            int productNameLength = productToCompare.Name.Length;               
            int cost;
            int minimum1, minimum2, minimum3;
            //size of int array
            int[,] d = new int[searchStringLength + 1, productNameLength + 1];
            //--------------
            //Checks if the strings are empty
            if (string.IsNullOrEmpty(searchedString))                           
            {                                                                  
                if (!string.IsNullOrEmpty(productToCompare.Name))               
                {
                    return searchStringLength;
                }
                return 0;
            }

            if (string.IsNullOrEmpty(productToCompare.Name))
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
                    cost = Convert.ToInt32(!(searchedString[i - 1] == productToCompare.Name[j - 1]));   
                                                                                                        

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
            //if string Length is under 5 the max changes to the string is 1
            if (searchedStringLength < 5)
            {                                                                   
                limitOfChanges = 1;
                if (limitOfChanges == charDiff)
                {
                    return true;
                }
            }
            //string length is between 5 and 10 the max changes to the string is 3
            else if (searchedStringLength < 10 && searchedStringLength >= 5)    
            {                                                                   
                limitOfChanges = 3;
                if (limitOfChanges >= charDiff)
                {
                    return true;
                }
            }
            //string length is between 10 and 20 the max changes to the string is 6
            else if (searchedStringLength >= 10 && searchedStringLength < 20)   
            {                                                                   
                limitOfChanges = 6;
                if (limitOfChanges >= charDiff)
                {
                    return true;
                }
            }
            //string length is over 20 the max changes tot he string is 9
            else if (searchedStringLength >= 20)                               
            {                                                                   
                limitOfChanges = 9;
                if (limitOfChanges >= charDiff)
                {
                    return true;
                }
            }
            //if none of the criteria is true, there is no matches for the product.
            return false;            
        }
        //----LevenSthein-END-----------------------

        public void groupSearch(Product productToMatchGroup)
        {

        }
        //----SEARCH-END---------------------

        //Creates product with storage and stocka as keyvalue, then add the product to the list
        public void CreateProduct(string name, string brand, decimal purchasePrice, Group group, bool discount, decimal discountPrice, decimal salePrice, Image image, params KeyValuePair<StorageRoom, int>[] storageRoomStockInput)
        {
            Product newProduct = new Product(name, brand, purchasePrice, group, discount, salePrice, discountPrice, image);
            
            foreach(KeyValuePair<StorageRoom, int> roomInput in storageRoomStockInput)
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
            foreach (Product product in ProductDictionary.Values)
            {
                product.StorageWithAmount.Remove(StorageRoomDictionary[id]);
            }
            StorageRoomDictionary.Remove(id);
        }
    }
}
