using acomba.zuper_api.Dto;
using AcoSDK;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace acomba.zuper_api.AcombaServices
{
    public interface IEmployeeService
    {
        Task<string> AddEmployee(EmployeeDto employee);
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
            if(error == 0)
            {
                employeeInt.EmNumber = employeeInt.PKey_EmNumber;
                employeeInt.EmName = employee.last_name;
                employeeInt.EmFirstName = employee.first_name;
                employeeInt.EmSortKey = employee.last_name;
                employeeInt.EmSIN = "000000000";
                employeeInt.EmGender = PA_GenderType.PA_Gender_Min;
                employeeInt.EmMaritalStatus = PA_MaritalStatusType.PA_MaritalStatus_Undefined;
                employeeInt.EmBirthdate = DateTime.Now;
                employeeInt.EmArrivalDate = DateTime.Now;
                employeeInt.EmActive = 1;

                //setting up payment mode
                employeeInt.EmPaymentMode = PA_PaymentModeType.PA_PaymentMode_DirectDeposit;
                employeeInt.EmInstitutionNumber = "000";
                employeeInt.EmBranchNumber = "0";
                employeeInt.EmAccountNumber = "0";

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
                employeeInt.FnDepartmentCP[1] =  GetDepartmentCardPos(GetDepartment(employee.designation));
                employeeInt.EmFunctionsCP[1] = 1;
                employeeInt.EmTotalFunctions= 1;
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
                if(error == 0)
                {
                    return "Success";
                }
                else
                {
                    string _error = "Error :" + Acomba.GetErrorMessage(error);
                    error = employeeInt.FreeCardNumber();
                    if(error == 0)
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
    }
}
