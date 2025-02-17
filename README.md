# NcsWpfApp  
A **.NET Framework WPF-based application** built using the **MVVM design pattern**, supporting **RFID login** and a **touchscreen interface**. This project is designed for **desktop environments**, featuring a clean UI and a structured architecture.

---

## 🔥 Features
✅ **MVVM Architecture** – Clean separation of concerns between UI, business logic, and data.  
✅ **RFID Login** – Secure authentication using RFID cards.  
✅ **Touchscreen Support** – Optimized for interactive user experience.  
✅ **Dependency Injection** – Loosely coupled components for better maintainability.  
✅ **Asynchronous Operations** – Uses `async/await` for smooth performance.  
✅ **Command Binding** – Implements `RelayCommand` for handling UI actions.  

---

## 📂 Project Structure
```
📦 NcsWpfApp
 ├── 📂 Ncs.WpfApp            # Main WPF project
 │   ├── 📂 Views             # UI (XAML)
 │   ├── 📂 ViewModels        # MVVM ViewModels
 │   ├── 📂 Models            # Data models
 │   ├── 📂 Services          # Business logic & API calls
 │   ├── 📂 Helpers           # Utility classes
 │   ├── 📂 Converters        # XAML Value Converters
 │   ├── 📂 Themes            # UI Styles & Themes

```

---

## 🛠️ Technologies Used
- **.NET Framework** *(Specify version: e.g., .NET Framework 4.8)*
- **WPF (Windows Presentation Foundation)**
- **MVVM Pattern**
- **C#**
- **XAML**
- **RFID Integration**
- **FluentValidation (if used for validation)**

---

## 🚀 Getting Started

### **1️⃣ Clone the Repository**
```sh
git clone https://github.com/hilmank/NcsWpfApp.git
cd NcsWpfApp
```

### **2️⃣ Open in Visual Studio**
- Open `NcsWpfApp.sln` in **Visual Studio 2022**.
- Restore NuGet Packages (if needed).
- Ensure the **RFID reader** is properly connected.

### **3️⃣ Run the Application**
- Press **F5** or click **Start Debugging** to launch the app.

---

## 🔧 Configuration

### **RFID Reader Setup**
- Ensure the RFID reader is connected to the correct **COM Port**.
- Modify settings in `app.config`.

### **App Settings**
- Change configurations in `App.xaml.cs` (if required).
- Update API URLs in `Services/` if using external APIs.

---

## 📌 Screenshots (Optional)
_Add UI screenshots here if available._

---

## 📜 License
_This project is licensed under the [MIT License](LICENSE)._  

