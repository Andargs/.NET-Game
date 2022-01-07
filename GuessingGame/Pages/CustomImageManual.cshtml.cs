using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using GuessingGame.Data;
using Microsoft.AspNetCore.Identity;
using MediatR;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Image.Pipelines;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using GuessingGame.Core.Domain.Image.Services;
using Microsoft.AspNetCore.Http;

namespace GuessingGame.Pages
{
    public class CustomImageManualModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
		
        private readonly IMediator _mediator;

        private readonly ApplicationDbContext _context;
        
        private readonly IWebHostEnvironment _env;

        private readonly IFileUploader _fileUploader;

        //Allows the user to slice an image however they may wish.
		public CustomImageManualModel(IMediator mediator, ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment env, IFileUploader fileUploader) {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
            _env = env ?? throw new System.ArgumentNullException(nameof(env));
            this._fileUploader = fileUploader ?? throw new System.ArgumentNullException(nameof(fileUploader));
        }

        public IFormFile FormFile { get; set; }

        public string ImageName {get; set;}

        public System.Drawing.Image[] ImageArray {get; set;}

        public string strSlices {get; set;}

        public string imageData {get; set;}
        
        public async Task<IActionResult> OnPostAsync(string imageData, string imageName)
        {
            if (imageData is not null && imageName is not null)
            {
                byte[] imageAsBytes = Convert.FromBase64String(imageData);
                Bitmap bmp;
                using (var ms = new MemoryStream(imageAsBytes))
                {
                    bmp = new Bitmap(ms);
                }
                //await _fileUploader.SliceCustomImage(bmp, imageName);
                await _fileUploader.FloodFill(bmp, imageName, new Point(1,1));
                return RedirectToPage("Index");
            }
            return Page();
        }
    }
     
}