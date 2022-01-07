using System;
using System.IO;
using MediatR;
using System.Drawing;
using System.Drawing.Imaging;
using GuessingGame.Data;
using GuessingGame.Core.Domain.Image;
using GuessingGame.Core.Domain.Image.Pipelines;
using Microsoft.AspNetCore.Hosting;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace GuessingGame.Core.Domain.Image.Services
{
    public class FileUploader : IFileUploader
    {
        private readonly ApplicationDbContext _db;

        private readonly IMediator _mediator;

        private readonly IWebHostEnvironment _env;

        public FileUploader(ApplicationDbContext db, IMediator mediator, IWebHostEnvironment env)
        {
            _db = db ?? throw new System.ArgumentNullException(nameof(db));
            _mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
            _env = env ?? throw new System.ArgumentNullException(nameof(env));
        }
        public System.Drawing.Image[] ImageArray {get; set;}
        public int xstart {get; set;}
        public int ystart {get; set;}

        /*Method that when called will slice an image into *numslices* amount of segments and store the
        images to different folders for full/propser images and the scattered images.*/
        public async Task<int> SliceImage(IFormFile FormFile, string ImageName, int numSlices)
        {
            //Calling the pipeline to retrive a custom map and id. 
            var MapAndId = await _mediator.Send(new GetCustomMapAndId.Request(ImageName));

            string FilePath = $"{_env.WebRootPath}/img/{MapAndId.folderName}_full/full.png";
            //Creating a new image instance and adding it to the database.
            var result = await _mediator.Send(new AddImage.Request(MapAndId.imageId, ImageName, MapAndId.folderName));
            if(result.Success)
            {
                //Copying the full image to "filepath".
                Directory.CreateDirectory($"{_env.WebRootPath}/img/{MapAndId.folderName}_full");
                using (var stream = System.IO.File.Create(FilePath))
                {
                    FormFile.CopyTo(stream);
                }

                var iter = Math.Sqrt(numSlices);
                //Image that will be sliced.
                var img = new Bitmap(FilePath);
                //Image that will be used for proposer.
                var proposerimg = new Bitmap(FilePath);
                ImageArray = new System.Drawing.Image[numSlices];
                int k = 0;
                //Loops through the width and height of the image to slice it.
                for(int x = 0; x < iter; x++)
                {
                    for(int y = 0; y < iter; y++)
                    {
                        //Slices the image into Rectangle segements.
                        System.Drawing.Rectangle r = new Rectangle(
                            Convert.ToInt32(x*(img.Width / iter)),
                            Convert.ToInt32(y*(img.Height / iter)),
                            Convert.ToInt32(img.Width / iter),
                            Convert.ToInt32(img.Height / iter)
                        );
                        var TransparentImg = new Bitmap(img.Width, img.Height, PixelFormat.Format32bppArgb);
                        TransparentImg.MakeTransparent();
                        var scattered = img.Clone(r, img.PixelFormat);
                        using (Graphics g = Graphics.FromImage(TransparentImg))            
                        {
                            g.DrawImage(scattered, r);
                            ImageArray[k] = TransparentImg;                
                        }
                        var averageX = Convert.ToSingle((r.Left + r.Right)/2);
                        var averageY = Convert.ToSingle((r.Top + r.Bottom)/2);
                        using (Graphics g = Graphics.FromImage(proposerimg))
                        {
                            g.DrawRectangle(new Pen(Color.White, 3), r);
                            g.DrawString($"{k}", new Font("Arial", 20, FontStyle.Bold), new SolidBrush(Color.Red) , averageX, averageY, new StringFormat());
                        }
                        
                        k++;

                    }
                }
                if(ImageArray.Length == numSlices)
                {
                    proposerimg.Save(($"{_env.WebRootPath}/img/{MapAndId.folderName}_full/proposer.png"));
                    Directory.CreateDirectory($"{_env.WebRootPath}/img/{MapAndId.folderName}_scattered");
                    for(int i = 0; i < ImageArray.Length; i++)
                    {
                        ImageArray[i].Save(($"{_env.WebRootPath}/img/{MapAndId.folderName}_scattered/{i}.png")); 
                        
                    }
                    
                }

        }
        return await Task.FromResult(0);
    }
        
        //Method that will slice an image based on users drawing.
        public async Task<int> SliceCustomImage(Bitmap bmp, string ImageName)
        {
            //Adds image to the database
            var MapAndId = await _mediator.Send(new GetCustomMapAndId.Request(ImageName));
            var result = await _mediator.Send(new AddImage.Request(MapAndId.imageId, ImageName, MapAndId.folderName));
            if(result.Success) 
            {
                Directory.CreateDirectory($"{_env.WebRootPath}/img/{MapAndId.folderName}_full");
                bmp.Save($"{_env.WebRootPath}/img/{MapAndId.folderName}_full/full.png");                
                var imgnumber = 0;
                xstart = 0;
                ystart = 0;
                
                startTotal:
                var TransparentImg = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb); //Creates new bitmap to add component
                TransparentImg.MakeTransparent();
                int StartStop = 0;
                List<Point> pointlistX = new List<Point>();
                //Checks pixels in Y direction
                for(int x = xstart; x < bmp.Width-3; x++)  //Starts at x pixel set during run time
                { start:
                    for(int y = ystart; y < bmp.Height-3; y++) //Starts Y pixel at point set during run time
                    {
                        if(bmp.GetPixel(x, y).R == 255 && bmp.GetPixel(x, y).B == 255 && bmp.GetPixel(x, y).G == 10)  //Checks for color of pixel
                        {
                            if(x == xstart || x == bmp.Width-3 || y == ystart || y == bmp.Height-3) //Checks if pixel is outside of bounds
                            {
                                StartStop ++; // Increments to check if line starts at edge of image
                                if(StartStop == 2){
                                    goto end;  //Goes to x part of implementation
                                }
                            }
                            if(x == bmp.Width-3) //Completed Y direction, goes to x part of implementation
                            {
                                goto end;
                            }
                            x++;
                            goto start;  //Repeats the same step.
                            
                        } else{
                            pointlistX.Add(new Point(x,y)); //If no if statement checks out, x is incremented to move checker one step forward
                        }
                    }
                } end:
                List<Point> pointlistY = new List<Point>();
                int StartStopY = 0;
                //Checks pixels in X direction
                for(int y = ystart; y < bmp.Height-3; y++) //Starts Y pixel at point set during run time
                { startY:
                    for(int x = xstart; x < bmp.Width-3; x++) //Starts X pixel at point set during run time
                    {
                        if(bmp.GetPixel(x, y).R == 255 && bmp.GetPixel(x, y).B == 255 && bmp.GetPixel(x, y).G == 10) //Checks for color of pixel
                        {
                            if(x == xstart || x == bmp.Width-3 || y == ystart || y == bmp.Height-3) //Checks if pixel is out of bounds
                            {
                                StartStopY ++; // Increments to check if line starts at edge of image
                                if(StartStopY == 2){
                                    goto endY; // Goes to endY completing slicing
                                }
                            }
                            if(y == bmp.Height-3) //Checks if slicing is through the whole image
                            {
                              goto endY;
                            }                            
                            y++;
                            goto startY; //Increments Y and repeats
                            
                        } else{
                            pointlistY.Add(new Point(x,y));  // Adds new pointlist
                        }
                    }
                } endY:
                var pointlist = pointlistX.Union(pointlistY);  //Concatenates both list
                using (Graphics g = Graphics.FromImage(TransparentImg))
                {
                    using (Brush brush = new TextureBrush(bmp))
                    {
                        g.FillPolygon(brush, pointlist.ToArray());  //Fills in the section currently added to the list
                    }
                }
                TransparentImg.Save($"{_env.WebRootPath}/img/{MapAndId.folderName}_full/{imgnumber}.png");  //Saves image
                imgnumber++;
                if(imgnumber > 25)
                {
                    goto endTotal;  //Ends the complete image. Added to stop slicing from adding images forever if it encounters a bug
                }
                if (pointlist.Max(t => t.X) >= bmp.Width-5 && pointlist.Max(t => t.Y) >= bmp.Height-5){ //Checks if last pixel added is at the end of the image
                    goto endTotal;
                }
                else if (pointlist.Max(t => t.X) >= bmp.Width-5 && pointlist.Max(t => t.Y) < bmp.Height-3) //Checks if it needs to run again because section added didnt reach the end of the image
                {
                    xstart = 0;
                    ystart = pointlist.Max(t => t.Y) + 5;
                    goto startTotal; //Goes back to start
                }
                else{  //Sets pointlist to the highest X value and the lowest Y value and runs again
                    xstart = pointlist.Max(t => t.X) + 5;
                    ystart = pointlist.Min(t => t.Y);
                    goto startTotal;   
                }
                

            }
            endTotal:
            return await Task.FromResult(0);   
        }

        //Unfinished custom slicer. We would continiue on this implementation, but due to time constraints we decided to leave it at our previous implementation,
        //and focus on fixing bugs and completing tests.
        //If you want to test this code, call upon it beneath the call for SliceCustomImage in CustomImageManual.cshtml.cs
        public async Task<int> FloodFill(Bitmap bmp, string ImageName, Point pt){
            Directory.CreateDirectory($"{_env.WebRootPath}/img/{1}_full"); //Creates directory
            bmp.Save($"{_env.WebRootPath}/img/{1}_full/full.png");  // Saves the full image
            var targetcolor = new Color(); 
            targetcolor = Color.FromArgb(255, 10, 255);  //Finds the color of the line drawn by a user
            Stack<Point> pixels = new Stack<Point>();
            var TransparentImg = new Bitmap(bmp.Width, bmp.Height, PixelFormat.Format32bppArgb);
            TransparentImg.MakeTransparent();  //Creates a bitmap for the image
            pixels.Push(pt);
            while (pixels.Count > 0){
                Point a = pixels.Pop();
                if (a.X < bmp.Width && a.X > 0 && a.Y < bmp.Height && a.Y > 0){  //Checks if image is within bounds
                    if (bmp.GetPixel(a.X, a.Y) == targetcolor){  //Checks if the pixel poped is equal to target color
                        if (a.X >= bmp.Width && a.Y >= bmp.Height){  //If the pixels are equal or close to bounds, ends method
                            goto end;
                        }
                        else if (a.X <= bmp.Width){  //Checks if x can be incremented, does so if possible
                            a.X += 1;
                            if (a.X == bmp.Width){
                                Console.WriteLine($"Ferdig med {a.Y}/420 linjer");
                                a.X=1;
                                a.Y += 1;
                            }
                            pixels.Push(new Point(a.X, a.Y));
                        } else if (a.X >= bmp.Width-3){  //if a.x is close to the width, increment Y instead
                            a.Y += 1;
                            if (a.Y == bmp.Height){
                                goto end;
                            }
                            pixels.Push(new Point(1, a.Y + 1));
                        }
                    } else {
                        var pixel = bmp.GetPixel(a.X, a.Y);  //Pixel is not target color.
                        TransparentImg.SetPixel(a.X, a.Y, pixel);  //Gets pixel and draws it on the transparent map
                        if (a.X >= bmp.Width && a.Y >= bmp.Height){  //Checks if the pixel is through the game
                            goto end;
                        }
                        else if (a.X < bmp.Width){  //Checks if X can be incremented, does so if possible
                            a.X += 1;
                            if (a.X == bmp.Width){
                                a.X=1;
                                a.Y += 1;
                            }
                            pixels.Push(new Point(a.X, a.Y));
                        } else if (a.X > bmp.Width){  //If X cant be incremented, increments Y
                            a.Y += 1;
                            if (a.Y == bmp.Height){
                                goto end;
                            }
                            pixels.Push(new Point(1, a.Y + 1));
                        }
                    }
                } else {
                    goto end;
                }
            }
            end:
            TransparentImg.Save($"{_env.WebRootPath}/img/{1}_full/{1}.png"); // Saves full image sliced based upon the purple line.
            return await Task.FromResult(0);
        }
}
}