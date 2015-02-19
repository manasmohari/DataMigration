namespace DataMigration
{
    partial class Migrate
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
            this.lstConfigItems = new System.Windows.Forms.CheckedListBox();
            this.btnMigrate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lstConfigItems
            // 
            this.lstConfigItems.CheckOnClick = true;
            this.lstConfigItems.FormattingEnabled = true;
            this.lstConfigItems.Items.AddRange(new object[] {
            "ActivityInstances",
            "Categories",
            "DataItems",
            "Expressions",
            "ExtensionPoints",
            "Pages",
            "Rules"});
            this.lstConfigItems.Location = new System.Drawing.Point(55, 39);
            this.lstConfigItems.Name = "lstConfigItems";
            this.lstConfigItems.Size = new System.Drawing.Size(158, 109);
            this.lstConfigItems.TabIndex = 0;
            // 
            // btnMigrate
            // 
            this.btnMigrate.Location = new System.Drawing.Point(175, 222);
            this.btnMigrate.Name = "btnMigrate";
            this.btnMigrate.Size = new System.Drawing.Size(75, 23);
            this.btnMigrate.TabIndex = 1;
            this.btnMigrate.Text = "Migrate";
            this.btnMigrate.UseVisualStyleBackColor = true;
            this.btnMigrate.Click += new System.EventHandler(this.btnMigrate_Click);
            // 
            // Migrate
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(279, 272);
            this.Controls.Add(this.btnMigrate);
            this.Controls.Add(this.lstConfigItems);
            this.Name = "Migrate";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.CheckedListBox lstConfigItems;
        private System.Windows.Forms.Button btnMigrate;
    }
}

