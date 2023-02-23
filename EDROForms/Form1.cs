namespace EDROForms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            FormControl.UpdateSubsector(textBox1.Text);
        }

        private void buttonUpdateOutput_Click(object sender, EventArgs e)
        {
        }
    }
}