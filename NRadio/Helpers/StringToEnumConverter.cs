using System;
using Windows.UI.Xaml.Data;

namespace NRadio.Helpers
{
    public class StringToEnumConverter : IValueConverter
    {
        public Type EnumType { get; set; }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (parameter is string enumString)
                return Enum.Parse(EnumType, enumString);

            throw new ArgumentException("parameter must be an Enum name!");
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            if (value is Enum enumValue)
                return enumValue.ToString();

            throw new ArgumentException("value must be an Enum!");
        }
    }
}
