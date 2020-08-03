using System;
using System.Collections.Generic;
using System.Configuration.Internal;
using System.Globalization;
using System.IO;

using CsvHelper;
using CsvHelper.Configuration;
using CsvHelper.TypeConversion;

namespace ValidationWeb.Utility
{
    public static class Csv
    {
        public static byte[] WriteCsvToMemory<T>(IEnumerable<T> records)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);

            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture))
            {
                csvWriter.Configuration.TypeConverterCache.AddConverter<DateTime>(new MidnightDateTimeConverter());
                csvWriter.Configuration.TypeConverterCache.AddConverter<DateTime?>(new MidnightDateTimeConverter());
                csvWriter.Configuration.TypeConverterCache.AddConverter<string>(new LongNumericStringValueConverter());
                csvWriter.Configuration.TypeConverterCache.AddConverter<int>(new LongIntegerToStringConverter());
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }

        private class LongNumericStringValueConverter : StringConverter
        {
            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                var stringValue = value as string;

                if (stringValue == null)
                {
                    return null;
                }

                int intVal;
                long longVal;
                if (stringValue.Length >= 9 && (int.TryParse(stringValue, out intVal) || long.TryParse(stringValue, out longVal)))
                {
                    return $"=\"{base.ConvertToString(value, row, memberMapData)}\"";
                }

                return base.ConvertToString(value, row, memberMapData);
            }
        }

        private class LongIntegerToStringConverter : Int32Converter
        {
            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                if (value == null)
                {
                    return null;
                }

                var intValue = (int)value;
                var stringValue = intValue.ToString();
                if (stringValue.Length >= 9)
                {
                    return $"=\"{stringValue}\"";
                }

                return base.ConvertToString(value, row, memberMapData);
            }
        }

        private class MidnightDateTimeConverter : DateTimeConverter
        {
            public override string ConvertToString(object value, IWriterRow row, MemberMapData memberMapData)
            {
                if (value == null)
                {
                    return base.ConvertToString(value, row, memberMapData);
                }

                var date = (DateTime)value;

                return (int)date.TimeOfDay.TotalSeconds == 0 ? 
                           date.ToString("MM/dd/yyyy") : 
                           base.ConvertToString(value, row, memberMapData);
            }
        }
    }
}