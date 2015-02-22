using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using MagicLibrary;

namespace MyMagicCollection.Caliburn
{
    /// <summary>
    /// Class <c>CaliburnMEFMvvmAppBootstrapper</c>.
    /// </summary>
    internal class CaliburnMEFMvvmAppBootstrapper : Bootstrapper<IShell>, IDisposable
    {
        /// <summary>
        /// The <c>container</c>
        /// </summary>
        private CompositionContainer _container;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            _container.Dispose();
        }

        /// <summary>
        /// Override to configure the framework and setup your IoC container.
        /// </summary>
        protected override void Configure()
        {
            var catalog = new AggregateCatalog(AssemblySource.Instance.Select(x => new AssemblyCatalog(x)).OfType<ComposablePartCatalog>());

            var exeFolder = new FileInfo(Assembly.GetEntryAssembly().Location).Directory.FullName;
            var assemblies = Directory.EnumerateFiles(exeFolder, "*.dll", SearchOption.TopDirectoryOnly)
                .Select(f => Assembly.LoadFile(f))
                .ToArray();

            foreach (var item in assemblies)
            {
                catalog.Catalogs.Add(new AssemblyCatalog(item));
            }

            _container = new CompositionContainer(catalog);
            MefHelper.Container = _container;

            var batch = new CompositionBatch();

            batch.AddExportedValue<IWindowManager>(new WindowManager());
            batch.AddExportedValue<IEventAggregator>(new EventAggregator());
            batch.AddExportedValue(_container);

            _container.Compose(batch);
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="key">        The key.</param>
        /// <returns>Returns an instance of <see cref="System.Object"/>.</returns>
        /// <exception cref="System.Exception">This exception will be thrown when XXX.</exception>
        protected override object GetInstance(Type serviceType, string key)
        {
            string contract = "";
            try
            {
                contract = string.IsNullOrEmpty(key) ? AttributedModelServices.GetContractName(serviceType) : key;
                var exports = _container.GetExportedValues<object>(contract).ToArray();

                if (exports.Any())
                {
                    return exports.First();
                }
            }
            catch (Exception error)
            {
                int debug = 0;
            }

            throw new Exception(string.Format("Could not locate any instances of contract {0}.", contract));
        }

        /// <summary>
        /// Gets all instances.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <returns>Returns an instance of <see cref="object"/>.</returns>
        protected override IEnumerable<object> GetAllInstances(Type serviceType)
        {
            return _container.GetExportedValues<object>(AttributedModelServices.GetContractName(serviceType));
        }

        protected override void OnStartup(object sender, System.Windows.StartupEventArgs e)
        {
            base.OnStartup(sender, e);

            Application.MainWindow.SizeToContent = SizeToContent.Manual;
            Application.MainWindow.WindowState = WindowState.Maximized;
        }

        /// <summary>
        /// Override this to provide an IoC specific implementation.
        /// </summary>
        /// <param name="instance">The instance to perform injection on.</param>
        protected override void BuildUp(object instance)
        {
            _container.SatisfyImportsOnce(instance);
        }
    }
}