﻿namespace WindowsFormsApp1
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
            this.firstName = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.lastName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dobMonth = new System.Windows.Forms.TextBox();
            this.dobDay = new System.Windows.Forms.TextBox();
            this.dobYear = new System.Windows.Forms.TextBox();
            this.url = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.isAlreadyRunning = new System.Windows.Forms.CheckBox();
            this.button1 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // firstName
            // 
            this.firstName.Location = new System.Drawing.Point(88, 12);
            this.firstName.Name = "firstName";
            this.firstName.Size = new System.Drawing.Size(100, 20);
            this.firstName.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "First Name:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(11, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Last Name:";
            // 
            // lastName
            // 
            this.lastName.Location = new System.Drawing.Point(88, 52);
            this.lastName.Name = "lastName";
            this.lastName.Size = new System.Drawing.Size(100, 20);
            this.lastName.TabIndex = 3;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 94);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "Date of Birth:";
            // 
            // dobMonth
            // 
            this.dobMonth.Location = new System.Drawing.Point(88, 94);
            this.dobMonth.Name = "dobMonth";
            this.dobMonth.Size = new System.Drawing.Size(29, 20);
            this.dobMonth.TabIndex = 5;
            // 
            // dobDay
            // 
            this.dobDay.Location = new System.Drawing.Point(123, 94);
            this.dobDay.Name = "dobDay";
            this.dobDay.Size = new System.Drawing.Size(25, 20);
            this.dobDay.TabIndex = 6;
            // 
            // dobYear
            // 
            this.dobYear.Location = new System.Drawing.Point(154, 94);
            this.dobYear.Name = "dobYear";
            this.dobYear.Size = new System.Drawing.Size(50, 20);
            this.dobYear.TabIndex = 7;
            // 
            // url
            // 
            this.url.Location = new System.Drawing.Point(88, 169);
            this.url.Name = "url";
            this.url.ReadOnly = true;
            this.url.Size = new System.Drawing.Size(514, 20);
            this.url.TabIndex = 8;
            this.url.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(15, 169);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(32, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "URL:";
            this.label4.Visible = false;
            // 
            // isAlreadyRunning
            // 
            this.isAlreadyRunning.AutoSize = true;
            this.isAlreadyRunning.Location = new System.Drawing.Point(88, 226);
            this.isAlreadyRunning.Name = "isAlreadyRunning";
            this.isAlreadyRunning.Size = new System.Drawing.Size(115, 17);
            this.isAlreadyRunning.TabIndex = 10;
            this.isAlreadyRunning.Text = "Is Already Running";
            this.isAlreadyRunning.UseVisualStyleBackColor = true;
            this.isAlreadyRunning.Visible = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(88, 131);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 11;
            this.button1.Text = "Search";
            this.button1.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.isAlreadyRunning);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.url);
            this.Controls.Add(this.dobYear);
            this.Controls.Add(this.dobDay);
            this.Controls.Add(this.dobMonth);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lastName);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.firstName);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox firstName;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox lastName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox dobMonth;
        private System.Windows.Forms.TextBox dobDay;
        private System.Windows.Forms.TextBox dobYear;
        private System.Windows.Forms.TextBox url;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox isAlreadyRunning;
        private System.Windows.Forms.Button button1;
    }
}

