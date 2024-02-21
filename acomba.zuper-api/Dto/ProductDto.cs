namespace acomba.zuper_api.Dto
{
    public class ProductResponse : ResponseData
    {
       public ProductDto data { get; set; }
    }
    public class Products : ResponseData
    {
        public List<ProductDto1> data { get; set; }
        
    }

    public class ProductCategory
    {
        public string category_uid { get; set; }
        public string category_name { get; set; }
    }

    public class ProductDto
    {
        public string? _id { get; set; }
        public string? product_uid { get; set; }
        public string? company_id { get; set; }
        public int? product_no { get; set; }
        public string? product_id { get; set; }
        public string? product_category { get; set; }
        public string? product_image { get; set; }
        public string? product_barcode { get; set; }
        public List<object>? product_files { get; set; }
        public string? product_name { get; set; }
        public string? product_description { get; set; }
        public string? product_type { get; set; }
        public List<meta_data>? meta_data { get; set; }
        public string? product_manual_link { get; set; }
        public List<Location>? location_availability { get; set; }
        public bool? track_quantity { get; set; }
        public int? quantity { get; set; }
        public int? min_quantity { get; set; }
        public string? currency { get; set; }
        public double? price { get; set; }
        public double? purchase_price { get; set; }
        public bool? has_custom_tax { get; set; }
        public bool? is_available { get; set; }
        //public bool? is_deleted { get; set; }
        public object? tax { get; set; }
    }
    public class ProductDto1
    {
        public string product_uid { get; set; }
        public string product_id { get; set; }
        public ProductCategory? product_category { get; set; }
        public string product_image { get; set; }
        public string product_barcode { get; set; }
        public List<object> product_files { get; set; }
        public string product_name { get; set; }
        public string product_description { get; set; }
        public string product_type { get; set; }
        public List<CustomField>? meta_data { get; set; }
        public string product_manual_link { get; set; }
        //public List<Location>? location_availability { get; set; }
        public bool track_quantity { get; set; }
        public int quantity { get; set; }
        public int min_quantity { get; set; }
        public string currency { get; set; }
        public int price { get; set; }
        public object purchase_price { get; set; }
        public bool has_custom_tax { get; set; }
        public bool is_available { get; set; }
        public bool is_deleted { get; set; }
        public CreatedBy created_by { get; set; }
        public DateTime created_at { get; set; }
        public DateTime updated_at { get; set; }
        public int product_no { get; set; }
        public string uom { get; set; }
        public string id { get; set; }
    }
    public class Product
    {
        public ProductDto product { get; set; }
    }
    public class Location
    {
        public string? location { get; set; }
        public int? quantity { get; set; }
        public int? min_quantity { get; set; }
        public List<string>? serial_nos { get; set; }
    }
    public class meta_data
    {
        public string? label { get; set; }
        public string? value { get; set; }
    }
}
