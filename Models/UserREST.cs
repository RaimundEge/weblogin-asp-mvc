using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;

namespace WebLogin.Models
{
    public class User
    {
        public string username;
        public string fullname;
        public string password;

        public override string ToString()
        {
            return "User: " + fullname + " (" + username + "," + password + ")";
        }
    }

    public class UserREST
    {
        private readonly HttpClient Client = new HttpClient();

        public UserREST(string url, string secret)
        {
            Client.BaseAddress = new Uri(url);
            Client.DefaultRequestHeaders.Accept.Clear();
            Client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));
            var byteArray = Encoding.ASCII.GetBytes(secret);
            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        }

        public async Task<User> getUser(string name)
        {
            Console.WriteLine("getting user: " + name);
            User user = new User();

            HttpResponseMessage response = await Client.GetAsync("users/" + name);
            if (response.IsSuccessStatusCode)
            {
                //get data as Json string 
                string data = await response.Content.ReadAsStringAsync();
                MemoryStream ms = new MemoryStream(Encoding.Unicode.GetBytes(data));
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(user.GetType());
                user = (User) serializer.ReadObject(ms);
                Console.WriteLine("got: " + user);
                ms.Close();
            }            
            return user;
        }

        public async Task<string> registerUser(User user)
        {
            // stringify user
            MemoryStream ms = new MemoryStream();
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(user.GetType());
            serializer.WriteObject(ms, user);
            string jsonString = Encoding.Default.GetString(ms.ToArray());
            Console.WriteLine("user as json: " + jsonString);
            HttpResponseMessage response = await Client.PostAsync("users/", new StringContent(jsonString, Encoding.UTF8, "application/json"));
            if (response.IsSuccessStatusCode)
            {
                return "New user registered: " + user.fullname;
            }
            else
            {
                return "Error while registering";
            }
        }
    }
}