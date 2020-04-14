using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BT_DataAccess.Interfaces;
using BT_DataModels;
using Serilog;

namespace BTProb.Services
{
    public class JobService
    {
        private readonly IRepository _jobsRepository;

        public JobService(IRepository repository)
        {
            _jobsRepository = repository;
        }

        public async Task<Job> GetJobById(int id)
        {
            if (id > 0)
            {
                var job = await _jobsRepository.GetById(id);

                return job;
            }
            else
            {
                return null;
            }
        }

        public async Task<int> CreateJob(string jobName, string jobType, string filePath = "")
        {
            Job job = new Job
            {
                File = filePath,
                Name = jobName,
                FileAttached = string.IsNullOrEmpty(filePath) ? false : true
            };

            JobType t;
            if (Enum.TryParse(jobType, out t))
            {
                job.Type = t;
                Log.Information($"JobType valid parse: {t.ToString()} for job {jobName}, type : {jobType}, with filePath: {filePath}");
            }
            else
            {
                job.Type = JobType.Unspecified;
                Log.Information($"JobType set to {job.Type.ToString()}; Unvalid parse for job {jobName}, type : {jobType}, with filePath: {filePath}");
            }

            Log.Information($"Creating Job ....");
            return await _jobsRepository.Create(job);
        }

        public async Task<bool> UpdateJob(Job job)
        {
            Log.Information($"Updating Job {job.ID} ....");
            var result = await _jobsRepository.Update(job);
            return result == 1;
        }

        public async Task DeleteJob(int id)
        {
            _jobsRepository.Delete(id);
        }

        public async Task<List<Job>> GetAllJobs()
        {
            return await _jobsRepository.GetAll();
        }
    }
}
