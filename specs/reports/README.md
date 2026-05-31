# Specyfikacja funkcjonalności zgłaszania użytkowników

## Opis funkcjonalności

Funkcjonalność zgłaszania użytkowników umożliwia użytkownikom aplikacji zgłaszanie innych użytkowników, którzy naruszają zasady społeczności. Zgłoszenia są przechowywane w bazie danych i mogą być zarządzane przez administratorów.

## Komponenty

### Baza danych
- **Tabela `Reports`**:
  - `Id`: Unikalny identyfikator zgłoszenia (GUID).
  - `ReportedUserId`: Identyfikator zgłoszonego użytkownika (GUID).
  - `ReportingUserId`: Identyfikator użytkownika zgłaszającego (GUID).
  - `Content`: Treść zgłoszenia (string, max 200 znaków).

### Backend
- **`ReportController`**: Kontroler API obsługujący żądania HTTP związane ze zgłoszeniami.
  - `POST /api/Report`: Tworzy nowe zgłoszenie.
  - `GET /api/Report`: Pobiera listę wszystkich zgłoszeń.
  - `GET /api/Report/{id}`: Pobiera szczegóły zgłoszenia o podanym identyfikatorze.
  - `DELETE /api/Report/{id}`: Usuwa zgłoszenie o podanym identyfikatorze.

- **`ReportService`**: Serwis biznesowy obsługujący logikę związaną ze zgłoszeniami.
  - `CreateReportAsync`: Tworzy nowe zgłoszenie.
  - `GetAllReportsAsync`: Pobiera listę wszystkich zgłoszeń.
  - `GetReportByIdAsync`: Pobiera szczegóły zgłoszenia o podanym identyfikatorze.
  - `DeleteReportAsync`: Usuwa zgłoszenie o podanym identyfikatorze.

- **`ReportRepository`**: Repozytorium obsługujące operacje na bazie danych związane ze zgłoszeniami.
  - `AddAsync`: Dodaje nowe zgłoszenie do bazy danych.
  - `GetAllAsync`: Pobiera listę wszystkich zgłoszeń z bazy danych.
  - `GetByIdAsync`: Pobiera zgłoszenie o podanym identyfikatorze z bazy danych.
  - `DeleteAsync`: Usuwa zgłoszenie o podanym identyfikatorze z bazy danych.

### Frontend
- **Przycisk "Reportuj"**: Dodany do profilu użytkownika, umożliwia zgłoszenie danego użytkownika.
- **Modal zgłoszenia**: Okno dialogowe, w którym użytkownik może wpisać treść zgłoszenia i potwierdzić jego wysłanie.
- **Panel administracyjny**: Sekcja "Manage Reports" w panelu administracyjnym, umożliwiająca administratorom przeglądanie i zarządzanie zgłoszeniami.

## Przepływy

### Przepływ zgłaszania użytkownika
1. Użytkownik wchodzi na profil innego użytkownika.
2. Kliknięcie przycisku "Reportuj" otwiera modal zgłoszenia.
3. Użytkownik wpisuje treść zgłoszenia i kliknięcie "Submit" wysyła zgłoszenie.
4. Zgłoszenie jest zapisywane w bazie danych.

### Przepływ zarządzania zgłoszeniami przez administratora
1. Administrator wchodzi w panel administracyjny.
2. Wybiera sekcję "Manage Reports".
3. Przegląda listę zgłoszeń.
4. Może usunąć zgłoszenie lub podjąć inne działania (np. zbanować użytkownika).

## Diagramy

Diagramy sekwencji i klas są dostępne w pliku [reports.puml](reports.puml).