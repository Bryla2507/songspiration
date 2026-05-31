# ADR-0008: Implementacja Bezpiecznego Resetu Hasła

## Status
Zaakceptowany

## Kontekst
W ramach zapewnienia użytkownikom możliwości odzyskiwania dostępu do kont (w przypadku zapomnienia hasła) zaistniała potrzeba implementacji mechanizmu "Password Reset".
Kluczowym wymaganiem było zagwarantowanie bezpieczeństwa procesu, aby uniknąć ataków typu "Account Takeover" oraz przejmowania kont w wyniku zgadnięcia linku resetującego. Wymagane było wygenerowanie czasowego poświadczenia i przesłanie go w bezpiecznym, asynchronicznym kanale do prawowitego właściciela konta.

## Decyzja
Zdecydowano się na następującą architekturę i podejście:
1.  **Generowanie Tokenów (AuthTokens):** 
    *   Wprowadzono dedykowaną tabelę/encję `AuthToken`, aby oddzielić tokeny autoryzacyjne od głównych danych użytkownika w encji `User`. Pozwala to na pełną historię żądań oraz łatwe unieważnianie pojedynczych prób.
    *   Tokeny (`TokenHash`) są generowane jako kryptograficznie bezpieczne i unikalne identyfikatory (`Guid.NewGuid().ToString("N")`).
    *   Każdy token resetujący ma ściśle określony czas ważności (`ExpiryDate`), ustawiony na 1 godzinę.
    *   Wprowadzono flagę `IsRevoked`, która zapobiega podwójnemu użyciu tego samego linku do zmiany hasła. W momencie pierwszego skutecznego użycia token staje się bezwartościowy.

2.  **Dystrybucja Linku:** 
    *   Do przesyłania wiadomości służy interfejs `IEmailSender`, co pozwala na uniezależnienie logiki aplikacji od dostawcy usług e-mail. Link zawiera wygenerowany token przesyłany jako parametr zapytania (query param) pod adres wskazany w konfiguracji frontendu.

3.  **Horyzontalna Ochrona Prywatności:** 
    *   Endpoint żądający resetu hasła (w `UserService.ForgotPasswordAsync`) celowo nie zwraca informacji, czy użytkownik istnieje w bazie danych. Pozwala to zabezpieczyć aplikację przed zjawiskiem tzw. "User Enumeration" (odgadywania bazy e-maili na podstawie różnicy w odpowiedziach).

4.  **Bezpieczne składowanie nowego hasła:**
    *   Podczas samego procesu resetowania użyto istniejącego algorytmu `BCrypt.Net.BCrypt.HashPassword()`, zachowując jednolity standard szyfrowania z logowaniem.

## Konsekwencje
*   **Pozytywne:**
    *   Podwyższenie bezpieczeństwa poprzez zastosowanie unikalnych, ograniczonych czasowo i jednorazowych tokenów z wykorzystaniem osobnej encji `AuthToken`.
    *   Zabezpieczenie przed atakami Enumeration.
*   **Negatywne / Do monitorowania:** 
    *   Z czasem w bazie danych może nagromadzić się wiele nieużytych, wygasłych rekordów w tabeli `AuthTokens`. Będzie to wymagać zaimplementowania mechanizmu (np. cyklicznego joba/serwisu w tle), który będzie czyścił bazę z przestarzałych tokenów, aby zapobiegać jej nadmiernemu rozrostowi.