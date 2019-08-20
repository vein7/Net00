using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WinForm {
    public partial class Form1 : Form {
        int count;
        public Form1() {
            InitializeComponent();
        }

        private async void MItemA_Click(object sender, EventArgs e) {
            lbl1.Text = $"{++count}....";
            lbl1.Text = await Task.Run(Test1);
        }

        string Test1() {
            Task.Delay(3000).Wait();
            return new string('a', count * 2);
        }
    }
}
