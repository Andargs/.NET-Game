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
using GuessingGame.Core.Domain.Image.Services;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using GuessingGame;
using Microsoft.AspNetCore.Http;

namespace GuessingGame.Pages
{
    public class CustomImageModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IFileUploader _fileUploader;
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        //Allows the user to upload a custom image and slice it automatically.
        //The user is able to specify the amount of slices the image should be split into.
		public CustomImageModel(IMediator mediator, ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment env, IFileUploader fileUploader) {
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _context = context ?? throw new System.ArgumentNullException(nameof(context));
            _userManager = userManager ?? throw new System.ArgumentNullException(nameof(userManager));
            _env = env ?? throw new System.ArgumentNullException(nameof(env));
            this._fileUploader = fileUploader ?? throw new System.ArgumentNullException(nameof(fileUploader));
        }
        
        public IFormFile FormFile { get; set; }

        public string ImageName {get; set;}

        public string strSlices {get; set;}

        public async Task<IActionResult> OnPost(IFormFile FormFile, string ImageName, string strSlices)
        {
            if (FormFile is not null && ImageName is not null && strSlices is not null)
            {
                int numSlices = Int32.Parse(strSlices);
                await _fileUploader.SliceImage(FormFile, ImageName, numSlices);
                return RedirectToPage("Index");
            }
            return Page();
        }        

    }  
}