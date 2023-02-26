namespace EDROForms
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.labInputSubsector = new System.Windows.Forms.Label();
            this.labelOutputSubsector = new System.Windows.Forms.Label();
            this.buttonUpdateOutput = new System.Windows.Forms.Button();
            this.labelCurrentRoute = new System.Windows.Forms.Label();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.buttonLoadRoute = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(33, 140);
            this.textBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(110, 23);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // labInputSubsector
            // 
            this.labInputSubsector.AutoSize = true;
            this.labInputSubsector.Location = new System.Drawing.Point(33, 116);
            this.labInputSubsector.Name = "labInputSubsector";
            this.labInputSubsector.Size = new System.Drawing.Size(90, 15);
            this.labInputSubsector.TabIndex = 1;
            this.labInputSubsector.Text = "Input Subsector";
            // 
            // labelOutputSubsector
            // 
            this.labelOutputSubsector.AutoSize = true;
            this.labelOutputSubsector.Location = new System.Drawing.Point(46, 191);
            this.labelOutputSubsector.Name = "labelOutputSubsector";
            this.labelOutputSubsector.Size = new System.Drawing.Size(97, 15);
            this.labelOutputSubsector.TabIndex = 2;
            this.labelOutputSubsector.Text = "OutputSubsector";
            this.labelOutputSubsector.Click += new System.EventHandler(this.labelOutputSubsector_Click);
            // 
            // buttonUpdateOutput
            // 
            this.buttonUpdateOutput.Location = new System.Drawing.Point(50, 167);
            this.buttonUpdateOutput.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonUpdateOutput.Name = "buttonUpdateOutput";
            this.buttonUpdateOutput.Size = new System.Drawing.Size(82, 22);
            this.buttonUpdateOutput.TabIndex = 3;
            this.buttonUpdateOutput.Text = "Update";
            this.buttonUpdateOutput.UseVisualStyleBackColor = true;
            this.buttonUpdateOutput.Click += new System.EventHandler(this.buttonUpdateOutput_Click);
            // 
            // labelCurrentRoute
            // 
            this.labelCurrentRoute.AutoSize = true;
            this.labelCurrentRoute.Location = new System.Drawing.Point(434, 98);
            this.labelCurrentRoute.Name = "labelCurrentRoute";
            this.labelCurrentRoute.Size = new System.Drawing.Size(89, 15);
            this.labelCurrentRoute.TabIndex = 4;
            this.labelCurrentRoute.Text = "(Current Route)";
            this.labelCurrentRoute.Click += new System.EventHandler(this.label1_Click);
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.InitialDirectory = "@\"C:\\Users\\brownhr\\Documents\\\"";
            this.openFileDialog1.RestoreDirectory = true;
            this.openFileDialog1.Title = "Import Route file";
            this.openFileDialog1.FileOk += new System.ComponentModel.CancelEventHandler(this.openFileDialog1_FileOk);
            // 
            // buttonLoadRoute
            // 
            this.buttonLoadRoute.Location = new System.Drawing.Point(448, 72);
            this.buttonLoadRoute.Name = "buttonLoadRoute";
            this.buttonLoadRoute.Size = new System.Drawing.Size(75, 23);
            this.buttonLoadRoute.TabIndex = 5;
            this.buttonLoadRoute.Text = "Load Route";
            this.buttonLoadRoute.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.buttonLoadRoute.UseVisualStyleBackColor = true;
            this.buttonLoadRoute.Click += new System.EventHandler(this.buttonLoadRoute_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(700, 338);
            this.Controls.Add(this.buttonLoadRoute);
            this.Controls.Add(this.labelCurrentRoute);
            this.Controls.Add(this.buttonUpdateOutput);
            this.Controls.Add(this.labelOutputSubsector);
            this.Controls.Add(this.labInputSubsector);
            this.Controls.Add(this.textBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "MainForm";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private TextBox textBox1;
        private Label labInputSubsector;
        private Label labelOutputSubsector;
        private Button buttonUpdateOutput;
        private Label labelCurrentRoute;
        private OpenFileDialog openFileDialog1;
        private Button buttonLoadRoute;
    }
}