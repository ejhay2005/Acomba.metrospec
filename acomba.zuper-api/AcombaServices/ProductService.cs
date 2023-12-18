using acomba.zuper_api.Dto;
using AcoSDK;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Newtonsoft.Json;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.AcombaServices
{
    public interface IProductService 
    {
        Task<string> AddProduct(ProductDto product);
        Task<string> UpdateProduct(ProductDto product);
        Task<object> GetProduct(string _productId);
        Task<List<string>> ImportProducts(List<ProductDto1> products);
        Task<object> ImportProductsToZuper();
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
            if(price != null || price != 0)
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
            if(qty != null || qty != 0)
            {
                productInt.PrMaximumQty = qty.Value;
                
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
        public async Task<List<string>> ImportProducts(List<ProductDto1> products)
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
                        var gpNumber = p.meta_data.Count() == 0 ? string.Empty : p.meta_data.Where(i => i.label == "Group").FirstOrDefault().value.ToString();
                        var upc = p.meta_data.Count() == 0 ? string.Empty : p.meta_data.Where(i => i.label== "Upc").FirstOrDefault().value.ToString();
                        var qty = p.quantity == 0 ? 1 : p.quantity;
                        var _insert = InsertProduct(p.product_id, gpNumber, p.product_name, qty, p.price, upc);
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
        #region Import Product to Zuper
        public async Task<object> ImportProductsToZuper()
        {
            try
            {
                int count = 1000; // number of products to import
                int cardpos = 1; //CardPos of the first customer file to consult
                int error;
                var customFields = new List<CustomField>();
                var productList = new List<ProductDto>();

                _connection.OpenConnection();

                error = productInt.GetCards(cardpos, count);
                if(error == 0 || productInt.CursorUsed > 0)
                {
                    for (int i = 0; i < productInt.CursorUsed; i++)
                    {
                        productInt.Cursor = i;
                        if (productInt.PrStatus == 0)
                        {

                            var product = new ProductDto()
                            {
                                product_type = GetProductGroupDesc(productInt.PrProductGroupNumber),
                                product_id = productInt.PrNumber,
                                product_name = productInt.PrDescription[1],
                                price = productInt.PrSellingPrice[0, 1],
                                track_quantity = true,
                                quantity = Convert.ToInt32(productInt.PrQtyOnHand),
                                min_quantity = Convert.ToInt32(productInt.PrMaximumQty)
                            };
                            productList.Add(product);

                        }
                    }
                }
                _connection.CloseConnection();
                var results = await ImportToZuper(productList);
                return results;
            }
            catch(Exception ex)
            {
                _connection.CloseConnection();
                return ex.Message;
            }
        }
        private string GetProductGroupDesc(int id)
        {
            var productGroupInt = new AcoSDK.ProductGroup();
            int error,cardpos;

            productGroupInt.BlankKey();
            productGroupInt.PKey_PGNumber = id;
            error = productGroupInt.FindKey(1, false);
            if(error == 0)
            {
                cardpos = productGroupInt.Key_PGCardPos;
                error = productGroupInt.GetCard(cardpos);

                if(error == 0)
                {
                    return productGroupInt.PGDescription;
                }
                else
                {
                    return "";
                }
            }
            else
            {
                return "";
            }
            
        }
        private async Task<List<ResponseResult>> ImportToZuper(List<ProductDto> _products)
        {
            var results = new List<ResponseResult>();

            foreach (var e in _products)
            {
                var _http = new HttpClient();
                _http.DefaultRequestHeaders.Add("Accept", "application/json");
                _http.DefaultRequestHeaders.Add("x-api-key", _configuration["MetricApiKey"]);
                var response = await _http.PostAsJsonAsync($"{_configuration["ZuperUrl"]}/product", e);
                var responseBody = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ResponseResult>(responseBody);
                results.Add(result);
            }

            return results;
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