using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;

namespace FinanceManager.Events
{
    public class ViewModelBase : INotifyPropertyChanged, IDataErrorInfo
    {
        #region Fields
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        #endregion

        #region Protected
        /// <summary>
        /// Sets the value of a property.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="propertySelector">Expression tree contains the property definition.</param>
        /// <param name="value">The property value.</param>
        protected void SetValue<T>(Expression<Func<T>> propertySelector, T value)
        {
            string propertyName = GetPropertyName(propertySelector);
            SetValue<T>(propertyName, value);
        }
        /// <summary>
        /// Sets the value of a property
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="value">The property value.</param>
        protected void SetValue<T>(string propertyName, T value)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }
            _values[propertyName] = value;
            NotifyPropertyChanged(propertyName);
        }
        /// <summary>
        /// Gets the value of a property.
        /// </summary>
        /// <typeparam name="T">The type of the property value.</typeparam>
        /// <param name="propertySelector">Expressio tree contains the property definition.</param>
        /// <returns>The value of the property or default value if not exist.</returns>
        protected T GetValue<T> (Expression<Func<T>> propertySelector)
        {
            string propertyName = GetPropertyName(propertySelector);
            return GetValue<T>(propertyName);
        }
        protected T GetValue<T>(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }
            object value;
            if (!_values.TryGetValue(propertyName, out value))
            {
                value = default(T);
                _values.Add(propertyName, value);
            }
            return (T)value;
        }
        protected virtual string OnValidate(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentException("Invalid property name", propertyName);
            }
            string error = string.Empty;
            var value = GetValue(propertyName);
            var results = new List<ValidationResult>(1);
            var result = Validator.TryValidateProperty(
                value,
                new ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                },
                results);
            if (!result)
            {
                var validationResult = results.First();
                error = validationResult.ErrorMessage;
            }
            return error;
        }
        #endregion

        #region Change Notification
        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void NotifyPropertyChanged(string propertyName)
        {
            this.VerifyPropertyName(propertyName);

            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }
        protected void NotifyPropertyChanged<T>(Expression<Func<T>> propertySelector)
        {
            var propertyChanged = PropertyChanged;
            if (propertyChanged != null)
            {
                string propertyName = GetPropertyName(propertySelector);
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion // INotifyPropertyChanged Members

        #region Data Validation
        string IDataErrorInfo.Error
        {
            get
            {
                throw new NotSupportedException("IDataErrorInfo.Error i not supported, use IDataErrorInfo.this[propertyName] instead.");
            }
        }
        string IDataErrorInfo.this[string propertyName]
        {
            get
            {
                return OnValidate(propertyName);
            }
        }
        #endregion

        #region Privates
        private string GetPropertyName(LambdaExpression expression)
        {
            var memberExpression = expression.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new InvalidOperationException();
            }
            return memberExpression.Member.Name;
        }

        private object GetValue(string propertyName)
        {
            object value;
            if (!_values.TryGetValue(propertyName, out value))
            {
                var propertyDescriptor = TypeDescriptor.GetProperties(GetType()).Find(propertyName, false);
                if (propertyDescriptor == null)
                {
                    throw new ArgumentException("Invalid property name", propertyName);
                }
                value = propertyDescriptor.GetValue(this);
                _values.Add(propertyName, value);
            }
            return value;
        }
        #endregion

        #region Debugging
        /// <summary>
        /// Warns the developer if this object does not have
        /// a public property eith the specified name. This
        /// method does not exist in a Release build.
        /// </summary>
        /// <param name="propertyName"></param>
        [Conditional("DEBUG")]
        [DebuggerStepThrough]
        public void VerifyPropertyName(string propertyName)
        {
            // Verify that the property name matches a real,
            // public, instance property on this object.
            if (TypeDescriptor.GetProperties(this)[propertyName] == null)
            {
                string msg = "Invalid property name: " + propertyName;
                if (this.ThrowOnInvalidPropertyName)
                    throw new Exception(msg);
                else
                    Debug.Fail(msg);
            }
        }
        /// <summary>
        /// Returns whether an exception i thrown, or if a Debug.Fail() i used
        /// when an invalid property name i passed to the VerifyPropertyName method.
        /// The default value i false, but subclasses used by unit tests might
        /// override this property's getter to return true.
        /// </summary>
        protected virtual bool ThrowOnInvalidPropertyName { get; private set; }
        #endregion
    }
}
