namespace puppet_master
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.bt_create_server = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // bt_create_server
            // 
            this.bt_create_server.Location = new System.Drawing.Point(197, 12);
            this.bt_create_server.Name = "bt_create_server";
            this.bt_create_server.Size = new System.Drawing.Size(75, 36);
            this.bt_create_server.TabIndex = 0;
            this.bt_create_server.Text = "Create Server";
            this.bt_create_server.UseVisualStyleBackColor = true;
            this.bt_create_server.Click += new System.EventHandler(this.bt_create_server_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.bt_create_server);
            this.Name = "Form1";
            this.Text = "Puppet Master";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button bt_create_server;
    }
}