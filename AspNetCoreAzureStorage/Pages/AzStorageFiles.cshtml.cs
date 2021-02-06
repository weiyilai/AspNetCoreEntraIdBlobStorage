﻿using AspNetCoreAzureStorage.FilesProvider.AzureStorageAccess;
using AspNetCoreAzureStorage.FilesProvider.SqlDataAccess;
using AspNetCoreAzureStorage.FilesProvider.ViewModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
using System.Collections.Generic;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace AspNetCoreAzureStorage.Pages
{
    public class AzStorageFilesModel : PageModel
    {
        private readonly AzureStorageProvider _azureStorageService;

        [BindProperty]
        public FileDescriptionShort FileDescriptionShort { get; set; }

        public AzStorageFilesModel(AzureStorageProvider azureStorageService)
        {
            _azureStorageService = azureStorageService;
        }

        public void OnGet()
        {
            FileDescriptionShort = new FileDescriptionShort
            {
                Description = "enter description"
            };
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var fileInfos = new List<(string FileName, string ContentType)>();
            if (ModelState.IsValid)
            {
                if (!IsMultipartContentType(HttpContext.Request.ContentType))
                {
                    ModelState.AddModelError("FileDescriptionShort.File", "not a MultipartContentType");
                    return Page();
                }

                foreach (var file in FileDescriptionShort.File)
                {
                    if (file.Length > 0)
                    {
                        var fileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.ToString().Trim('"');

                        fileInfos.Add((fileName, file.ContentType));

                        await _azureStorageService.AddNewFile(new BlobFileUpload
                        {
                            Name = fileName,
                            Description = FileDescriptionShort.Description,
                            UploadedBy = HttpContext.User.Identity.Name
                        },
                        file);
                    }
                }
            }

            var files = new UploadedFileResult
            {
                FileInfos = fileInfos,
                Description = FileDescriptionShort.Description,
                CreatedTimestamp = DateTime.UtcNow,
                UpdatedTimestamp = DateTime.UtcNow,
            };

            // Add to Azure Table storage

            return Page();
        }


        private static bool IsMultipartContentType(string contentType)
        {
            return !string.IsNullOrEmpty(contentType) && contentType.IndexOf("multipart/", StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}