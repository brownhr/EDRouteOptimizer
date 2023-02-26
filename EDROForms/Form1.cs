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

        private void labelOutputSubsector_Click(object sender, EventArgs e)
        {

        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void openFileDialog1_FileOk(object sender, System.ComponentModel.CancelEventArgs e)
        {

        }

        private void buttonLoadRoute_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                labelCurrentRoute.Text = openFileDialog1.FileName;
            }
        }

        private void OptimizeRoute(string inputFilename, string outputFilename)
        {



        }
    }
}