using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNetCoreAzureStorage.FilesProvider.AzureStorageAccess;
using AspNetCoreAzureStorage.FilesProvider.SqlDataAccess;
using AspNetCoreAzureStorage.FilesProvider.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AspNetCoreAzureStorage.Pages
{
    public class ListFilesModel : PageModel
    {
        private readonly AzureStorageProvider _azureStorageService;
        private readonly FileDescriptionProvider _fileDescriptionProvider;

        [BindProperty]
        public IEnumerable<FileDescriptionDto> FileDescriptions { get; set; }

        [BindProperty]
        public string FileName { get; set; }

        public ListFilesModel(AzureStorageProvider azureStorageService,
            FileDescriptionProvider fileDescriptionProvider)
        {
            _azureStorageService = azureStorageService;
            _fileDescriptionProvider = fileDescriptionProvider;
        }

        public void OnGet()
        {
            FileDescriptions = _fileDescriptionProvider.GetAllFiles();
        }

        public async Task<ActionResult> OnGetDownloadFile(string fileName)
        {
            var file = await _azureStorageService.DownloadFile(fileName);

            return File(file.Value.Content, file.Value.ContentType, fileName);
        }

    }
}