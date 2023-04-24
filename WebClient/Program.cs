using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebClient
{
    static class Program
    {
        private static void Main(string[] args)
        {
            RunAsync().GetAwaiter().GetResult();

        }

        static HttpClient client = new HttpClient();

        /// <summary>
        /// Генерация случайных данных пользователя
        /// </summary>
        /// <returns></returns>
        private static Customer RandomCustomer()
        {
            string[] FirstName = new[] { "Alex", "Vasya", "Kolya", "Gled", "Sergey", "Max", "Balmy", "Hot", "Sweltering", "Scorching" };
            string[] LastName = new[] { "Alexof", "Vasonov", "Kolobov", "Gledov", "Sereda", "Maximov", "Balmonov", "Hotova", "Sweltov", "Scorchingov" };

            var rand = new Random();

            Customer customer = new Customer
            {
                Id = rand.Next(1, 1000),
                Firstname = FirstName[rand.Next(FirstName.Length)],
                Lastname = LastName[rand.Next(LastName.Length)]
            };
            return customer;
        }

        /// <summary>
        /// Вывод данных пользователя на консоль
        /// </summary>
        /// <param name="customer"></param>
        static void ShowCustomer(Customer customer)
        {
            if (customer != null)
                Console.WriteLine($"Id: {customer.Id}\tFirst Name: " +
                    $"{customer.Firstname}\tLast Name: {customer.Lastname}");
            else
                Console.WriteLine("Пользователя не существует.");
        }

        /// <summary>
        /// Создание нового пользователя
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        static async Task<Uri> CreateCustomerAsync(Customer customer)
        {
            HttpResponseMessage response = await client.PostAsJsonAsync(
                "api/customer", customer);
            response.EnsureSuccessStatusCode();

            // return URI of the created resource.
            return response.Headers.Location;
        }

        /// <summary>
        /// Получение данных пользователя от сервера
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        static async Task<Customer> GetCustomerAsync(string Id)
        {
            Customer customer = null;
            HttpResponseMessage response = await client.GetAsync($"api/customer/{Id}");
            if (response.IsSuccessStatusCode)
            {
                customer = await response.Content.ReadAsAsync<Customer>();
            }
            return customer;
        }

        /// <summary>
        /// Изменение данных пользователя
        /// </summary>
        /// <param name="customer"></param>
        /// <returns></returns>
        static async Task<Customer> UpdateCustomerAsync(Customer customer)
        {
            HttpResponseMessage response = await client.PutAsJsonAsync(
                $"api/customer/{customer.Id}", customer);
            response.EnsureSuccessStatusCode();

            // Deserialize the updated product from the response body.
            customer = await response.Content.ReadAsAsync<Customer>();
            return customer;
        }

        /// <summary>
        /// Удаление пользователя на сервере
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        static async Task<HttpStatusCode> DeleteCustomerAsync(int id)
        {
            HttpResponseMessage response = await client.DeleteAsync(
                $"api/customer/{id}");
            return response.StatusCode;
        }


        static async Task RunAsync()
        {
            client.BaseAddress = new Uri("http://localhost:5000");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                // Принимаем с консоли Id пользователя
                Console.WriteLine("Укажите Id пользователя:");
                string consoleInput = Console.ReadLine();

                // Запрашиваем данные пользователя с сервера
                Customer customer = await GetCustomerAsync(consoleInput);

                // Отображаем данные пользователя в консоли
                ShowCustomer(customer);

                // Создаем нового пользователя со случайными данными
                customer = RandomCustomer();

                // Отправляем данные созданного пользователя на сервер
                var url = await CreateCustomerAsync(customer);
                Console.WriteLine($"Created at {url}");

                // Запрашиваем созданного пользователя с сервера по Id
                customer = await GetCustomerAsync(url.Query.Remove(0, 4));

                // Отображаем созданного пользователя на кансоле
                ShowCustomer(customer);


            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            Console.ReadLine();
        }
    }
}