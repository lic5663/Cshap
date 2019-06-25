using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Winform
{
    class MyForm : System.Windows.Forms.Form
    {

    }

    class Program : System.Windows.Forms.Form
    {
        static void Main(string[] args)
        {
            //Application.Run(new Program());
            MyForm form = new MyForm();
            form.Click += new EventHandler(sender,EventArgs) =>
            {
                Application.Exit();
            });

            Application.Run(form);
            
        }
    }
}
