using ClosedXML.Excel;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization.NamingConventions;
using YamlDotNet.Serialization;
using System.Text.Json.Serialization;

namespace CsvConverter
{
    public class CsvColumnMapper
    {
        // Holds the column mapping loaded from YAML
        private readonly Dictionary<string, (int ColumnIndex, string Header)>? columnMapping;
        private readonly ILogger<CsvColumnMapper> logger;
        public CsvColumnMapper(ILogger<CsvColumnMapper> logger, string configFile)
        {
            this.logger = logger;
            columnMapping = LoadColumnMapping(configFile);
        }
        public async Task<bool> ConvertAsync(string inputFile, string outputFile, CancellationToken token)
        {
            if (columnMapping == null)
            {
                logger.LogError("Column mapping is missing.");
                return false;
            }

            try
            {
                logger.LogInformation($"Starting conversion for file: {inputFile}");

                // Step 1: Read CSV file
                var csvLines = await ReadCsvFile(inputFile, token);
                if (csvLines == null) return false;

                // Step 2: Parse header row
                var csvColumnIndex = ParseCsvHeaders(csvLines[0]);
                if (csvColumnIndex == null) return false;

                // Step 3: Validate column mappings
                if (!ValidateColumnMappings(csvColumnIndex)) return false;

                // Step 4: Convert CSV to XLSX
                WriteToExcel(csvLines, csvColumnIndex, outputFile, token);

                logger.LogInformation($"CSV file successfully converted to XLSX at: {outputFile}");
                return true;
            }
            catch (OperationCanceledException)
            {
                logger.LogWarning($"Conversion canceled for file: {inputFile}");
                return false;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error processing file {inputFile}: {ex.Message}");
                return false;
            }
        }

        private async Task<string[]?> ReadCsvFile(string inputFile, CancellationToken token)
        {
            try
            {
                var csvLines = await File.ReadAllLinesAsync(inputFile, token);
                logger.LogInformation($"Read {csvLines.Length} lines from {inputFile}");
                return csvLines;
            }
            catch (Exception ex)
            {
                logger.LogError($"Failed to read CSV file {inputFile}: {ex.Message}");
                return null;
            }
        }

        private Dictionary<string, int>? ParseCsvHeaders(string headerRow)
        {
            var csvHeaders = headerRow.Split(';');
            logger.LogInformation($"Parsed {csvHeaders.Length} columns in the header row");

            var csvColumnIndex = new Dictionary<string, int>();
            for (int i = 0; i < csvHeaders.Length; i++)
            {
                csvColumnIndex[csvHeaders[i]] = i;
            }

            return csvColumnIndex;
        }

        private bool ValidateColumnMappings(Dictionary<string, int> csvColumnIndex)
        {
            if (columnMapping == null)
            {
                logger.LogError("Column mapping is missing.");
                return false;
            }

            var missingColumns = columnMapping.Keys.Where(key => !csvColumnIndex.ContainsKey(key)).ToList();

            if (missingColumns.Count != 0)
            {
                logger.LogWarning($"Missing required columns in the CSV file: {string.Join(", ", missingColumns)}");
                return true;
            }

            logger.LogInformation("All required columns are present in the CSV file.");
            return true;
        }

        private void WriteToExcel(string[] csvLines, Dictionary<string, int> csvColumnIndex, string outputFile, CancellationToken token)
        {
            if (columnMapping == null)
            {
                logger.LogError("Column mapping is missing.");
                return;
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Sheet1");

            // Write headers
            foreach (var mapping in columnMapping)
            {
                worksheet.Cell(1, mapping.Value.ColumnIndex).Value = mapping.Value.Header;
            }
            logger.LogInformation("Headers written to the Excel file.");

            // Write data rows
            for (int rowIndex = 1; rowIndex < csvLines.Length; rowIndex++)
            {
                token.ThrowIfCancellationRequested();
                var csvRow = csvLines[rowIndex].Split(';');

                foreach (var mapping in columnMapping)
                {
                    if (csvColumnIndex.TryGetValue(mapping.Key, out int csvIndex))
                    {
                        worksheet.Cell(rowIndex + 1, mapping.Value.ColumnIndex).Value = csvRow[csvIndex];
                    }
                }
            }
            logger.LogInformation("Data rows written to the Excel file.");

            // Auto-fit columns and save
            worksheet.Columns().AdjustToContents();
            workbook.SaveAs(outputFile);
        }

        private Dictionary<string, (int ColumnIndex, string Header)>? LoadColumnMapping(string yamlFilePath)
        {
            try
            {
                var deserializer = new DeserializerBuilder()
                    .WithNamingConvention(CamelCaseNamingConvention.Instance)
                    .Build();

                using var reader = new StreamReader(yamlFilePath);
                var config = deserializer.Deserialize<MappingConfig>(reader);
                var mapping = new Dictionary<string, (int ColumnIndex, string Header)>();

                foreach (var column in config.Columns)
                {
                    mapping[column.Input] = (column.OutputIndex, column.Output);
                }

                return mapping;
            }
            catch (Exception ex)
            {
                logger.LogError($"Error loading YAML file: {ex.Message}");
                return null;
            }
        }
    }
}
