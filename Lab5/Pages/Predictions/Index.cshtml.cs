using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Lab5.Data;
using Lab5.Models;
using Azure.Storage.Blobs;
using Azure;

namespace Lab5.Pages.Predictions
{
    public class IndexModel : PageModel
    {
        private readonly Lab5.Data.PredictionDataContext _context;
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string containerName = "predicitions";

        public IndexModel(Lab5.Data.PredictionDataContext context, BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _context = context;
        }

        public IList<Prediction> Prediction { get;set; } = default!;

        public async Task<IActionResult> OnGetAsync()
        {
            if (_context.Predictions != null)
            {
                BlobContainerClient containerClient;
                try
                {
                    containerClient = await _blobServiceClient.CreateBlobContainerAsync(containerName, Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                }
                catch (RequestFailedException)
                {
                    containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
                    return Page();
                }
                List<Prediction> predictions = new();
                foreach (var blob in containerClient.GetBlobs())
                {
                    predictions.Add(new Prediction { FileName = blob.Name, Question = Question.Earth, Url = containerClient.GetBlobClient(blob.Name).Uri.AbsoluteUri });
                }

                Prediction = await _context.Predictions.ToListAsync();
               
            }
            return Page();
        }
    }
}
