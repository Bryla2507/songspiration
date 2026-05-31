# Specyfikacja Funkcjonalności: Ranking Użytkowników i Relacje (Likes, Editor's Choice)

## 1. Opis Funkcjonalności

Moduł ten zapewnia identyfikację, kategoryzację i promowanie twórców wewnątrz społeczności SongSpiration. Pozwala na tworzenie relacji polubień między użytkownikami a pinami (zakładka "Polubione" oraz interakcje z elementami w Feedzie), wyliczanie dynamicznych rankingów twórców, a także przyznawanie wyróżnień z poziomu Panelu Administratora ("Editor's Choice").

## 2. Realizowane Przypadki Użycia (Use Cases)

1. **Przeglądanie Rankingu Top Użytkowników:**
   * Użytkownik wyświetla widok liderów, gdzie zintegrowane są rankingi na podstawie ilości pobrań (`DownloadsCount`) i polubień (`Likes`).
2. **Sekcja Editor's Choice:**
   * Użytkownik może przeglądać zweryfikowanych, najlepszych twórców oznaczonych przez administrację wyróżnieniem `IsEditorChoice`.
3. **Nadawanie Polubień (Likes):**
   * Użytkownik klikając serce na pinie, generuje relację `Like` między swoim kontem a konkretnym pinem.
4. **Filtrowanie i Sortowanie w Feedzie po Polubieniach:**
   * Możliwość posortowania publicznego feedu pinów za pomocą opcji "Najwięcej polubień".

## 3. Komponenty Systemu

### 3.1. Modele i Baza Danych (Data Access Layer)

Zgodnie z wymaganiami Entity Framework Core relacja `Like` została zmapowana jako encja z jawnymi kluczami obcymi do `User` oraz `Pin`. W tabeli `Pin` utrzymywana jest własność `DownloadsCount` a w tabeli `User` flaga `IsEditorChoice`.

W `UserRepository` zaimplementowano precyzyjne metody dla agregacji:
* `GetEditorChoiceUsersAsync(int limit)`
* `GetUsersByMostLikesAsync(int limit)` (korzystająca z `.SelectMany(p => p.Likes).Count()`)
* `GetUsersByMostDownloadsAsync(int limit)` (korzystająca z `.Sum(p => p.DownloadsCount)`)

### 3.2. Logika Biznesowa (Business Logic Layer)

Utworzono `RankingService` zintegrowany z `IRankingService`.
Klasa ta konwertuje złożone obiekty `User` pochodzące z repozytorium na bezpieczne, zagregowane struktury przesyłane do klienta: `UserRankingDto`.
Dto to uwzględnia: `Id`, `DisplayName`, `AvatarUrl`, `IsEditorChoice`, `TotalLikes` oraz `TotalDownloads`.

Przeprowadzono pełne testy jednostkowe (`RankingServiceTests`):
* `GetEditorsChoiceAsync_ShouldReturnMappedDtos`
* `GetTopByDownloadsAsync_ShouldCalculateTotalDownloads`
* `GetTopByLikesAsync_ShouldCalculateTotalLikes`

### 3.3. API i Kontrolery

Wystawiono endpointy do zarządzania rankingami w obrębie `RankingController`:
* `GET /api/Ranking/editors-choice` (Query: `limit`)
* `GET /api/Ranking/likes` (Query: `limit`)
* `GET /api/Ranking/downloads` (Query: `limit`)

Odpowiadają one kodem HTTP 200 (Ok) i zwracają `IEnumerable<UserRankingDto>`.

### 3.4. Frontend (Interfejs Użytkownika)

* **Komponenty i Store:** Stan aplikacji (Pinia/Vuex lub Composition API ref) komunikuje się z nowymi endpointami API. 
* Zapytania HTTP wysyłane są przez usługę dostępu z użyciem Axios/Fetch.
* **Integracja UI:** Wynikowe dane renderowane są jako kafelki / wiersze z profilami liderów, prezentując informację w estetycznych podsumowaniach. Polubienia wpływają na natychmiastowe zmiany w widoku dzięki `localStorage` dla stanu polubień klienta (reaktywność).
