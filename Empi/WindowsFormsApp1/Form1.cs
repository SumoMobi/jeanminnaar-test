using System.Windows.Forms;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        public Form1(string firstName, string lastName, string dobMonth, string dobDay, string dobYear, string url, bool isAlreadyRunning)
        {
            InitializeComponent();
            this.Text = "EMPI Search";
            this.firstName.Text = firstName;
            this.lastName.Text = lastName;
            this.dobDay.Text = dobDay;
            this.dobMonth.Text = dobMonth;
            this.dobYear.Text = dobYear;
            this.url.Text = url;
            this.isAlreadyRunning.Checked = isAlreadyRunning;
        }
    }
}
