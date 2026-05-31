# Specyfikacja Projektu: SongSpiration (Wersja Rozszerzona)

## 1. Cel Projektu
**SongSpiration** to aplikacja webowa służąca do kolekcjonowania i przeglądania inspiracji muzycznych w formie wizualnych "pinów". Platforma dedykowana jest muzykom, umożliwiając im kategoryzację pomysłów, podgląd nut oraz współdzielenie plików tabulatur.

---

## 2. Stos Technologiczny
* **Frontend:** Vue 3
* **Backend:** C# / ASP.NET Core Web API (.NET 10/8)
* **Baza danych:** SQL Server + Entity Framework Core

---

## 3. Architektura Systemu i Funkcjonalności

### 3.1. System Użytkowników i Autoryzacja
* **Rejestracja:** Zakładanie nowego konta z wykorzystaniem adresu e-mail, nazwy użytkownika oraz hasła.
* **Logowanie:** Uwierzytelnianie przy pomocy e-maila i hasła. Sukces logowania nagradzany jest wygenerowaniem tokena JWT, który jest używany do autoryzacji w systemie.
* **Autoryzacja (JWT):** Zabezpieczenie API chronionych ścieżek nagłówkiem `Authorization: Bearer <token>`. Wykorzystanie `sessionStorage` do po stronie frontendu.
* **Role systemowe:** Podział użytkowników na zwykłych klientów oraz Administratorów, uwzględniając panel administracyjny (CRUD).

### 3.2. Zarządzanie Treścią (Pins & Tabs)
* **Upload:** Formularz przesyłania plików `.gp` wraz z metadanymi - tytuł, gatunki muzyczne.
* **Automatyzacja:** Instrument zostanie wybrany samemu na podstawie przesłanego pliku `.gp`.
* **Podgląd wizualny:** Przechowywanie grafik reprezentujących zapis nutowy dla każdego pinu.

### 3.3. System Filtrowania i Wyszukiwania
* **Filtrowanie po Instrumencie:** Szybki wybór kategorii: Gitara, Bas, Perkusja.
* **Filtrowanie po Gatunku:** Możliwość kategoryzowania pinów według gatunków muzycznych.
* **Sortowanie:** Sortowanie po popularności, polubieniach lub według daty publikacji.

### 3.4. System Rankingu i Relacji Użytkowników
* **Ranking po polubieniach:** Zestawienie użytkowników, których piny zgromadziły sumarycznie najwięcej polubień.
* **Ranking po pobraniach:** Zestawienie twórców, których piny są najczęściej pobierane (zliczana wartość `DownloadsCount`).
* **Wybór Redakcji (Editor's Choice):** Prestiżowe wyróżnienie przyznawane przez administratorów systemu określonym użytkownikom (flaga `IsEditorChoice`).
* **Relacje i interakcje:** System pozwala na tworzenie relacji polubień (`Like`) między użytkownikiem a pinem, wpływając bezpośrednio na algorytmy rankingowe.

---

## 4. Zadania Warstwy Logiki Biznesowej
* **Service Layer:** Implementacja serwisów `UserService`, `PinService`, `CollectionService` oraz `RankingService`.
* **Unit Testing:** Testy rejestracji, mechanizmu filtrowania, zapisu plików, a także metod kalkulacji agregacji dla rankingu (np. `RankingServiceTests`).

---

## 5. Model Danych (Wysokopoziomowy)
| Tabela | Opis |
|---|---|
| `User` | Dane konta, profil (display_name, avatar), role, flaga `IsEditorChoice`. |
| `Pin` | Metadane pina, właściciel, lokalizacja pliku `.gp`, licznik pobrań `DownloadsCount`. |
| `Genre` | Lista gatunków muzycznych. |
| `Like` | Relacja polubień użytkownik ↔ pin z datą utworzenia. |

---

## 6. Interfejs Użytkownika (UI)

### 6.1. Widoki publiczne i nawigacja
- **Navbar:** Logo, wyszukiwarka, skróty do `Upload` i `Profilu`.
- **Strona główna:** Responsywny grid pinów z infinite scroll.

### 6.2. Karta pina i szczegóły
- **Karta:** Miniatura, tytuł, autor, akcje (`play`, `like`, `save`).
- **Szczegóły:** Pełny podgląd AlphaTab, odtwarzacz MIDI, edycja (dla właściciela).

### 6.3. Filtrowanie i wyszukiwanie
- Pasek filtrów: instrumenty, gatunki (chips), wyszukiwarka tekstowa.

### 6.4. Widok Profilu i Zarządzanie Kontem

Strona profilu (`/profile/{id}` lub `/profile/me`) służy do prezentacji tożsamości użytkownika, jego statystyk oraz zgromadzonych treści muzycznych.

#### 6.4.1. Struktura Wizualna i Nawigacja
* **Dostęp:** Wejście na profil następuje poprzez interakcję z elementami nawigacyjnymi (avatar/nazwa) w górnym pasku aplikacji.
* **Sekcja nagłówka (Header):** * Prezentacja danych publicznych: zdjęcie profilowe (Avatar), nazwa wyświetlana (`DisplayName`) oraz opis biograficzny (`Bio`).
    * Statystyki aktywności: łączna liczba dodanych pinów oraz suma otrzymanych polubień.
    * Przycisk akcji **"Edytuj profil"**, dostępny dla właściciela konta.
* **Przełącznik widoku (Tabs):** Interfejs umożliwiający zmianę wyświetlanej zawartości w obrębie tej samej strony:
    * **Dodane:** Grid z pinami utworzonymi przez użytkownika.
    * **Polubione:** Grid z pinami, które użytkownik oznaczył jako ulubione.

#### 6.4.2. Funkcjonalność i Integracja z BLL
* **Prezentacja Treści:** Widok wykorzystuje `GetPinsByUserIdAsync` do pobierania własnych publikacji oraz odpowiednie filtry dla treści polubionych.
* **Zarządzanie Profilem:** * Formularz edycji umożliwia aktualizację pól `DisplayName`, `AvatarUrl` oraz `Bio`.
    * Proces aktualizacji danych jest obsługiwany przez metodę `UpdateProfileAsync`.
* **Bezpieczeństwo i Usuwanie:** * Wyświetlanie przycisków edycji i usuwania jest uzależnione od uprawnień zalogowanego użytkownika (widoczne tylko dla właściciela).
    * Możliwość trwałego usunięcia konta poprzez `Usuń`
### 6.3. Szczegółowy opis strony głównej (Feed)

Główny widok aplikacji (`HomeView.vue`) odpowiada za prezentację publicznych pinów oraz zaawansowane filtrowanie treści muzycznych.

#### 6.3.1. Mechanizm Filtrowania i Sortowania
Filtrowanie odbywa się reaktywnie po stronie klienta na zestawie danych pobranych z API:
*   **Wyszukiwanie tekstowe:** Filtrowanie po tytule utworu (niezależne od wielkości liter).
*   **Filtr Instrumentu:** Wybór kategorii na podstawie mapowania: Gitara (0), Bas (1), Perkusja (2).
*   **Filtr Gatunku:** Dynamicznie generowana lista unikalnych gatunków na podstawie tagów przypisanych do pobranych aktualnie pinów.
*   **Kryteria Sortowania:**
    *   `Najnowsze`: Sortowanie według daty utworzenia (`createdAt`).
    *   `Alfabetycznie`: Sortowanie według tytułu utworu.
    *   `Najwięcej polubień`: Sortowanie według liczby otrzymanych polubień (`likeCount`).
*   **Kierunek Sortowania:** Możliwość zmiany pomiędzy porządkiem rosnącym (`asc`) a malejącym (`desc`).
*   **Resetowanie:** Funkcja przywracania domyślnych ustawień wszystkich filtrów jednym kliknięciem.

#### 6.3.2. Integracja z API i Obsługa Stanu
*   **Autoryzacja:** Zapytania do endpointu `/api/Pins` wymagają poprawnego tokena JWT z `sessionStorage`. Wykrycie statusu 401 skutkuje natychmiastowym wylogowaniem i przekierowaniem do `LoginView`.
*   **Przetwarzanie danych:** Dane z API są mapowane w locie (m.in. budowanie ścieżek do plików `.gp` oraz transformacja uproszczonych list gatunków na obiekty UI).
*   **System Polubień:** Widok integruje się z `localStorage` w celu śledzenia stanu polubień (wykorzystując `visitorId` oraz listę `likedPins`), co pozwala na natychmiastową informację wizualną o interakcji użytkownika.
*   **Tryb demonstracyjny:** W przypadku braku połączenia z serwerem lub niepoprawnej konfiguracji środowiska, aplikacja automatycznie ładuje dane typu *Mock* dla zachowania ciągłości prezentacji UI.

### 6.4. Autoryzacja i konto

- **Logowanie (`LoginView`):** Formularz logowania e-mailem i hasłem. Po pomyślnym zalogowaniu, aplikacja zapisuje token `JWT` i nazwę użytkownika w `sessionStorage`, by w prawym górnym rogu strony wyświetlać inicjały użytkownika.
- **Rejestracja (`RegisterView`):** Samodzielny widok zbierający wymagane dane użytkownika, posiadający walidację długości i struktury hasła, z możliwością przejścia do logowania po sukcesie.

- **Profil użytkownika:**
  - nagłówek profilu: awatar, `display_name`, krótki opis/bio (opcjonalne), statystyki (liczba dodanych pinów, suma polubień otrzymanych, liczba kolekcji)
  - przycisk `edit profile` tylko dla właściciela profilu

- **Zakładki (tabs) na profilu:**
  - `Dodane piny` — grid jak w feedzie, z akcjami właściciela: `edit`, `delete` (opcjonalnie: `promote`)
  - `Polubione` — grid pinów polubionych przez użytkownika; możliwość szybkiego `unlike` oraz `save` do kolekcji

### 6.5. Upload i edycja pina
- Formularz przesyłania i edycji metadanych pina (tylko właściciel).

### 6.6. Kolekcje i social
- Zarządzanie kolekcjami, udostępnianie linków, system polubień.

- **Kolekcje:** użytkownik może tworzyć i zarządzać kolekcjami oraz dodawać do nich piny (`save`). UI powinno umożliwiać wybór kolekcji z listy (dropdown/modal) oraz podstawowe operacje (utwórz/zmień nazwę/usuń kolekcję) zgodnie z `CollectionService`.

- **Udostępnianie (`share`):** kopiowanie linku do pina, opcjonalnie systemowe udostępnianie (Web Share API) na mobile.

- **Polubienia:** ikona „serduszko” przełącza stan polubienia; licznik polubień aktualizuje się po akcji.

---

## 7. Architektura i Decyzje Projektowe (ADR)
Wszelkie kluczowe decyzje technologiczne i architektoniczne podejmowane w projekcie są dokumentowane w formacie **ADR (Architecture Decision Records)**. Znajdują się one w katalogu `specs/adr/`.

Obecnie zarejestrowane decyzje:
* **[ADR-0001](../adr/0001-record-architecture-decisions.md):** Zapisywanie decyzji architektonicznych (ADR)
* **[ADR-0002](../adr/0002-use-vue3-for-frontend.md):** Wybór Vue 3 na framework frontendowy
* **[ADR-0003](../adr/0003-use-dotnet-8-web-api-for-backend.md):** Wybór C# i ASP.NET Core (.NET 8) do budowy API
* **[ADR-0004](../adr/0004-use-ef-core-and-sql-server.md):** Użycie MS SQL Server oraz Entity Framework Core
* **[ADR-0005](../adr/0005-use-jwt-for-stateless-auth.md):** Użycie JWT (JSON Web Tokens) do bezstanowej autoryzacji
* **[ADR-0006](../adr/0006-use-alphatab-for-score-rendering.md):** Użycie biblioteki AlphaTab do renderowania tabulatur
* **[ADR-0007](../adr/0007-user-ranking-and-relations.md):** Implementacja Systemu Rankingu i Relacji Użytkowników
