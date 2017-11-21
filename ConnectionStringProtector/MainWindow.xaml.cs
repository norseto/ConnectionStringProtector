using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;

namespace ConnectionStringProtector {
	/// <summary>
	/// MainWindow.xaml の相互作用ロジック
	/// </summary>
	public partial class MainWindow {
		public MainWindow() {
			InitializeComponent();
			OnCreate();
		}

		private void OnCreate() {
			selectButton.Click += (sender, args) => { ChooseAndProtect();};
			msdnLink.RequestNavigate += (sender, args) => {
				Process.Start(new ProcessStartInfo(args.Uri.AbsoluteUri));
				args.Handled = true;
			};
		}

		private void ChooseAndProtect() {
			var dialog = new OpenFileDialog {
				Filter = "Config Files (*.config)|*.config"
			};
			var result = dialog.ShowDialog();
			if (result.HasValue && result.Value) {
				var path = dialog.FileName;
				ToggleProtection(path);
			}
		}

		private void ToggleProtection(string path) {
			try {
				var model = new ConnectionStringProtectorModel {
					TargetPath = path
				};
				model.OpenConfiguration();
				var stat = model.ToggleConfigEncryption();
				var mes = stat
					? Properties.Resources.Protected
					: Properties.Resources.Unprotected;
				MessageBox.Show(mes, Properties.Resources.Done,
					MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (NoConnectionStringConfigException) {
				MessageBox.Show(Properties.Resources.NoConnection,
					Properties.Resources.FailedToProcess,
						MessageBoxButton.OK, MessageBoxImage.Error);
			}
			catch (Exception ex) {
				MessageBox.Show(ex.Message, Properties.Resources.FailedToProcess,
					MessageBoxButton.OK, MessageBoxImage.Error);
			}
		}
	}
}
