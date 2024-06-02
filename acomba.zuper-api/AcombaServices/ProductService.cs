using acomba.zuper_api.Dto;
using AcoSDK;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System.Linq;
using System.Text.RegularExpressions;
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
        Task<object> GetAllProduct(int cardpos, int niche, int activateNumcard);
    }
    public class ProductService : IProductService
    {
        private readonly IConfiguration _configuration;
        private readonly IAcombaConnection _connection;
        private AcoSDK.Product003 productInt = new AcoSDK.Product003();
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
                    

                    var _insert = InsertProduct(product);
                    if (_insert == 0)
                    {
                        _connection.CloseConnection();
                        return "Added product completed successfully";

                    }
                    else
                    {
                        error = productInt.FreeCardNumber();
                        _connection.CloseConnection();

                        return "Error:" + Acomba.GetErrorMessage(error) + "Inserting Product failed.";
                    }
                }
                else
                {
                    error = productInt.FreeCardNumber();
                    _connection.CloseConnection();
                    return "Error:" + Acomba.GetErrorMessage(error) + "Reserve number error";
                }
            }
            catch (Exception ex)
            {

               var error = productInt.FreeCardNumber();
                _connection.CloseConnection();
                return ex.Message;
            }



        }

        private int InsertProduct(ProductDto product)
        {
            var _productGroup = !product.meta_data.Where(i => i.label == "Product Group").Any() ? "0" : product.meta_data.Where(i => i.label == "Product Group").FirstOrDefault().value;
            //var _productSupplier = !product.meta_data.Where(i => i.label == "Supplier Product").Any() ? string.Empty : product.meta_data.Where(i => i.label == "Supplier Product").FirstOrDefault().value;
            var _location = !product.meta_data.Where(i => i.label == "Location").Any() || string.IsNullOrEmpty(product.meta_data.Where(i => i.label == "Location").FirstOrDefault().value) ? "WAREHOUSE" : product.meta_data.Where(i => i.label == "Location").FirstOrDefault().value;
            var _productUpc = !product.meta_data.Where(i => i.label == "UPC").Any() ? string.Empty : product.meta_data.Where(i => i.label == "UPC").FirstOrDefault().value;
            var _productUnitCode = !product.meta_data.Where(i => i.label == "Unit Code").Any() ? string.Empty : product.meta_data.Where(i => i.label == "Unit Code").FirstOrDefault().value;
            string pattern = @"\d+";

            var _matchgroup = Regex.Match(_productGroup, pattern);

            productInt.PrNumber = product.product_id;
            productInt.PrActive = 1;

            productInt.PrSellingPrice[0, 1] = product.price == null ? 0 : product.price.Value;
            productInt.PrLocation = _location;
            productInt.PrProductGroupCP = GetProductGroupCardPos(Convert.ToInt32(_matchgroup.Value));
            productInt.PrSellingPrice[0,1] = product.price.Value;
            productInt.PrDescription[1] = product.product_description;
            productInt.PrTotalOnHand = product.quantity.Value;
            productInt.PrAvailableQty = product.quantity.Value;
            productInt.PrQtyOnHand = product.quantity.Value;
            productInt.PrTotalAvailableQty = product.quantity.Value;
            productInt.PrMaximumQty = product.quantity.Value;
            //productInt.PrProductSuppliersCP[1] = GetProductSupplierCardPos(_productSupplier);
            productInt.PrUPC = _productUpc;
            productInt.PrLongUnitCode = _productUnitCode;
           
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
        private int GetProductSupplierCardPos(string supplier)
        {
            var productSupplier = new AcoSDK.ProductSupplier();
            int error;
            int noOfIndex = 1;
            productSupplier.PKey_PSCatalogNumber = supplier;
           
            error = productSupplier.FindKey(noOfIndex, false);
            if(error == 0)
            {
                return productSupplier.Key_PSCardPos;
            }
            else
            {
                return 0;
            }
        }
        //private async Task AddProductSupplier()
        //{
        //    AcoSDK.Supplier _supplier = new AcoSDK.Supplier();

        //}
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
                        var _productGroup = !product.meta_data.Where(i => i.label == "Product Group").Any() ? "0" : product.meta_data.Where(i => i.label == "Product Group").FirstOrDefault().value;
                        var _location = !product.meta_data.Where(i => i.label == "Location").Any() || string.IsNullOrEmpty(product.meta_data.Where(i => i.label == "Location").FirstOrDefault().value) ? "WAREHOUSE" : product.meta_data.Where(i => i.label == "Location").FirstOrDefault().value;
                        //var _productSupplier = !product.meta_data.Where(i => i.label == "Supplier Product").Any() ? string.Empty : product.meta_data.Where(i => i.label == "Supplier Product").FirstOrDefault().value;
                        var _productUpc = !product.meta_data.Where(i => i.label == "UPC").Any() ? string.Empty : product.meta_data.Where(i => i.label == "UPC").FirstOrDefault().value;
                        var _productUnitCode = !product.meta_data.Where(i => i.label == "Unit Code").Any() ? string.Empty : product.meta_data.Where(i => i.label == "Unit Code").FirstOrDefault().value;
                        
                        string pattern = @"\d+";

                        var _matchgroup = Regex.Match(_productGroup, pattern);

                        productInt.PrSellingPrice[0, 1] = product.price == null ? 0 : product.price.Value;
                        productInt.PrLocation = _location;
                        productInt.PrProductGroupCP = GetProductGroupCardPos(Convert.ToInt32(_matchgroup.Value));
                        productInt.PrSellingPrice[0, 1] = product.price.Value;
                        productInt.PrDescription[1] = product.product_description;
                        productInt.PrTotalOnHand = product.quantity.Value;
                        productInt.PrAvailableQty = product.quantity.Value;
                        productInt.PrQtyOnHand = product.quantity.Value;
                        productInt.PrTotalAvailableQty = product.quantity.Value;
                        productInt.PrMaximumQty = product.quantity.Value;
                        //productInt.PrProductSuppliersCP[1] = GetProductSupplierCardPos(_productSupplier);
                        productInt.PrUPC = _productUpc;
                        productInt.PrLongUnitCode = _productUnitCode;

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
                    error = productInt.FreeCard();
                    return "Error :" + Acomba.GetErrorMessage(error);
                }
            }
            catch (Exception ex)
            {
                var error = productInt.FreeCard();
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

                        var _product = new ProductDto()
                        {

                        };
                        var _insert = InsertProduct(_product);
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
                _connection.OpenConnection();

                int total = 0; // number of products to import
                int cardpos = 2; //CardPos of the first product file to consult
                int error;
                total = productInt.NumCards();
                //int error;
                var customFields = new List<CustomField>();
                var productList = new List<ProductDto>();

                

                error = productInt.GetCards(cardpos, total);
                if(error == 0 || productInt.CursorUsed > 0)
                {
                    for (int i = 0; i < productInt.CursorUsed; i++)
                    {
                        productInt.Cursor = i;
                        if (productInt.PrStatus == 0)
                        {

                            var _custom = new List<meta_data>
                            {
                                new meta_data { label = "Product Group", value = productInt.PrProductGroupNumber.ToString() + " - " + GetProductGroupDetail(productInt.PrProductGroupNumber)},
                                new meta_data { label = "UPC", value = productInt.PrUPC},
                                new meta_data { label = "Location", value = "WAREHOUSE"},
                                new meta_data { label = "Unit Code", value = productInt.PrLongUnitCode}
                            };

                            var product = new ProductDto()
                            {
                                product_type = productInt.PrSortKey[1] == "Service" ? "SERVICE" : "PRODUCT",
                                product_id = productInt.PrNumber,
                                product_name = productInt.PrNumber,
                                product_description = string.IsNullOrEmpty(productInt.PrDescription[1]) ? string.Empty : productInt.PrDescription[1],
                                product_category = productInt.PrSortKey[1] == "Service" ? "78832e60-b2d3-11ed-855a-6be43bc97eab" : "781ddce0-b2d3-11ed-a148-6bb858e65421", //"784f2610-b2d3-11ed-884d-312422fc19c0", //default Product
                                is_available = true,
                                price = productInt.PrSellingPrice[0, 1],
                                track_quantity = true,
                                currency = "CAD",
                                quantity = productInt.PrQtyOnHand >= 0 ? 0 : Convert.ToInt32(productInt.PrQtyOnHand),
                                min_quantity = 2,
                                meta_data = _custom,
                                has_custom_tax = false,
                                tax = new { tax_exempt = false },
                                location_availability = new List<Location>() { new Location() { location = "597cdec0-b2d4-11ed-a148-6bb858e65421",quantity = productInt.PrQtyOnHand >= 0 ? 0 : Convert.ToInt32(productInt.PrQtyOnHand), min_quantity = 2} }
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
                return ex.Message.ToString();
            }
        }
        private string GetProductGroupDetail(int id)
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
            try
            {
                var results = new List<ResponseResult>();

                foreach (var e in _products)
                {
                    var _reqBody = new Dto.Product()
                    {
                        product = e
                    };

                    var _http = new HttpClient();
                    _http.DefaultRequestHeaders.Add("Accept", "application/json");
                    _http.DefaultRequestHeaders.Add("x-api-key", _configuration["MetricApiKey"]);
                    var response = await _http.PostAsJsonAsync($"{_configuration["ZuperUrl"]}/product", _reqBody);
                    var responseBody = response.Content.ReadAsStringAsync().Result;
                    var result = JsonConvert.DeserializeObject<ResponseResult>(responseBody);
                    results.Add(result);
                }

                return results;
            }
            catch(Exception ex)
            {
                throw new Exception($"Error in importing,{ex.Message}");
            }
           
        }
        #endregion
        #region GetAllProduct

        #endregion
        public async Task<object> GetAllProduct(int cardpos, int niche,int activateNumcard)
        {
            var results = new List<object>();
            try
            {
               
               
                //int error;

                _connection.OpenConnection();

                int total = 0; // number of products to import
                // int cardpos = 8; //CardPos of the first product file to consult
                var totalProduct = productInt.NumCards();
                int error;
                if (activateNumcard == 1)
                {
                    total = totalProduct;
                }
                else
                {
                    total = niche;
                }

                error = productInt.GetCards(cardpos, total);
                if (error == 0 || productInt.CursorUsed > 0)
                {
                    for (int i = 0; i < productInt.CursorUsed; i++)
                    {
                        
                        productInt.Cursor = i;
                        if (productInt.PrStatus == 0 || productInt.PrActive == 0 )
                        {
                            var _custom = new List<meta_data>
                            {
                                new meta_data { label = "Product Group", value = productInt.PrProductGroupNumber.ToString() + " - " + GetProductGroupDetail(productInt.PrProductGroupNumber)},
                                new meta_data { label = "UPC", value = productInt.PrUPC},
                                new meta_data { label = "Location", value = "WAREHOUSE"},
                                new meta_data { label = "Unit Code", value = productInt.PrLongUnitCode}
                            };

                            var product = new ProductDto()
                            {
                                product_type = productInt.PrSortKey[1] == "Service" ? "SERVICE" : "PRODUCT",
                                product_id = productInt.PrNumber,
                                product_name = productInt.PrNumber,
                                product_description = string.IsNullOrEmpty(productInt.PrDescription[1]) ? string.Empty : productInt.PrDescription[1],
                                product_category = productInt.PrSortKey[1] == "Service" ? "78832e60-b2d3-11ed-855a-6be43bc97eab" : "781ddce0-b2d3-11ed-a148-6bb858e65421", //"784f2610-b2d3-11ed-884d-312422fc19c0", //default Product
                                is_available = true,
                                price = productInt.PrSellingPrice[0, 1],
                                track_quantity = true,
                                currency = "CAD",
                                quantity = productInt.PrQtyOnHand >= 0 ? 0 : Convert.ToInt32(productInt.PrQtyOnHand),
                                min_quantity = 2,
                                meta_data = _custom,
                                has_custom_tax = false,
                                tax = new { tax_exempt = false },
                                location_availability = new List<Location>() { new Location() { location = "597cdec0-b2d4-11ed-a148-6bb858e65421", quantity = productInt.PrQtyOnHand >= 0 ? 0 : Convert.ToInt32(productInt.PrQtyOnHand), min_quantity = 2 } }
                            };
                            
                            results.Add(product);

                        }
                    }
                }
                else
                {
                    Console.WriteLine(error);
                }
                _connection.CloseConnection();
                return results;
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }
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