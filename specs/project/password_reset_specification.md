# Specyfikacja Funkcjonalności: Reset Hasła (Password Reset)

## 1. Cel i Opis Funkcjonalności
Funkcjonalność resetowania hasła umożliwia użytkownikowi odzyskanie dostępu do konta w przypadku zapomnienia hasła. Proces składa się z dwóch etapów: żądania resetu poprzez podanie adresu e-mail oraz ustalenia nowego hasła po wejściu z unikalnego linku z tokenem autoryzacyjnym, wysłanego na podany adres e-mail.

## 2. Zmiany w Modelu Danych (Domenowym)
Wykorzystywana jest tabela/encja `AuthToken`, która przechowuje tymczasowe tokeny powiązane z kontem użytkownika.
### Tabela `AuthToken`
*   `Id` (Guid) - klucz główny.
*   `UserId` (Guid) - identyfikator użytkownika z tabeli `User`.
*   `TokenHash` (string) - unikalny, wygenerowany kryptograficznie token przypisany do żądania.
*   `TokenType` (Enum: TokenType) - określa cel tokena (w tym przypadku `PasswordReset`).
*   `ExpiryDate` (DateTime) - czas ważności tokena (np. 1 godzina od wygenerowania).
*   `IsRevoked` (bool) - flaga oznaczająca, czy token został już użyty (zapobiega ponownemu użyciu).

## 3. Warstwa Dostępu do Danych (Repozytoria)
W projekcie wykorzystywany jest bezpośredni dostęp do `SongSpirationDbContext` w warstwie BLL do operacji na tabeli `AuthTokens`. Używane są standardowe metody Entity Framework Core (`Add`, `Update`, `SaveChangesAsync`).

## 4. Warstwa Logiki Biznesowej (BLL)
W serwisie `UserService` (implementującym `IUserService`) zdefiniowane są dwie metody:
*   `Task ForgotPasswordAsync(ForgotPasswordDto dto)`: Wyszukuje użytkownika po e-mailu. Jeśli istnieje, generuje nowy `AuthToken` (typ `PasswordReset`) z określonym czasem ważności, zapisuje go w bazie i wywołuje `IEmailSender.SendEmailAsync` do wysłania linku z tokenem do użytkownika.
*   `Task ResetPasswordAsync(ResetPasswordDto dto)`: Weryfikuje token (czy istnieje, czy nie wygasł, czy nie jest użyty, typ = `PasswordReset`). W przypadku poprawnej weryfikacji tokenu, generuje nowy hash hasła (BCrypt) dla powiązanego użytkownika, nadpisuje hasło w bazie, a token oznacza jako `IsRevoked = true`.

## 5. Kontrolery (API)
Funkcjonalność udostępniana jest w klasie (prawdopodobnie w `UserController` lub `AuthController`):
*   `POST /api/users/forgot-password` - przyjmuje `ForgotPasswordDto` (zawiera `Email`).
*   `POST /api/users/reset-password` - przyjmuje `ResetPasswordDto` (zawiera `Token` i `NewPassword`).

## 6. Frontend
Aplikacja kliencka udostępnia interfejs graficzny dla procesu:
*   **Widok `ForgotPasswordView`:** Formularz z polem na adres e-mail i przyciskiem "Wyślij link resetujący".
*   **Widok `ResetPasswordView`:** Odbiera z parametrów zapytania w URL token dostępu (`?token=...`). Formularz zawiera dwa pola na hasło ("Nowe hasło", "Potwierdź nowe hasło"). Po poprawnej walidacji i zatwierdzeniu wysyła żądanie do API. W przypadku sukcesu przenosi użytkownika do okna logowania.