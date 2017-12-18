using System;
using System.ComponentModel;
using System.Globalization;
using Cake.Core.IO;

namespace Cake.Converters
{
    internal sealed class FilePathConverter : TypeConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                return new FilePath(stringValue);
            }
            throw new NotSupportedException("Can't convert value to file path.");
        }
    }
}
