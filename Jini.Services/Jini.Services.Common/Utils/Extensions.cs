using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Gyldendal.Common.WebUtils.Models;
using Gyldendal.Jini.Services.Common.ErrorHandling;
using Kendo.Mvc;

namespace Gyldendal.Jini.Services.Common.Utils
{
    public static class Extensions
    {
        /// <summary>
        /// Contract use for Jini Service Name
        /// </summary>
        public const string JiniServiceName = "Jini Services";

        public static string GetDescription(this Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());

            DescriptionAttribute[] attributes =
                (DescriptionAttribute[])fi.GetCustomAttributes(
                typeof(DescriptionAttribute),
                false);

            if (attributes.Length > 0)
                return attributes[0].Description;
            return value.ToString();
        }

        /// <summary>
        /// Get values of enumerations
        /// </summary>
        /// <typeparam name="T">The enumeration whose values are required</typeparam>
        /// <returns>All values of the enumeration</returns>
        public static IEnumerable<T> GetValues<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        /// <summary>
        /// Get the description of the enumeration
        /// </summary>
        /// <param name="value">The enumeration value</param>
        /// <returns>The </returns>
        public static ErrorDetail GetErrorDetail(this ErrorCodes value)
        {
            var description = value.GetDescription();
            return new ErrorDetail
            {
                Description = description,
                Code = (ulong)value,
                OriginatingSystem = JiniServiceName
            };
        }

        /// <summary>
        /// Parses an int64 to enumeration value
        /// </summary>
        /// <typeparam name="T">The enumeration</typeparam>
        /// <param name="val">The ulong value</param>
        /// <returns>The enumeration value if valid.</returns>
        /// <exception cref="InvalidEnumArgumentException">If value is not valid enumeration value</exception>
        public static T GetEnum<T>(ulong val)
        {
            var enumVal = (T)Enum.ToObject(typeof(T), val);

            if (!Enum.IsDefined(typeof(T), enumVal))
                return default(T);

            //throw new InvalidEnumArgumentException("Provided value is not valid.");

            return enumVal;
        }
        public static List<FilterDescriptor> ToFilterDescriptor(this IList<IFilterDescriptor> filters)
        {
            var result = new List<FilterDescriptor>();
            if (!filters.Any()) return result;
            foreach (var filter in filters)
            {
                switch (filter)
                {
                    case FilterDescriptor descriptor:
                        result.Add(descriptor);
                        break;
                    case CompositeFilterDescriptor compositeFilterDescriptor:
                        result.AddRange(compositeFilterDescriptor.FilterDescriptors.ToFilterDescriptor());
                        break;
                }
            }
            return result;
        }
    }
}