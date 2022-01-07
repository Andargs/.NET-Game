using System;
using System.IO;
using System.Collections.Generic;
using GuessingGame.Core.Domain.Image;
using System.Linq;
using System.Text;
using GuessingGame.Data;

namespace GuessingGame.Core.Domain.Image.Services
{
    public class FileReader
    {
        public List<Image> Images = new List<Image>();

        public static string ImageMappingPath = "./Content/ImageMapping/imageMapping.csv";
        
        public static string LabelMappingPath = "./Content/LabelMapping/labelMapping.csv";

        public static string ImgPath = "./wwwroot/img/";
        
        
        public List<string[]> IMcolumn {get;set;} =  new List<string[]>();
        
        public List<string[]> LMcolumn {get;set;} = new List<string[]>();

        public List<string> ScatterFolders = new List<string>();

        //Method for reading the files and directories concerning the images and populating the List of Image classes.
        public void ReadFile()
        {
            var dirs = new DirectoryInfo(ImgPath).GetDirectories();
            foreach(var dir in dirs)
            {
                var dirName = dir.Name.Split('_');
                if(dirName[0] == "Custom")
                {
                    continue;
                }
                if(dirName[3] == "scattered")
                {
                    var scattername = dirName[0]+"_"+dirName[1]+"_"+dirName[2];
                    ScatterFolders.Add(scattername);    
                }
            } 
            char[] whitespace = new char[] { ' ', '\t' };
            string[] IMlines = File.ReadAllLines(ImageMappingPath, Encoding.UTF8); // Reads the imageMapping.csv file
            foreach(string line in IMlines)
            {               
                IMcolumn.Add(line.Split(whitespace));
                                                        
            } 
            string[] LMlines = File.ReadAllLines(LabelMappingPath, Encoding.UTF8); // Reads the labelMapping.cvs file
            foreach(string line in LMlines)
            {   
                string ImageName = "";
                var split = line.Split(whitespace);
                // Check here to see if the image name contains more than one word and adding the complete image name.
                if(split.Length > 2)
                {
                    for(int i = 1; i < split.Length; i++)
                    {
                        if(i+1 == split.Length)
                        {
                            ImageName += $"{split[i]}";
                            break;
                        }
                        ImageName += $"{split[i]} ";
                    }
                    split[1] = ImageName;
                    LMcolumn.Add(split);
                } else {
                    LMcolumn.Add(line.Split(whitespace));
                }                                      
            }
            foreach(string scatter in ScatterFolders)
            {
                int id = Int32.Parse(IMcolumn.Find(item => item[0] == scatter)[1]);
                string name = LMcolumn.Find(item => Int32.Parse(item[0]) == id)[1];
                Images.Add(new Image{ImageId = id, ImageName = name, ImageMap = scatter});
            }
        }
    }
}