namespace acomba.zuper_api.Dto
{
    public class InvoiceRequest
    {
        public string? invoice_uid { get; set; }
        public Invoice? invoice { get; set; }
        public string? company_uid { get; set; }
        public DateTime? triggered_at { get; set; }
        public int? max_retries { get; set; }
    }
    public class Invoice
    {
        public string? customer { get; set; }
        public string? company_uid { get; set; }
        public CustomerBillingAddress? customer_billing_address { get; set; }
        public CustomerAddress? customer_service_address { get; set; }
        public string? job { get; set; }
        public string? prefix { get; set; }
        public string? reference_no { get; set; }
        public string? due_date { get; set; }
        public string? invoice_date { get; set; }
        public string? payment_term { get; set; }
        public string? remarks { get; set; }
        public List<string>? tags { get; set; }
        public string? template { get; set; }
        public string? estimate { get; set; }
        public List<LineItemDetails>? line_items { get; set; }
        public double? sub_total { get; set; }
        public List<Attachment>? attachments { get; set; }
        public List<NoteDetail>? notes { get; set; }
        public DiscountDetail? discount { get; set; }
        public List<TaxDetail>? tax { get; set; }
        public List<CustomFieldWebhook>? custom_fields { get; set; }
    }

    public class LineItemDetails
    {
        public string? product_id { get; set; }
        public string? product_uid { get; set; }
        public string? name { get; set; }
        public string? image { get; set; }
        public string? brand { get; set; }
        public string? specification { get; set; }
        public string? description { get; set; }
        public double? unit_price { get; set; }
        public int? quantity { get; set; }
        public double? discount { get; set; }
        public string? discount_type { get; set; }
        public string? location_uid { get; set; }
        public string? location_name { get; set; }
        public int? total { get; set; }
    }
    public class Attachment
    {
        public string? file_name { get; set; }
        public string? url { get; set; }
    }
    public class NoteDetail
    {
        public string? content { get; set; }
        public bool? is_private { get; set; }
    }
    public class DiscountDetail
    {
        public string? type { get; set; }
        public string? discount_label { get; set; }
        public string? discount_applicability { get; set; }
        public int? percent { get; set; }
        public string? value { get; set; }
    }
    public class TaxDetail
    {
        public string? tax_uid { get; set; }
    }
}
