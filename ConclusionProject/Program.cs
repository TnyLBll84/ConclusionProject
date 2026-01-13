using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using ConsoleTables;

namespace ConclusionProject
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Fetching 5 Random Users concurrently:");

            int numberOfUsers = 5;
            Task<User>[] userTasks = new Task<User>[numberOfUsers];

            for (int i = 0; i < numberOfUsers; i++)
            {
                userTasks[i] = FetchRandomUserAsync();
            }

            User[] users = await Task.WhenAll(userTasks);

            // Display users using ConsoleTable
            var table = new ConsoleTable("Gender", "Name", "Email", "Country");

            foreach (var user in users)
            {
                table.AddRow(
                    user.Gender,
                    user.Name,
                    user.Email,
                    user.Country
                );
            }

            table.Write();
        }

        static async Task<User> FetchRandomUserAsync()
        {
            string url = "https://randomuser.me/api/";
            using HttpClient client = new HttpClient();

            string response = await client.GetStringAsync(url);
            var doc = JsonDocument.Parse(response);

            var root = doc.RootElement.GetProperty("results")[0];

            string gender = root.GetProperty("gender").GetString();
            string title = root.GetProperty("name").GetProperty("title").GetString();
            string firstname = root.GetProperty("name").GetProperty("first").GetString();
            string lastname = root.GetProperty("name").GetProperty("last").GetString();
            string email = root.GetProperty("email").GetString();
            string country = root.GetProperty("location").GetProperty("country").GetString();

            return new User
            {
                Gender = gender,
                Name = $"{title} {firstname} {lastname}",
                Email = email,
                Country = country
            };
        }
    }

    public class User
    {
        public string Gender { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
    }
}
