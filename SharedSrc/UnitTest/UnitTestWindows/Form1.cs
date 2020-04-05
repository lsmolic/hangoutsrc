using System;
using System.Windows.Forms;

namespace Hangout.Shared.UnitTest {
	public partial class UnitestMainWindow : Form {
		public UnitestMainWindow(string fileToOpen) {
			InitializeComponent();

			this.ResultsBrowser.Url = new Uri(fileToOpen);
		}

		private void button1_Click(object sender, EventArgs e) {
			Application.Exit();
		}

		private void UnitestMainWindow_KeyDown(object sender, EventArgs e) {

		}

		private void UnitestMainWindow_Load(object sender, EventArgs e) {

		}
	}
}
