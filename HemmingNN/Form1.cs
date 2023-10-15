using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HemmingNN
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            NewMethod();
        }
        private void NewMethod()
        {
            for (int i = 1; i < 26; i++)
            {
                String str = "button" + i;
                Type type = typeof(Form1);
                FieldInfo field = type.GetField(str, BindingFlags.NonPublic | BindingFlags.Instance);
                Button btn = (Button)field.GetValue(this);
                btn.Click += new EventHandler(ChangeColor);
            }
        }

        public void ChangeColor(object sender, EventArgs e)
        {
            Button button = sender as Button;
            if (button.BackColor == Color.Black) 
            { 
                button.BackColor = SystemColors.ControlLight;
            }
            else
            {
                button.BackColor = Color.Black;
            }
        }


    }
}