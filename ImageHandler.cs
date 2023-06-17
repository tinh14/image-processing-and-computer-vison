using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace LamThienTinh
{
    public class ImageHandler
    {
        // Các hằng số trong lớp
        public const float RED_CONST = 0.3f;
        public const float GREEN_CONST = 0.59f;
        public const float BLUE_CONST = 0.11f;
        public const int GREYSCALE_RANGE = 256;
        public const int SMALLEST_NUMBER = int.MinValue;
        public const float PIXEL_TO_CM = 118.11f;

        // Lấy ảnh xám với phương pháp get set pixel
        // image: Ảnh đầu vào
        public static Bitmap getGreyscaleImageWithGetSetPixcelMethod(Image image)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            // Đi qua các điểm ảnh, lấy giá trị mới từ công thức bên dưới
            // Các hằng số đã được các nhà nghiên cứu đưa ra, nên cứ áp dụng hoy
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    int g = (int)((color.R * RED_CONST) + (color.G * GREEN_CONST) + (color.B * BLUE_CONST));
                    bitmap.SetPixel(i, j, Color.FromArgb(g, g, g));
                }
            }

            return bitmap;
        }

        // Lấy ảnh xám với phương pháp lockbit
        // Không hiểu thuật toán này nên không giải thích được :((
        public static Bitmap getGreyscaleImageWithLockbitMethod(Image image)
        {
            Bitmap bitmap = new Bitmap(image);
            Rectangle rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            BitmapData bitmapData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
            IntPtr intPtr = bitmapData.Scan0;
            int length = bitmapData.Stride * bitmap.Height;
            byte[] values = new byte[length];
            Marshal.Copy(intPtr, values, 0, length);
            int index = 0;
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    byte g = Convert.ToByte(values[index + 2] * RED_CONST + values[index + 1] * GREEN_CONST + values[index] + BLUE_CONST);
                    values[index] = values[index + 1] = values[index + 2] = g;
                    index += 3;
                }
            }
            return bitmap;
        }

        // Lấy ảnh âm bản
        // image: Ảnh đầu vào
        public static Bitmap getNegativeImage(Image image)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            // Đi qua các điểm ảnh, lấy khoảng giá trị ngược lại với giá trị hiện tại
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    
                    int redVal = GREYSCALE_RANGE - 1 - color.R;
                    int greenVal = GREYSCALE_RANGE - 1 - color.G;
                    int blueVal = GREYSCALE_RANGE - 1 - color.B;

                    bitmap.SetPixel(i, j, Color.FromArgb(redVal, greenVal, blueVal));
                }
            }
            return bitmap;
        }

        // Kiểm tra ảnh đầu vào là ảnh xám hay ảnh màu
        // Trả về true: ảnh xám, false: ảnh màu
        // image: Ảnh cần kiểm tra
        public static bool isGreyscaleImage(Image image)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            // Đi qua các điểm ảnh, nếu giá trị R G B của mỗi điểm ảnh khác nhau thì là ảnh màu
            // Ngược lại là ảnh xám
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    if ((color.R != color.G) || (color.R != color.B) || (color.G != color.B))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        // Lấy phần từ lớn nhất trong mảng
        // h[]: Mảng đầu vào
        private static int getMax(int[] h)
        {
            int mx = h[0];
            for (int i = 1; i < h.Length; i++)
            {
                if (h[i] > mx)
                {
                    mx = h[i];
                }
            }
            return mx;
        }

        // Lấy tổ chức đồ từ ảnh xám
        // image: Ảnh cần lấy tổ chức đồ
        public static Bitmap[] getHistogramFromGreyscaleImage(Image image)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            // Tạo mảng đại diện cho histogram
            int[] h = new int[GREYSCALE_RANGE];

            // Đi qua các điểm ảnh, lưu số lượng điểm ảnh tại mỗi mức xám
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    h[bitmap.GetPixel(i, j).R]++;
                }
            }

            // Tìm số số lượng ảnh lớn nhất có chung một mức xám
            int mx = getMax(h);

            bitmap = new Bitmap(GREYSCALE_RANGE, mx);

            // Tạo đối tượng graphic để vẽ histogram
            Graphics g = Graphics.FromImage(bitmap);

            // Vẽ khung hình chữ nhật với chữ trắng, chiều dài 256 và chiều rộng dựa trên số điểm ảnh lớn nhất
            g.FillRectangle(Pens.White.Brush, new Rectangle(0, 0, GREYSCALE_RANGE, mx));

            // Bắt đầu vẽ
            for (int i = 0; i < GREYSCALE_RANGE; i++)
            {
                g.DrawLine(Pens.Gray, new Point(i, mx), new Point(i, mx - h[i]));
            }

            return new Bitmap[]{bitmap};
        }

        // Lấy 3 tổ chức đồ với 3 màu riêng biệt từ ảnh màu
        // image: Ảnh cần lấy tổ chức đồ
        public static Bitmap[] getHistogramFromColorImage(Image image)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            // Tạo 3 mảng đại diện cho 3 histogram
            int[] hR = new int[GREYSCALE_RANGE];
            int[] hG = new int[GREYSCALE_RANGE];
            int[] hB = new int[GREYSCALE_RANGE];

            // Đi qua các điểm ảnh, lưu số lượng điểm ảnh tại mỗi mức xám
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    hR[bitmap.GetPixel(i, j).R]++;
                    hG[bitmap.GetPixel(i, j).G]++;
                    hB[bitmap.GetPixel(i, j).B]++;
                }
            }

            // Tìm số số lượng ảnh lớn nhất có chung một mức xám
            int mxR = getMax(hR);
            int mxG = getMax(hG);
            int mxB = getMax(hB);

            // Tạo 3 đối tượng bitmap, đại diện cho 3 màu riêng biệt
            Bitmap bitmapR = new Bitmap(GREYSCALE_RANGE, mxR);
            Bitmap bitmapG = new Bitmap(GREYSCALE_RANGE, mxG);
            Bitmap bitmapB = new Bitmap(GREYSCALE_RANGE, mxB);

            // Tạo đối tượng graphic để vẽ histogram
            Graphics gR = Graphics.FromImage(bitmapR);
            Graphics gG = Graphics.FromImage(bitmapG);
            Graphics gB = Graphics.FromImage(bitmapB);

            // Vẽ khung hình chữ nhật với chữ trắng, chiều dài 256 và chiều rộng dựa trên số điểm ảnh lớn nhất
            gR.FillRectangle(Pens.White.Brush, new Rectangle(0, 0, GREYSCALE_RANGE, mxR));
            gG.FillRectangle(Pens.White.Brush, new Rectangle(0, 0, GREYSCALE_RANGE, mxG));
            gB.FillRectangle(Pens.White.Brush, new Rectangle(0, 0, GREYSCALE_RANGE, mxB));

            // Bắt đầu vẽ
            for (int i = 0; i < GREYSCALE_RANGE; i++)
            {
                gR.DrawLine(Pens.Red, new Point(i, mxR), new Point(i, mxR - hR[i]));
                gG.DrawLine(Pens.Green, new Point(i, mxG), new Point(i, mxG - hG[i]));
                gB.DrawLine(Pens.Blue, new Point(i, mxB), new Point(i, mxB - hB[i]));
            }

            return new Bitmap[]{bitmapR, bitmapG, bitmapB};
        }

        // Lấy ảnh mới với giá trị đầu vào
        // Image: Ảnh cần chỉnh độ sáng
        // n: Từ -50 đến 50
        public static Bitmap adjustBrightness(Image image, int n)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            // Lấy chiều dài, rộng của ảnh
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Đi qua các điểm ảnh cộng với giá trị n
            // Nếu giá trị độ sáng mới dưới mức 0 thì mặc định là 0, trên mức 255 thì mặc định là 255
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    int rVal = color.R + n;
                    int gVal = color.G + n;
                    int bVal = color.B + n;

                    rVal = (rVal < 0) ? 0 : rVal;
                    rVal = (rVal > GREYSCALE_RANGE - 1) ? GREYSCALE_RANGE - 1 : rVal;

                    gVal = (gVal < 0) ? 0 : gVal;
                    gVal = (gVal > GREYSCALE_RANGE - 1) ? GREYSCALE_RANGE - 1 : gVal;

                    bVal = (bVal < 0) ? 0 : bVal;
                    bVal = (bVal > GREYSCALE_RANGE - 1) ? GREYSCALE_RANGE - 1 : bVal;

                    bitmap.SetPixel(i, j, Color.FromArgb(rVal, gVal, bVal));
                }
            }
            return bitmap;
        }

        // Lấy ảnh mới với giá trị đầu vào
        // Image: Ảnh cần chỉnh độ tương phản
        // n: Từ 0.1 đến 2.0
        public static Bitmap adjustContrast(Image image, float n)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            // Lấy chiều dài, rộng của ảnh
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Đi qua các điểm ảnh nhân với giá trị n
            // Nếu giá trị độ sáng mới dưới mức 0 thì mặc định là 0, trên mức 255 thì mặc định là 255
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    int rVal = (int)((float)color.R * n);
                    int gVal = (int)((float)color.G * n);
                    int bVal = (int)((float)color.B * n);

                    rVal = (rVal < 0) ? 0 : rVal;
                    rVal = (rVal > GREYSCALE_RANGE - 1) ? GREYSCALE_RANGE - 1 : rVal;

                    gVal = (gVal < 0) ? 0 : gVal;
                    gVal = (gVal > GREYSCALE_RANGE - 1) ? GREYSCALE_RANGE - 1 : gVal;

                    bVal = (bVal < 0) ? 0 : bVal;
                    bVal = (bVal > GREYSCALE_RANGE - 1) ? GREYSCALE_RANGE - 1 : bVal;

                    bitmap.SetPixel(i, j, Color.FromArgb(rVal, gVal, bVal));
                }
            }

            return bitmap;
        }

        // Lấy ảnh nhị phân
        // Image: Ảnh đầu vào
        // n: mức phân ngưỡng từ 0 đến 255
        public static Bitmap getBinaryImage(Image image, int n)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            // Lấy chiều dài, rộng của ảnh
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Đi qua các điểm ảnh nếu giá trị độ sáng mới < mức n thì mặc định là 0, >= mức n thì mặc định là 255
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    Color color = bitmap.GetPixel(i, j);
                    int val = (color.R < n) ? 0 : GREYSCALE_RANGE - 1;
                    bitmap.SetPixel(i, j, Color.FromArgb(val, val, val));
                }
            }
            return bitmap;
        }

        // Cải thiện độ tương phản
        // image: Ảnh đầu vào
        public static Bitmap improveContrast(Image image)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            int width = bitmap.Width;
            int height = bitmap.Height;

            // Lấy số điểm ảnh
            int numPix = width * height;

            // Tạo histogram
            int[] h = new int[GREYSCALE_RANGE];

            // Đi qua các điểm ảnh, lưu số lượng điểm ảnh tại mỗi mức xám
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    h[bitmap.GetPixel(i, j).R]++;
                }
            }

            // Tạo hàm mật độ xác suất cho mỗi điểm ảnh
            int[] c = new int[GREYSCALE_RANGE];
            
            c[0] = h[0];
            
            // Thuật toán tính tổng tính lũy
            // Ta gọi c[i] là tổng từ h[0] tới h[i]
            // Thay vì c[3] = h[0] + h[1] ... + h[3] thì c[3] = c[2] + h[3]
            for (int i = 1; i < GREYSCALE_RANGE; i++)
            {
                c[i] = c[i - 1] + h[i];
            }

            // Chuẩn hóa histogram hn(x) = h(x) / n
            for (int i = 0; i < GREYSCALE_RANGE; i++)
            {
                h[i] = (int)((float)(GREYSCALE_RANGE - 1) * c[i] / (numPix));
            }

            // Cập nhật lại giá trị cho các điểm ảnh
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    int g = h[bitmap.GetPixel(i, j).R];
                    bitmap.SetPixel(i, j, Color.FromArgb(g, g, g));
                }
            }

            return bitmap;
        }

        // Lấy ảnh đã được resize
        // image: Ảnh cần resize
        // percentage: phần trăm tỷ lệ ảnh được resize
        public static Bitmap getResizedImage(Image image, int percentage)
        {
            // Tạo object bitmap
            Bitmap bitmap = new Bitmap(image);

            // Lấy chiều dài, rộng của ảnh
            int width = bitmap.Width;
            int height = bitmap.Height;

            // Chiều dài, rộng mới của ảnh
            int newWidth;
            int newHeight;

            // Nếu percentage > 0 thì tăng size ảnh, ngược lại thì giảm size ảnh
            if (percentage > 0)
            {
                newWidth = width + (int)(width * (float)percentage / 100);
                newHeight = height + (int)(height * (float)percentage / 100);
                
            }else 
            {
                percentage = Math.Abs(percentage);
                newWidth = width - (int)(width * (float)percentage / 100);
                newHeight = height - (int)(height * (float)percentage / 100);
            }

            return new Bitmap(bitmap, newWidth, newHeight);
        }

        // Lấy ảnh đã được xoay
        // image: Ảnh cần xoay
        // rotateLeft: true: thao tác xoay trái, false: thao tác xoay phải
        public static Bitmap getRotatedImage(Image image, bool rotateLeft)
        {
            if (rotateLeft)
            {
                image.RotateFlip(RotateFlipType.Rotate270FlipNone);
            }
            else
            {
                image.RotateFlip(RotateFlipType.Rotate90FlipNone);
            }
            return new Bitmap(image);
        }

        // Cắt ảnh
        // image: Ảnh đầu vào
        // is3x4CmImage: ảnh đầu vào là ảnh 3x4
        public static Bitmap getCroppedImage(Image image, bool is3x4CmImage)
        {
            Bitmap bitmap = new Bitmap(image);
            
            int width = bitmap.Width;
            int height = bitmap.Height;

            int newWidth;
            int newHeight;

            if (is3x4CmImage)
            {
                newWidth = (int)(3 * PIXEL_TO_CM);
                newHeight = (int)(4 * PIXEL_TO_CM);
            }
            else
            {
                newWidth = (int)(4 * PIXEL_TO_CM);
                newHeight = (int)(6 * PIXEL_TO_CM);
            }
            return new Bitmap(bitmap, newWidth, newHeight);
        }

        // Cộng 2 ảnh có cùng size
        // image1: Ảnh 1
        // image2: Ảnh 2
        public static Bitmap combineTwoImages(Image image1, Image image2)
        {
            Bitmap bitmap1 = new Bitmap(image1);
            Bitmap bitmap2 = new Bitmap(image2);

            Bitmap newBitmap = new Bitmap(bitmap1.Width, bitmap1.Height);

            for (int i = 0; i < newBitmap.Width; i++)
            {
                for (int j = 0; j < newBitmap.Height; j++)
                {
                    Color color1 = bitmap1.GetPixel(i, j);
                    Color color2 = bitmap2.GetPixel(i, j);
                    int rVal = Math.Min(color1.R + color2.R, GREYSCALE_RANGE - 1);
                    int gVal = Math.Min(color1.G + color2.G, GREYSCALE_RANGE - 1);
                    int bVal = Math.Min(color1.B + color2.B, GREYSCALE_RANGE - 1);
                    newBitmap.SetPixel(i, j, Color.FromArgb(rVal, gVal, bVal));
                }
            }

            return newBitmap;
        }

        // Từ 2 ảnh có cùng size
        // image1: Ảnh 1
        // image2: Ảnh 2
        public static Bitmap splitTwoImages(Image image1, Image image2)
        {
            Bitmap bitmap1 = new Bitmap(image1);
            Bitmap bitmap2 = new Bitmap(image2);

            Bitmap newBitmap = new Bitmap(bitmap1.Width, bitmap1.Height);

            for (int i = 0; i < newBitmap.Width; i++)
            {
                for (int j = 0; j < newBitmap.Height; j++)
                {
                    Color color1 = bitmap1.GetPixel(i, j);
                    Color color2 = bitmap2.GetPixel(i, j);
                    int rVal = Math.Max(color1.R - color2.R, 0);
                    int gVal = Math.Max(color1.G - color2.G, 0);
                    int bVal = Math.Max(color1.B - color2.B, 0);
                    newBitmap.SetPixel(i, j, Color.FromArgb(rVal, gVal, bVal));
                }
            }

            return newBitmap;
        }

        // Phát hiện đường biên
        // image: Ảnh cần detect
        public static Bitmap edgeDetection(Image image){
            
            // Lấy ảnh nhị phân với phân ngưỡng mặc định
            Bitmap bitmap = getBinaryImage(image, GREYSCALE_RANGE / 2);

            // Tạo đối tượng bitmap
            Bitmap edgeBitmap = new Bitmap(bitmap.Width, bitmap.Height);

            // Mặt nạ chập
            int[,] gx = new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } };
            int[,] gy = new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } };

            for (int i = 1; i < bitmap.Width - 1; i++)
            {
                for (int j = 1; j < bitmap.Height - 1; j++)
                {
                    int pixelX = 0, pixelY = 0;
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            pixelX += gx[x + 1, y + 1] * bitmap.GetPixel(i + x, j + y).R;
                            pixelY += gy[x + 1, y + 1] * bitmap.GetPixel(i + x, j + y).R;
                        }
                    }

                    int magnitude = (int)Math.Sqrt(pixelX * pixelX + pixelY * pixelY);
                    
                    magnitude = (magnitude > GREYSCALE_RANGE - 1) ? GREYSCALE_RANGE - 1 : magnitude;
                    magnitude = (magnitude < 0) ? 0 : magnitude;

                    edgeBitmap.SetPixel(i, j, Color.FromArgb(magnitude, magnitude, magnitude));
                }
            }

            return edgeBitmap;
        }
    }
}
