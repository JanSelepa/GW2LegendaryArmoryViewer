﻿using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows;

namespace gw2lav {
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application {

		public App() {
			AppDomain.CurrentDomain.AssemblyResolve += OnResolveAssembly;
		}

		/*
		 * Used for embedding *.dll into main .exe file
		 */
		private static Assembly OnResolveAssembly(object sender, ResolveEventArgs args) {
			Assembly executingAssembly = Assembly.GetExecutingAssembly();
			AssemblyName assemblyName = new AssemblyName(args.Name);

			var path = assemblyName.Name + ".dll";
			if (assemblyName.CultureInfo.Equals(CultureInfo.InvariantCulture) == false) path = String.Format(@"{0}\{1}", assemblyName.CultureInfo, path);

			using (Stream stream = executingAssembly.GetManifestResourceStream(path)) {
				if (stream == null) return null;

				var assemblyRawBytes = new byte[stream.Length];
				stream.Read(assemblyRawBytes, 0, assemblyRawBytes.Length);
				return Assembly.Load(assemblyRawBytes);
			}
		}

	}
}
