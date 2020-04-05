namespace Hangout.Shared.UnitTest {
	partial class UnitestMainWindow {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.ResultsBrowser = new System.Windows.Forms.WebBrowser();
			this.SuspendLayout();
			// 
			// ResultsBrowser
			// 
			this.ResultsBrowser.AllowWebBrowserDrop = false;
			this.ResultsBrowser.Dock = System.Windows.Forms.DockStyle.Fill;
			this.ResultsBrowser.Location = new System.Drawing.Point(0, 0);
			this.ResultsBrowser.MinimumSize = new System.Drawing.Size(20, 20);
			this.ResultsBrowser.Name = "ResultsBrowser";
			this.ResultsBrowser.ScriptErrorsSuppressed = true;
			this.ResultsBrowser.Size = new System.Drawing.Size(558, 353);
			this.ResultsBrowser.TabIndex = 1;
			this.ResultsBrowser.WebBrowserShortcutsEnabled = false;
			// 
			// UnitestMainWindow
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(558, 353);
			this.Controls.Add(this.ResultsBrowser);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.SizableToolWindow;
			this.Name = "UnitestMainWindow";
			this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
			this.Text = "Unitest";
			this.Load += new System.EventHandler(this.UnitestMainWindow_Load);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.WebBrowser ResultsBrowser;
	}
}

