using BT_DataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace BT_JobService
{
    class Program
    {
        public const string URL = "https://localhost:44377";

        static void Main(string[] args)
        {
            Console.WriteLine("BT Console App!");
       
            Console.WriteLine("Calling Api for jobs!");
            List<Job> jobs = GetJobsFromApi().Result;
            Console.WriteLine("Jobs Retrieved!");

            Console.WriteLine("Calling Api for processing jobs!");
            ProcessJobsWithApi(jobs);

            Console.WriteLine("Done!");
            Console.ReadLine();
        }

        public static async Task ProcessJobsWithApi(List<Job> jobs)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

                var r = string.Empty;
                foreach (Job job in jobs)
                {
                    int jobId = job.ID;
                    HttpResponseMessage response = client.GetAsync($"api/v1/directprocess/processjob/?jobId={jobId}").Result; // use .Result instead of await

                    if (response.IsSuccessStatusCode)
                    {
                        r = await response.Content.ReadAsStringAsync();
                    }
                    Console.WriteLine(r);
                }
            }
        }

        public static async Task<List<Job>> GetJobsFromApi()
        {
            var jobs = new List<Job>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(URL);
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));


                HttpResponseMessage response = client.GetAsync("api/v1/jobs").Result; // use .Result instead of await
                Console.WriteLine(response.StatusCode);

                //response.EnsureSuccessStatusCode();
                var jsonJobs = string.Empty;
                if (response.IsSuccessStatusCode)
                {
                    jsonJobs = await response.Content.ReadAsStringAsync();
                    jobs = JsonConvert.DeserializeObject<List<Job>>(jsonJobs);
                }

            }

            return jobs;
        }
    }
}
