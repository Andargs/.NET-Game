using GuessingGame.Core.Domain.Image;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System;
using GuessingGame.Core.Domain.Image.Pipelines;
using GuessingGame.Data;
using MediatR;
using System.Linq;

namespace GuessingGame.Core.Domain.Image.Services
{
    public class ImageCombiner : IImageCombiner
    {
        private readonly ApplicationDbContext _db;

        private readonly IMediator _mediator;

        public ImageCombiner(ApplicationDbContext db, IMediator mediator)
        {
            _db = db;
            _mediator = mediator;
        }

        /*Method that runs on startup that will create a combination of all the segments for an image into
        a full image and store them as propser.png*/
        public async Task<int> Startup()
        {
            int imagecount = 0;
            var Images = await _mediator.Send(new GetImages.Request());
            foreach(var image in Images)
            {
                imagecount++;
                // A Image for the full image including the line segements and tile number. 
                // Starting off with "0.png as a template."
                var bmap = new Bitmap($"../GuessingGame/wwwroot/img/{image.ImageMap}_scattered/0.png");
                // A Full image of all the segments without the line segments and tile number. 
                // Starting off with "0.png as a template."
                var bmapfull = new Bitmap($"../GuessingGame/wwwroot/img/{image.ImageMap}_scattered/0.png");
                using (Graphics g = Graphics.FromImage(bmap))
                    {
                        g.DrawString($"0", new Font("Arial", 14), new SolidBrush(Color.Red), 10.0f, 10.0f, new StringFormat());
                    }
                for(int i = 1; i < Directory.GetFiles(Path.Combine("../GuessingGame/wwwroot/img/",image.ImageMap + "_scattered"), "", SearchOption.TopDirectoryOnly).Length; i++)
                {
                    //Checks if there are any image segments left for processing.
                    if(File.Exists($"../GuessingGame/wwwroot/img/{image.ImageMap}_scattered/{i}.png"))
                    {
                        List<Point> pointList = new List<Point>();
                        var newbitmap = new Bitmap($"../GuessingGame/wwwroot/img/{image.ImageMap}_scattered/{i}.png");
                        var newbitmapfull = new Bitmap($"../GuessingGame/wwwroot/img/{image.ImageMap}_scattered/{i}.png");
                        for(int x = 1; x < newbitmap.Width-1; x++)
                        {
                            for(int y = 1; y < newbitmap.Height-1; y++)
                            {   
                                if(newbitmap.GetPixel(x, y).A != 0) //checks if the pixel is not transparent.
                                {
                                    //Checks if any neighbouring pixel is transparent.
                                    //Only interested in the border.
                                    if(newbitmap.GetPixel(x-1, y).A == 0 || newbitmap.GetPixel(x+1, y).A == 0 || newbitmap.GetPixel(x, y-1).A == 0 || newbitmap.GetPixel(x, y+1).A == 0)
                                    {
                                        pointList.Add(new Point(x, y));
                                    }
                                }
                            }

                        }
                        //A bit of a roundabout way of determining the largest and smallest XY-coordinates, but for some reason this
                        //approach yielded the best results.
                        int Xmin = 2000;
                        int Xmax = 0;
                        int Ymin = 2000;
                        int Ymax = 0;
                        for(var k = 0; k < pointList.Count; k++)
                        {   if(pointList[k].X < Xmin)
                            {
                                Xmin = pointList[k].X;
                            }
                            if(pointList[k].X > Xmax)
                            {
                               Xmax = pointList[k].X; 
                            }

                            if(pointList[k].Y < Ymin)
                            {
                               Ymin = pointList[k].Y; 
                            }

                            if(pointList[k].Y > Ymax)
                            {
                               Ymax = pointList[k].Y; 
                            }
                            
                            using (Graphics g = Graphics.FromImage(newbitmap))
                            {
                                //Drawing a '*' character for each point returned better visual results than drawig a line
                                //from (x1, y1) --> (x2, y2).
                                g.DrawString("*", new Font("Arial", 4), new SolidBrush(Color.White) ,Convert.ToSingle(pointList[k].X), Convert.ToSingle(pointList[k].Y), new StringFormat());
                            }   
                        }
                        //Finds the average XY-coordinates of the segment for us to draw the tile number at.
                        var averageX = Convert.ToSingle((Xmax+Xmin)/2);
                        var averageY = Convert.ToSingle((Ymax+Ymin)/2);
                        using (Graphics g = Graphics.FromImage(newbitmap))
                        {
                            g.DrawString($"{i}", new Font("Arial", 14), new SolidBrush(Color.Red), averageX, averageY, new StringFormat());
                        }
                        //Draws the edited segment onto the propser image template.
                        System.Drawing.Rectangle r = new Rectangle(0, 0, Convert.ToInt32(newbitmap.Width), Convert.ToInt32(newbitmap.Height));
                        using (Graphics g = Graphics.FromImage(bmap))
                        {
                            g.DrawImage(newbitmap, r);
                        }
                        //Draws the unedited segment onto the full image template.
                        using (Graphics g = Graphics.FromImage(bmapfull))
                        {
                            g.DrawImage(newbitmapfull, r);
                        }  
                    }
                }
                Directory.CreateDirectory($"../GuessingGame/wwwroot/img/{image.ImageMap}_full");
                bmap.Save($"../GuessingGame/wwwroot/img/{image.ImageMap}_full/proposer.png", ImageFormat.Png);
                bmapfull.Save($"../GuessingGame/wwwroot/img/{image.ImageMap}_full/full.png", ImageFormat.Png);
            }
            
            return await Task.FromResult(0); 
        }
        //Method for finding a the tile that corresponds to the given XY-coordinates.
        public string FindTile(int x, int y, string map, int Height, int Width)
        {
            double RelativeX = 1.0;
            double RelativeY = 1.0;
            string tilenotfound = "Tile not found";
            Bitmap bmp = new Bitmap($"../GuessingGame/wwwroot/img/{map}_full/full.png");
            double BWidth = bmp.Width;
            double BHeight = bmp.Height;
            if(Height > 0 && Width > 0)
            {
                RelativeX = BWidth/Width;
                RelativeY = BHeight/Height;   
            }
            var imgTiles = Directory.GetFiles($"../GuessingGame/wwwroot/img/{map}_scattered");
            for(int i = 0; i < imgTiles.Length; i++)
            {
                //Checks if the file exists before attempting to access it.
                if(File.Exists($"../GuessingGame/wwwroot/img/{map}_scattered/{i}.png"))
                {
                    var tempbmp = new Bitmap($"../GuessingGame/wwwroot/img/{map}_scattered/{i}.png");
                    //Checking if the pixel at the given XY-coordinates isn't transparent.
                    if(tempbmp.GetPixel((int)Math.Floor(x * RelativeX), (int)Math.Floor(y * RelativeY)).A != 0)
                    {
                        string tile = $"{i}.png";
                        return tile;
                    }   
                }
            }
            return tilenotfound; //no tiles found at the given coordinates.
        }
    }
}