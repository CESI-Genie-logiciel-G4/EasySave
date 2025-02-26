<section align="center">
  <h1>
    💾 EasySave
  </h1>

  <a href="./LICENSE">  
    <img src="https://img.shields.io/badge/License-MIT-yellow.svg" alt="License: MIT">
  </a>

  <p>
    The <strong>EasySave</strong> project by ProSoft aims to develop a versatile and user-friendly backup software.
  </p>

  <div>
    <a href="https://learn.microsoft.com/en-us/dotnet/csharp/" target="_blank">
      <img src="https://img.shields.io/badge/Language-C%23-purple.svg" alt="C# Documentation">
    </a>
    <a href="https://learn.microsoft.com/en-us/dotnet/whats-new/dotnet-9" target="_blank">
      <img src="https://img.shields.io/badge/Framework-.NET%209-darkblue.svg" alt=".NET 9 Documentation">
    </a>
    <a href="https://www.sonarsource.com/" target="_blank">
      <img src="https://img.shields.io/badge/Code%20Quality-SonarQube-4E9BCD.svg" alt="SonarQube">
    </a>
  </div>
</section>

</br>

## 📚 Overview

</br>

<div align="center">
  <img src="./assets/easysave-v2.png" alt="EasySave 2.0" width="70%">
</div>

</br>

<p align="justify">
  <strong>EasySave 2.0</strong> enables users to create and execute an unlimited number of backup tasks,
   supporting simultaneous operations and secure encryption. The application features a graphical interface, allowing users to play, pause, and stop tasks. 
   
   User global settings are available to configure the application to their needs.
</p>

</br>

<div align="center">
  <table>
    <tr>
      <td>💾 Backup tasks</td>
      <td>🔒 Secure encryption</td>
      <td>💻 Graphical interface</td>
    </tr>
    <tr>
      <td>🔄 Multithreaded operations</td>
      <td>⏯️ Play/Pause/Stop</td>
      <td>🔧 Configurable settings</td>
    </tr>
    <tr>
      <td>🔄 Follows best practices</td>
      <td>🌐 English and French support</td>
      <td>📈 Real-time logging</td>
    </tr>
    <tr>
      <td>🖥️ Multiplatform</td>
      <td>✅ Tested</td>
      <td>📚 Documented</td>
    </tr>
  </table>
</div>

</br>

## 📖 Documentation

Comprehensive user documentation is available to help you get started and understand the application's features.

**User Guide**: Learn how to use EasySave and navigate the application's menu.

**[Read the Full Documentation](./USER-GUIDE.md)**

</br>

## 📦 Installation

Download and execute the latest version of **EasySave** from the [EasySave v2.0 Release](https://github.com/CESI-Genie-logiciel-G4/EasySave/releases/tag/v2.0).

If you are building from source, ensure **.NET 9** is installed before running the application.  

The **EasySave** solution consists of **multiple projects**:

``` python
EasySave/                # Main solution
│
├── src/                 # Directory for source code Projects
│   ├── EasySave/        # Core application
│   └── Logger/          # Logging module
│
└── test/                # Directory for unit tests Projects
    ├── EasySave.Tests/  # Unit tests for EasySave
    └── Logger.Tests/    # Unit tests for Logger
```

</br>


## 🚀 Getting Started

Useful commands to build and test the application are available in the root directory.

```bash
dotnet
├── build                # Builds the application
└── test                 # Executes unit tests
```

To execute the application, navigate to the **EasySave** core project and run the following command:

```bash
dotnet run 
├── Gui                  # Launches the graphical UI
└── Console              # Launches the console UI (default option)
```

</br>

<div align="center">
  <table style="width: 100%;">
    <thead>
      <tr>
        <th>Category</th>
        <th>Library</th>
        <th>Description</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td>.NET Development</td>
        <td>
          <ul style="list-style-type: none; padding: 0;">
            <li><img src="https://img.shields.io/badge/Framework-.NET%209-black.svg" alt=".NET 9"></li>
            <li><img src="https://img.shields.io/badge/IDE-Rider-black.svg" alt="Rider"></li>
            <li><img src="https://img.shields.io/badge/IDE-Visual%20Studio-black.svg" alt="Visual Studio"></li>
          </ul>
        </td>
        <td>Popular IDEs for .NET development.</td>
      </tr>
      <tr>
        <td>Software Testing</td>
        <td>
          <ul style="list-style-type: none; padding: 0;">
            <li><img src="https://img.shields.io/badge/Testing-xUnit-black.svg" alt="xUnit"></li>
            <li><img src="https://img.shields.io/badge/Coverage-dotnet-black.svg" alt="dotnet coverage"></li>
            <li><img src="https://img.shields.io/badge/Quality-SonarQube-black.svg" alt="SonarQube"></li>
          </ul>
        </td>
        <td>Unit testing, coverage, and code quality tools.</td>
      </tr>
      <tr>
        <td>UI Framework</td>
        <td>
          <ul style="list-style-type: none; padding: 0;">
            <li><img src="https://img.shields.io/badge/UI-Avalonia-black.svg" alt="Avalonia"></li>
            <li><img src="https://img.shields.io/badge/Icons-Material%20Icons-black.svg" alt="Material Icons"></li>
          </ul>
        </td>
        <td>Framework for building cross-platform UIs and icon library.</td>
      </tr>
      <tr>
        <td>Documentation</td>
        <td>
          <ul style="list-style-type: none; padding: 0;">
            <li><img src="https://img.shields.io/badge/Docs-Doxygen-black.svg" alt="Doxygen"></li>
          </ul>
        </td>
        <td>Generates documentation from source code.</td>
      </tr>
      <tr>
        <td>Multiplatform</td>
        <td>
          <ul style="list-style-type: none; padding: 0;">
            <li><img src="https://img.shields.io/badge/OS-MacOS-black.svg" alt="MacOS"></li>
            <li><img src="https://img.shields.io/badge/OS-Linux-black.svg" alt="Linux"></li>
            <li><img src="https://img.shields.io/badge/OS-Windows-black.svg" alt="Windows"></li>
          </ul>
        </td>
        <td>Supports multiple operating systems.</td>
      </tr>
      <tr>
        <td>Management</td>
        <td>
          <ul style="list-style-type: none; padding: 0;">
            <li><img src="https://img.shields.io/badge/Repo-GitHub-black.svg" alt="GitHub"></li>
            <li><img src="https://img.shields.io/badge/PM-Jira-black.svg" alt="Jira"></li>
          </ul>
        </td>
        <td>Version control and project management.</td>
      </tr>
    </tbody>
  </table>
</div>

</br>

## ✍️ Authors 

- [Florent **MAILLARD-SALIN**](mailto:florent.maillardsalin@viacesi.fr)
- [Brieuc **OLIVIERI**](mailto:brieuc.olivieri@viacesi.fr)
- [Lilian **BAUDRY**](mailto:lilian.baudry@viacesi.fr)

</br>

<p align="center">
  2025 © <strong>ProSoft</strong>™. All rights reserved.
</p>