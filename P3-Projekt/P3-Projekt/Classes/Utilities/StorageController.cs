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

        /////////--------------------SEARCH---------------------------------
        public List<Product> SearchForProduct(string searchedString)
        {
            bool wordIsMatched = false;
            List<string> produtNames = new List<string>();
            List<Product> productsToReturn = new List<Product>();

            foreach (Product p in ProductDictionary.Values)                     //checks if the searched string is
            {                                                                   //matching with a product name.
                if (p.Name == searchedString)
                {
                    productsToReturn.Add(p);
                    wordIsMatched = true;
                }
                produtNames.Add(p.Name);
            }

            if (!wordIsMatched)                                                 //if af matching name is not found
            {                                                                   //the string will undergo different 
                foreach (Product p in ProductDictionary.Values)                 //searching methods.
                {
                    levenshteinsSearch(searchedString, p, ref productsToReturn);    //levenshteins will try to autocorrect the string
                                                                                    //and suggest items with similar names to the string.
                    groupSearch(p);
 
                }
                return productsToReturn;
            }
            else
            {
                return productsToReturn;        //will be called if a matching word is found
            }
        }

        //----Levensthein---------------------
        public void levenshteinsSearch(string searchedString, Product productCheck, ref List<Product> productsToReturn)//setup for levenshteins
        {
            int charDifference = computeLevenshteinsDistance(searchedString, productCheck); //getting the chardifference between the searchedstring and the productname
            if (evaluateStringLimit(searchedString, productCheck.Name, charDifference)) //Evaluate if the chardifference is in between the changelimit of the string
            {
                productsToReturn.Add(productCheck);
            }
        }

        public int computeLevenshteinsDistance(string searchedString, Product productToCompare)
        {
            int searchStringLength = searchedString.Length;                     //searchString Length
            int productNameLength = productToCompare.Name.Length;               //productname Length
            int cost;
            int minimum1, minimum2, minimum3;
            int[,] d = new int[searchStringLength + 1, productNameLength + 1];  //size of int array

            if (string.IsNullOrEmpty(searchedString))                           //--------------
            {                                                                   //Checks if the strings
                if (!string.IsNullOrEmpty(productToCompare.Name))               //are empty
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
                return 0;                                                       //-------------
            }

            for (int i = 0; i <= d.GetUpperBound(0); ++i)                       //GetUpperBound gets the index of the last
            {                                                                   //element in the array.
                d[i, 0] = i;
            }                                                                   //(0) and (1) is to differentiate between the
                                                                                //first and second element in the array.
            for (int i = 0; i <= d.GetUpperBound(1); ++i)
            {
                d[0, i] = i;
            }

            for (int i = 1; i <= d.GetUpperBound(0); ++i)
            {
                for (int j = 1; j <= d.GetUpperBound(1); ++j)
                {
                    cost = Convert.ToInt32(!(searchedString[i - 1] == productToCompare.Name[j - 1]));   //will convert a boolean to int, depending if a char is different
                                                                                                        //different between the two strings.

                    minimum1 = d[i - 1, j] + 1;                 //takes the element in the previous row i
                    minimum2 = d[i, j - 1] + 1;                 //takes the element in the previous column j
                    minimum3 = d[i - 1, j - 1] + cost;          //takes the element in the previous column j and previos row i, and adds the cost of changing a char, +1 og or +0
                    d[i, j] = Math.Min(Math.Min(minimum1, minimum2), minimum3);                         //the minmum of the 3 will be put into the 2-dimensial array at row i column j                 
                    
                    //for a array example, see step 1-7 https://people.cs.pitt.edu/~kirk/cs1501/Pruhs/Spring2006/assignments/editdistance/Levenshtein%20Distance.htm
                }
            }
            return d[d.GetUpperBound(0), d.GetUpperBound(1)];   //returns the value of the last coloumn and last row of the array, which is the amount that is needed to change between the words.


        }

        public bool evaluateStringLimit(string searchedString, string productName, int charDiff)
        {
            int limitOfChanges;
            int searchedStringLength = searchedString.Length;
            if (searchedStringLength < 5)                                       //if string Length is under 5
            {                                                                   //the max changes to the string is 1
                limitOfChanges = 1;
                if (limitOfChanges == charDiff)
                {
                    return true;
                }
            }
            else if (searchedStringLength < 10 && searchedStringLength >= 5)    //string length is between 5 and 10
            {                                                                   //the max changes to the string is 3
                limitOfChanges = 3;
                if (limitOfChanges >= charDiff)
                {
                    return true;
                }
            }
            else if (searchedStringLength >= 10 && searchedStringLength < 20)   //string length is between 10 and 20
            {                                                                   //the max changes to the string is 6
                limitOfChanges = 6;
                if (limitOfChanges >= charDiff)
                {
                    return true;
                }
            }
            else if (searchedStringLength >= 20)                                //string length is over 20
            {                                                                   //the max changes tot he string is 9
                limitOfChanges = 9;
                if (limitOfChanges >= charDiff)
                {
                    return true;
                }
            }
            return false;             //if none of the criteria is true, there is no matches for the product.
        }
        //----LevenSthein-END-----------------------

        public void groupSearch(Product productToMatchGroup)
        {

        }




        //----SEARCH-END----------------------------

        public void CreateProduct(string name, string brand, decimal purchasePrice, string group, bool discount, decimal discountPrice, Image image, params KeyValuePair<StorageRoom, int>[] storageRoomStockInput)
        {
            Product newProduct = new Product(name, brand, purchasePrice, group, discount, discountPrice, image);

            foreach (StorageRoom roomDictionary in StorageRoomDictionary.Values)
            {
                newProduct.StorageWithAmount.Add(roomDictionary, 0);
            }

            foreach (KeyValuePair<StorageRoom, int> roomInput in storageRoomStockInput)
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
            foreach (Product product in ProductDictionary.Values)
            {
                product.StorageWithAmount.Remove(StorageRoomDictionary[ID]);
            }
            StorageRoomDictionary.Remove(ID);
        }
    }
}
