﻿namespace acomba.zuper_api.Dto
{
    public class ProductResponse : ResponseData
    {
        public List<ProductDto> data { get; set; }
    }

    public class ProductCategory
    {
        public string category_uid { get; set; }
        public string category_name { get; set; }
    }

    public class ProductDto
    {
        public string product_uid { get; set; }
        public string product_id { get; set; }
        public ProductCategory product_category { get; set; }
        public string product_image { get; set; }
        public string product_barcode { get; set; }
        public List<object> product_files { get; set; }
        public string product_name { get; set; }
        public string product_description { get; set; }
        public string product_type { get; set; }
        public List<object> meta_data { get; set; }
        public string product_manual_link { get; set; }
        public List<object> location_availability { get; set; }
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
}
