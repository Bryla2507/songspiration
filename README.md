# SongSpiration 

**SongSpiration** to aplikacja webowa służąca do kolekcjonowania i przeglądania inspiracji muzycznych w formie wizualnych "pinów". Platforma dedykowana jest muzykom, umożliwiając im kategoryzację pomysłów, podgląd nut oraz współdzielenie plików tabulatur.

---

## 📌 Aktualny Stan Projektu

### **Struktura Projektu**
```
songspiration/
├── backend/
│   ├── SongSpiration.API/          # Warstwa API (ASP.NET Core Web API)
│   ├── SongSpiration.BLL/         # Warstwa Logiki Biznesowej (BLL)
│   ├── SongSpiration.DAL/         # Warstwa Dostępu do Danych (DAL)
│   ├── SongSpiration.Models/      # Warstwa Modeli
│   └── SongSpiration.Tests/       # Testy jednostkowe
├── specs/                         # Specyfikacja i dokumentacja
└── songspiration.sln              # Rozwiązanie Visual Studio
```

### **Co jest już zrobione?**
1. **Modele danych** – Zdefiniowane encje (`User`, `Pin`, `Genre`, `Like`, `AuthToken`, `PinGenre`) oraz enumeracje (`Instrument`, `PinVisibility`, `TokenType`).
2. **Interfejsy serwisów** – Zdefiniowane interfejsy dla `PinService` i `UserService` w warstwie BLL.
3. **DTOs** – Modele transferu danych dla pinów (`PinDtos`) i użytkowników (`UserDtos`).
4. **Struktura projektu** – Podział na warstwy (API, BLL, DAL, Models).
5. **Specyfikacja** – Pliki `spec.md`, `class-diagram.puml`, `db-erd.puml` opisujące wymagania i modele danych.

### **Co zostało do zrobienia?**
| Warstwa          | Zadania                                                                 |
|------------------|------------------------------------------------------------------------|
| **API**          | Implementacja kontrolerów (`UserController`, `PinController`).        |
|                  | Konfiguracja autoryzacji (JWT) i walidacji danych.                    |
| **BLL**          | Pełna implementacja serwisów (`PinService`, `UserService`).            |
|                  | Logika filtrowania, sortowania, polubień.                              |
| **DAL**          | Implementacja repozytoriów (`UserRepository`, `PinRepository`).       |
|                  | Konfiguracja Entity Framework Core (kontekst bazy danych, migracje).   |
| **Baza Danych**  | Utworzenie migracji i schematu bazy danych (SQL Server).              |
| **Testy**        | Testy jednostkowe dla serwisów i repozytoriów.                         |
|                  | Testy integracyjne dla API.                                            |
| **UI**           | Implementacja frontendowa (Vue 3) – brak kodu w repozytorium.          |

---

## 🎯 Plany i Cele

### **Architektura Systemu**
Projekt opiera się na **architekturze warstwowej**, która zapewnia:
- **Separację odpowiedzialności** (warstwa prezentacji, logiki biznesowej, dostępu do danych).
- **Łatwość testowania** (możliwość mockowania warstw).
- **Skalowalność** (każda warstwa może być rozwijana niezależnie).

#### **Diagram Architektury**
![Architektura Systemu SongSpiration](https://raw.githubusercontent.com/kacpereqo/songspiration/addreadme/specs/project/architecture-diagram.png)

> **Opis warstw:**
> - **Interfejs Użytkownika (Vue 3):** Strona główna (Feed), Widok pinu (AlphaTab), Profil użytkownika, Upload/edycja pinu.
> - **Warstwa API (ASP.NET Core):** Kontrolery (`UserController`, `PinController`), Autoryzacja (JWT), Walidacja danych.
> - **Warstwa Logiki Biznesowej (C# / .NET 8):** Serwisy (`UserService`, `PinService`), Logika filtrowania, DTOs.
> - **Warstwa Dostępu do Danych (Entity Framework Core):** Repozytoria (`UserRepository`, `PinRepository`).
> - **Warstwa Modeli:** Encje (`User`, `Pin`, `Genre`), Enumeracje (`Instrument`, `PinVisibility`).
> - **Baza Danych (SQL Server):** Tabele (`User`, `Pin`, `Genre`, `Like`), Relacje (`PinGenre` jako many-to-many).

---

## 🤖 Metodologia Współpracy z Agentami AI

### **1. GitHub Copilot**
- **Zastosowanie**:
  - Wspomaganie pisania kodu (generowanie fragmentów kontrolerów, serwisów, testów).
  - Automatyczne uzupełnianie sygnatur metod, komentarzy i dokumentacji.
- **Przykłady użycia**:
  - Generowanie testów jednostkowych dla `PinService`.
  - Tworzenie szkieletów kontrolerów API na podstawie interfejsów.

### **2. Cline (Agent AI)**
- **Zastosowanie**:
  - Analiza kodu i struktury projektu.
  - Generowanie dokumentacji (np. `README.md`, diagramy PlantUML).
  - Planowanie architektury i podział zadań między członków zespołu.
- **Przykłady użycia**:
  - Utworzenie diagramu architektury systemu (patrz sekcja [Diagram Architektury](#diagram-architektury)).
  - Opis aktualnego stanu projektu i planów rozwoju.

### **3. Współpraca z Zespołem**
| Osoba       | Zakres Odpowiedzialności                                  | Współpraca z AI                          |
|-------------|----------------------------------------------------------|------------------------------------------|
| **Ola**     | Architektura, dokumentacja, wsparcie AI.                | Cline (planowanie), Copilot (kod).       |
| **Filip**   | Warstwa Logiki Biznesowej (BLL), testy jednostkowe.      | Copilot (generowanie testów).           |
| **Oskar**   | Warstwa Dostępu do Danych (DAL), specyfikacja systemu.  | Cline (analiza modeli), Copilot (kod).  |
| **Kacper**  | Dokumentacja, modele, diagramy UML.                      | Cline (generowanie diagramów).          |

---

## 🛠 Stos Technologiczny

| Warstwa               | Technologie                          |
|-----------------------|--------------------------------------|
| **Interfejs Użytkownika** | Vue 3, AlphaTab (renderowanie nut)  |
| **Backend**           | ASP.NET Core Web API (.NET 8)        |
| **Baza Danych**       | SQL Server, Entity Framework Core   |
| **Autoryzacja**       | JWT (JSON Web Tokens)                |
| **Testy**             | xUnit, Moq                           |
| **Narzędzia AI**      | GitHub Copilot, Cline                |

---

## 🚀 Jak Uruchomić Projekt?

### **Wymagania**
- `.NET 8 SDK` (dla backendowej części projektu).
- `Node.js` (dla frontendowej części, gdy będzie zaimplementowana).
- `SQL Server` (lokalna instancja lub Docker).
- `Visual Studio 2022` lub `VS Code` (z rozszerzeniami dla C# i Vue).

### **Instrukcje**
1. Sklonuj repozytorium:
   ```bash
   git clone git@github.com:kacpereqo/songspiration.git
   ```
2. Otwórz rozwiązanie w Visual Studio:
   ```bash
   cd songspiration
   start songspiration.sln
   ```
3. Przywróć paczki NuGet:
   ```bash
   dotnet restore
   ```
4. Uruchom projekt API:
   ```bash
   cd backend/SongSpiration.API
   dotnet run
   ```
   API będzie dostępne pod adresem: `https://localhost:5001` lub `http://localhost:5000`.

---

## 📂 Podział Pracy

| Osoba       | Zadania                                                                 |
|-------------|------------------------------------------------------------------------|
| **Ola**     | Architektura, dokumentacja, wsparcie AI, koordynacja zespołu.         |
| **Filip**   | Implementacja serwisów (BLL) i testów jednostkowych.                  |
| **Oskar**   | Implementacja repozytoriów (DAL) i migracji bazy danych.              |
| **Kacper**  | Dokumentacja projektowa, diagramy UML, modele danych.                 |

---

## 📝 Licencja
Projekt jest udostępniony na licencji **MIT**. Szczegóły w pliku `LICENSE`.

---
