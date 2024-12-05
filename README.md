# CsvConverter

CsvConverter is a lightweight and flexible tool for converting CSV files to Excel (XLSX) format, with support for column mapping using a YAML configuration file. The application is built with .NET 8 and features a Windows Forms GUI.

---

## Features

- Convert CSV files to Excel (XLSX) format.
- Customize column mappings using a YAML configuration file.
- Automatically handle missing or mismatched columns with clear error logging.
- User-friendly GUI with support for selecting multiple files.
- Progress tracking and cancelation support during conversions.

---

## Installation

### Prerequisites
- **Windows 10 or later** (for running .NET 8 applications).
- **.NET 8 Runtime**: Download and install from the [official .NET website](https://dotnet.microsoft.com/).

### Download
1. Visit the [Releases](https://github.com/KooleControls/CsvConverter/releases) page of this repository.
2. Download the latest version (zip file or installer).
3. Extract the downloaded archive or run the installer.

---

## Usage

1. Launch the application.
2. Add CSV files to convert by selecting **File > Add Files**.
3. Ensure the `Config.yaml` file is present in the application directory to define column mappings.
4. Click the **Convert** button to start processing the files.
5. Monitor progress using the built-in progress bar.
6. View detailed logs for any errors or missing columns in the log section.

### YAML Configuration Example

The `Config.yaml` file specifies the column mapping from the CSV input to the Excel output.

```yaml
Columns:
  - Input: "CSV Column Name 1"
    OutputIndex: 1
    Output: "Excel Header 1"
  - Input: "CSV Column Name 2"
    OutputIndex: 2
    Output: "Excel Header 2"
```

Place this file in the same directory as the application executable.

---

## Development

### Prerequisites
- Visual Studio 2022 or later with .NET Desktop Development workload installed.

### Building the Project
1. Clone this repository:
   ```bash
   git clone https://github.com/KooleControls/CsvConverter/csvconverter.git
   cd csvconverter
   ```
2. Open the project in Visual Studio.
3. Build the solution to generate the executable.

---

## Contributing

Contributions are welcome! Please follow these steps:
1. Fork the repository.
2. Create a new branch for your feature or bug fix.
3. Commit and push your changes.
4. Submit a pull request.

---

## License

This project is licensed under the [MIT License](LICENSE).

---

## Acknowledgments

- [ClosedXML](https://github.com/ClosedXML/ClosedXML) for working with Excel files.
- [YamlDotNet](https://github.com/aaubry/YamlDotNet) for parsing YAML configuration files.

