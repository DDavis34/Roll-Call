# Roll Call : Team 6

[Live link to website](https://rollcallpf2e-gvcedwadapcgb4aw.centralus-01.azurewebsites.net)

## Members

Dillon Davis (DDavis34) - SCRUM Master\
Timothy Posley (Tposley) - UI/UX Designer\
Michael Russelburg (MichaelRDot) - Software Architect\
Corbin Brescher (JosiahMatriarch) - Senior Developer\
John Holcomb (johnholcomb10) - Product Tester

## About Our Website

This is the semester-long project of Team 6 for CSC 4330, __Roll Call__. It is a _Pathfinder Adventures 2E_ character creator capable of connecting players to their DM. It is focused on providing the necessary information and guides needed for new players to understand _Pathfinder_'s character creator system, while also having an interesting and simple UI that can keep veteran players engaged. Characters can be shared with your DM, allowing for quicker,more thought-out, and personalized _Pathfinder_ runs.

### Known Platform Compatibility

- Tested on Chrome, Edge, Firefox
- Designed with responsiveness in mind for 280px - 1920px wide
- _All instructions for running the project are meant for Windows 11_

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
  - C# Dev Kit :: ms-dotnettools.csdevkit
  - C# :: ms-dotnettools.csharp (_should be automatically installed alongside C# Dev Kit_)
  - .NET Install Tool :: ms-dotnettools.VS Code-dotnet-runtime (_should be automatically installed alongside C# or C# Dev Kit_)

- [.NET SDK (version 10.0.203)](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-10.0.203-windows-x64-installer)

- [npm (version 11.9.0)](https://nodejs.org/en/download) (_will automatically download alongside Node.js_)

#### Not required but recommended

- Other helpful VS Code extensions:
  - IntelliCode for C# Dev Kit :: ms-dotnettools.VS Codeintellicode-csharp
  - Tailwind CSS Intellisense :: bradlc.vscode-tailwindcss
  - HTML CSS Support :: ecmel.vscode-html-css

### Downloading Dependencies

1. Download [VS Code](https://code.visualstudio.com/download). After installing VS Code, extensions can be found by searching their names within the search bar in the "Extensions" tab (`Ctrl+Shift+X`) on VS Code (extension IDs are listed above).
2. You will have to download and install the [.NET SDK](https://dotnet.microsoft.com/en-us/download/dotnet/thank-you/sdk-10.0.203-windows-x64-installer) if you have not already done so. You may have to restart your device once the SDK is installed. Once installed, open Command Prompt (type cmd into your taskbar's searchbar), then enter the command `dotnet --version` to verify your installation was successful.
3. Additionally, download and install the [GitHub Desktop app](https://desktop.github.com/download/) to make commits easier and to create clone/pull requests straight from the app. You should  log in using your GitHub.com account to tie commits and requests to you easier. [Git Bash](https://git-scm.com/downloads) should be automatically downloaded with GitHub Desktop. However, you can also download separately, if needed.
4. Download and install [npm](https://nodejs.org/en/download) (automatically installed alongside Node.js). Scroll down on the page to where it says "get a prebuilt Node.js," and select "Windows Installer." Once installed, run PowerShell as an administrator (type Powershell into your taskbar's searchbar, then right-click on Powershell and select Run as Administrator). In PowerShell, use the command `Set-ExecutionPolicy RemoteSigned` to enable scripts (enter `Y` or `A` when prompted), then `npm -v` to verify your npm installation.

### Getting Started with the Project

1. Restart your VS Code and any terminals you currently have open. This will ensure your VS Code has access to all the programs you have downloaded so far.
2. When in the Roll-Call repository on GitHub.com, make sure the master branch is selected and clone the repository onto your device to access all files. You can do that by first copying [this link](https://github.com/MichaelRdot/Roll-Call.git).
3. After copying the link, go to the GitHub Desktop app and click "File" on the top left, then select "Clone Repository" and paste the repo link under the URL tab.

### Setting up Project to Make Changes and Run Afterwards

1. In VS Code, hit "File" in the top left, then select "Open Folder" in the dropdown.
2. Navigate to the folder where you cloned the repository to. Select the folder named Roll-Call, and hit "Select Folder." If it prompts you whether to trust the owner of the folder, select Yes.
3. Once you have made any changes or just want to run the project, open a terminal by selecting Terminal > New Terminal from the top menu. First, use `dotnet dev-certs https --trust` to trust the local development certificate. If you are prompted to trust the certificate, select Yes. Next, enter the command `cd PF2EGM` to navigate to the project directory. Then, enter the command `dotnet run` to build the project.
4. Once the project builds, it should automatically open a webpage in your default browser with a localhost URL. If it does not, check your terminal or Roll-Call/PF2EGM/Properties/launchSetting.json for a local URL (ex. http://localhost:5144).
5. Whenever you save changes to project files after building the project, you will have to rebuild it by pressing `CTRL+C` in your terminal, then re-entering the command `dotnet run`.
