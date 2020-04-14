using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;
using BT_DataModels;
using BTProb.Models;
using BTProb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace BTProb.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DirectProcessController : BaseController
    {
        private readonly JobService _jobService;
        private readonly FileService _fileService;

        public DirectProcessController(JobService jobService, FileService fileService)
        {
            _fileService = fileService;
            _jobService = jobService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobsAndProcess()
        {
            var url = string.Format("{0}://{1}/", HttpContext.Request.Scheme, HttpContext.Request.Host);
            List<Job> jobs = await _jobService.GetAllJobs();
            string status = string.Empty;

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(url);

                foreach (var item in jobs)
                {
                    try
                    {
                        int jobId = item.ID;
                        HttpResponseMessage response = await client.GetAsync($"api/v1/DirectProcess/ProcessJob?jobId={jobId}");

                        status = string.Join(", ", response.StatusCode);
                        Log.Information($"{response.StatusCode}");
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, $"{ex.Message}");
                    }
                }
            }

            return await FromResult(func: async () => $"Jobs status  {status}");
        }

        [HttpGet("ProcessJob")]
        public async Task<IActionResult> ProcessJob(int jobId)
        {
            if (jobId <= 0)
            {
                return await FromResult(func: async () => StatusCode((int)HttpStatusCode.BadRequest, "Bad id!"));
            }

            Job job = await _jobService.GetJobById(jobId);

            // do code for job process

            //mark as job processed and save job to db
            job.Processed = true;
            job.ProcessedDate = DateTime.UtcNow;

            _jobService.UpdateJob(job);

            return await FromResult(func: async () => "Job with id:" + jobId + " was processed");
        }

        [HttpGet("ProcessJobXml")]
        public async Task<IActionResult> ProcessJobAsXml(int jobId)
        {
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("Response");
            if (jobId <= 0)
            {
                root.SetAttribute("R", "Bad id!");
                xml.AppendChild(root);
                return new ContentResult { Content = xml.OuterXml, ContentType = "application/xml" };
            }

            Job job = await _jobService.GetJobById(jobId);

            // do code for job process

            //mark as job processed and save job to db
            job.Processed = true;
            job.ProcessedDate = DateTime.UtcNow;

            _jobService.UpdateJob(job);

            
            root.SetAttribute("R", "Job with id:" + jobId + " was processed");
            xml.AppendChild(root);

            return new ContentResult { Content = xml.OuterXml, ContentType = "application/xml" };
        }

        //descriebed Flow of a Direct Process Job, with file attached, ending with deletion
        [HttpPost]
        public async Task<IActionResult> DirectProcess(IFormFile file, [FromQuery]JobFileModel model)
        {
            //check for file; add error messages if none
            string jobError = string.Empty;
            string fileError = string.Empty;
            if (file == null || file.Length == 0)
            {
                fileError = "No file selected";
                Log.Error($"{fileError}");
            }

            if (ModelState.IsValid)
            {
                Log.Information($"DirectProcess started for job {model.JobName}, type : {model.JobType}, with filePath: {model.FilePath}");
                int jobId = await _jobService.CreateJob(model.JobName, model.JobType, model.FilePath);

                if (jobId > 0)
                {
                    Log.Information($"Job {jobId} was created!");

                    //do code for any other actions needed in the process

                    string fileName = string.Concat(file.FileName, "-", jobId);
                    Log.Information($"Async Write associated file {fileName} to FileDirectory, for jobId : {jobId}");
                    _fileService.WriteFileToDisk(file, fileName);

                    Log.Information($"Start JobProcess for JobId : {jobId}");
                    
                    var job = await _jobService.GetJobById(jobId);
                    Log.Information($"Job retrieved from DB for JobId : {jobId}");

                    if (job == null)
                    {
                        Log.Error($"No Job found on DB for JobId : {jobId}");
                        return await FromResult(func: async () => StatusCode((int)HttpStatusCode.NotFound, "No job found!"));
                    }

                    // do code for job process

                    //mark as job processed and save job to db
                    job.Processed = true;
                    job.ProcessedDate = DateTime.UtcNow;
                    bool r = await _jobService.UpdateJob(job);
                    if (!r)
                    {
                        Log.Error($"Job {jobId} was NOT updated in DB.");
                        return await FromResult(func: async () => StatusCode((int)HttpStatusCode.FailedDependency, "Job NOT updated!"));
                    }

                    /**read file, process and write file to proccessed directory**/
                    //reading file by lines, from WriteDirectory location
                    var fileLines = await _fileService.ReadFile(fileName);

                    //do code for process file lines, ex:
                    foreach (string line in fileLines)
                    {
                        // do code for processing
                    }

                    //write file to processed directory
                    bool f = await _fileService.WriteProcessedFile(fileName, fileLines);
                    if (!f)
                    {
                        Log.Error($"File {fileName} NOT written to Processed Directory");
                        return await FromResult(func: async () => StatusCode((int)HttpStatusCode.FailedDependency, "File NOT written to Processed Directory!"));
                    }

                    /**do any code needed for a processed job and file**/

                    //perform deletion on job
                    job.IsDeleted = true;
                    _jobService.DeleteJob(jobId);

                    return await FromResult(func: async () => "Job with id:" + jobId + " was processed and deleted");
                }
                else
                {
                    jobError = "Job was NOT created";
                    Log.Error($"{jobError}");
                }
            }

            //add model requirements to errors
            string errorMessage = string.Join(", ", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
            if (fileError != string.Empty)
            {
                errorMessage = string.Join(", ", errorMessage, fileError, jobError);
            }
            return await FromResult(func: async () => BadRequest(errorMessage ?? "Bad Request"));
        }
    }
}