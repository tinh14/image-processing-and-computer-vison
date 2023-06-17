using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace LamThienTinh
{
    public partial class Histogram : Form
    {
        Form1 parent;
        Action action;
        
        // Form được sử dụng chung cho nhiều chức năng
        // parent: form chính
        // action: hành động của người dùng
        public Histogram(Form1 parent, Action action)
        {
            InitializeComponent();

            this.parent = parent;
            this.action = action;

            checkAction();

            trackBar_MouseUp(null, null);
            trackBar_Scroll(null, null);
        }

        // Kiểm tra hành động, hiển thị các thông tin tương ứng
        private void checkAction()
        {
            switch (this.action)
            {
                case Action.SLIDING:
                    renderHistogramSliding();
                    break;
                case Action.STRETCHING:
                    renderHistogramStretching();
                    break;
                case Action.THRESHOLDING:
                    renderHistogramThresholding();
                    break;
                case Action.EQUALIZATION:
                    renderHistogramEqualization();
                    break;
                default:
                    renderDefaultHistogram();
                    break;
            }
        }

        // Hiển thị giao diện mặc định
        private void renderDefaultHistogram()
        {
            trackBar.Visible = false;
            valueLb.Visible = false;
        }

        // Hiển thị giao diện trượt tổ chức đồ
        private void renderHistogramSliding()
        {
            this.Text += " Sliding";
            trackBar.Minimum = -50;
            trackBar.Maximum = 50;
            trackBar.Value = 0;
            trackBar_Scroll(null, null);
        }

        // Hiển thị giao diện căng tổ chức đồ
        private void renderHistogramStretching()
        {
            this.Text += " Stretching";
            trackBar.Minimum = 1;
            trackBar.Maximum = 20;
            trackBar.Value = 10;
            trackBar_Scroll(null, null);
        }

        // Hiển thị giao diện phân ngưỡng tổ chức đồ
        private void renderHistogramThresholding()
        {
            this.Text += " Thresholding";
            trackBar.Minimum = 0;
            trackBar.Maximum = 255;
            trackBar.Value = 128;
            trackBar_Scroll(null, null);
        }

        // Hiển thị giao diện cân bằng tổ chức đồ
        private void renderHistogramEqualization()
        {
            this.Text += " Equalization";
            trackBar.Visible = false;
            valueLb.Visible = false;
        }

        // Hiển thị tổ chức đồ
        // image: Ảnh đầu vào
        private void renderHistogram(Image image)
        {
            
            bool isGreyscaleImage = ImageHandler.isGreyscaleImage(image);

            Bitmap[] bms;

            if (isGreyscaleImage)
            {
                bms = ImageHandler.getHistogramFromGreyscaleImage(image);
                renderHistogramFromGreyscaleImage(bms);
            }
            else
            {
                bms = ImageHandler.getHistogramFromColorImage(image);
                renderHistogramFromColorImage(bms);
            }
        }
           
        // Hiển thị tổ chức đồ cho ảnh xám
        private void renderHistogramFromGreyscaleImage(Bitmap[] bitmaps)
        {
            pictureBox2.Image = bitmaps[0];
        }

        // Hiển thị tổ chức đồ cho ảnh màu
        private void renderHistogramFromColorImage(Bitmap[] bitmaps)
        {
            pictureBox1.Image = bitmaps[0];
            pictureBox2.Image = bitmaps[1];
            pictureBox3.Image = bitmaps[2];
        }

        // Sự kiện mouse up
        private void trackBar_MouseUp(object sender, MouseEventArgs e)
        {
            // Lấy giá trị từ trackbar
            int value = trackBar.Value;

            // Lấy ảnh nguồn từ form chính
            Image image = parent.getSrcImage();

            Bitmap bitmap;

            switch (action)
            {
                case Action.SLIDING:
                    bitmap = ImageHandler.adjustBrightness(image, value);
                    break;
                case Action.STRETCHING:
                    bitmap = ImageHandler.adjustContrast(image, (float)value / 10);
                    break;
                case Action.THRESHOLDING:
                    bitmap = ImageHandler.getBinaryImage(image, value);
                    break;
                case Action.EQUALIZATION:
                    bitmap = ImageHandler.improveContrast(image);
                    break;
                default:
                    bitmap = new Bitmap(image);
                    break;
            }

            // Set ảnh mới vào ảnh đích
            parent.setDesImage(bitmap);

            // Lấy lại ảnh đích
            image = parent.getDesImage();

            // Hiển thị histogram cho ảnh đích
            renderHistogram(image);
        }

        // Sự kiện scroll
        // Đáng lẽ các lệnh trong sự kiện mouse up sẽ được viết ở đây
        // nhưng do điều kiện máy yếu nên mới tách ra
        private void trackBar_Scroll(object sender, EventArgs e)
        {
            // Hiển thị giá trị của trackbar
            float value = trackBar.Value;

            if (action == Action.STRETCHING)
            {
                value /= 10;
            }

            valueLb.Text = value.ToString();
        }
    }
}
