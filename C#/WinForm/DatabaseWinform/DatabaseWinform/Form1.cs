using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace DatabaseWinform
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void BtnSearch_Click(object sender, EventArgs e)
        {
            Employee employee = new Employee();

            try
            {
                employee = HrDAC.Instance.GetEmployee(long.Parse(tbEmployeeID.Text));

                tbFirstName.Text = employee.First_name;
                tbLastName.Text = employee.Last_name;
                tbEmail.Text = employee.Email;
                tbPhoneNumber.Text = employee.Phone_number;
                tbHireDate.Text = employee.Hire_date.ToShortDateString();   //shortDateString : 년 일 월만 출력
                tbJobID.Text = employee.Job_id;
                tbSalary.Text = employee.Salary.ToString();
                tbCommissionPCT.Text = employee.Commission_pct.ToString();
                tbManagerID.Text = employee.Manager_id.ToString();
                tbDepartmentID.Text = employee.Department_id.ToString();
            }
            catch (Exception)
            {

            }

        }

        private void BtnGetEmployees_Click(object sender, EventArgs e)
        {
            lvEmployees.Items.Clear();

            List<Employee> employees = HrDAC.Instance.GetEmployees();
            foreach (Employee employee in employees)
            {
                ListViewItem item = new ListViewItem(employee.Employee_id.ToString());
                item.SubItems.Add(employee.First_name + ' ' + employee.Last_name);
                item.SubItems.Add(employee.Email);
                item.SubItems.Add(employee.Phone_number);
                item.SubItems.Add(employee.Hire_date.ToShortDateString());
                item.SubItems.Add(employee.Job_id);
                item.SubItems.Add(employee.Salary.ToString());
                item.SubItems.Add(employee.Commission_pct.ToString());
                item.SubItems.Add(employee.Manager_id.ToString());
                item.SubItems.Add(employee.Department_id.ToString());
                lvEmployees.Items.Add(item);
            }
            tbEmpCount.Text = (lvEmployees.Items.Count).ToString();
        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void Label6_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
            lvEmployees.Columns.Add("직원ID", 70);
            lvEmployees.Columns.Add("이름");
            lvEmployees.Columns.Add("이메일");
            lvEmployees.Columns.Add("전화번호", 100);
            lvEmployees.Columns.Add("고용일자", 100);
            lvEmployees.Columns.Add("직업번호");
            lvEmployees.Columns.Add("급여");
            lvEmployees.Columns.Add("Commission_pct", 150);
            lvEmployees.Columns.Add("관리자 번호", 100);
            lvEmployees.Columns.Add("부서 번호", 100);

            //lvEmployees.View = View.Details;
        }
    }
}
