using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string firstName, string lastName, string dobMonth, string dobDay, string dobYear, string url)
        {
            InitializeComponent();
            this.firstName.Text = firstName;
            this.lastName.Text = lastName;
            this.dobDay.Text = dobDay;
            this.dobMonth.Text = dobMonth;
            this.dobYear.Text = dobYear;
            this.url.Text = url;
        }
    }
}
