
<div align="center">

# 📘 **User Guide - EasySave**

![Date](https://img.shields.io/badge/Date-2025_02_06-blue)
![Version](https://img.shields.io/badge/Version-1.0-green)

**Welcome to our user Wiki!** This document explains how to use our app.

</div>

## **Introduction**


<br/>

**EasySave** is a robust, multi-platform, console-based backup tool. It enables users to **create**, **manage**, and **execute** backup jobs with ease.
Whether performing **full** or **differential** backups, EasySave ensures your data is securely backed up.

<br/>

## **Main Features**

### **Navigating the Menu**

Using an interactive menu system, each action is selectable by entering its corresponding number.

<div align="center">

💡 *To cancel an operation, enter **exit** at any time.*

</div>


### **Creating a Backup Job**
A backup job is a named link between two **existing** files. Once created, it allows copying content from the source to the destination on demand.

Two backup strategies are available:
- **Full**: Recopies all files at each execution.
- **Differential**: Copies only recently modified files.

> **Limit**: The community version allows creating up to **5** backup jobs.

<div align="center">

💡 *Relative paths and network drives are also supported.*

</div>

<br/>

### **Executing Backups**
EasySave offers flexible management of backup jobs through simplified expressions:


<div align="center">
  <table style="width:50%">
    <tr>
      <td><code>1</code></td>
      <td>Launch a single job.</td>
    </tr>
    <tr>
      <td><code>1-3</code></td>
      <td>Launch all jobs within a specified range.</td>
    </tr>
    <tr>
      <td><code>1;3;5</code></td>
      <td>Launch specific jobs.</td>
    </tr>
    <tr>
      <td><code>*</code></td>
      <td>Launch all saved jobs.</td>
    </tr>
  </table>
</div>

<br/>

### **Real-Time Monitoring**

- Displays backup progress status.
- Immediate indication of encountered errors.
- Automatically generates logs in `.easysave/logs` within the executable directory.

<br/>

### **Available Languages**
The application is **bilingual** (French and English) and can be extended to other languages as needed.

<br/>

## **Need Help?**
📌 Visit our **[GitHub](#)** or contact us on **LinkedIn** for further inquiries.

<br/>
<br/>
<br/>

<div align="center">

🎯 With **EasySave**, simplify your backups and keep full control over your files effortlessly!

</div>