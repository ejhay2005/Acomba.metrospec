using acomba.zuper_api.Dto;
using AcoSDK;
using AcoX0114;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore.Query;
using System.Collections.Immutable;

namespace acomba.zuper_api.AcombaServices
{
    public interface IinvoiceService
    {
        Task<string> AddInvoice(InvoiceRequest invoiceRequest);
    }
    public class InvoiceService : IinvoiceService
    {
        private readonly IConfiguration _configuration;
        private readonly IAcombaConnection _connection;
        private AcoSDK.Transaction _transactionInt = new AcoSDK.Transaction();
        private AcoSDK.AcombaX Acomba = new AcoSDK.AcombaX();
        private int error;
        public InvoiceService(IConfiguration configuration, IAcombaConnection connection)
        {
            _configuration = configuration;
            _connection = connection;
        }

        #region Add Invoice
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
                int r = 1;
                _transactionInt.BlankCard();

                error = controlCustomerInt.GetCard(1);

                if (error == 0)
                {
                    _transactionInt.InInvoiceType = InvoicingType.ITp_Invoice;
                    _transactionInt.InReference = invoiceRequest.invoice_uid;
                    _transactionInt.InDescription = "Description de la facture";
                    _transactionInt.InCurrentDay = 1;
                    _transactionInt.InTransactionActive = 1;
                    customerNumber = invoiceRequest.invoice.customer;

                    _transactionInt.InCustomerSupplierCP = GetCustomerCardPos(customerNumber);
                    if (_transactionInt.InCustomerSupplierCP > 0)
                    {
                        GetCustomerInfo(_transactionInt.InCustomerSupplierCP);

                        
                        for(int i = 0; i < invoiceRequest.invoice.line_items.Count(); i++)
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
                                    _transactionInt.ILProductGroupCP[_transactionInt.TANumLines] = productInt.PrProductGroupCP;
                                    _transactionInt.ILInvoicedQty[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].quantity.Value;
                                    productFind = true;
                                }
                            }

                            if (!productFind)
                            {
                                _transactionInt.ILDescription[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].name;
                                _transactionInt.ILSellingPrice[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].unit_price.Value;
                                _transactionInt.ILProductGroupCP[_transactionInt.TANumLines] = GetProductGroupCardPos(1);
                                //_transactionInt.ILInvoicedQty[_transactionInt.TANumLines] = invoiceRequest.invoice.line_items[i].quantity.Value;
                            }
                        }

                        

                        error = _transactionInt.AddCard();
                        if (error == 0)
                        {
                            return "Adding invoice completed successfully";
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
                else
                {
                    return "Error :" + Acomba.GetErrorMessage(error);
                }

            }
            catch (Exception ex)
            {
                throw;
            }
        }
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
                _transactionInt.InInvoicedToCP = customerInt.CuInvoicedToCP;
                _transactionInt.InReceivableOffset = customerInt.CuReceivable;
                _transactionInt.InTaxGroupCP = customerInt.CuTaxGroupCP;
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
        //#region Add Invoice
        //public async Task<string> AddInvoice(InvoiceRequest invoiceRequest)
        //{
        //    try
        //    {
        //        _connection.OpenConnection();
        //        int error;
        //        AcoSDK.ControlCustomer controlCustomerInt = new AcoSDK.ControlCustomer();
        //        AcoSDK.Product productInt = new AcoSDK.Product();
        //        AcoSDK.Tax taxInt = new AcoSDK.Tax();


        //        string customerNumber;
        //        bool productFind;
        //        double taxeFederale;
        //        int r = 1;

        //        _transactionInt.BlankCard();

        //        error = controlCustomerInt.GetCard(1);

        //        if (error == 0)
        //        {
        //            _transactionInt.InInvoiceType = InvoicingType.ITp_Invoice;
        //            _transactionInt.InReference = invoiceRequest.invoice_uid;
        //            _transactionInt.InDescription ="";
        //            _transactionInt.InCurrentDay = 1;
        //            _transactionInt.InTransactionActive = 1;

        //            customerNumber = invoiceRequest.invoice.customer;

        //            _transactionInt.InCustomerSupplierCP = GetCustomerCardPos(customerNumber);
        //            if (_transactionInt.InCustomerSupplierCP > 0)
        //            {
        //                GetCustomerInfo(_transactionInt.InCustomerSupplierCP);

        //                _transactionInt.TANumLines = invoiceRequest.invoice.line_items.ToList().Count();

        //                _transactionInt.ILType[1] = InvoicingLineType.IL_Invoice;

        //                _transactionInt.ILLineNumber[1] = 1;

        //                _transactionInt.ILProductNumber[1] = "Estimate";

        //                _transactionInt.ILProductCP[1] = GetProductCardPos(_transactionInt.ILProductNumber[1]);

        //                productFind = false;

        //                if (_transactionInt.ILProductCP[1] > 0)
        //                {
        //                    productInt.BlankCard();

        //                    error = productInt.GetCard(_transactionInt.ILProductCP[1]);
        //                    if (error == 0)
        //                    {
        //                        _transactionInt.ILDescription[1] = productInt.PrDescription[1];
        //                        _transactionInt.ILSellingPrice[1] = productInt.PrSellingPrice[0, 1];
        //                        _transactionInt.ILProductGroupCP[1] = productInt.PrProductGroupCP;

        //                        productFind = true;
        //                    }
        //                }

        //                if (!productFind)
        //                {
        //                    _transactionInt.ILDescription[1] = "Estimate estate1";
        //                    _transactionInt.ILSellingPrice[1] = 44.21;
        //                    _transactionInt.ILProductGroupCP[1] = GetProductGroupCardPos(1);
        //                    _transactionInt.ILInvoicedQty[1] = 1;
        //                }

        //                error = _transactionInt.AddCard();
        //                if (error == 0)
        //                {
        //                    return "Adding invoice completed successfully";
        //                }
        //                else
        //                {
        //                    return "Error :" + Acomba.GetErrorMessage(error);
        //                }
        //            }
        //            else
        //            {
        //                return "Error :" + Acomba.GetErrorMessage(error);
        //            }

        //        }
        //        else
        //        {
        //            return "Error :" + Acomba.GetErrorMessage(error);
        //        }

        //    }
        //    catch (Exception ex)
        //    {
        //        throw;
        //    }
        //}
        //private int GetCustomerCardPos(string customerNumber)
        //{
        //    AcoSDK.Customer customerInt = new AcoSDK.Customer();

        //    customerInt.PKey_CuNumber = customerNumber;
        //    error = customerInt.FindKey(1, false);

        //    if (error == 0)
        //    {
        //        return customerInt.Key_CuCardPos;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
        //private void GetCustomerInfo(int customerNumber)
        //{
        //    AcoSDK.Customer customerInt = new AcoSDK.Customer();
        //    error = customerInt.GetCard(customerNumber);
        //    if (error == 0)
        //    {
        //        _transactionInt.InInvoicedToCP = customerInt.CuInvoicedToCP;
        //        _transactionInt.InReceivableOffset = customerInt.CuReceivable;
        //        _transactionInt.InTaxGroupCP = customerInt.CuTaxGroupCP;
        //    }
        //    else
        //    {
        //        Console.WriteLine("Error:" + Acomba.GetErrorMessage(error));
        //    }
        //}
        //private int GetProductCardPos(string number)
        //{
        //    AcoSDK.Product productInt = new AcoSDK.Product();
        //    productInt.PKey_PrNumber = number;
        //    error = productInt.FindKey(1, false);
        //    if (error == 0)
        //    {
        //        return productInt.Key_PrCardPos;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
        //private int GetProductGroupCardPos(int number)
        //{
        //    AcoSDK.ProductGroup productGroupInt = new AcoSDK.ProductGroup();
        //    productGroupInt.PKey_PGNumber = number;
        //    error = productGroupInt.FindKey(1, false);
        //    if (error == 0)
        //    {
        //        return productGroupInt.Key_PGCardPos;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}
        //#endregion
        #region Add Invoice Complete

        #endregion

    }
}
