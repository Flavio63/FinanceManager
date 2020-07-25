using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace FinanceManager.Models.Enumeratori
{
    public static class EnumExtensions
    {
        /// <summary>
        /// A generic extension method that aids in reflecting
        /// and retrieving any attribute that is applied to an `Enumeratori`.
        /// </summary>
        public static string GetDisplayName(this Enum enumValue)
        {
            var attr = GetDisplayAttribute(enumValue);
            return attr != null ? attr.Name : enumValue.ToString();
        }

        public static string GetDescription(this Enum enumValue)
        {
            var attr = GetDisplayAttribute(enumValue);
            return attr != null ? attr.Description : enumValue.ToString();
        }

        private static DisplayAttribute GetDisplayAttribute(object value)
        {
            Type type = value.GetType();
            if (!type.IsEnum) { throw new ArgumentException(string.Format("Type {0} is not an enum", type)); }
            var field = type.GetField(value.ToString());
            return field == null ? null : field.GetCustomAttribute<DisplayAttribute>();
        }
    }
}
