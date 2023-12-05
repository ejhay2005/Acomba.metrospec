﻿using acomba.zuper_api.Dto;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.AcombaServices
{
    public interface IProductService 
    {
        Task<string> AddProduct(ProductDto product);
        Task<string> UpdateProduct(ProductDto product);
        Task<object> GetProduct(string _productId);
        Task<List<string>> ImportProducts(List<ProductDto> products);
    }
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly IAcombaConnection _connection;
        private AcoSDK.Product productInt = new AcoSDK.Product();
        private AcoSDK.AcombaX Acomba = new AcoSDK.AcombaX();
        public ProductService(IConfiguration configuration, IAcombaConnection acombaConnection)
        {
            _configuration = configuration;
            _connection = acombaConnection;
        }
        #region Add Product
        public async Task<string> AddProduct(ProductDto product)
        {
            try
            {
                _connection.OpenConnection();
                int error;
                //Open connection

                //initialize property
                productInt.BlankKey();
                productInt.BlankCard();

                //set primarykey
                productInt.PKey_PrNumber = product.product_id;
                error = productInt.ReserveCardNumber();
                if (error == 0)
                {

                    var _insert = InsertProduct(product.product_id, product.meta_data[1].value, product.product_name, product.quantity.Value, product.price.Value ,product.meta_data[2].value);
                    if (_insert == 0)
                    {
                        _connection.CloseConnection();
                        return "Added product completed successfully";

                    }
                    else
                    {
                        error = productInt.FreeCardNumber();
                        _connection.CloseConnection();

                        return "Error:" + Acomba.GetErrorMessage(error);
                    }
                }
                else
                {
                    error = productInt.FreeCardNumber();
                    _connection.CloseConnection();
                    return "Error:" + Acomba.GetErrorMessage(error);
                }
            }
            catch (Exception ex)
            {

                _connection.CloseConnection();
                return ex.Message;
            }



        }

        private int InsertProduct(string productNumber, string productGroupNumber, string desc, int? qty,double? price,string upc)
        {

            productInt.PrNumber = productNumber;
            productInt.PrActive = 1;
            if(price != null)
            {
                productInt.PrSellingPrice[0,1] = price.Value;
            }
            
            if (!string.IsNullOrEmpty(upc))
            {
                productInt.Key_PrUPC = upc;
            }
            if (!string.IsNullOrEmpty(productGroupNumber))
            {
                productInt.PrProductGroupCP = GetProductGroupCardPos(Convert.ToInt32(productGroupNumber));
            }
            if (!string.IsNullOrEmpty(desc))
            {
                productInt.PrDescription[1] = desc;
            }
            if(qty != null)
            {
                productInt.PrQtyOnHand = qty.Value;
            }
           

            var error = productInt.AddCard();

            return error;


        }
        private int GetProductGroupCardPos(int productGroupNumber)
        {
            AcoSDK.ProductGroup productGroupInt = new AcoSDK.ProductGroup();
            int error;
            int noOfIndex = 1;
            productGroupInt.PKey_PGNumber = productGroupNumber;
            error = productGroupInt.FindKey(noOfIndex, false);
            if (error == 0)
            {
                return productGroupInt.Key_PGCardPos;
            }
            else
            {
                return 0;
            }
        }
        #endregion
        #region Update Product 
        public async Task<string> UpdateProduct(ProductDto product)
        {
            try
            {
                _connection.OpenConnection();
                int error, cardPos, FreeIt;
                const int noIndex = 1;

                productInt.BlankKey();
                productInt.PKey_PrNumber = product.product_id;

                //Searching for the product to obtain their CardPos
                error = productInt.FindKey(noIndex, false);
                if (error == 0)
                {
                    cardPos = productInt.Key_PrCardPos;
                    //check the product found
                    error = productInt.ReserveCard(cardPos);
                    if (error == 0)
                    {
                        if (string.IsNullOrEmpty(product.meta_data[1].value))
                        {
                            productInt.PrProductGroupCP = GetProductGroupCardPos(Convert.ToInt32(product.meta_data[1].value));
                        }
                        if (product.price != null)
                        {
                            productInt.PrSellingPrice[0, 1] = product.price.Value;
                        }

                        if (!string.IsNullOrEmpty(product.meta_data[2].value))
                        {
                            productInt.Key_PrUPC = product.meta_data[2].value;
                        }
                        productInt.PrDescription[1] = product.product_name;
                        productInt.PrQtyOnHand = product.quantity.Value;
                        productInt.PrActive = product.is_available != true ? 0 : 1;
                        
                        FreeIt = 1;
                        error = productInt.ModifyCard(true);

                        if (error == 0)
                        {
                            return "Update product successfully.";
                        }
                        else
                        {
                            error = productInt.FreeCard();
                            return "Error : " + Acomba.GetErrorMessage(error);
                        }
                    }
                    else
                    {
                        return "Error :" + Acomba.GetErrorMessage(error);
                    }
                }
                else
                {
                    return "Error :" + Acomba.GetErrorMessage(error);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
        #region Get Product
        public async Task<object> GetProduct(string _productId)
        {
            try
            {
                _connection.OpenConnection();
                int error, cardPos;
                const int noIndex = 1;

                productInt.BlankKey();
                productInt.PKey_PrNumber = _productId;

                //Searching for the product to obtain their CardPos
                error = productInt.FindKey(noIndex, false);
                if (error == 0)
                {
                    cardPos = productInt.Key_PrCardPos;
                    //check the product found
                    error = productInt.GetCard(cardPos);
                    if (error == 0)
                    {
                        var _prod = new AcombaProduct()
                        {
                            ProductId = productInt.PrNumber,
                            ProductName = productInt.PrDescription[1],
                            SellingPrice = productInt.PrSellingPrice[0,1],
                            Quantity = productInt.PrQtyOnHand
                        };

                        return _prod;
                    }
                    else
                    {
                        return "Error :" + Acomba.GetErrorMessage(error);
                    }
                }
                else
                {
                    return "Error :" + Acomba.GetErrorMessage(error);
                }
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        #endregion
        #region Import Product From Zuper to Acomba
        public async Task<List<string>> ImportProducts(List<ProductDto> products)
        {
            try
            {
                
                //Open connection
                _connection.OpenConnection();

                int error;
                var results = new List<string>();
                foreach (var p in products)
                {
                    //initialize property
                    productInt.BlankKey();
                    productInt.BlankCard();

                    //set primarykey
                    productInt.PKey_PrNumber = p.product_id;
                    error = productInt.ReserveCardNumber();
                    if (error == 0)
                    {
                        var _insert = InsertProduct(p.product_id, p.meta_data[1].value, p.product_name, p.quantity.Value, p.price.Value, p.meta_data[2].value);
                        if (_insert == 0)
                        {
                            string res = "Product: " + p.product_id + " successfully added.";
                            results.Add(res);
                        }
                        else
                        {
                            string res = "Failed to insert Product: " + p.product_id + "Error : " + Acomba.GetErrorMessage(_insert);
                            results.Add(res);
                            error = productInt.FreeCardNumber();
                        }
                    }
                    else
                    {
                        string res = "Error :" + Acomba.GetErrorMessage(error);
                        results.Add(res);
                    }

                }
                _connection.CloseConnection();
                return results;
            }
            catch (Exception ex)
            {
                _connection.CloseConnection();
                throw;
            }





        }
        #endregion
    }
}
//try
//{
//    _connection.OpenConnection();
//    int error, cardPos, FreeIt;
//    const int noIndex = 1;

//    productInt.BlankKey();
//    productInt.PKey_PrNumber = product.product_id;

//    //Searching for the product to obtain their CardPos
//    error = productInt.FindKey(noIndex, false);
//    if (error == 0)
//    {
//        cardPos = productInt.Key_PrCardPos;
//        //check the product found
//        error = productInt.ReserveCard(cardPos);
//        if (error == 0)
//        {
//            if (string.IsNullOrEmpty(product.meta_data[2].value))
//            {
//                productInt.PrProductGroupCP = GetProductGroupCardPos(Convert.ToInt32(product.meta_data[2].value));
//            }
//            productInt.PrDescription[1] = product.product_name;
//            productInt.PrMaximumQty = product.quantity.Value;
//            productInt.PrActive = product.is_available != true ? 0 : 1;
//            FreeIt = 1;
//            error = productInt.ModifyCard(true);

//            if (error == 0)
//            {
//                return "Update product successfully.";
//            }
//            else
//            {
//                error = productInt.FreeCard();
//                return "Error : " + Acomba.GetErrorMessage(error);
//            }
//        }
//        else
//        {
//            return "Error :" + Acomba.GetErrorMessage(error);
//        }
//    }
//    else
//    {
//        return "Error :" + Acomba.GetErrorMessage(error);
//    }