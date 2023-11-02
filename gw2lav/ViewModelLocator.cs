using gw2lav.Service;
using gw2lav.ViewModel;

namespace gw2lav {

	class ViewModelLocator {

		private IocContainer _IocContainer;

		public LegendaryViewModel Main => _IocContainer.Create<LegendaryViewModel>();

		public ViewModelLocator() {

			_IocContainer = new IocContainer();

			_IocContainer.Register<IDialogService, DialogService>();
			_IocContainer.Register<IArgsService, ArgsService>();
			_IocContainer.Register<IUpdateService, UpdateService>();
			_IocContainer.Register<IRegistryService, RegistryService>();
			_IocContainer.Register<IApiService, ApiService>();

			_IocContainer.Register<LegendaryViewModel, LegendaryViewModel>();

		}

	}

}
