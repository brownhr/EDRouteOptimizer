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
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(38, 187);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(125, 27);
            this.textBox1.TabIndex = 0;
            this.textBox1.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // labInputSubsector
            // 
            this.labInputSubsector.AutoSize = true;
            this.labInputSubsector.Location = new System.Drawing.Point(38, 154);
            this.labInputSubsector.Name = "labInputSubsector";
            this.labInputSubsector.Size = new System.Drawing.Size(112, 20);
            this.labInputSubsector.TabIndex = 1;
            this.labInputSubsector.Text = "Input Subsector";
            // 
            // labelOutputSubsector
            // 
            this.labelOutputSubsector.AutoSize = true;
            this.labelOutputSubsector.Location = new System.Drawing.Point(520, 190);
            this.labelOutputSubsector.Name = "labelOutputSubsector";
            this.labelOutputSubsector.Size = new System.Drawing.Size(120, 20);
            this.labelOutputSubsector.TabIndex = 2;
            this.labelOutputSubsector.Text = "OutputSubsector";
            // 
            // buttonUpdateOutput
            // 
            this.buttonUpdateOutput.Location = new System.Drawing.Point(74, 244);
            this.buttonUpdateOutput.Name = "buttonUpdateOutput";
            this.buttonUpdateOutput.Size = new System.Drawing.Size(94, 29);
            this.buttonUpdateOutput.TabIndex = 3;
            this.buttonUpdateOutput.Text = "Update";
            this.buttonUpdateOutput.UseVisualStyleBackColor = true;
            this.buttonUpdateOutput.Click += new System.EventHandler(this.buttonUpdateOutput_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.buttonUpdateOutput);
            this.Controls.Add(this.labelOutputSubsector);
            this.Controls.Add(this.labInputSubsector);
            this.Controls.Add(this.textBox1);
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
    }
}