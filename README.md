# FolderSync

**FolderSync** is a simple C# command-line utility that keeps a *replica* folder synchronized with a *source* folder.  
It performs **one-way synchronization**, making the replica an exact copy of the source at scheduled intervals. All file operations (copy, update, delete) are logged to both the console and a log file.

---

## Features

- One-way, recursive folder synchronization (source → replica)
- Periodic sync at a user-defined interval (in seconds)
- Logs every file operation (create, update, delete) to both the console and a log file
- Simple command-line usage
- Fast and reliable (uses MD5 checks for file changes)
- Written in C# (.NET 6 or newer)

---

## Requirements

- **.NET 6 SDK or newer** ([Download here](https://dotnet.microsoft.com/download))
- Windows 10/11 recommended (but works on Linux/macOS if you use forward slashes in paths)
- No third-party synchronization libraries used

---

## Build Instructions

1. **Clone the repository:**
    ```sh
    git clone https://github.com/DHrymajllo/FolderSync.git
    cd FolderSync
    ```

2. **Build the executable:**
    ```sh
    dotnet publish -c Release -r win-x64 --self-contained false
    ```
    - The compiled program will be at  
      `bin\Release\net6.0\win-x64\publish\FolderSync.exe`

---

## Usage

### **Command-line Example**

```sh
FolderSync.exe "C:\source" "C:\replica" 10 "C:\logs\sync.log"
