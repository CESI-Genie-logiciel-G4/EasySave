<div align="center">

# 📘 **User Guide - EasySave**

![Date](https://img.shields.io/badge/Date-2025_02_28-blue)
![Version](https://img.shields.io/badge/Version-3.0-green)

**Welcome to our user Wiki!** This document explains how to use our app.

</div>

## **Introduction**


<br/>

**EasySave** is a graphical, multi-platform backup tool. It enables users to **create**, **manage**, and **execute** backup jobs with ease.
Whether performing **full** or **differential** backups, EasySave ensures your data is securely backed up.

You can **download** and **execute** the EasySave app from the following link: [EasySave v3.0 Release](https://github.com/CESI-Genie-logiciel-G4/EasySave/releases/tag/v3.0).

## **Main Features**

### **Navigating the Menu**

EasySave's menu is divided into three main sections:

- **Backup Jobs**: Manage and execute backup jobs.
- **Create a Backup Job**: Define a backup job by specifying a source and destination.
- **History**: View the history of executed backup jobs.
- **Settings**: Configure the application's language, logs output format and more.

<br/>


### **Creating a Backup Job**
A backup job is a named link between two **existing** files. Once created, it allows copying content from the source to the destination on demand.

Three backup strategies are available:
- **Full**: Recopies all files at each execution.
- **Synthetic Full**: Only new files are copied, creating a full backup by merging changes with the last 
full backup.
- **Differential**: Copies only new or modified files since the last full backup, saving them in a separate location.

<div align="center">

💡 *Relative paths and network drives are also supported.*

</div>

<br/>

### **Task Status Indicators**

EasySave provides visual feedback on backup tasks through color-coded progress bars, making it easy to monitor their status at a glance.

<div align="center">
  <table>
    <thead>
      <tr>
        <th>Status</th>
        <th>Color</th>
        <th>Description</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td><strong>Pending</strong></td>
        <td>⬛ Gray (Indeterminate)</td>
        <td>Task is queued and waiting to start</td>
      </tr>
      <tr>
        <td><strong>Running</strong></td>
        <td>🟦 DodgerBlue</td>
        <td>Task is actively processing files</td>
      </tr>
      <tr>
        <td><strong>Paused</strong></td>
        <td>🟧 Orange</td>
        <td>Task has been temporarily paused by the user</td>
      </tr>
      <tr>
        <td><strong>Completed</strong></td>
        <td>🟩 Green</td>
        <td>Task has successfully completed all operations</td>
      </tr>
      <tr>
        <td><strong>Failed</strong></td>
        <td>🟥 Red</td>
        <td>Task encountered an error and could not complete</td>
      </tr>
      <tr>
        <td><strong>Blocked</strong></td>
        <td>🟧 Orange (Indeterminate)</td>
        <td>Task is waiting for another process or resource</td>
      </tr>
      <tr>
        <td><strong>Canceled</strong></td>
        <td>⬛ Gray</td>
        <td>Task was manually canceled by the user</td>
      </tr>
      <tr>
        <td><strong>Wainting</strong></td>
        <td>🟧 Orange</td>
        <td>Task is waiting for another task to complete</td>
      </tr>
    </tbody>
  </table>
</div>

The task status is reflected in real-time through the UI, allowing users to quickly identify which tasks require attention.

### **File Encryption**

EasySave encrypts files with specified extensions using AES when configured in backup tasks.

<div align="center">
  <table>
    <thead>
      <tr>
        <th>Feature</th>
        <th>Description</th>
      </tr>
    </thead>
    <tbody>
      <tr>
        <td><strong>Encryption Algorithm</strong></td>
        <td>AES CBC (Advanced Encryption Standard)</td>
      </tr>
      <tr>
        <td><strong>Key Storage</strong></td>
        <td><code>.easysave/security</code> directory</td>
      </tr>
      <tr>
        <td><strong>Configuration</strong></td>
        <td>File extensions defined in settings</td>
      </tr>
      <tr>
        <td><strong>Activation</strong></td>
        <td>Per-backup task setting</td>
      </tr>
    </tbody>
  </table>
</div>

### **Settings**

EasySave offers a range of persistent settings to customize the application to your needs:
- **Language**: Choose between English and French.
- **Logs Output Format**: Select between XML, JSON, or Console.
- **Security**: Enable or disable file encryption.
- **File Extensions**: Define which file types to encrypt or prioritize.
- **Concurrent Heavy File Limit**: Set the maximum size of files to process concurrently.
- **Business App Detection**: Enable or disable the automatic detection of business applications.


### **Real-Time Monitoring**

- Displays backup progress status.
- Immediate indication of encountered errors.
- Automatically generates logs in `.easysave/logs` within the executable directory.
- Logs can be configured to output in `XML`, `JSON`, or `Console` format and are displayed in real-time in the interface.
- All logs are journaled for persistent record-keeping.

<br/>

### **Available Languages**
The application is **bilingual** (French and English) and can be extended to other languages as needed.

<br/>

## **Need Help?**
📌 Visit our **GitHub** or contact us on **LinkedIn** for further inquiries.

<br/>
<br/>
<br/>

<div align="center">

🎯 With **EasySave**, simplify your backups and keep full control over your files effortlessly!

</div>