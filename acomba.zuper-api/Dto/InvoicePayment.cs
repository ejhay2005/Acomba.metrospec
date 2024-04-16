namespace acomba.zuper_api.Dto
{
    public class InvoicePayment
    {
        public string invoice_uid { get; set; }
        public string company_uid { get; set; }
        public string payment_uid { get; set; }
        public string done_by { get; set; }
        public double amount_paid { get; set; }
        public string? remarks { get; set; }

    }
}
