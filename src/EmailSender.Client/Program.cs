using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using EmailService.Core.Models;

namespace EmailSender.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using var client = new HttpClient();
            client.BaseAddress = new System.Uri("https://localhost:61814");

            var email = new EmailRequest
            {
                To = "test@example.com",
                Subject = "Test Email",
                Body = "This is a test email from the console client."
            };

            var response = await client.PostAsJsonAsync("/api/email/send", email);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                System.Console.WriteLine("Email sent successfully!");
                System.Console.WriteLine(result);
            }
            else
            {
                System.Console.WriteLine($"Error sending email: {response.StatusCode}");
            }
        }
    }
}