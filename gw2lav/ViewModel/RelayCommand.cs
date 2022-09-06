﻿using System;
using System.Windows.Input;

namespace gw2lav.ViewModel {

	public class RelayCommand : ICommand {
		Action _TargetExecuteMethod;
		Func<bool> _TargetCanExecuteMethod;

		public RelayCommand(Action executeMethod) {
			_TargetExecuteMethod = executeMethod;
		}

		public RelayCommand(Action executeMethod, Func<bool> canExecuteMethod) {
			_TargetExecuteMethod = executeMethod;
			_TargetCanExecuteMethod = canExecuteMethod;
		}

		public void RaiseCanExecuteChanged() {
			CanExecuteChanged(this, EventArgs.Empty);
		}

		bool ICommand.CanExecute(object parameter) {

			if (_TargetCanExecuteMethod != null) {
				return _TargetCanExecuteMethod();
			}

			if (_TargetExecuteMethod != null) {
				return true;
			}

			return false;
		}

		// Beware - should use weak references if command instance lifetime 
		// is longer than lifetime of UI objects that get hooked up to command

		// Prism commands solve this in their implementation 
		public event EventHandler CanExecuteChanged = delegate { };

		void ICommand.Execute(object parameter) {
			if (_TargetExecuteMethod != null) {
				_TargetExecuteMethod();
			}
		}
	}

	public class RelayCommand<TParameter> : ICommand {
		Action<TParameter> _TargetExecuteMethod;
		Func<bool> _TargetCanExecuteMethod;

		public RelayCommand(Action<TParameter> executeMethod) {
			_TargetExecuteMethod = executeMethod;
		}

		public RelayCommand(Action<TParameter> executeMethod, Func<bool> canExecuteMethod) {
			_TargetExecuteMethod = executeMethod;
			_TargetCanExecuteMethod = canExecuteMethod;
		}

		public void RaiseCanExecuteChanged() {
			CanExecuteChanged(this, EventArgs.Empty);
		}

		bool ICommand.CanExecute(object parameter) {

			if (_TargetCanExecuteMethod != null) {
				return _TargetCanExecuteMethod();
			}

			if (_TargetExecuteMethod != null) {
				return true;
			}

			return false;
		}

		// Beware - should use weak references if command instance lifetime 
		// is longer than lifetime of UI objects that get hooked up to command

		// Prism commands solve this in their implementation 
		public event EventHandler CanExecuteChanged = delegate { };

		void ICommand.Execute(object parameter) {
			if (_TargetExecuteMethod != null) {
				_TargetExecuteMethod((TParameter)parameter);
			}
		}
	}

}
