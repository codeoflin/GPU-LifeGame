using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LiftGame
{
	public partial class SelectForm : Form
	{
		public int iRet = -1;
		public SelectForm()
		{
			InitializeComponent();
		}

		private void button1_Click(object sender, EventArgs e)
		{
			iRet = comboBox1.SelectedIndex;
			this.Close();
		}

		public static int Show(string[] SelectItem)
		{
			SelectForm SF = new SelectForm();
			foreach (var item in SelectItem)
			{
				SF.comboBox1.Items.Add(item);
			}
			SF.comboBox1.SelectedIndex = 0;
			SF.ShowDialog();
			return SF.iRet;
		}
	}
}
