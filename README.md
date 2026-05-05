# Roll Call : Team 6

[Live link to website](https://youtu.be/dQw4w9WgXcQ?si=W1QA91vpuoDlkPvq)

## Members

Dillon Davis (DDavis34) - SCRUM Master\
Timothy Posley (Tposley) - UI/UX Designer\
Michael Russelburg (MichaelRDot) - Software Architect\
Corbin Brescher (JosiahMatriarch) - Senior Developer\
John Holcomb (johnholcomb10) - Product Tester

## About Our Website

This is the semester-long project of Team 6 for CSC 4330, __Roll Call__. It is a _Pathfinder 2E_ character creator capable of connecting players to their DM. It is focused on providing the necessary information and guides needed for new players to understand _Pathfinder_'s character creator system, while also having an interesting and simple UI that can keep veteran players engaged. Characters can be shared with your DM, allowing for quicker,more thought-out, and personalized _Pathfinder_ runs.

### Known Platform Compatibility

- Tested on Chrome, Edge, Firefox
- Designed with responsiveness in mind for 280px - 1920px wide

### Important Links

[Kanban Board](https://github.com/MichaelRdot/Roll-Call/issues)\
[Designs](https://www.figma.com/make/xWvil0qVnhfGsvBB30UfMl/Pathfinder-2E-Character-Builder?p=f)\
[C# Styles Guide](https://www.dofactory.com/csharp-coding-standards)\
[HTML Style Guide](https://www.w3schools.com/htmL/html5_syntax.asp)

## How to Run Dev and Test Environment (Windows 11)

### Dependencies

- [Git Bash (version 2.49.0)](https://git-scm.com/downloads)

- [GitHub Desktop (version 3.5.8)](https://desktop.github.com/download/)

- [VS Code (version 1.118.1)](https://code.visualstudio.com/download) + extensions within VS Code:
  - C# :: ms-dotnettools.csharp
  - C# Dev Kit :: ms-dotnettools.csdevkit
  - .NET Install Tool (should be automatically installed alongside C# or C# Dev Kit) :: ms-dotnettools.VS Code-dotnet-runtime

- [.NET SDK (version 10.0.203)](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-10.0.203-windows-x64-installer)

#### Not required but recommended

- Other helpful VS Code extensions:
  - IntelliCode for C# Dev Kit :: ms-dotnettools.VS Codeintellicode-csharp
  - HTML CSS Support :: ecmel.vscode-html-css

### Downloading Dependencies

1. Download VS Code (link in Dependencies section). After installing VS Code, extensions can be found by searching their names within the search bar in the "Extensions" tab on VS Code (link for each extension in Dependencies section).
2. You will likely get an error within VS Code stating that a .NET SDK could not be found. You will have to download and install the .NET SDK (link in Dependencies section) if you have not already done so. You will have to restart your device once the SDK is installed.
3. Additionally, download the GitHub desktop app and Git Bash (links in Dependencies section) to make commits easier and to create clone/pull requests straight from the app.

### Getting Started with the Project

1. Make sure the Main branch is selected on GitHub.com and clone the repository onto your device to access all files. You can do this by copying the HTTPS link that shows up when clicking the "Code" button on GitHub.com when within this repository.
2. After copying the link, go to the GitHub Desktop app and click "File" on the top left, then select "Clone Repository" and paste the repo link under the URL tab.

### Setting up Project to Make Changes and Run Afterwards

1. In VS Code, hit "File" in the top left, then select "Open Folder" in the dropdown.
2. Navigate to the folder where you cloned the repository to. Select the folder named Roll-Call, and hit "Select Folder."
3. Once you have made any changes or just want to run the project, open a terminal by selecting Terminal > New Terminal from the top menu. Use the command `cd PF2EGM` to navigate to the correct folder, then use `dotnet run`.
4. Once the project builds, it should automatically open a webpage in your default browser with URL localhost.
5. Whenever you save changes to project files after building the project, you will have to rebuild it by
