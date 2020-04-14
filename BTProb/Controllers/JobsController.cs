using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using BT_DataModels;
using BTProb.Models;
using BTProb.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BTProb.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class JobsController : BaseController
    {
        private readonly JobService _jobService;

        public JobsController(JobService service)
        {
            _jobService = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            if (id <= 0)
            {
                return await FromResult(func: async () => StatusCode((int)HttpStatusCode.BadRequest, "Bad id!"));
            }
            var job = await _jobService.GetJobById(id);
            if (job == null)
            {
                return await FromResult(func: async () => StatusCode((int)HttpStatusCode.NotFound, "No job found!"));
            }

            return await FromResult(func: async () => job);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllJobs()
        {
            var jobs = await _jobService.GetAllJobs();
            if (jobs == null || jobs.Count == 0)
            {
                return await FromResult(func: async () => StatusCode((int)HttpStatusCode.NotFound, "No jobs found!"));
            }

            return await FromResult(func: async () => jobs);
        }

        [HttpGet("xml")]
        public async Task<IActionResult> GetAllJobsIdsAsXml()
        {
            XmlDocument xml = new XmlDocument();
            XmlElement root = xml.CreateElement("jobsIds");
            xml.AppendChild(root);

            var jobs = await _jobService.GetAllJobs();
            if (jobs == null || jobs.Count == 0)
            {
                XmlElement child = xml.CreateElement("jobId");
                child.SetAttribute("ID", "0");
                return new ContentResult { Content = xml.OuterXml, ContentType = "application/xml" };
            }
            foreach (var job in jobs)
            {
                XmlElement child = xml.CreateElement("jobId");
                child.SetAttribute("ID", job.ID.ToString());
                root.AppendChild(child);
            }

            return new ContentResult { Content = xml.OuterXml, ContentType = "application/xml" };
        }

        [HttpPost]
        public async Task<IActionResult> Post(JobModel model)
        {
            return await FromResult(func: async () => "Job with id:" + await _jobService.CreateJob(model.JobName, model.JobType) + " was saved");
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromQuery]int id)
        {
            _jobService.DeleteJob(id);
            return await FromResult(func: async () => Ok("Deleted"));
        }
    }
}
