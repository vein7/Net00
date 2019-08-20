namespace WinForm {
    partial class Form1 {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent() {
            this.menu1 = new System.Windows.Forms.MenuStrip();
            this.mItemA = new System.Windows.Forms.ToolStripMenuItem();
            this.mItemB = new System.Windows.Forms.ToolStripMenuItem();
            this.lbl1 = new System.Windows.Forms.Label();
            this.menu1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menu1
            // 
            this.menu1.Items.AddRange(new[] { this.mItemA, this.mItemB });
            this.menu1.Location = new System.Drawing.Point(0, 0);
            this.menu1.Name = "menu1";
            this.menu1.Size = new System.Drawing.Size(284, 24);
            this.menu1.TabIndex = 0;
            this.menu1.Text = "menuStrip1";
            // 
            // mItemA
            // 
            this.mItemA.Name = "mItemA";
            this.mItemA.Size = new System.Drawing.Size(27, 20);
            this.mItemA.Text = "A";
            this.mItemA.Click += this.MItemA_Click;
            // 
            // mItemB
            // 
            this.mItemB.Name = "mItemB";
            this.mItemB.Size = new System.Drawing.Size(26, 20);
            this.mItemB.Text = "B";
            // 
            // lbl1
            // 
            this.lbl1.AutoSize = true;
            this.lbl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbl1.Location = new System.Drawing.Point(0, 24);
            this.lbl1.Name = "lbl1";
            this.lbl1.Size = new System.Drawing.Size(35, 13);
            this.lbl1.TabIndex = 1;
            this.lbl1.Text = "label1";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 262);
            this.Controls.Add(this.lbl1);
            this.Controls.Add(this.menu1);
            this.MainMenuStrip = this.menu1;
            this.Name = "Form1";
            this.Text = "Form1";
            this.menu1.ResumeLayout(false);
            this.menu1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menu1;
        private System.Windows.Forms.ToolStripMenuItem mItemA;
        private System.Windows.Forms.ToolStripMenuItem mItemB;
        private System.Windows.Forms.Label lbl1;
    }
}

