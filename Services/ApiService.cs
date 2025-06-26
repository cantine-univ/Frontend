using System.Net.Http;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using FrontendApp.Models;
using Microsoft.JSInterop;
using System.Collections.Generic;

namespace FrontendApp.Services
{
    public class ApiService
    {
        private readonly HttpClient _httpClient;
        private readonly IJSRuntime _js;

        public string? LastError { get; private set; }

        public ApiService(HttpClient httpClient, IJSRuntime js)
        {
            _httpClient = httpClient;
            _js = js;
        }

        private async Task SetAuthHeaderAsync()
        {
            var token = await _js.InvokeAsync<string>("localStorage.getItem", "token");
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // 🔐 Connexion
        public async Task<AuthResponseDto?> LoginAsync(UserLoginDto login)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Auth/login", login);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }
            return null;
        }

        // 👤 Inscription
        public async Task<AuthResponseDto?> RegisterAsync(UserRegisterDto register)
        {
            try
            {
                LastError = null;
                var response = await _httpClient.PostAsJsonAsync("api/Auth/register", register);
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                }

                var error = await response.Content.ReadAsStringAsync();
                LastError = $"Erreur {response.StatusCode}: {error}";
                Console.WriteLine("❌ Erreur backend : " + LastError);
                return null;
            }
            catch (Exception ex)
            {
                LastError = $"Exception: {ex.Message}";
                Console.WriteLine("❗ Exception RegisterAsync : " + LastError);
                return null;
            }
        }

        // 🍽️ Obtenir les menus
        public async Task<List<MenuDto>> GetMenusAsync()
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync("api/Menu");
            response.EnsureSuccessStatusCode();
            var menus = await response.Content.ReadFromJsonAsync<List<MenuDto>>();
            return menus ?? new List<MenuDto>();
        }

        public async Task<HttpResponseMessage> UploadMenuImage(MultipartFormDataContent content)
        {
            return await _httpClient.PostAsync("api/menu/uploadimage", content);
        }

        // 📋 Obtenir les réservations
        public async Task<List<ReservationDto>> GetReservationsAsync()
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync("api/Reservation");
            response.EnsureSuccessStatusCode();
            var reservations = await response.Content.ReadFromJsonAsync<List<ReservationDto>>();
            return reservations ?? new List<ReservationDto>();
        }

        // ➕ Créer une réservation
        public async Task<ReservationDto?> CreateReservationAsync(ReservationCreateDTO reservation)
        {
            await SetAuthHeaderAsync();

            var response = await _httpClient.PostAsJsonAsync("api/Reservation", reservation);
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ReservationDto>();
            }

            var errorText = await response.Content.ReadAsStringAsync();
            Console.WriteLine("❌ Erreur lors de la création de réservation : " + errorText);
            return null;
        }



        public async Task<bool> DeleteReservationAsync(int reservationId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"api/Reservation/{reservationId}");

                if (response.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Erreur lors de l'annulation de la réservation : {response.StatusCode} - {errorContent}");
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception lors de l'annulation de la réservation : {ex.Message}");
                return false;
            }
        }

        // 📝 Créer Menu
        public async Task<bool> CreateMenuAsync(MenuDto menu)
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.PostAsJsonAsync("api/Menu", menu);
            return response.IsSuccessStatusCode;
        }


        // 📝 Supprimer Menu
        public async Task<bool> DeleteMenuAsync(int menuId)
        {
            await SetAuthHeaderAsync(); // 🔐 important
            var response = await _httpClient.DeleteAsync($"api/Menu/{menuId}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> AjouterAnnotationAsync(AnnotationCreateDTO annotation)
        {
            await SetAuthHeaderAsync(); 
            var response = await _httpClient.PostAsJsonAsync("api/Annotation", annotation);
    
            return response.IsSuccessStatusCode;
        }

        // Récupère toutes les annotations (probablement pour un rôle Admin)
        public async Task<List<AnnotationDTO>> GetAllAnnotationsAsync()
        {
            await SetAuthHeaderAsync();
            var response = await _httpClient.GetAsync("api/Annotation/all");

            if (response.IsSuccessStatusCode)
            {
                var annotations = await response.Content.ReadFromJsonAsync<List<AnnotationDTO>>();
                return annotations ?? new List<AnnotationDTO>();
            }
            else if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized ||
                     response.StatusCode == System.Net.HttpStatusCode.Forbidden)
            {
                
                return new List<AnnotationDTO>();
            }
            return new List<AnnotationDTO>();
        }

        // Récupère les annotations de l'utilisateur actuel
        public async Task<List<AnnotationDTO>> GetAnnotationsParUtilisateurAsync()
        {
            try
            {
                await SetAuthHeaderAsync();
                var response = await _httpClient.GetAsync("api/Annotation/mes-annotations"); 

                if (response.IsSuccessStatusCode)
                {
                    var annotations = await response.Content.ReadFromJsonAsync<List<AnnotationDTO>>();
                    return annotations ?? new List<AnnotationDTO>();
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    LastError = $"Erreur de récupération des annotations de l'utilisateur : {response.StatusCode} - {errorContent}";
                    Console.WriteLine(LastError);
                    return new List<AnnotationDTO>();
                }
            }
            catch (Exception ex)
            {
                LastError = $"Exception lors de la récupération des annotations de l'utilisateur : {ex.Message}";
                Console.WriteLine(LastError);
                return new List<AnnotationDTO>();
            }
        }



    }
}
