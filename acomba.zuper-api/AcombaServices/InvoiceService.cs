using acomba.zuper_api.Dto;
using AcoSDK;
using AcoX0114;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Immutable;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.AcombaServices
{
    public interface IinvoiceService
    {
        Task<string> AddInvoice(InvoiceRequest invoiceRequest);
        Task<string> AddInvoiceWebhook(InvoiceRequest invoiceRequest,CustomerDto _customer);
        Task<object> GetInvoice(string invoiceId);
        Task<object> GetInvoices();
        Task<string> CustomerPayment(InvoicePayment _payment, CustomerDto _customer);
    }
    public class InvoiceService : IinvoiceService
    {
        private readonly IConfiguration _configuration;
        private readonly IAcombaConnection _connection;
        private AcoSDK.Transaction027 _transactionInt = new AcoSDK.Transaction027();
        private AcoSDK.Invoicing _invoicingInt = new AcoSDK.Invoicing();
        private AcoSDK.CustomerTerm customerTermInt = new AcoSDK.CustomerTerm();
        private AcoSDK.ControlCustomer controlCustomerInt = new AcoSDK.ControlCustomer();
        private AcoSDK.Tax taxInt = new AcoSDK.Tax();
        private AcoSDK.AcombaX Acomba = new AcoSDK.AcombaX();
        
        private int error;
        public InvoiceService(IConfiguration configuration, IAcombaConnection connection)
        {
            _configuration = configuration;
            _connection = connection;
        }

        #region Add Invoice

        public async Task<string> AddInvoiceWebhook(InvoiceRequest invoiceRequest, CustomerDto _customer)
        {
            try
            {
                _connection.OpenConnection();
                int error;
               
                AcoSDK.Product productInt = new AcoSDK.Product();
               
               

                string customerNumber;
                bool productFind;
                double taxeFederale;
                int r = 1;

                var _description = !invoiceRequest.invoice.custom_fields.Where(i => i.label == "Description").Any() ? null : invoiceRequest.invoice.custom_fields.FirstOrDefault(i => i.label == "Description").value;
                var _po = invoiceRequest.invoice.custom_fields.Where(i => i.label == "PO").Any() ? null : invoiceRequest.invoice.custom_fields.FirstOrDefault(i => i.label == "PO").value;
                var _terms = invoiceRequest.invoice.custom_fields.Where(i => i.label == "Terms").Any() ? null : invoiceRequest.invoice.custom_fields.FirstOrDefault(i => i.label == "Terms").value;
                var _invoiceId = invoiceRequest.invoice.custom_fields.Where(i => i.label == "Acomba Invoice ID").Any() ? null : invoiceRequest.invoice.custom_fields.FirstOrDefault(i => i.label == "Acomba Invoice ID").value;
                var _tvq = !invoiceRequest.invoice.tax.Where(i => i.tax_name == "TVQ").Any() ? 0 : invoiceRequest.invoice.tax.Where(i => i.tax_name == "TVQ").FirstOrDefault().tax_amount.Value;
                var _tps = !invoiceRequest.invoice.tax.Where(i => i.tax_name == "TPS").Any() ? 0 : invoiceRequest.invoice.tax.Where(i => i.tax_name == "TPS").FirstOrDefault().tax_amount.Value;
                _transactionInt.BlankCard();

                error = controlCustomerInt.GetCard(1);

                if (error == 0)
                {
                    
                    _transactionInt.InInvoiceNumber = _invoiceId;
                    _transactionInt.InInvoiceType = InvoicingType.ITp_Invoice;
                    _transactionInt.InReference = invoiceRequest.invoice.reference_no;
                    _transactionInt.InDescription = _description;
                    _transactionInt.InCurrentDay = 1;
                    _transactionInt.InTransactionActive = 1;
                    _transactionInt.InInvoiceSubTotal = invoiceRequest.invoice.sub_total.Value;
                    _transactionInt.InSales_Taxes[991] = _tvq + _tps;
                   
                    
                    _transactionInt.InInvoiceTotal = invoiceRequest.invoice.total == null ? invoiceRequest.invoice.sub_total.Value : invoiceRequest.invoice.total.Value;
                    customerNumber =_customer.custom_fields.Where(i => i.label.Contains("Project")).FirstOrDefault().value;
                  
                    //_transactionInt.InReceivableOffset = 1;

                    _transactionInt.InCustomerSupplierCP = GetCustomerCardPos(customerNumber);
                    if (_transactionInt.InCustomerSupplierCP > 0)
                    {
                        GetCustomerInfo(_transactionInt.InCustomerSupplierCP);


                        for (int i = 0; i < invoiceRequest.invoice.line_items.Count(); i++)
                        {
                            _transactionInt.TANumLines++;
                            _transactionInt.ILType[_transactionInt.TANumLines] = InvoicingLineType.IL_Invoice;

                            _transactionInt.ILLineNumber[_transactionInt.TANumLines] = _transactionInt.TANumLines;



                            _transactionInt.ILProductNumber[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].product_id;

                            _transactionInt.ILProductCP[_transactionInt.TANumLines] = GetProductCardPos(_transactionInt.ILProductNumber[_transactionInt.TANumLines]);

                            productFind = false;

                            if (_transactionInt.ILProductCP[_transactionInt.TANumLines] > 0)
                            {
                                productInt.BlankCard();

                                error = productInt.GetCard(_transactionInt.ILProductCP[_transactionInt.TANumLines]);
                                if (error == 0)
                                {
                                    _transactionInt.ILDescription[_transactionInt.TANumLines] = productInt.PrDescription[1];
                                    _transactionInt.ILSellingPrice[_transactionInt.TANumLines] = productInt.PrSellingPrice[0, 1];
                                   _transactionInt.ILInvoicedQty[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].quantity.Value;
                                    _transactionInt.ILOrderedQty[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].quantity.Value;
                                    _transactionInt.ILTotalAmount[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].total;
                                    _transactionInt.ILProductGroupCP[_transactionInt.TANumLines] = productInt.PrProductGroupCP;
                                    _transactionInt.ILProjectNumber[_transactionInt.TANumLines] = customerNumber;
                                    _transactionInt.ILTaxesIncluded[_transactionInt.TANumLines] = 0;
                                    _transactionInt.ILProductCPLinkedToSerialNumber[_transactionInt.TANumLines] = 0;
                                    productFind = true;
                                }
                            }

                            if (!productFind)
                            {
                                
                                _transactionInt.ILDescription[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].name;
                                _transactionInt.ILSellingPrice[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].unit_price.Value;
                                _transactionInt.ILOrderedQty[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].quantity.Value;
                                _transactionInt.ILInvoicedQty[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].quantity.Value;
                                _transactionInt.ILTotalAmount[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].total;
                                _transactionInt.ILProductGroupCP[_transactionInt.TANumLines] = GetProductGroupCardPos(600);
                                _transactionInt.ILProjectNumber[_transactionInt.TANumLines] = customerNumber;

                            }
                        }
                        if(_tps != 0)
                        {
                            _transactionInt.TANumLines++;
                            _transactionInt.ILType[_transactionInt.TANumLines] = InvoicingLineType.IL_Tax;
                            _transactionInt.ILLineNumber[_transactionInt.TANumLines] = 991;
                            _transactionInt.ILProductNumber[_transactionInt.TANumLines] = "G.S.T.";
                           
                            taxInt.BlankKey();

                            taxInt.PKey_TaNumber = "G.S.T.";

                            error = taxInt.FindKey(1, false);
                            if(error == 0)
                            {
                                _transactionInt.ILProductCP[_transactionInt.TANumLines] = taxInt.Key_TaCardPos;
                                _transactionInt.ILCharterCP[_transactionInt.TANumLines] = taxInt.TaCollectedTaxesCharterCP;
                                _transactionInt.ILTotalAmount[_transactionInt.TANumLines] = _tps;
                            }
                            else
                            {
                                return "Error in tax :" + Acomba.GetErrorMessage(error);
                            }
                        }
                        if (_tvq != 0)
                        {
                            _transactionInt.TANumLines++;
                            _transactionInt.ILType[_transactionInt.TANumLines] = InvoicingLineType.IL_Tax;
                            _transactionInt.ILLineNumber[_transactionInt.TANumLines] = 992;
                            _transactionInt.ILProductNumber[_transactionInt.TANumLines] = "Q.S.T.";

                            taxInt.BlankKey();

                            taxInt.PKey_TaNumber = "Q.S.T.";

                            error = taxInt.FindKey(1, false);
                            if (error == 0)
                            {
                                _transactionInt.ILProductCP[_transactionInt.TANumLines] = taxInt.Key_TaCardPos;
                                _transactionInt.ILCharterCP[_transactionInt.TANumLines] = taxInt.TaCollectedTaxesCharterCP;
                                _transactionInt.ILTotalAmount[_transactionInt.TANumLines] = _tvq;
                            }
                            else
                            {
                                return "Error in tax :" + Acomba.GetErrorMessage(error);
                            }
                        }

                        error = _transactionInt.AddCard();
                        if (error == 0)
                        {
                            return "Adding invoice completed successfully";
                        }
                        else
                        {
                            return "Error in adding invoice :" + Acomba.GetErrorMessage(error);
                        }
                    }
                    else
                    {
                        return $"Error in Customer Supplies:{customerNumber}" + Acomba.GetErrorMessage(error);
                    }

                }
                else
                {
                    return "Error Customer not found:" + Acomba.GetErrorMessage(error);
                }

            }
            catch (Exception ex)
            {
                return "Error :" + ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message;
            }
        }

        public async Task<string> AddInvoice(InvoiceRequest invoiceRequest)
        {
            try
            {
                _connection.OpenConnection();
                int error;
                AcoSDK.ControlCustomer controlCustomerInt = new AcoSDK.ControlCustomer();
                AcoSDK.Product productInt = new AcoSDK.Product();
                AcoSDK.Tax taxInt = new AcoSDK.Tax();


                string customerNumber;
                bool productFind;
                double taxeFederale;
                
                _transactionInt.BlankCard();

                error = controlCustomerInt.GetCard(1);

                if (error == 0)
                {
                    _transactionInt.InInvoiceType = InvoicingType.ITp_Invoice;
                    _transactionInt.InReference = invoiceRequest.invoice.reference_no;
                    _transactionInt.InDescription = invoiceRequest.invoice.reference_no;
                    _transactionInt.InCurrentDay = 1;
                    _transactionInt.InTransactionActive = 1;
                    _transactionInt.InInvoiceSubTotal = invoiceRequest.invoice.sub_total.Value;
                    _transactionInt.InInvoiceTotal = invoiceRequest.invoice.total == null ? invoiceRequest.invoice.sub_total.Value : invoiceRequest.invoice.total.Value;
                    customerNumber = invoiceRequest.invoice.customer_billing_address.last_name + " " + invoiceRequest.invoice.customer_billing_address.first_name;

                    _transactionInt.InCustomerSupplierCP = GetCustomerCardPos(customerNumber);

                    if (_transactionInt.InCustomerSupplierCP > 0)
                    {
                        GetCustomerInfo(_transactionInt.InCustomerSupplierCP);


                        for (int i = 0; i < invoiceRequest.invoice.line_items.Count(); i++)
                        {
                            _transactionInt.TANumLines++;
                            _transactionInt.ILType[_transactionInt.TANumLines] = InvoicingLineType.IL_Invoice;

                            _transactionInt.ILLineNumber[_transactionInt.TANumLines] = _transactionInt.TANumLines;

                            _transactionInt.ILDescription[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].name;
                            _transactionInt.ILSellingPrice[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].unit_price.Value;
                            _transactionInt.ILOrderedQty[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].quantity.Value;
                            _transactionInt.ILInvoicedQty[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].quantity.Value;
                            _transactionInt.ILTotalAmount[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].total;
                            _transactionInt.ILProductGroupCP[_transactionInt.TANumLines] = 600;
                        }
   
                        error = _transactionInt.AddCard();
                        if (error == 0)
                        {
                            return "Adding invoice completed successfully";
                        }
                        else
                        {
                            return "Error in adding invoice :" + Acomba.GetErrorMessage(error);
                        }
                    }
                    else
                    {
                        return $"Error in Customer Supplies:{customerNumber}" + Acomba.GetErrorMessage(error);
                    }

                }
                else
                {
                    return "Error Customer not found:" + Acomba.GetErrorMessage(error);
                }

            }
            catch (Exception ex)
            {
                return "Error :" + ex.InnerException.Message == null ? ex.Message : ex.InnerException.Message;
            }
        }
    
        //public async Task<string> AddInvoice2(InvoiceRequest invoiceRequest)
        //{
        //    try
        //    {
        //        _connection.OpenConnection();
        //        int error;
        //        AcoSDK.ControlCustomer controlCustomerInt = new AcoSDK.ControlCustomer();
        //        AcoSDK.Product productInt = new AcoSDK.Product();
        //        AcoSDK.Tax taxInt = new AcoSDK.Tax();

        //        var invoice = new Transaction();

        //        invoice.
        //    }
        //    catch (Exception ex)
        //    {

        //    }
        //}
        private int GetCustomerCardPos(string customerNumber)
        {
            AcoSDK.Customer customerInt = new AcoSDK.Customer();

            customerInt.PKey_CuNumber = customerNumber;
            error = customerInt.FindKey(1, false);

            if (error == 0)
            {
                return customerInt.Key_CuCardPos;
            }
            else
            {
                return 0;
            }
        }
        private void GetCustomerInfo(int customerNumber)
        {
            AcoSDK.Customer customerInt = new AcoSDK.Customer();
            error = customerInt.GetCard(customerNumber);
            if (error == 0)
            {
                error = 
                error = customerTermInt.GetCard(customerInt.CuTermCP);
                if(error == 0)
                {
                    _transactionInt.InTermType = customerTermInt.CTType;
                    _transactionInt.InTermDescription = customerTermInt.CTDescription;
                    _transactionInt.InTermPercent = customerTermInt.CTPercent;
                    _transactionInt.InNumberOfDays = customerTermInt.CTNumberOfDays;
                    _transactionInt.InDueDays = customerTermInt.CTDueDays;
                    _transactionInt.InNumberOfPayments = customerTermInt.CTNumberOfPayments;
                    _transactionInt.InPeriodicity = customerTermInt.CTPeriodicity;
                    _transactionInt.InDaysBetweenPayments = customerTermInt.CTDaysBetweenPayments;
                    _transactionInt.InFirstPayment = customerTermInt.CTFirstPayment;
                    _transactionInt.InNumberOfSchedules = customerTermInt.CTNumberOfSchedules;
                    _transactionInt.InInvoicedToCP = customerInt.CuInvoicedToCP;
                    _transactionInt.InReceivableOffset = customerInt.CuReceivable;
                    _transactionInt.InTaxGroupCP = customerInt.CuTaxGroupCP;
                    _transactionInt.InEqualPayments = customerTermInt.CTEqualPayments;
                    _transactionInt.InTaxGroupNumber = customerInt.CuTaxGroupNumber;
                    _transactionInt.InTermNumber = customerInt.CuTermNumber;
                   
                }
               
            }
            else
            {
                Console.WriteLine("Error:" + Acomba.GetErrorMessage(error));
            }
        }
        private int GetProductCardPos(string number)
        {
            AcoSDK.Product productInt = new AcoSDK.Product();
            productInt.PKey_PrNumber = number;
            error = productInt.FindKey(1, false);
            if (error == 0)
            {
                return productInt.Key_PrCardPos;
            }
            else
            {
                return 0;
            }
        }
        private int GetProductGroupCardPos(int number)
        {
            AcoSDK.ProductGroup productGroupInt = new AcoSDK.ProductGroup();
            productGroupInt.PKey_PGNumber = number;
            error = productGroupInt.FindKey(1, false);
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
        #region Customer Payment
        public async Task<string> CustomerPayment(InvoicePayment _payment, CustomerDto _customer)
        {
            try
            {
                _connection.OpenConnection();

                var customerInt = new AcoSDK.Customer();
                var customerPaymentInt = new AcoSDK.CustomerPayment();
                var customerPaymentLineInt = new AcoSDK.CustomerPaymentLine();
                var invoiceArInt = new AcoSDK.InvoiceAR();

                int error ;
                int paymentCardPos;
                double firstPayment = 0;
                double secondPayment = 0;
                double payments = 0;
                string successMessage;
                List<double> paymentsList = new List<double>();
                //get acomba customer id

                var customerNumber = _customer.custom_fields.Where(i => i.label.Contains("Project")).FirstOrDefault().value;

                customerInt.PKey_CuNumber = customerNumber;

                error = customerInt.FindKey(1, true);
                if (error == 0)
                {
                    //Customer associated with payment

                    customerPaymentInt.CPCustomerCP = customerInt.Key_CuCardPos;
                    customerPaymentInt.CPReceivable = customerInt.CuReceivable;
                    customerPaymentInt.CPCurrentDay = 1;
                    customerPaymentInt.CPPaymentDate = DateTime.Now;

                    //Type of payment made
                    customerPaymentInt.CPPaymentType = PaymentARType.PM_Payment;

                    paymentCardPos = Convert.ToInt32(AcoSDK.Constant.IsBeingCreated);
                   
                    error = customerPaymentInt.ReserveCard(paymentCardPos);
                    customerPaymentInt.FreeCardNumber();
                    if (error == 0)
                    {
                        invoiceArInt.BlankKey();
                        invoiceArInt.Key_InCustomerCP = customerInt.Key_CuCardPos;
                        invoiceArInt.Key_InTransactionType = TransARType.TA_Invoice;

                        error = invoiceArInt.SearchKey(3, false);
                        while (error == 0 && invoiceArInt.Key_InTransactionType == TransARType.TA_Invoice && invoiceArInt.Key_InCustomerCP == customerInt.Key_CuCardPos)
                        {
                            payments = GetSoldeTransAR(invoiceArInt.Key_InCardPos);
                            error = customerPaymentLineInt.PayInvoice(customerPaymentInt.CardPos, invoiceArInt.Key_InCardPos, payments, 0);
                            if (error == 0)
                            {
                                paymentsList.Add(payments);
                            }
                            
                            
                            error = invoiceArInt.NextKey(1, false);
                        }
                       
                        
                        //if(error == 0 && invoiceArInt.Key_InTransactionType == TransARType.TA_Invoice && invoiceArInt.Key_InCustomerCP == customerInt.Key_CuCardPos)
                        //{
                        //    payments = GetSoldeTransAR(invoiceArInt.Key_InCardPos);

                        //    error = customerPaymentLineInt.PayInvoice(customerPaymentInt.CardPos, invoiceArInt.Key_InCardPos, payments, 0);

                        //    if (error == 0)
                        //    {
                        //        paymentsList.Add(payments);
                        //        error = invoiceArInt.NextKey(3, false);
                        //        while (error == 0 && invoiceArInt.Key_InTransactionType == TransARType.TA_Invoice && invoiceArInt.Key_InCustomerCP == customerInt.Key_CuCardPos)
                        //        {
                        //            payments = GetSoldeTransAR(invoiceArInt.Key_InCardPos);
                        //            error = customerPaymentLineInt.PayInvoice(customerPaymentInt.CardPos, invoiceArInt.Key_InCardPos, payments, 0);
                                   
                        //        }
                        //    }
                        //}
                        
                        if(error != 0)
                        {
                            customerPaymentInt.CPPaymentMode[1] = PaymentARMode.PAR_Check;
                            customerPaymentInt.CPReceivedAmount[1] = paymentsList.Sum();

                            error = customerPaymentInt.ModifyCard(false);
                            if(error == 0 )
                            {
                                _connection.CloseConnection();
                                return "Adding the payment slip completed successfully";
                            }
                            else
                            {
                                customerPaymentInt.FreeCard();
                                _connection.CloseConnection();
                                return "Error Customer Payment :" + Acomba.GetErrorMessage(error);
                                
                            }
                        }
                    }
                    else
                    {
                        customerPaymentInt.FreeCard();
                        _connection.CloseConnection();
                        return "Error Customer Payment :" + Acomba.GetErrorMessage(error);
                    }
                    _connection.CloseConnection();
                    return "";
                }
                else
                {
                    customerInt.FreeCard();
                    return "Error Customer :" + Acomba.GetErrorMessage(error);
                }

                
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
        }

        private double GetSoldeTransAR(int cardpos)
        {
            var customerPaymentLineInt = new AcoSDK.CustomerPaymentLine();
            var invoiceArInt = new AcoSDK.InvoiceAR();
            double pay;
            int error;

            error = invoiceArInt.GetCard(cardpos);
            if(error == 0)
            {
                pay = invoiceArInt.InTransactionTotal;

                customerPaymentLineInt.Key_CPInvoiceARCP = cardpos;

                error = customerPaymentLineInt.SearchKey(1, false);

                while(error == 0)
                {
                    pay = pay - customerPaymentLineInt.Key_CPAmount;

                    error = customerPaymentLineInt.NextKey(1, false);
                }
            }
            else
            {
               throw new Exception("Error InvoiceAR not found:" + Acomba.GetErrorMessage(error));
            }
            return pay;
        }
        #endregion
        #region Add Invoice Complete

        #endregion
        #region Get Invoice
        public async Task<object> GetInvoice(string invoiceId)
        {
            _connection.OpenConnection();

            AcoSDK.Invoicing _invoicing = new AcoSDK.Invoicing();
            AcoSDK.Transaction _transaction = new AcoSDK.Transaction();
            //AcoSDK.Transaction _tr = new AcoSDK.Transaction();
            int error, cardpos;
            object _invoice = "";

            _invoicing.BlankKey();

            _invoicing.Key_InInvoiceNumber = invoiceId;
            _invoicing.Key_InInvoiceType = InvoicingType.ITp_Invoice;

                
            error = _invoicing.NextKey(1, false);

            while (error == 0 && _invoicing.Key_InInvoiceType == InvoicingType.ITp_Invoice)
            {
                error = _transaction.GetCard(_invoicing.Key_InCardPos);
                var refNo = _transaction.InReference;
                if(error == 0 && refNo == "ref12345")
                {

                     _invoice = new
                    {
                        id = _transaction.InInvoiceNumber,
                        key = _transaction.PKey_InInvoiceNumber,
                        status = _transaction.InStatus,
                        total = _transaction.InInvoiceTotal,
                        type = _transaction.InInvoiceType,
                        reference = _transaction.InReference,
                        description = _transaction.InDescription,
                        currentday = _transaction.InCurrentDay,
                        transaction_active = _transaction.InTransactionActive,
                        subtotal = _transaction.InInvoiceSubTotal,

                        
                    };
                    return _invoice;
                }
                else
                {
                    
                }
               
            }
            _connection.CloseConnection();
            return _invoice;
        }
        public async Task<object> GetInvoices()
        {
            _connection.OpenConnection();
            int total = 0; // number of customers to import
            int cardpos = 2; //CardPos of the first customer file to consult
            int error;
            var _list = new List<object>();
            AcoSDK.InvoiceAP _transaction = new AcoSDK.InvoiceAP();

            total = _transaction.NumCards();

            // _transaction.PKey_InInvoiceNumber = invoiceId;

            error = _transaction.GetCards(cardpos, 100);
            if (error == 0 || _transaction.CursorUsed > 0)
            {
                for (int i = 0; i < _transaction.CursorUsed; i++)
                {
                    _transaction.Cursor = i;
                    if (_transaction.InStatus == 0)
                    {
                        var _invoice = new
                        {
                            id = _transaction.InInvoiceNumber,
                            pkey = _transaction.Key_InInvoiceNumber,
                            type = _transaction.InType,
                            reference = _transaction.InReference,
                            description = _transaction.InDescription,
                            currentday = _transaction.InCurrentDay,
                            transaction_active = _transaction.InActive,
                            subtotal = _transaction.InSubTotal,
                            //total = _transaction.InInvoiceTotal,
                        };
                        _list.Add(_invoice);
                    }
                       
                }

            }
            _connection.CloseConnection();
            return _list;

        }
        #endregion

    }
}
