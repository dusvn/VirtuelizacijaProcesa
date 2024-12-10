# 📊 **Virtualization Process**

## 📘 **Project Overview**
This project processes **CSV files** containing **forecast and measured data**. It validates files, performs essential **calculations**, and delivers results to the client in an organized manner. The system supports **distributed computing** concepts with **microservices**, offering **cloud integration** for remote access and enabling future **real-time processing** capabilities.

---

## ✨ **Key Features**

### 📂 **File Validation**
- **Date Check**: Ensures that the date in the file name matches the timestamps within the file.  
- **Hour Count Verification**: Checks for a valid number of hourly entries (excluding the header), considering **daylight saving time** adjustments.  

---

### 🖥️ **User Interface**
- **File Import**:  
  Users can select a directory containing subdirectories named **forecast** and **measured**, and import all CSV files starting with these names.  
---

### 🔗 **Distributed Computing**
- **Server-Driven Calculations**:  
  A server performs calculations on imported files, handling large-scale computation needs efficiently.  
- **Microservice Architecture**:  
  Microservices are used to **reduce memory load** while manipulating and processing files.  

---

### 📈 **Data Processing Flow**
1. **Data Extraction**:  
   Data is extracted from CSV files and used to create **Load objects**, which are updated for each timestamp.  
2. **Audit Creation**:  
   After processing each file, an **Audit object** is created and stored in the database, ensuring a comprehensive log of processed files.  

---

### 🧮 **Calculations**
- **Load Check**:  
  The system ensures that both **ForecastValue** and **MeasuredValue** are present before starting calculations.  
- **Types of Calculations**:  
  - **Absolute Percentage Deviation**  
  - **Squared Deviation**  

---

### 📤 **Result Delivery**
- **Event-Driven Updates**:  
  When calculations are complete, an **event** triggers to update the database and notify the client.  
- **Result Storage**:  
  Results are stored in a **new directory named "results"**, with file names reflecting the calculation date.  

---

### 🧩 **Interface Description**
- **IFileHandling Interface**:  
  Contains a method called **SendFiles**, which processes a list of uploaded files and returns a list of files with calculated values.  
  The calculated files are stored in the same directory as the original files.  


