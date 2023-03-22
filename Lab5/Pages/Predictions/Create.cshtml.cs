using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Lab5.Data;
using Lab5.Models;
using Azure.Storage.Blobs;
using System.Drawing;
using Azure;
using System.Runtime.CompilerServices;

namespace Lab5.Pages.Predictions
{
    public class CreateModel : PageModel
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly string earthContainerName = "earthimages";
        private readonly string computerContainerName = "computerimages";
        private readonly Lab5.Data.PredictionDataContext _context;
        private readonly string containerName = "predictions";
        [BindProperty]
        public Prediction Prediction { get; set; }


        public CreateModel(Lab5.Data.PredictionDataContext context, BlobServiceClient blobServiceClient)
        {
            _blobServiceClient = blobServiceClient;
            _context = context;
        }

        public async Task OnGet()
        {

        }


        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            BlobContainerClient containerClient = null;
            Prediction.FileName = file.FileName;
            if (Prediction.Question.Equals(Question.Earth))
            {
                try
                {
                    containerClient = await _blobServiceClient.CreateBlobContainerAsync(earthContainerName);
                    containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                }
                catch(RequestFailedException)
                {
                    containerClient = _blobServiceClient.GetBlobContainerClient(earthContainerName);
                }

            }
            else if (Prediction.Question.Equals(Question.Computer))
            {
                try
                {
                    containerClient = await _blobServiceClient.CreateBlobContainerAsync(computerContainerName);
                    containerClient.SetAccessPolicy(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);
                }
                catch (RequestFailedException)
                {
                    containerClient = _blobServiceClient.GetBlobContainerClient(computerContainerName);
                }

            }
            try
            {
                string randomFileName = Path.GetRandomFileName();
                var blockBlob = containerClient.GetBlobClient(randomFileName);
                if(await blockBlob.ExistsAsync())
                {
                    await blockBlob.DeleteAsync();
                }
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    memoryStream.Position = 0;
                    await blockBlob.UploadAsync(memoryStream);
                    memoryStream.Close();
                }
            }
            catch (RequestFailedException)
            {
                return Page();
            }

            //_context.Predictions.Add(Prediction);
            //await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}

