
# C# MaxMind GeoIP2 Failed RDP Event Logger

A C# script that logs failed RDP events, including the country of origin and timestamp of each attempt. This project can be used in conjunction with Microsoft Sentinel and Azure to create a visual map of login attempts.

This script was developed for the following SOC project blog:[How to Visualize Failed RDP Events Using C# & GeoIP2 in Azure](https://logos-red.com/blog/how-to-visualize-failed-rdp-events-using-c-geoip2-in-azure/)

I couldn't find practical .NET examples for similar needs, so I hope this helps others.

![image](https://github.com/user-attachments/assets/2ce35fe7-22b9-4fcd-a70f-395fa2d264c5)
## Features
- Logs failed RDP login attempts with country of origin.
- Utilizes MaxMind’s GeoIP2 database locally for accurate location data.
- Designed to run in a Windows environment as a Windows service, making it suitable for SOC and security-focused deployments.
## Installation

### Prerequisites

1. **.NET SDK**: Install the .NET SDK if you want to compile the application yourself.
   - Download from [Microsoft .NET SDK](https://dotnet.microsoft.com/download).
   
2. **.NET Runtime**: If you only want to run the compiled application, download the .NET Runtime.
   - Available at [Microsoft .NET Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0).

3. **MaxMind GeoIP2 Country Database**: Create a MaxMind account and download the free GeoLite2 Country database (`.mmdb` file).
   - Visit the [MaxMind GeoIP2 Download Page](https://support.maxmind.com/hc/en-us/articles/4408216129947-Download-and-Update-Databases).

### Steps to Build and Run



1. **Initialize a .NET Console Project**:
    ```bash
    dotnet new console -n GeoLogger
    cd GeoLogger
    dotnet add package MaxMind.GeoIP2
    dotnet add package Microsoft.Extensions.Hosting.WindowsServices
    ```

2. **Replace `Program.cs`**: Copy and replace the default `Program.cs` in your project folder with the `Program.cs` file from this repository.

3. **Update Paths**:
    - In `Program.cs`, replace:
      - `<REPLACE_WITH_DATABASE_PATH>` with the path to your `.mmdb` GeoIP database file.
      - `<REPLACE_WITH_OUTPUT_LOCATION>` with your preferred output log file path.

4. **Build and Run the Program**:
    - To run directly:
      ```bash
      dotnet run
      ```
    - Alternatively, to compile and run as an executable:
      ```bash
      dotnet publish -c Release -o ./output
      ```

### Running as an .exe (Pre-compiled)

1. **Download the `.exe` from Releases**:
   - Ensure the .NET Runtime is installed (see above).
   
2. **Keep Files in `C:\MaxMind`**:
   - Place your `.mmdb` file in `C:\MaxMind` to simplify paths.

3. **Run the .exe**:
   - Execute the `.exe` to start logging failed RDP events.

### Example

![image](https://github.com/user-attachments/assets/95fa796a-e361-4c3b-90fa-09603dd6ddf7)
