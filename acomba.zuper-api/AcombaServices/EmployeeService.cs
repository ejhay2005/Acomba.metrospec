using acomba.zuper_api.Dto;
using AcoSDK;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.AcombaServices
{
    public interface IEmployeeService
    {
        Task<string> AddEmployee(EmployeeDto employee);
        Task<string> UpdateEmployee(EmployeeDto employee);
    }
    public class EmployeeService : IEmployeeService
    {
        private readonly IConfiguration _configuration;
        private readonly IAcombaConnection _connection;
        private AcoSDK.Employee employeeInt = new AcoSDK.Employee();
        private AcoSDK.FunctionX functionInt = new AcoSDK.FunctionX();
        private AcoSDK.Department departmentInt = new AcoSDK.Department();
        private AcoSDK.TimeSheet timeSheetInt = new AcoSDK.TimeSheet();
        private AcoSDK.AcombaX Acomba = new AcoSDK.AcombaX();
        public EmployeeService(IConfiguration configuration, IAcombaConnection connection)
        {
            _configuration = configuration;
            _connection = connection;
        }
        #region Add employee
        public async Task<string> AddEmployee(EmployeeDto employee)
        {

            int error;
            _connection.OpenConnection();


            employeeInt.BlankCard();
            employeeInt.BlankKey();

            employeeInt.InitDefaultEmployeValues();
            //initialize employee primary key
            employeeInt.PKey_EmNumber = employee.emp_code;

            //reserve
            error = employeeInt.ReserveCardNumber();
            if (error == 0)
            {

                //setting up custom_fields
                string _gender =  !employee.custom_fields.Where(i => i.label == "Gender").Any() ? string.Empty : employee.custom_fields.Where(i => i.label == "Gender").FirstOrDefault().value;
                string _marital = !employee.custom_fields.Where(i => i.label == "Marital Status").Any() ? string.Empty : employee.custom_fields.Where(i => i.label == "Marital Status").FirstOrDefault().value;
                string _language = !employee.custom_fields.Where(i => i.label == "Language").Any() ? string.Empty : employee.custom_fields.Where(i => i.label == "Language").FirstOrDefault().value;
                DateTime _birth = !employee.custom_fields.Where(i => i.label == "Birth").Any() ? DateTime.Now : Convert.ToDateTime(employee.custom_fields.Where(i => i.label == "Birth").FirstOrDefault().value);
                DateTime _arrival = !employee.custom_fields.Where(i => i.label == "Hiring").Any() ? DateTime.Now : Convert.ToDateTime(employee.custom_fields.Where(i => i.label == "Hiring").FirstOrDefault().value);
                DateTime _departure = !employee.custom_fields.Where(i => i.label == "Start").Any() ? DateTime.Now : Convert.ToDateTime(employee.custom_fields.Where(i => i.label == "Start").FirstOrDefault().value);
                string _address = !employee.custom_fields.Where(i => i.label == "Address").Any() ? string.Empty : employee.custom_fields.Where(i => i.label == "Address").FirstOrDefault().value;
                string _city = !employee.custom_fields.Where(i => i.label == "City").Any() ? string.Empty : employee.custom_fields.Where(i => i.label == "City").FirstOrDefault().value;
                string _country = !employee.custom_fields.Where(i => i.label == "Country").Any() ? string.Empty : employee.custom_fields.Where(i => i.label == "Country").FirstOrDefault().value;
                string _paymentMode = !employee.custom_fields.Where(i => i.label == "Mode").Any() ? string.Empty : employee.custom_fields.Where(i => i.label == "Mode").FirstOrDefault().value;
                string _institution = !employee.custom_fields.Where(i => i.label == "Institution").Any() ? "000" : employee.custom_fields.Where(i => i.label == "Institution").FirstOrDefault().value;
                string _branchCode = !employee.custom_fields.Where(i => i.label == "Branch").Any() ? "00000" : employee.custom_fields.Where(i => i.label == "Branch").FirstOrDefault().value;
                string _accountNo = !employee.custom_fields.Where(i => i.label == "Account Number").Any() ? "0" : employee.custom_fields.Where(i => i.label == "Account Number").FirstOrDefault().value;

                //personal information
                employeeInt.EmNumber = employeeInt.PKey_EmNumber;
                employeeInt.EmName = employee.last_name;
                employeeInt.EmFirstName = employee.first_name;
                employeeInt.EmSortKey = employee.last_name;
                employeeInt.EmSIN = "000000000";
                employeeInt.EmGender = GetGenderType(_gender);
                employeeInt.EmMaritalStatus =GetMaritalStatus(_marital);
                employeeInt.EmBirthdate = _birth;
                employeeInt.EmArrivalDate = _arrival;
                employeeInt.EmDepartureDate = _departure;
                employeeInt.EmLanguage = GetLanguage(_language);
                employeeInt.EmAddress = _address;
                employeeInt.EmCity = _city;
                employeeInt.EmISOCountryCode = _country;
                employeeInt.EmActive = 1;

                //setting up payment mode
                employeeInt.EmPaymentMode = GetPaymentModeType(_paymentMode);
                employeeInt.EmInstitutionNumber = _institution;
                employeeInt.EmBranchNumber = _branchCode;
                employeeInt.EmAccountNumber = _accountNo;

                //federal
                employeeInt.Em_Fed_IT_CalculationMethod = PA_FederalIncomeTaxType.PA_Fed_IT_None;
                //Exemption
                employeeInt.EmUseFederalTableForPersonalExemption = 0;
                employeeInt.Em_Fed_IT_PersonalExemption = 0;
                //Deduction
                employeeInt.Em_Fed_IT_RemoteAreaExemption = 0;
                //Additional tax to be withheld
                employeeInt.Em_Fed_IT_AdditionnalIncomeTax = 0;
                //Additional deduction
                employeeInt.Em_Fed_IT_AdditionnalDeduction = 0;
                //Department
                employeeInt.FnDepartmentCP[1] = GetDepartmentCardPos(GetDepartment(employee.designation));
                employeeInt.EmFunctionsCP[1] = 1;
                employeeInt.EmTotalFunctions = 1;
                employeeInt.FnOrder[1] = 1;
                employeeInt.FnDescription[1] = employee.designation;
                employeeInt.FnActive[1] = 1;
                employeeInt.FnReference[1] = 1;

                //employeeInt.set_Fn_Line_Amount(1, 701, 0);
                //employeeInt.set_Fn_Line_Amount(1, 702, 20.54);
                //employeeInt.set_Fn_Line_Amount(1, 703, 0);
                //employeeInt.set_Fn_Line_Amount(1, 704, 0);
                //employeeInt.set_Fn_Line_Amount(1, 716, 10);
                //employeeInt.set_Fn_Line_Amount(1, 717, 10);
                //employeeInt.set_Fn_Line_Amount(1, 718, 60);
                //employeeInt.set_Fn_Line_Amount(1, 719, 0);
                //employeeInt.set_Fn_Line_Amount(1, 720, 11.62);
                //employeeInt.set_Fn_Line_Amount(1, 721, 0);

                //The employee is paid an hourly wage for this role
                employeeInt.set_Fn_Conditions_OnOff(1, PA_ConditionType.PA_Cond_HourlyWage, 1);

                employeeInt.set_Fn_Conditions_OnOff(1, PA_ConditionType.PA_Cond_CSST, -1);
                employeeInt.set_Fn_Conditions_OnOff(1, PA_ConditionType.PA_Cond_GroupInsurance, -1);
                employeeInt.set_Fn_Conditions_OnOff(1, PA_ConditionType.PA_Cond_Tips, -1);
                employeeInt.set_Fn_Conditions_OnOff(1, PA_ConditionType.PA_Cond_ContributionFSS, -1);
                employeeInt.set_Fn_Conditions_OnOff(1, PA_ConditionType.PA_Cond_UnionDues, -1);


                error = employeeInt.AddCard();
                if (error == 0)
                {
                    return "Success";
                }
                else
                {
                    string _error = "Error :" + Acomba.GetErrorMessage(error);
                    error = employeeInt.FreeCardNumber();
                    if (error == 0)
                    {
                        return _error;
                    }
                    else
                    {
                        return "Error :" + Acomba.GetErrorMessage(error);
                    }

                }
            }
            else
            {
                error = employeeInt.FreeCardNumber();
                return "Error :" + Acomba.GetErrorMessage(error);
            }

        }
        private int GetDepartmentCardPos(int id)
        {
            int error;
            departmentInt.PKey_DeNumber = 1;
            error = departmentInt.FindKey(1, true);
            if (error == 0)
            {
                return departmentInt.Key_DeCardPos;
            }
            else
            {
                return 0;
            }
        }
        //Get Gender Type
        private PA_GenderType GetGenderType(string gender)
        {
            switch (gender)
            {
                case "Male":
                    return PA_GenderType.PA_Gender_Male;
                case "Female":
                    return PA_GenderType.PA_Gender_Female;
                default:
                    return PA_GenderType.PA_Gender_Min;
            }
        }
        // Get Marital Status Type
        private PA_MaritalStatusType GetMaritalStatus(string status)
        {
            switch(status)
            {
                case "Single":
                    return PA_MaritalStatusType.PA_MaritalStatus_Single;
                case "Married":
                    return PA_MaritalStatusType.PA_MaritalStatus_Married;
                case "Separated":
                    return PA_MaritalStatusType.PA_MaritalStatus_Separated;
                case "Common Law":
                    return PA_MaritalStatusType.PA_MaritalStatus_CommonLaw;
                case "Widowed":
                    return PA_MaritalStatusType.PA_MaritalStatus_Widowed;
                case "Divorced":
                    return PA_MaritalStatusType.PA_MaritalStatus_Divorced;
                case "Membr. of Religious Order":
                    return PA_MaritalStatusType.PA_MaritalStatus_Religious;
                default:
                    return PA_MaritalStatusType.PA_MaritalStatus_Undefined;
            }
        }

        //Get Language
        private int GetLanguage(string language)
        {
            switch(language)
            {
                case "French":
                    return 12;
                case "English":
                    return 9;
                case "Danish":
                    return 6;
                case "Dutch":
                    return 19;
                case "Finnish":
                    return 11;
                case "German":
                    return 7;
                case "Italian":
                    return 16;
                case "Hungarian":
                    return 14;
                default:
                    return 9;
            }
        }
        private PA_PaymentModeType GetPaymentModeType(string mode)
        {
            switch (mode)
            {
                case "Check":
                    return PA_PaymentModeType.PA_PaymentMode_Check;
                case "Direct Deposit":
                    return PA_PaymentModeType.PA_PaymentMode_DirectDeposit;
                default:
                    return PA_PaymentModeType.PA_PaymentMode_DirectDeposit;
            }
        }
        //private int CreateDepartment(int id,string name)
        //{
        //    int error;
        //    departmentInt.BlankCard();
        //    departmentInt.BlankKey();

        //    departmentInt.PKey_DeNumber = id;

        //    error = departmentInt.ReserveCardNumber();
        //    if(error == 0)
        //    {
        //        departmentInt.DeNumber = departmentInt.PKey_DeNumber;
        //        departmentInt.DeDescription = name;

        //        error = employeeInt.AddCard();
        //        if(error == 0)
        //        {
        //            return id;
        //        }
        //        else
        //        {
        //            return 0;
        //        }
        //    }
        //    else
        //    {
        //        error = employeeInt.FreeCardNumber();
        //        return 0 ;
        //    }

        //}
        private int GetDepartment(string deptName)
        {
            switch (deptName)
            {
                case "Administrator":
                    return 3;
                case "Field Executive":
                    return 4;
                case "Team Leader":
                    return 5;
                default: return 1;
            }
        }
        #endregion
        #region Update Employee
        public async Task<string> UpdateEmployee(EmployeeDto employee)
        {
            int error;
            const int noIndex = 1;

            employeeInt.BlankKey();
            employeeInt.PKey_EmNumber = employee.emp_code;

            error = employeeInt.FindKey(1,false);
            if (error == 0)
            {
                employeeInt.EmNumber = employeeInt.PKey_EmNumber;
                employeeInt.EmName = employee.last_name;
                employeeInt.EmFirstName = employee.first_name;
                employeeInt.EmSortKey = employee.last_name;

                error = employeeInt.ModifyCard(true);
                
                if (error == 0)
                {
                    return "Update completed successfully";
                }
                else
                {
                    error = employeeInt.FreeCard();
                    string _error = Acomba.GetErrorMessage(error);
                    if (error != 0)
                    {
                        return "Error: " + Acomba.GetErrorMessage(error);
                    }
                    return "Error: " + _error;
                }
            }
            else
            {
                return "Error: " + Acomba.GetErrorMessage(error);
            }

        }
        #endregion
        #region Add Employee Timesheet
        //public async Task<string> AddTimesheets(TimesheetsDto timesheetsDto)
        //{


        //}
        #endregion
    }
}
