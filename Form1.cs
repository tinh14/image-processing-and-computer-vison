using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing.Imaging;

namespace LamThienTinh
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Lấy ảnh nguồn
        public Image getSrcImage()
        {
            return srcPictureBox.Image;
        }

        // Lấy ảnh đích
        public Image getDesImage()
        {
            return desPictureBox.Image;
        }

        // Set ảnh nguồn
        public void setSrcImage(Bitmap bitmap)
        {
            srcPictureBox.Image = bitmap;
        }

        // Set ảnh đích
        public void setDesImage(Bitmap bitmap)
        {
            desPictureBox.Image = bitmap;
        }

        // Sự kiện mở file ảnh
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult res = dialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                srcPictureBox.Image = new Bitmap(dialog.FileName);
                desPictureBox.Image = null;
            }
        }

        // Sự kiện lưu ảnh đích ra file
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const string TITLE = "Save Image";
            const string INITIAL_DIRECTORY = "D:\\";
            const string DEFAULT_EXTENSION = "jpg";
            const string FILTER = "image file (*.BMP, *.JPG, *.JPEG | *.bmp, *.jpg, *.jpeg)";
            const bool OVERWRITE_PROMPT = true;

            SaveFileDialog dialog = new SaveFileDialog();
            dialog.Title = TITLE;
            dialog.InitialDirectory = INITIAL_DIRECTORY;
            dialog.DefaultExt = DEFAULT_EXTENSION;
            dialog.Filter = FILTER;
            dialog.OverwritePrompt = OVERWRITE_PROMPT;

            DialogResult result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                desPictureBox.Image.Save(dialog.FileName);
            }
        }

        // Sự kiện thoát
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // Sự kiện lấy ảnh xám theo phương pháp get set pixel
        private void setGetPixcelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesImage(ImageHandler.getGreyscaleImageWithGetSetPixcelMethod(srcPictureBox.Image));
        }

        // Sự kiện lấy ảnh xám theo phương pháp lockbit
        private void lockbitToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            setDesImage(ImageHandler.getGreyscaleImageWithLockbitMethod(srcPictureBox.Image));
        }

        // Sự kiện lấy ảnh âm bản
        private void negativeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesImage(ImageHandler.getNegativeImage(srcPictureBox.Image));
        }

        // Sự kiện lấy ảnh nhị phân
        private void binaryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int DEFAULT_VALUE = ImageHandler.GREYSCALE_RANGE / 2;
            setDesImage(ImageHandler.getBinaryImage(srcPictureBox.Image, DEFAULT_VALUE));
        }

        // Sự kiện lấy tổ chức đồ
        private void defaultHistogramToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Action action = Action.HISTOGRAM;

            Histogram form = new Histogram(this, action);
            form.ShowDialog();
        }

        // Sự kiện trượt tổ chức đồ
        private void slidingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Action action = Action.SLIDING;

            Histogram form = new Histogram(this, action);
            form.ShowDialog();
        }

        // Sự kiện căng tổ chức đồ
        private void stretchingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Action action = Action.STRETCHING;

            Histogram form = new Histogram(this, action);
            form.ShowDialog();
        }

        // Sự kiện phân ngưỡng tổ chức đồ
        private void thresholdingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Action action = Action.THRESHOLDING;

            Histogram form = new Histogram(this, action);
            form.ShowDialog();
        }

        // Sự kiện cân bằng tổ chức đồ
        private void equalizationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Action action = Action.EQUALIZATION;

            Histogram form = new Histogram(this, action);
            form.ShowDialog();
        }

        // Sự kiện tăng kích thước ảnh
        private void zoomInToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int percentage = 10;
            Image image = (getDesImage() == null) ? getSrcImage() : getDesImage();
            setDesImage(ImageHandler.getResizedImage(image, percentage));
        }

        // Sự kiện giảm kích thước ảnh
        private void zoomOutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            const int percentage = -10;
            Image image = (getDesImage() == null) ? getSrcImage() : getDesImage();
            setDesImage(ImageHandler.getResizedImage(image, percentage));
        }

        // Sự kiện xoay trái ảnh
        private void leftToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool rotateLeft = true;
            Image image = (getDesImage() == null) ? getSrcImage() : getDesImage();
            setDesImage(ImageHandler.getRotatedImage(image, rotateLeft));
        }

        // Sự kiện xoay phải ảnh
        private void rightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool rotateLeft = false;
            Image image = (getDesImage() == null) ? getSrcImage() : getDesImage();
            setDesImage(ImageHandler.getRotatedImage(image, rotateLeft));
        }

        // Sự kiện cắt ảnh 3x4
        private void x4ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool is3x4CmImage = true;
            setDesImage(ImageHandler.getCroppedImage(getSrcImage(), is3x4CmImage));
        }

        // Sự kiện cắt ảnh 4x6
        private void x6ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool is3x4CmImage = false;
            setDesImage(ImageHandler.getCroppedImage(getSrcImage(), is3x4CmImage));
        }

        // Sự kiện cộng 2 ảnh
        private void mergeImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image image2;
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult res = dialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                image2 = new Bitmap(dialog.FileName);
                setDesImage(ImageHandler.combineTwoImages(getSrcImage(), image2));
            }
        }

        // Sự kiện trừ 2 ảnh
        private void splitImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Image image2;
            OpenFileDialog dialog = new OpenFileDialog();
            DialogResult res = dialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                image2 = new Bitmap(dialog.FileName);
                setDesImage(ImageHandler.splitTwoImages(getSrcImage(), image2));
            }
        }

        // Sự kiện phát hiện đường biên
        private void edgeDetectionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            setDesImage(ImageHandler.edgeDetection(getSrcImage()));
        }


    }
}
