using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseWinform
{
    class HrDAC
    {
        private string connectionString;
        private static readonly HrDAC instance = new HrDAC();   // 시작하자마자 하나 만듬
        
        private HrDAC() // 생성자가 private이므로 외부에선 만들 수 없다.
        {
            //connectionString = "DATA SOURCE=XE;User Id=c##hr;Password=tiger";
            connectionString = Properties.Settings.Default.ConnectionString;
        }
        public static HrDAC Instance
        {
            get
            {
                return instance;
            }
        }
        
        public List<Employee> GetEmployees()
        {
            List<Employee> employees = new List<Employee>();
            string sql = "SELECT * FROM EMPLOYEES";

            using (OracleConnection conn = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand(sql, conn)
                {
                    CommandType = CommandType.Text,
                    BindByName = true
                };
                conn.Open();

                using (OracleDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())   // 한줄 읽힌다
                    {
                        Employee employee = new Employee();
                        employee.Employee_id = reader.GetInt64(reader.GetOrdinal("EMPLOYEE_ID"));
                        employee.First_name = reader.GetString(reader.GetOrdinal("FIRST_NAME"));
                        employee.Last_name = reader.GetString(reader.GetOrdinal("LAST_NAME"));
                        employee.Email = reader.GetString(reader.GetOrdinal("EMAIL"));
                        employee.Phone_number = reader.GetString(reader.GetOrdinal("PHONE_NUMBER"));
                        employee.Hire_date = reader.GetDateTime(reader.GetOrdinal("HIRE_DATE"));
                        employee.Job_id = reader.GetString(reader.GetOrdinal("JOB_ID"));
                        employee.Salary = reader.GetDouble(reader.GetOrdinal("SALARY"));
                        employee.Commission_pct = reader.IsDBNull(reader.GetOrdinal("COMMISSION_PCT")) ? 0.0 : reader.GetDouble(reader.GetOrdinal("COMMISSION_PCT"));
                        employee.Manager_id = reader.IsDBNull(reader.GetOrdinal("MANAGER_ID")) ? 0 : reader.GetInt64(reader.GetOrdinal("MANAGER_ID"));
                        employee.Department_id = reader.IsDBNull(reader.GetOrdinal("DEPARTMENT_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("DEPARTMENT_ID"));

                        employees.Add(employee);
                    }
                }
            }
            return employees;
        }
        

        public Employee GetEmployee(long id)
        {
            Employee employee = new Employee();
            string sql = "SELECT * FROM EMPLOYEES WHERE EMPLOYEE_ID = :EMPLOYEE_ID";
            using(OracleConnection conn = new OracleConnection(connectionString))
            {
                OracleCommand cmd = new OracleCommand(sql, conn)
                {
                    CommandType = CommandType.Text,
                    BindByName = true
                };
                cmd.Parameters.Add(":EMPLOYEE_ID", OracleDbType.Long).Value = id;
                conn.Open();

                using (OracleDataReader reader = cmd.ExecuteReader(CommandBehavior.SingleRow))
                {
                    if(reader.Read())
                    {
                        employee.Employee_id = reader.GetInt64(reader.GetOrdinal("EMPLOYEE_ID"));
                        //employee.Employee_id = reader.GetInt64(0);    // 위랑 같은 뜻이다.
                        employee.First_name = reader.GetString(reader.GetOrdinal("FIRST_NAME"));
                        employee.Last_name = reader.GetString(reader.GetOrdinal("LAST_NAME"));
                        employee.Email = reader.GetString(reader.GetOrdinal("EMAIL"));
                        employee.Phone_number = reader.GetString(reader.GetOrdinal("PHONE_NUMBER"));
                        employee.Hire_date = reader.GetDateTime(reader.GetOrdinal("HIRE_DATE"));
                        employee.Job_id = reader.GetString(reader.GetOrdinal("JOB_ID"));
                        employee.Salary = reader.GetDouble(reader.GetOrdinal("SALARY"));
                        employee.Commission_pct = reader.IsDBNull(reader.GetOrdinal("COMMISSION_PCT")) ? 0.0 : reader.GetDouble(reader.GetOrdinal("COMMISSION_PCT"));
                        employee.Manager_id = reader.IsDBNull(reader.GetOrdinal("MANAGER_ID")) ? 0 : reader.GetInt64(reader.GetOrdinal("MANAGER_ID"));
                        employee.Department_id = reader.IsDBNull(reader.GetOrdinal("DEPARTMENT_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("DEPARTMENT_ID"));
                    }
                }
            }
            return employee;
        }
    }
}
