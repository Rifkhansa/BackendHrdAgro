using System;
/*using BackendHrdAgro.Controllers.Batch;
using BackendHrdAgro.Models;
using BackendHrdAgro.Models.Batch;*/
using Quartz;

namespace BackendHrdAgro.Services.Scheduler
{
	public class IntegrateScheduler : IJob
    {
        public string BatchId { get; set; }
        public IWebHostEnvironment Environment { get; set; }

        public IntegrateScheduler()
        {

        }

        public Task Execute(IJobExecutionContext context)
        {
            Console.WriteLine("Environmet " + BatchId);
            Console.WriteLine("Batch " + Environment.ContentRootPath);
            return Task.CompletedTask;
        }
    }
}

