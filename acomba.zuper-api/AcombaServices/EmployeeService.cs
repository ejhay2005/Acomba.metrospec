using acomba.zuper_api.Dto;
using AcoSDK;
using Newtonsoft.Json;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.AcombaServices
{
    public interface IEmployeeService
    {
        Task<string> AddEmployee(EmployeeDto employee);
        Task<string> UpdateEmployee(EmployeeDto employee);
        Task<object> ExportEmployees();
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

            //employeeInt.InitDefaultEmployeValues();
            //initialize employee primary key
            employeeInt.PKey_EmNumber = employee.emp_code;

            //reserve
            error = employeeInt.ReserveCardNumber();
            if (error == 0)
            {

                //setting up custom_fields
                string SIN = !employee.custom_fields.Where(i => i.label == "SIN").Any() ? "000000000" : employee.custom_fields.Where(i => i.label == "SIN").FirstOrDefault().value;
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
                string _mainEmail = !employee.custom_fields.Where(i => i.label.Contains("Email")).Any() ? string.Empty : employee.custom_fields.Where(i => i.label.Contains("Email")).FirstOrDefault().value;
                int _personalExempt = !employee.custom_fields.Where(i => i.label == "Personal Exemption").Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label == "Personal Exemption").FirstOrDefault().value);
                int _designatedRemote = !employee.custom_fields.Where(i => i.label.Contains("Designated Remote Areas")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("Designated Remote Areas")).FirstOrDefault().value);
                int _incomeTax = !employee.custom_fields.Where(i => i.label.Contains("Income Tax")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("Income Tax")).FirstOrDefault().value);
                int _additionalDeduction = !employee.custom_fields.Where(i => i.label.Contains("Additional Deduction")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("Additional Deduction")).FirstOrDefault().value);
                string _department = !employee.custom_fields.Where(i => i.label == "Department").Any() ? string.Empty : employee.custom_fields.Where(i => i.label == "Department").FirstOrDefault().value;
                int _ccq = !employee.custom_fields.Where(i => i.label.Contains("CCQ")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("CCQ")).FirstOrDefault().value);
                //earnings
                int _101 = !employee.custom_fields.Where(i => i.label.Contains("101")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("101")).FirstOrDefault().value);
                int _102 = !employee.custom_fields.Where(i => i.label.Contains("102")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("102")).FirstOrDefault().value);
                int _103 = !employee.custom_fields.Where(i => i.label.Contains("103")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("103")).FirstOrDefault().value);
                int _104 = !employee.custom_fields.Where(i => i.label.Contains("104")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("104")).FirstOrDefault().value);
                int _109 = !employee.custom_fields.Where(i => i.label.Contains("109")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("109")).FirstOrDefault().value);
                int _118 = !employee.custom_fields.Where(i => i.label.Contains("118")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("118")).FirstOrDefault().value);
                int _119 = !employee.custom_fields.Where(i => i.label.Contains("119")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("119")).FirstOrDefault().value);
                int _120 = !employee.custom_fields.Where(i => i.label.Contains("120")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("120")).FirstOrDefault().value);
                int _121 = !employee.custom_fields.Where(i => i.label.Contains("121")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("121")).FirstOrDefault().value);
                int _122 = !employee.custom_fields.Where(i => i.label.Contains("122")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("122")).FirstOrDefault().value);
                int _123 = !employee.custom_fields.Where(i => i.label.Contains("123")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("123")).FirstOrDefault().value);
                int _124 = !employee.custom_fields.Where(i => i.label.Contains("124")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("124")).FirstOrDefault().value);
                int _125 = !employee.custom_fields.Where(i => i.label.Contains("125")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("125")).FirstOrDefault().value);
                int _126 = !employee.custom_fields.Where(i => i.label.Contains("126")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("126")).FirstOrDefault().value);
                int _127 = !employee.custom_fields.Where(i => i.label.Contains("127")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("127")).FirstOrDefault().value);
                int _130 = !employee.custom_fields.Where(i => i.label.Contains("130")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("130")).FirstOrDefault().value);
                int _131 = !employee.custom_fields.Where(i => i.label.Contains("131")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("131")).FirstOrDefault().value);
                int _132 = !employee.custom_fields.Where(i => i.label.Contains("132")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("132")).FirstOrDefault().value);
                //Benefits
                int _310 = !employee.custom_fields.Where(i => i.label.Contains("310")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("310")).FirstOrDefault().value);
                int _316 = !employee.custom_fields.Where(i => i.label.Contains("316")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("316")).FirstOrDefault().value);
                int _317 = !employee.custom_fields.Where(i => i.label.Contains("317")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("317")).FirstOrDefault().value);
                //Contributions
                int _404 = !employee.custom_fields.Where(i => i.label.Contains("404")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("404")).FirstOrDefault().value);
                int _421 = !employee.custom_fields.Where(i => i.label.Contains("421")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("421")).FirstOrDefault().value);
                int _422 = !employee.custom_fields.Where(i => i.label.Contains("422")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("422")).FirstOrDefault().value);
                int _424 = !employee.custom_fields.Where(i => i.label.Contains("424")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("424")).FirstOrDefault().value);
                int _425 = !employee.custom_fields.Where(i => i.label.Contains("425")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("425")).FirstOrDefault().value);
                //CCQ Employer Contribution
                int _501 = !employee.custom_fields.Where(i => i.label.Contains("501")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("501")).FirstOrDefault().value);
                int _502 = !employee.custom_fields.Where(i => i.label.Contains("502")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("502")).FirstOrDefault().value);
                int _503 = !employee.custom_fields.Where(i => i.label.Contains("503")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("503")).FirstOrDefault().value);
                int _504 = !employee.custom_fields.Where(i => i.label.Contains("504")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("504")).FirstOrDefault().value);
                int _506 = !employee.custom_fields.Where(i => i.label.Contains("506")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("506")).FirstOrDefault().value);
                int _507 = !employee.custom_fields.Where(i => i.label.Contains("507")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("507")).FirstOrDefault().value);
                //Employee Deduction
                int _716 = !employee.custom_fields.Where(i => i.label.Contains("716")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("716")).FirstOrDefault().value);
                int _717 = !employee.custom_fields.Where(i => i.label.Contains("717")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("717")).FirstOrDefault().value);
                int _719 = !employee.custom_fields.Where(i => i.label.Contains("719")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("719")).FirstOrDefault().value);
                int _720 = !employee.custom_fields.Where(i => i.label.Contains("720")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("720")).FirstOrDefault().value);
                int _721 = !employee.custom_fields.Where(i => i.label.Contains("721")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("721")).FirstOrDefault().value);
                int _722 = !employee.custom_fields.Where(i => i.label.Contains("722")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("722")).FirstOrDefault().value);
                int _723 = !employee.custom_fields.Where(i => i.label.Contains("723")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("723")).FirstOrDefault().value);
                //CCQ Employee Deduction
                int _801 = !employee.custom_fields.Where(i => i.label.Contains("801")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("801")).FirstOrDefault().value);
                int _802 = !employee.custom_fields.Where(i => i.label.Contains("802")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("802")).FirstOrDefault().value);
                int _803 = !employee.custom_fields.Where(i => i.label.Contains("803")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("803")).FirstOrDefault().value);
                int _804 = !employee.custom_fields.Where(i => i.label.Contains("804")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("804")).FirstOrDefault().value);
                int _805 = !employee.custom_fields.Where(i => i.label.Contains("805")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("805")).FirstOrDefault().value);
                int _806 = !employee.custom_fields.Where(i => i.label.Contains("806")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("806")).FirstOrDefault().value);
                int _809 = !employee.custom_fields.Where(i => i.label.Contains("809")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("809")).FirstOrDefault().value);
                int _810 = !employee.custom_fields.Where(i => i.label.Contains("810")).Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label.Contains("810")).FirstOrDefault().value);
                //HR Banks
                int _1 = !employee.custom_fields.Where(i => i.label =="1").Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label == "1").FirstOrDefault().value);
                int _10 = !employee.custom_fields.Where(i => i.label =="10").Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label == "10").FirstOrDefault().value);
                int _11 = !employee.custom_fields.Where(i => i.label== "11").Any() ? 0 : Convert.ToInt32(employee.custom_fields.Where(i => i.label == "11").FirstOrDefault().value);
                //****************************************************************************
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

                //Telephone
                employeeInt.EmPhoneNumber[PhoneType.Ph_Number] = employee.work_phone_number;
                employeeInt.EmPhoneDescription[PhoneType.Ph_Number] = "Telephone";
                employeeInt.EmPhoneNumber[PhoneType.Ph_User1] = employee.mobile_phone_number;
                employeeInt.EmPhoneDescription[PhoneType.Ph_User1] = "Home Phone";
                employeeInt.EmPhoneNumber[PhoneType.Ph_User2] = employee.home_phone_number;
                employeeInt.EmPhoneDescription[PhoneType.Ph_User2] = "Home Phone";
                //email
                employeeInt.EmEMail[EMailType.EMail_One] = _mainEmail ;

                //setting up payment mode
                employeeInt.EmPaymentMode = GetPaymentModeType(_paymentMode);
                employeeInt.EmInstitutionNumber = _institution;
                employeeInt.EmBranchNumber = _branchCode;
                employeeInt.EmAccountNumber = _accountNo;
                
                //federal
                employeeInt.Em_Fed_IT_CalculationMethod = PA_FederalIncomeTaxType.PA_Fed_IT_Salary;
                //Exemption
                employeeInt.EmUseFederalTableForPersonalExemption = _personalExempt;
                employeeInt.Em_Fed_IT_PersonalExemption = _personalExempt;
                //Deduction
                employeeInt.Em_Fed_IT_RemoteAreaExemption = _designatedRemote;
                //Additional tax to be withheld
                employeeInt.Em_Fed_IT_AdditionnalIncomeTax = _incomeTax;
                //Additional deduction
                employeeInt.Em_Fed_IT_AdditionnalDeduction = _additionalDeduction;
                
                //Department
                employeeInt.FnDepartmentCP[1]= GetDepartmentCardPos(GetDepartment(_department));
                employeeInt.EmFunctionsCP[1] = 1;
                employeeInt.EmTotalFunctions = 1;
                employeeInt.FnOrder[1] = 1;
                employeeInt.FnDescription[1] = employee.designation;
                employeeInt.FnActive[1] = 1;
                employeeInt.FnReference[1] = 1;
                //The employee is paid an hourly wage for this role
                employeeInt.Fn_Conditions_OnOff[1, PA_ConditionType.PA_Cond_HourlyWage] = 1;
                employeeInt.Fn_Conditions_OnOff[1, PA_ConditionType.PA_Cond_RegularSalary] = 0;
                employeeInt.Fn_Conditions_OnOff[1, PA_ConditionType.PA_Cond_Commissions] = 0;

                employeeInt.Fn_Conditions_OnOff[1, PA_ConditionType.PA_Cond_CSST] = -1;
                employeeInt.Fn_Conditions_OnOff[1, PA_ConditionType.PA_Cond_GroupInsurance] = -1;
                employeeInt.Fn_Conditions_OnOff[1, PA_ConditionType.PA_Cond_Tips] = -1;
                employeeInt.Fn_Conditions_OnOff[1, PA_ConditionType.PA_Cond_ContributionFSS] = -1;
                employeeInt.Fn_Conditions_OnOff[1, PA_ConditionType.PA_Cond_UnionDues] = -1;

                //Earnings
                employeeInt.set_Fn_Line_Amount(1, 102, _101);
                employeeInt.set_Fn_Line_Amount(1, 102, _102);
                employeeInt.set_Fn_Line_Amount(1, 103, _103);
                employeeInt.set_Fn_Line_Amount(1, 104, _104);
                employeeInt.set_Fn_Line_Amount(1, 109, _109);
                employeeInt.set_Fn_Line_Amount(1, 118, _118);
                employeeInt.set_Fn_Line_Amount(1, 119, _119);
                employeeInt.set_Fn_Line_Amount(1, 120, _120);
                employeeInt.set_Fn_Line_Amount(1, 121, _121);
                employeeInt.set_Fn_Line_Amount(1, 122, _122);
                employeeInt.set_Fn_Line_Amount(1, 123, _123);
                employeeInt.set_Fn_Line_Amount(1, 124, _124);
                employeeInt.set_Fn_Line_Amount(1, 125, _125);
                employeeInt.set_Fn_Line_Amount(1, 126, _126);
                employeeInt.set_Fn_Line_Amount(1, 127, _127);
                employeeInt.set_Fn_Line_Amount(1, 130, _130);
                employeeInt.set_Fn_Line_Amount(1, 131, _131);
                employeeInt.set_Fn_Line_Amount(1, 132, _132);
                //Benefits
                employeeInt.set_Fn_Line_Amount(1, 310, _310);
                employeeInt.set_Fn_Line_Amount(1, 316, _316);
                employeeInt.set_Fn_Line_Amount(1, 317, _317);
                //Contributions
                employeeInt.set_Fn_Line_Amount(1, 404, _404);
                employeeInt.set_Fn_Line_Amount(1, 421, _421);
                employeeInt.set_Fn_Line_Amount(1, 422, _422);
                employeeInt.set_Fn_Line_Amount(1, 424, _424);
                employeeInt.set_Fn_Line_Amount(1, 425, _425);
                //CCQ Employer Deduction
                employeeInt.set_Fn_Line_Amount(1, 501, _501);
                employeeInt.set_Fn_Line_Amount(1, 502, _502);
                employeeInt.set_Fn_Line_Amount(1, 503, _503);
                employeeInt.set_Fn_Line_Amount(1, 504, _504);
                employeeInt.set_Fn_Line_Amount(1, 506, _506);
                employeeInt.set_Fn_Line_Amount(1, 507, _507);
                //Employee Deduction
                employeeInt.set_Fn_Line_Amount(1, 716, _716);
                employeeInt.set_Fn_Line_Amount(1, 717, _717);
                employeeInt.set_Fn_Line_Amount(1, 719, _719);
                employeeInt.set_Fn_Line_Amount(1, 720, _720);
                employeeInt.set_Fn_Line_Amount(1, 721, _721);
                employeeInt.set_Fn_Line_Amount(1, 722, _722);
                employeeInt.set_Fn_Line_Amount(1, 723, _723);
                //CCQ Employee Deduction
                employeeInt.set_Fn_Line_Amount(1, 801, _801);
                employeeInt.set_Fn_Line_Amount(1, 802, _802);
                employeeInt.set_Fn_Line_Amount(1, 803, _803);
                employeeInt.set_Fn_Line_Amount(1, 804, _804);
                employeeInt.set_Fn_Line_Amount(1, 805, _805);
                employeeInt.set_Fn_Line_Amount(1, 806, _806);
                employeeInt.set_Fn_Line_Amount(1, 809, _809);
                employeeInt.set_Fn_Line_Amount(1, 810, _810);
                //HR Banks
                employeeInt.set_Fn_Line_Amount(1, 1, _1);
                employeeInt.set_Fn_Line_Amount(1, 10, _10);
                employeeInt.set_Fn_Line_Amount(1, 11, _11);

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
        #region Importing of Employees
        public async Task<object> ExportEmployees()
        {
            int count = 10;
            int error;
            int cardpos = 1;
            var employeeList = new List<EmployeeDto>();
            
            try
            {
                _connection.OpenConnection();

                error = employeeInt.GetCards(cardpos, count);
                if (error == 0 || employeeInt.CursorUsed > 0)
                {
                    for (int i = 0; i < employeeInt.CursorUsed; i++)
                    {
                        employeeInt.Cursor = i;
                        if (employeeInt.EmStatus == 0)
                        {
                            var customFieldList = new List<CustomField>();

                            customFieldList.Add(CreateCustomField("Gender", SetGenderType(employeeInt.EmGender)));
                            customFieldList.Add(CreateCustomField("Marital Status", SetMaritalStatus(employeeInt.EmMaritalStatus)));
                            customFieldList.Add(CreateCustomField("Language", SetLanguage(employeeInt.EmLanguage)));
                            customFieldList.Add(CreateCustomField("Birth", employeeInt.EmBirthdate.ToShortDateString()));
                            customFieldList.Add(CreateCustomField("Hiring", employeeInt.EmArrivalDate.ToShortDateString()));
                            customFieldList.Add(CreateCustomField("Start", employeeInt.EmDepartureDate.ToShortDateString()));
                            customFieldList.Add(CreateCustomField("Birth", employeeInt.EmBirthdate.ToShortDateString()));
                            customFieldList.Add(CreateCustomField("Address", employeeInt.EmAddress));
                            customFieldList.Add(CreateCustomField("City", employeeInt.EmCity));
                            customFieldList.Add(CreateCustomField("Country", employeeInt.EmISOCountryCode));
                            customFieldList.Add(CreateCustomField("Mode", SetPaymentModeType(employeeInt.EmPaymentMode)));
                            customFieldList.Add(CreateCustomField("Institution", employeeInt.EmInstitutionNumber));
                            customFieldList.Add(CreateCustomField("Branch", employeeInt.EmBranchNumber));
                            customFieldList.Add(CreateCustomField("Account Number", employeeInt.EmAccountNumber));
                            customFieldList.Add(CreateCustomField("Email", employeeInt.EmEMail[EMailType.EMail_One]));
                            customFieldList.Add(CreateCustomField("Designated Remote Areas", employeeInt.Em_Fed_IT_RemoteAreaExemption.ToString()));
                            customFieldList.Add(CreateCustomField("Income Tax", employeeInt.Em_Fed_IT_AdditionnalIncomeTax.ToString()));
                            customFieldList.Add(CreateCustomField("Additional Deduction", employeeInt.Em_Fed_IT_AdditionnalDeduction.ToString()));

                            var _employee = new EmployeeDto()
                            {
                                role_id = GetRoleId(employeeInt.FnDescription[1]),
                                emp_code = employeeInt.EmNumber,
                                first_name = employeeInt.EmFirstName,
                                last_name = employeeInt.EmName,
                                email = employeeInt.EmEMail[EMailType.EMail_One],
                                designation = employeeInt.FnDescription[1],
                                password = "Metro2650",
                                confirm_password = "Metro2650",
                                work_phone_number = employeeInt.EmPhoneNumber[PhoneType.Ph_Number],
                                home_phone_number = employeeInt.EmPhoneNumber[PhoneType.Ph_User2],
                                mobile_phone_number = employeeInt.EmPhoneNumber[PhoneType.Ph_User1],
                                work_hours = DefaultWorkHours(),
                                custom_fields = customFieldList
                            };

                            employeeList.Add(_employee);
                        }

                    }
                    //var result = ImportEmployeesToZuper(employeeList);

                    return employeeList;
                }
                else
                {
                    return "Error :" + Acomba.GetErrorMessage(error);
                }
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            
        }
        
        #endregion
        #region Add Employee Timesheet
        //public async Task<string> AddTimesheets(TimesheetsDto timesheetsDto)
        //{


        //}
        #endregion
        #region Helper
        private int GetDepartmentCardPos(int id)
        {
            int error;
            departmentInt.PKey_DeNumber = id;
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
        private string SetGenderType(PA_GenderType gender)
        {
            switch (gender)
            {
                case PA_GenderType.PA_Gender_Male:
                    return "Male";
                case PA_GenderType.PA_Gender_Female:
                    return "Female";
                default:
                    return "";
            }
        }
        // Get Marital Status Type
        private PA_MaritalStatusType GetMaritalStatus(string status)
        {
            switch (status)
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
        private string SetMaritalStatus(PA_MaritalStatusType status)
        {
            switch (status)
            {
                case PA_MaritalStatusType.PA_MaritalStatus_Single:
                    return "Single";
                case PA_MaritalStatusType.PA_MaritalStatus_Married:
                    return "Married";
                case PA_MaritalStatusType.PA_MaritalStatus_Separated:
                    return "Separated";
                case PA_MaritalStatusType.PA_MaritalStatus_CommonLaw:
                    return "Common Law";
                case PA_MaritalStatusType.PA_MaritalStatus_Widowed:
                    return "Widowed";
                case PA_MaritalStatusType.PA_MaritalStatus_Divorced:
                    return "Divorced";
                case PA_MaritalStatusType.PA_MaritalStatus_Religious:
                    return "Membr. of Religious Order";
                default:
                    return "Undefined";
            }
        }
        //Get Language
        private int GetLanguage(string language)
        {
            switch (language)
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
        private string SetLanguage(int language)
        {
            switch (language)
            {
                case 12:
                    return "French";
                case 9:
                    return "English";
                case 6:
                    return "Danish";
                case 19:
                    return "Dutch";
                case 11:
                    return "Finnish";
                case 7:
                    return "German";
                case 16:
                    return "Italian";
                case 14:
                    return "Hungarian";
                default:
                    return "English";
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
        private string SetPaymentModeType(PA_PaymentModeType mode)
        {
            switch (mode)
            {
                case PA_PaymentModeType.PA_PaymentMode_Check:
                    return "Check";
                case PA_PaymentModeType.PA_PaymentMode_DirectDeposit:
                    return "Direct Deposit";
                default:
                    return "Direct Deposit";
            }
        }
        private int GetDepartment(string deptName)
        {
            switch (deptName)
            {
                case "Administrator":
                    return 2;
                case "Field Executive":
                    return 6;
                case "Team Leader":
                    return 6;
                default: return 1;
            }
        }
        private int GetRoleId(string role)
        {
            switch (role)
            {
                case "Administrator":
                    return 1;
                case "Team Leader":
                    return 3;
                case "Field Executive":
                    return 2;
                default: return 1;
            }
        }
        private CustomField CreateCustomField(string label, string value)
        {
            var customField = new CustomField()
            {
                label = label,
                value = value

            };
            return customField;
        }
        private List<WorkHour> DefaultWorkHours()
        {
            var workHours = new List<WorkHour>()
            {
                new WorkHour()
                {
                    day = "Sunday",
                    is_enabled = true,
                    start_time = "9:00 AM",
                    end_time = "5:00 PM",
                    work_mins = 480,
                }, 
                new WorkHour()
                {
                    day = "Monday",
                    is_enabled = true,
                    start_time = "9:00 AM",
                    end_time = "5:00 PM",
                    work_mins = 480,
                },
                 new WorkHour()
                {
                    day = "Tuesday",
                    is_enabled = true,
                    start_time = "9:00 AM",
                    end_time = "5:00 PM",
                    work_mins = 480,
                },
                  new WorkHour()
                {
                    day = "Wednesday",
                    is_enabled = true,
                    start_time = "9:00 AM",
                    end_time = "5:00 PM",
                    work_mins = 480,
                },
                   new WorkHour()
                {
                    day = "Thursday",
                    is_enabled = true,
                    start_time = "9:00 AM",
                    end_time = "5:00 PM",
                    work_mins = 480,
                },
                   new WorkHour()
                {
                    day = "Friday",
                    is_enabled = true,
                    start_time = "9:00 AM",
                    end_time = "5:00 PM",
                    work_mins = 480,
                },
                   new WorkHour()
                {
                    day = "Saturday",
                    is_enabled = true,
                    start_time = "9:00 AM",
                    end_time = "5:00 PM",
                    work_mins = 480,
                }

            };
            
            return workHours;
        }
        private async Task<List<ResponseResult>> ImportEmployeesToZuper(List<EmployeeDto> _employees)
        {
            var results = new List<ResponseResult>();

            foreach (var e in _employees)
            {
                var _http = new HttpClient();
                _http.DefaultRequestHeaders.Add("Accept", "application/json");
                _http.DefaultRequestHeaders.Add("x-api-key", _configuration["MetricApiKey"]);
                var response = await _http.PostAsJsonAsync($"{_configuration["ZuperUrl"]}/user", e);
                var responseBody = response.Content.ReadAsStringAsync().Result;
                var result = JsonConvert.DeserializeObject<ResponseResult>(responseBody);
                results.Add(result);
            }

            return results;

        }
        #endregion
    }
}
