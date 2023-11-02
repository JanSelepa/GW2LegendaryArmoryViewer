using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;

namespace gw2lav.Service {

	interface IArgsService {
		bool RestartAfterUpdate(string appFileName);
		bool HasArgCleanupUpdate();
	}

	class ArgsService : IArgsService {

		private const string ARG_CLEANUP_UPDATE = "-cleanup-update";

		private Dictionary<string, string> _Args;

		public ArgsService() {
			LoadAppArguments();
		}

		private void LoadAppArguments() {
			// sort arguments
			string[] argArray = Environment.GetCommandLineArgs();
			_Args = new Dictionary<string, string>();

			for (int i = 1; i < argArray.Length; i++) {
				if (argArray.Length == i + 1 || argArray[i + 1].StartsWith("-")) {
					_Args.Add(argArray[i], string.Empty);
				} else {
					_Args.Add(argArray[i], argArray[i + 1]);
					i++;
				}
			}
		}

		public bool HasArgCleanupUpdate() {
			return _Args.ContainsKey(ARG_CLEANUP_UPDATE);
		}

		public bool RestartAfterUpdate(string appFileName) {
			ProcessStartInfo processInfo = new ProcessStartInfo();
			processInfo.FileName = appFileName;
			processInfo.Arguments = ARG_CLEANUP_UPDATE;
			try {
				Process.Start(processInfo);
				Application.Current.Shutdown();
				return true;
			} catch { }
			return false;
		}
	}

}
