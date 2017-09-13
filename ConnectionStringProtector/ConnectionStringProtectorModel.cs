using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace ConnectionStringProtector {
	/// <summary>
	/// 接続文字列設定が存在しない場合の例外クラス
	/// </summary>
	public class NoConnectionStringConfigException : ConfigurationErrorsException {
		public NoConnectionStringConfigException() : base("No connection string config") {
			
		}
	}

	/// <summary>
	/// 接続文字列プロテクションモデルクラスです。
	/// 接続文字列の暗号化・復号化を行います。
	/// </summary>
	public class ConnectionStringProtectorModel {
		private Configuration config;

		/// <summary>
		/// 対象設定ファイル
		/// </summary>
		public string TargetPath { get; set; }

		public void OpenConfiguration() {
			if (TargetPath == null) {
				throw new InvalidOperationException("File not set.");
			}
			var map = new ExeConfigurationFileMap {
				ExeConfigFilename = TargetPath,
			};
			config = ConfigurationManager.OpenMappedExeConfiguration(
							map, ConfigurationUserLevel.None);
		}

		public bool ToggleConfigEncryption() {
			var section = config.GetSection("connectionStrings")
							as ConnectionStringsSection;
			if (section == null || section.ConnectionStrings.Count < 1
					|| AsEnumerable(section.ConnectionStrings).Count(
							s => s.ElementInformation.Source != null) < 1) {
				throw new NoConnectionStringConfigException();
			}

			var stat = false;
			if (section.SectionInformation.IsProtected) {
				// Remove encryption.
				section.SectionInformation.UnprotectSection();
			}
			else {
				// Encrypt the section.
				section.SectionInformation.ProtectSection(
					"DataProtectionConfigurationProvider");
				stat = true;
			}
			// Save the current configuration.
			config.Save();
			return stat;
		}

		IEnumerable<ConnectionStringSettings>
				AsEnumerable(ConnectionStringSettingsCollection collection) {
			for (int i = 0, l = collection.Count; i < l; i++) {
				yield return collection[i];
			}
		}
	}
}
