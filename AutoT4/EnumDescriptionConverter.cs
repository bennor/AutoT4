using System;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

namespace BennorMcCarthy.AutoT4
{
    public class EnumDescriptionConverter : EnumConverter
    {
        private readonly Type _enumType;

        public EnumDescriptionConverter(Type type)
            : base(type)
        {
            _enumType = type;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            if (value == null)
                return null;

            var field = _enumType.GetField(Enum.GetName(_enumType, value));
            var attribute = field.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>().FirstOrDefault();
            return attribute != null
                       ? attribute.Description
                       : value.ToString();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (stringValue == null)
                return Activator.CreateInstance(_enumType);

            var field = _enumType.GetFields()
                                 .FirstOrDefault(f => f.GetCustomAttributes(typeof(DescriptionAttribute), true).Cast<DescriptionAttribute>()
                                                       .Any(a => stringValue.Equals(a.Description, StringComparison.OrdinalIgnoreCase)));
            return field != null
                       ? field.GetValue(null)
                       : Enum.Parse(_enumType, stringValue);
        }
    }
}