using Microsoft.AspNetCore.Mvc;
using MyProject.CustomExceptions;
using MyProject.Services;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UserController> _logger;

    public UserController(IUserService userService, ILogger<UserController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    /* Oryginalny kod
    [HttpPost("delete/{id}")]
    public void Delete(uint id)
    {
        User user = _context.Users.FirstOrDefault(user => user.Id == id);
        _context.Users.Remove(user);
        _context.SaveChanges();
        Debug.WriteLine($"The user with Login={user.login} has been deleted");
        return Ok();
    }

    Kilka uwag dotyczących tego fragmentu kodu:

    1. W tym kodzie brakuje jakiejkolwiek autoryzacji.
    Warto dodać autoryzację, na przykład sprawdzając, czy żądający użytkownik ma odpowiednie uprawnienia do usuwania użytkowników.

    2.Nie ma żadnej walidacji danych wejściowych. Warto byłoby dodać walidację, aby upewnić się, 
    że przekazany identyfikator użytkownika jest prawidłowy i istnieje w bazie danych.

    3.Brak obsługi sytuacji, gdy użytkownik o podanym identyfikatorze nie istnieje w bazie danych. 
    W takiej sytuacji operacja usuwania nie powinna być kontynuowana, a klient powinien otrzymać odpowiedni komunikat o błędzie.

    4. Komunikat debugowania zawiera odwołanie do pola `login` obiektu użytkownika, 
    ale w C# nazwy pól są case-sensitive, więc jeśli pole nazywa się `Login`, to takie odwołanie spowoduje błąd kompilacji. 

    5. Metoda ma zwracać typ `void`, co jest sprzeczne z oczekiwanym typem zwracanym przez kontroler akcji typu HTTP POST. 
    Warto zmienić typ zwracany na `IActionResult` i zwrócić odpowiedni status HTTP w zależności od wyniku operacji usuwania.

    6. Warto rozważyć użycie bloku `using` lub odpowiednich mechanizmów DI (Dependency Injection) do zarządzania cyklem życia obiektu kontekstu bazy danych. 
    W tym przypadku brakuje takiego mechanizmu, co może prowadzić do wycieków zasobów.

    Poprawki zależą od kontekstu aplikacji i jej wymagań, ale te punkty powinny być brane pod uwagę podczas przeglądu kodu.
    */

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        try
        {
            await _userService.DeleteUserAsync(id);
            return NoContent(); // Zwracamy odpowiedź HTTP 204 No Content
        }
        catch (UserNotFoundException ex)
        {
            _logger.LogError(ex, $"User deletion failed: {ex.Message}");
            return NotFound(ex.Message); // Zwracamy odpowiedź HTTP 404 Not Found
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while processing the request.");
            return StatusCode(500, "An error occurred while processing your request."); // Zwracamy odpowiedź HTTP 500 Internal Server Error
        }
    }
}