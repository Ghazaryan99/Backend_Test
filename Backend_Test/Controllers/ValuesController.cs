using Backend_Test.Data;
using Backend_Test.Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Backend_Test.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        private readonly AppDbContext dbContext;

        public ValuesController(AppDbContext context)
        {
            dbContext = context;
        }

        [HttpGet]
        public string Get(string size, string BlurEffect, string GrayscaleEffect, string effect3, string radius)
        {
            Image image;
            List<Images> images = dbContext.images.Where(m => m.Changed == false).ToList();
            IFormFile postFile = Base64ToImage(images.Last());
            using (var memoryStream = new MemoryStream())
            {
                {
                    postFile.CopyToAsync(memoryStream);
                    using (var img = Image.FromStream(memoryStream))
                    {
                        if (size != null)
                            image = resizeImage(img, new Size(Convert.ToInt32(size), Convert.ToInt32(size)));
                        else
                            image = img;
                    }
                    Bitmap bitmap = new Bitmap(image);
                    if (BlurEffect != null)
                    {
                        bitmap = new Bitmap(image);
                        bitmap = Blur(bitmap, 10);
                    }
                    if (GrayscaleEffect != null)
                    {
                        bitmap = new Bitmap(image);
                        image = convertToGrayscale(bitmap);
                    }
                    if (radius != null)
                    {
                        bitmap = new Bitmap(image);
                        image = RoundCorners(bitmap, Convert.ToInt32(radius));
                    }
                }
            }
            return ImageToBase64(image);
        }

        [HttpPost]
        public async Task<string> Post()
        {
            var file = HttpContext.Request.Form.Files.Count > 0 ? HttpContext.Request.Form.Files[0] : null;
            var x = UploadedFile(file);
            Images image = new Images()
            {
                id = "1",
                Image = x,
                Changed = false,
                FileName = file.FileName,
                Name = file.Name
            };
            await dbContext.images.AddAsync(image);
            await dbContext.SaveChangesAsync();
            return "succsesss";
        }

        private static string ImageToBase64(Image image)
        {
            var imageStream = new MemoryStream();
            try
            {
                image.Save(imageStream, System.Drawing.Imaging.ImageFormat.Bmp);
                imageStream.Position = 0;
                var imageBytes = imageStream.ToArray();
                var ImageBase64 = Convert.ToBase64String(imageBytes);
                return ImageBase64;
            }
            catch (Exception ex)
            {
                return "Error converting image to base64!";
            }
        }

        private string UploadedFile(IFormFile Picture)
        {
            var file = Picture;
            if (file.Length > 0)
            {
                using (var ms = new MemoryStream())
                {
                    file.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    string s = Convert.ToBase64String(fileBytes);
                    return s;
                }
            }
            return null;
        }

        private IFormFile Base64ToImage(Images equipmentFiles)
        {
            byte[] bytes = Convert.FromBase64String(equipmentFiles.Image);
            MemoryStream stream = new MemoryStream(bytes);
            IFormFile file = new FormFile(stream, 0, bytes.Length, equipmentFiles.Name, equipmentFiles.FileName);
            return file;
        }

        public static Image resizeImage(Image imgToResize, Size size)
        {
            return (Image)(new Bitmap(imgToResize, size));
        }

        private static Bitmap Blur(Bitmap image, Int32 blurSize)
        {
            return Blur(image, new Rectangle(0, 0, image.Width, image.Height), blurSize);
        }

        private static Bitmap Blur(Bitmap image, Rectangle rectangle, Int32 blurSize)
        {
            Bitmap blurred = new Bitmap(image.Width, image.Height);

            using (Graphics graphics = Graphics.FromImage(blurred))
                graphics.DrawImage(image, new Rectangle(0, 0, image.Width, image.Height),
                    new Rectangle(0, 0, image.Width, image.Height), GraphicsUnit.Pixel);

            for (int xx = rectangle.X; xx < rectangle.X + rectangle.Width; xx++)
            {
                for (int yy = rectangle.Y; yy < rectangle.Y + rectangle.Height; yy++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    for (int x = xx; (x < xx + blurSize && x < image.Width); x++)
                    {
                        for (int y = yy; (y < yy + blurSize && y < image.Height); y++)
                        {
                            Color pixel = blurred.GetPixel(x, y);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }
                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    for (int x = xx; x < xx + blurSize && x < image.Width && x < rectangle.Width; x++)
                        for (int y = yy; y < yy + blurSize && y < image.Height && y < rectangle.Height; y++)
                            blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }
            return blurred;
        }

        private static Bitmap convertToGrayscale(Bitmap image)
        {
            Bitmap newImage = new Bitmap(image.Width, image.Height);
            for (int i = 0; i < image.Width; i++)
            {
                for (int x = 0; x < image.Height; x++)
                {
                    Color oc = image.GetPixel(i, x);
                    int grayScale = (int)((oc.R * 0.3) + (oc.G * 0.59) + (oc.B * 0.11));
                    Color nc = Color.FromArgb(oc.A, grayScale, grayScale, grayScale);
                    newImage.SetPixel(i, x, nc);
                }
            }
            return newImage;
        }

        public static Bitmap RoundCorners(Bitmap StartImage, int CornerRadius)
        {
            CornerRadius *= 2;
            Bitmap RoundedImage = new Bitmap(StartImage.Width, StartImage.Height);
            using (Graphics g = Graphics.FromImage(RoundedImage))
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                Brush brush = new TextureBrush(StartImage);
                GraphicsPath gp = new GraphicsPath();
                gp.AddArc(0, 0, CornerRadius, CornerRadius, 180, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0, CornerRadius, CornerRadius, 270, 90);
                gp.AddArc(0 + RoundedImage.Width - CornerRadius, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 0, 90);
                gp.AddArc(0, 0 + RoundedImage.Height - CornerRadius, CornerRadius, CornerRadius, 90, 90);
                g.FillPath(brush, gp);
                return RoundedImage;
            }
        }
    }
}
