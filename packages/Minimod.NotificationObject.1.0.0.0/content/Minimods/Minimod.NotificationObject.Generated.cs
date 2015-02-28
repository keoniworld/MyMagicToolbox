using System;
using System.ComponentModel;
using System.Linq.Expressions;
using Minimod.ExpressionExtensions;

namespace Minimod.NotificationObject
{
    /// <summary>
    /// Interace for objects that can raise PropertyChanged events
    /// </summary>
    public interface IRaisePropertyChanged : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets or sets a value indicating if this object actually raises the PropertyChanged event
        /// </summary>
        bool IsNotifying { get; set; }

        /// <summary>
        /// Raises the ProperyChanged event for the given property, if IsNotifying is set to true
        /// </summary>
        /// <param name="property">The name of the changed property</param>
        void RaisePropertyChanged(string property);

        /// <summary>
        /// Raises the ProperyChanged event for the given property, if IsNotifying is set to true
        /// </summary>
        /// <param name="property">The name of the changed property</param>
        void RaisePropertyChangedArgs(PropertyChangedEventArgs property);

        /// <summary>
        /// Raises the ProperyChanged event for the given property, if IsNotifying is set to true
        /// </summary>
        /// <param name="property">A member expression for the changed property. Example: 
        ///     <code>
        ///         this.RaisePropertyChanged(() => this.SomeProperty);
        ///     </code>
        /// </param>
        void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property);

        /// <summary>
        /// Raises the ProperyChanged event for all public properties, if IsNotifying is set to true
        /// </summary>
        void RaisePropertyChangedOnAllInstanceProperties();
    }

    /// <summary>
    /// Interace for objects that can raise PropertyChanging events
    /// </summary>
    public interface IRaisePropertyChanging : INotifyPropertyChanging
    {
        /// <summary>
        /// Gets or sets a value indicating if this object actually raises the PropertyChanging event
        /// </summary>
        bool IsNotifying { get; set; }

        /// <summary>
        /// Raises the PropertyChanging event for the given property, if IsNotifying is set to true
        /// </summary>
        /// <param name="property">The name of the changing property</param>
        void RaisePropertyChanging(string property);

        /// <summary>
        /// Raises the PropertyChanging event for the given property, if IsNotifying is set to true
        /// </summary>
        /// <param name="property">The name of the changing property</param>
        void RaisePropertyChangingArgs(PropertyChangingEventArgs property);

        /// <summary>
        /// Raises the PropertyChanging event for the given property, if IsNotifying is set to true
        /// </summary>
        /// <param name="property">A member expression for the changing property. Example: 
        ///     <code>
        ///         this.RaisePropertyChanging(() => this.SomeProperty);
        ///     </code>
        /// </param>
        void RaisePropertyChanging<TProperty>(Expression<Func<TProperty>> property);

        /// <summary>
        /// Raises the PropertyChanging event for all public properties, if IsNotifying is set to true
        /// </summary>
        void RaisePropertyChangingOnAllInstanceProperties();
    }

    public class NotificationObject : IRaisePropertyChanged, IRaisePropertyChanging
    {
        public NotificationObject()
        {
            this.IsNotifying = true;
        }

        /// <summary>
        /// Gets or sets a value indicating if this object raises PropertyChanged events or not.
        /// The default value is true.
        /// </summary>
        public virtual bool IsNotifying
        {
            get;
            set;
        }

        #region IRaisePropertyChanged

        /// <summary>
        /// Raises the PropertyChanged event
        /// </summary>
        /// <typeparam name="TProperty">Type of the changed property</typeparam>
        /// <param name="property">expression pointing to the changed property</param>
        public void RaisePropertyChanged<TProperty>(Expression<Func<TProperty>> property)
        {
            if (!IsNotifying)
                return;
            RaisePropertyChanged(property.PropertyName());
        }

        /// <summary>
        /// Raises the property changed event for all properties of this instance
        /// </summary>
        public void RaisePropertyChangedOnAllInstanceProperties()
        {
            if (!IsNotifying)
                return;

            RaisePropertyChanged(null);
        }

        /// <summary>
        ///  Raises the PropertyChanged event
        /// </summary>
        /// <param name="property">the name of the changed property</param>
        public void RaisePropertyChanged(string property)
        {
            if (!IsNotifying)
                return;
            RaisePropertyChangedArgs(new PropertyChangedEventArgs(property));
        }

        /// <summary>
        ///  Raises the PropertyChanged event
        /// </summary>
        /// <param name="property">the PropertyChangedEventArgs to be raised</param>
        public void RaisePropertyChangedArgs(PropertyChangedEventArgs property)
        {
            if (!IsNotifying)
                return;

            if (PropertyChanged != null)
            {
                PropertyChanged(this, property);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        #endregion

        #region IRaisePropertyChanging

        /// <summary>
        /// Raises the PropertyChanging event for the given property, if IsNotifying is set to true
        /// </summary>
        /// <param name="property">A member expression for the changing property. Example: 
        ///     <code>
        ///         this.RaisePropertyChanging(() => this.SomeProperty);
        ///     </code>
        /// </param>
        public void RaisePropertyChanging<TProperty>(Expression<Func<TProperty>> property) 
        {
            if (!IsNotifying)
                return;
            RaisePropertyChanging(property.PropertyName());
        }

        /// <summary>
        /// Raises the PropertyChanging event for all public properties, if IsNotifying is set to true
        /// </summary>
        public void RaisePropertyChangingOnAllInstanceProperties() 
        {
            if (!IsNotifying)
                return;
            RaisePropertyChanging(null);
        }

        /// <summary>
        /// Raises the PropertyChanging event for the given property, if IsNotifying is set to true
        /// </summary>
        /// <param name="property">The name of the changing property</param>
        public void RaisePropertyChanging(string property)
        {
            if (!IsNotifying)
                return;
            RaisePropertyChangingArgs(new PropertyChangingEventArgs(property));
        }

        /// <summary>
        /// Raises the PropertyChanging event for the given property, if IsNotifying is set to true
        /// </summary>
        /// <param name="property">The name of the changing property</param>
        public void RaisePropertyChangingArgs(PropertyChangingEventArgs property)
        {
            if (!IsNotifying)
                return;

            if (PropertyChanging != null)
            {
                PropertyChanging(this, property);
            }
        }

        public event PropertyChangingEventHandler PropertyChanging;

        #endregion IRaisePropertyChanging
    }
}
