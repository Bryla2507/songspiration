# ADR-0007: Implementacja Systemu Rankingu i Relacji Użytkowników

## Status
Zaakceptowany

## Kontekst
W ramach rozwoju aplikacji SongSpiration konieczne było wprowadzenie mechanizmów angażujących użytkowników oraz promujących wysokiej jakości treści. Należało zaprojektować system wyłaniający najaktywniejszych twórców i najlepsze piny. 

Zdecydowano się na implementację:
1.  **Systemu Polubień (Likes):** Możliwości interakcji użytkownika z pinem, co generuje relacje typu Wiele-do-Wielu (Many-to-Many).
2.  **Rankingów dynamicznych:** Wyliczania popularności użytkownika na podstawie sumarycznej liczby polubień wszystkich jego pinów oraz łącznej liczby pobrań plików `.gp` podpiętych pod jego piny.
3.  **Wyróżnień Redakcji (Editor's Choice):** Manualnego oznaczania przez administratorów użytkowników tworzących wyjątkowe treści.

## Decyzja
1.  **Model Danych i Relacje:** 
    *   Wprowadzono encję `Like` będącą tabelą łącznikową między `User` a `Pin`, przechowującą informację o dacie polubienia (`CreatedAt`).
    *   Dodano właściwości `DownloadsCount` w encji `Pin` w celu zliczania liczby pobrań.
    *   Rozszerzono model `User` o flagę `IsEditorChoice` typu `bool`.

2.  **Warstwa Repozytorium (Data Access):** 
    *   Zaimplementowano dedykowane zapytania w `UserRepository`, wykorzystujące LINQ i mechanizmy agregacji (np. `SelectMany(p => p.Likes).Count()`) bezpośrednio na poziomie bazy danych w celu optymalizacji wydajności dla rankingów.

3.  **Warstwa Serwisu i API:**
    *   Wydzielono domenę rankingów do `RankingService` obsługującego zapytania typu "Top N".
    *   Utworzono `RankingController` wystawiający odpowiednie endpointy (np. `/api/Ranking/likes`, `/api/Ranking/downloads`, `/api/Ranking/editors-choice`).
    *   Do komunikacji API->Klient zdefiniowano `UserRankingDto` przesyłające wyłącznie zagregowane statystyki, ograniczając w ten sposób obciążenie i ryzyko ujawnienia wrażliwych danych.

## Konsekwencje
*   **Pozytywne:** 
    *   Wyraźny podział odpowiedzialności między serwisami (SRP) - logika związana z popularnością nie obciąża `UserService`.
    *   Dobre skalowanie zapytań SQL do wyliczania polubień, dzięki zrzuceniu pracy na silnik bazy danych (Entity Framework Core przekłada `SelectMany.Count()` na odpowiednie `JOIN` i `COUNT` w SQL).
*   **Negatywne / Do monitorowania:** 
    *   Obliczanie w locie sumarycznej liczby polubień `OrderByDescending(u => u.Pins.SelectMany(p => p.Likes).Count())` przy bardzo dużej ilości danych (miliony rekordów) może wymagać w przyszłości dodania warstwy cache'owania (np. Redis) lub mechanizmu zmaterializowanych widoków (Materialized Views) i okresowego wyliczania rankingów w tle.
