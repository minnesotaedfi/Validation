﻿using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.UI;

using CsvHelper;

namespace ValidationWeb.Utility
{
    public class Csv
    {
        public static byte[] WriteCsvToMemory<T>(IEnumerable<T> records)
        {
            var memoryStream = new MemoryStream();
            var streamWriter = new StreamWriter(memoryStream);

            using (var csvWriter = new CsvWriter(streamWriter, CultureInfo.CurrentCulture))
            {
                csvWriter.WriteRecords(records);
                streamWriter.Flush();
                return memoryStream.ToArray();
            }
        }
    }
}