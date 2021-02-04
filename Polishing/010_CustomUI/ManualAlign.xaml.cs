using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserFunction;

namespace UserInterface
{
    /// <summary>
    /// Align.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class ManualAlign : UserControl
    {
        string Title = "ManualAlign";
        const double m_dResultFontSize = 100;

        double m_dScale = 1.0;

        SolidColorBrush m_brush             = new SolidColorBrush(Color.FromArgb(100, 200, 0, 0));
        SolidColorBrush m_brushSearch       = new SolidColorBrush(Color.FromArgb(100, 200, 200, 0));
        SolidColorBrush[] m_brushMil        = new SolidColorBrush[10];

        Label m_lbWidth = new Label();
        Label m_lbHeight = new Label();
        Label m_lbResolutionX = new Label();
        Label m_lbResolutionY = new Label();

        Rectangle m_manualRect = new Rectangle();
        Line line_X = new Line();
        Line line_Y = new Line();
        TextBlock tb_Count = new TextBlock();
        Rect m_rectManual = new Rect();

        ImageBrush m_ImgBrs;

        private double m_Theta = 360;
        public double dTheta { get { return m_Theta; } }

        private int m_nWidth;
        private int m_nHeight;
        private int m_nChannel;

        public int nWidth { get { return m_nWidth; } }
        public int nHeight { get { return m_nHeight; } }
        public int nChannel { get { return m_nChannel; } }

        private double m_dResolutionX = 1;
        private double m_dResolutionY = 1;
        public double dResolutionX
        {
            get { return m_dResolutionX; }
            set { m_dResolutionX = value; }
        }
        public double dResolutionY
        {
            get { return m_dResolutionY; }
            set { m_dResolutionY = value; }
        }

        int m_nClickCount = 0;

        public ManualAlign()
        {
            InitializeComponent();

            ComponentDispatcher.ThreadIdle += InitUI;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		void InitUI(object sender, EventArgs e)
        @brief	Align Control의 UI 초기화.
        @return	void
        @param	object      sender
        @param  EventArgs   e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:36
        */
        void InitUI(object sender, EventArgs e)
        {
            ComponentDispatcher.ThreadIdle -= InitUI;

            m_manualRect.Fill = new SolidColorBrush(Color.FromArgb(55,255,0,0));
            m_manualRect.StrokeThickness = 3;
            m_manualRect.Stroke = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            DoubleCollection dc1 = new DoubleCollection();
            dc1.Add(3 / m_dScale);
            dc1.Add(3 / m_dScale);
            m_manualRect.StrokeDashArray = dc1;
            m_manualRect.Visibility = Visibility.Visible;
            
            lib_Canvas.Children.Add(m_manualRect);
            lib_Canvas.Children.Add(line_X);
            lib_Canvas.Children.Add(line_Y);
            lib_Canvas.Children.Add(tb_Count);
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public double GetFitScale()
        @brief	Control의 화면에 이미지 크기 맞게 배율 계산.
        @return	
        @param	
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:39
        */
        public double GetFitScale()
        {
            double dScaleX = 1.0;
            double dScaleY = 1.0;
            try
            {
                if (m_nWidth > 0 && m_nHeight > 0)
                {
                    dScaleX = ctrl_Grid.ActualWidth / m_nWidth;
                    dScaleY = ctrl_Grid.ActualHeight / m_nHeight;
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + $"GetFitScale : {ex.Message}", UserEnum.EN_LOG_TYPE.ltVision);
            }
            return (dScaleY > dScaleX) ? dScaleX : dScaleY;
        }

        public void SetFitScale()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate ()
            {
                sl_Scale.Value = GetFitScale();
            }));
        }
        
        public void SetScale(double dScale)
        {
            sl_Scale.Value = dScale;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		internal void SetImage(WriteableBitmap wb, Stretch stretch = Stretch.None)
        @brief	WriteableBitmap Type을 Align Control에 Set.
        @return	void
        @param	WriteableBitmap wb      : Source Image.
        @param	Stretch         stretch : Stretch Option.
        @remark	
         - Stretch Option을 Fill로 할 경우, 배율이 맞지 않을 수 있음.
         - 배율 조정 + Fit을 원하면 GetFitScale을 사용하여 수동으로 화면 Scale 조정 할 것.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:40
        */
        internal void SetImage(WriteableBitmap wb, Stretch stretch = Stretch.None, bool bUpdateOrg = true)
        {
            m_ImgBrs = new ImageBrush(wb);
            m_ImgBrs.Stretch = stretch;
            if (m_ImgBrs.Stretch == Stretch.None)
            {
                lib_Canvas.Width = m_ImgBrs.ImageSource.Width;
                lib_Canvas.Height = m_ImgBrs.ImageSource.Height;
            }
            m_nWidth = (int)m_ImgBrs.ImageSource.Width;
            m_nHeight = (int)m_ImgBrs.ImageSource.Height;
            m_nChannel = 1;
            lib_Canvas.Background = m_ImgBrs;
            m_dScale = myScaleTransform.ScaleX;

            m_lbWidth.Content = m_nWidth.ToString("0.00");
            m_lbHeight.Content = m_nHeight.ToString("0.00");
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		public void SetImage(byte[] buff, int width, int height)
        @brief	Byte Array를 Align Control에 Set.
        @return	void
        @param	byte[] buff   : Source Image Byte Array.
        @param	int    width  : Image Width.
        @param	int    height : Image Height.
        @remark	
         - Byte Array를 Set 함.
         - Stretch None.
         - Thread 안전.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:40
        */
        public void SetImage(byte[] buff, int width, int height)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate ()
            {
                if (this.IsLoaded)
                {
                    WriteableBitmap wbm = new WriteableBitmap(width, height, 96, 96, PixelFormats.Gray8, null);
                    wbm.Lock();
                    wbm.WritePixels(new Int32Rect(0, 0, width, height), buff, width, 0);
                    wbm.AddDirtyRect(new Int32Rect(0, 0, width, height));
                    wbm.Unlock();
                    SetImage(wbm);
                    SetFitScale();
                }
            }));
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void SetImage(BitmapImage bmpimg, Stretch stretch = Stretch.None)
        @brief	WriteableBitmap Type을 Align Control에 Set.
        @return	void
        @param	BitmapImage     bmpimg  : Source Image.
        @param	Stretch         stretch : Stretch Option.
        @remark	
         - Stretch Option을 Fill로 할 경우, 배율이 맞지 않을 수 있음.
         - 배율 조정 + Fit을 원하면 GetFitScale을 사용하여 수동으로 화면 Scale 조정 할 것.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:40
        */
        public void SetImage(BitmapImage bmpimg, Stretch stretch = Stretch.None)
        {
            m_ImgBrs = new ImageBrush(bmpimg);
            m_ImgBrs.Stretch = stretch;

            if (m_ImgBrs.Stretch == Stretch.None)
            {
                lib_Canvas.Width = m_ImgBrs.ImageSource.Width;
                lib_Canvas.Height = m_ImgBrs.ImageSource.Height;
            }

            m_nWidth = (int)m_ImgBrs.ImageSource.Width;
            m_nHeight = (int)m_ImgBrs.ImageSource.Height;
            m_nChannel = 1;

            lib_Canvas.Background = new ImageBrush(bmpimg);
            m_dScale = myScaleTransform.ScaleX;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void SetImage(IntPtr ptr, double dWidth, double dHeight, double dChannel)
        @brief	IntPtr 이미지를 Contorl에 Set.
        @return	void
        @param	IntPtr ptr          : Source Image Integer Pointer
        @param	double dWidth       : Image Width
        @param	double dHeight      : Image Height
        @param	double dChannel     : Image Channel
        @remark	
         - Image Pointer를 Byte Array로 변환.
         - 변환된 ByteArray를 Writeable Bitmap으로 변환.
         - Writeable Bitmap을 Control에 Set.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:40
        */
        public void SetImage(IntPtr ptr, double dWidth, double dHeight, double dChannel)
        {
            int nImageLength = (int)(dWidth * dHeight * dChannel);
            byte[] imageData = new byte[nImageLength];
            Marshal.Copy(ptr, imageData, 0, nImageLength);
            WriteableBitmap wbm = new WriteableBitmap((int)dWidth, (int)dHeight, 96, 96, PixelFormats.Gray8, null);
            wbm.Lock();
            wbm.WritePixels(new Int32Rect(0, 0, (int)dWidth, (int)dHeight), imageData, (int)dWidth, 0);
            wbm.AddDirtyRect(new Int32Rect(0, 0, (int)dWidth, (int)dHeight));
            wbm.Unlock();
            SetImage(wbm);
        }

        
        //---------------------------------------------------------------------------
        /**	
        @fn		private void onMouseUp(object sender, MouseButtonEventArgs e)
        @brief	마우스 Up 이벤트.
        @return	void
        @param	object                  sender
        @param	MouseButtonEventArgs    e
        @remark	
         - Mouse Up 이벤트가 타지 않음.
         - 상위 이벤트에서 리턴되어 현재 컨트롤까지 도달 하지 않은 것 같음.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:47
        */
        private void onMouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pnt = e.GetPosition(lib_Canvas);

            switch(m_nClickCount)
            {
                case 0:
                    m_manualRect.Width = 0;
                    m_manualRect.Height = 0;
                    Canvas.SetLeft(m_manualRect, pnt.X);
                    Canvas.SetTop(m_manualRect, pnt.Y);
                    m_rectManual.X = pnt.X;
                    m_rectManual.Y = pnt.Y;
                    m_nClickCount++;
                    break;
                case 1:
                    m_manualRect.Width = Math.Abs(pnt.X - Canvas.GetLeft(m_manualRect));
                    m_rectManual.Width = m_manualRect.Width;
                    m_nClickCount++;
                    break;
                case 2:
                    m_manualRect.Height = Math.Abs(pnt.Y - Canvas.GetTop(m_manualRect));
                    m_rectManual.Height = m_manualRect.Height;
                    m_nClickCount = 0;
                    break;
            }
        }

        public void SetRectangle(Rect rect)
        {
            m_rectManual.X = rect.X;
            m_rectManual.Y = rect.Y;
            m_rectManual.Width = rect.Width;
            m_rectManual.Height = rect.Height;

            Canvas.SetLeft(m_manualRect, m_rectManual.X);
            Canvas.SetTop(m_manualRect, m_rectManual.Y);
            m_manualRect.Width = m_rectManual.Width;
            m_manualRect.Height = m_rectManual.Height;
        }

        public Rect GetRectangle()
        {
            return m_rectManual;
        }
        
        //---------------------------------------------------------------------------
        /**	
        @fn		public void fn_SaveImage(string strPath)
        @brief	Image Save.
        @return	void
        @param	string strPath : Image Path.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:34
        */
        public void fn_SaveImage(string strPath)
        {
            if (m_ImgBrs != null)
            {
                try
                {
                    WriteableBitmap bmp = m_ImgBrs.ImageSource as WriteableBitmap;

                    using (FileStream stream = new FileStream(strPath, FileMode.Create))
                    {
                        BmpBitmapEncoder encoder = new BmpBitmapEncoder();

                        encoder.Frames.Add(BitmapFrame.Create(bmp));
                        encoder.Save(stream);
                    }
                }
                catch (Exception ex)
                {
                    fn_WriteLog(this.Title + $"fn_SaveImage : {ex.Message}", UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
        }

        public void ClearCanvas()
        {
            if (lib_Canvas != null)
            {
                lib_Canvas.Background = Brushes.Transparent;
                m_ImgBrs = null;
            }
        }

        private void bn_Scale_Click(object sender, RoutedEventArgs e)
        {
            SetFitScale();
        }

        private void sl_Scale_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            Slider slider = sender as Slider;
            if (slider != null)
            {
                ZoomScale(slider.Value);
                lb_Scale.Content = slider.Value.ToString("x 0.000");
            }
        }

        private void ZoomScale(double dScale)
        {
            if (myScaleTransform != null)
            {
                double dCurPosRateX = (lib_ScrollViewer.HorizontalOffset + lib_ScrollViewer.ActualWidth  / 2) / lib_ScrollViewer.ExtentWidth;
                double dCurPosRateY = (lib_ScrollViewer.VerticalOffset   + lib_ScrollViewer.ActualHeight / 2) / lib_ScrollViewer.ExtentHeight;

                myScaleTransform.ScaleX = dScale;
                myScaleTransform.ScaleY = dScale;
                m_dScale = dScale;

                double dOffsetX = (nWidth  * dScale) * dCurPosRateX - lib_ScrollViewer.ActualWidth  / 2;
                double dOffsetY = (nHeight * dScale) * dCurPosRateY - lib_ScrollViewer.ActualHeight / 2;
                lib_ScrollViewer.ScrollToHorizontalOffset(dOffsetX);
                lib_ScrollViewer.ScrollToVerticalOffset(dOffsetY);
            }
        }

        private void onMouseMove(object sender, MouseEventArgs e)
        {
            Point pnt = e.GetPosition(lib_Canvas);
            try
            {
                m_fn_DrawCrossLine(pnt);
            }
            catch(Exception ex)
            {
                fn_WriteLog(this.Title + $"onMouseMove : {ex.Message}", UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        private void m_fn_DrawCrossLine(Point pnt)
        {
            line_X.Stroke = Brushes.Aqua;
            line_X.StrokeThickness = (1 / m_dScale);
            line_X.X1 = pnt.X;
            line_X.X2 = pnt.X;
            line_X.Y1 = 0;
            line_X.Y2 = lib_Canvas.ActualHeight;

            line_Y.Stroke = Brushes.Aqua;
            line_Y.StrokeThickness = (1 / m_dScale);
            line_Y.X1 = 0;
            line_Y.X2 = lib_Canvas.ActualWidth;
            line_Y.Y1 = pnt.Y;
            line_Y.Y2 = pnt.Y;

            switch(m_nClickCount)
            {
                case 0:
                    tb_Count.Text = $"기준점을 선택하세요.";
                    break;
                case 1:
                    tb_Count.Text = $"폭을 선택하세요.";
                    break;
                case 2:
                    tb_Count.Text = $"높이를 선택하세요.";
                    break;
            }
            tb_Count.FontSize = 20 / m_dScale;
            tb_Count.Foreground = Brushes.Aquamarine;
            Canvas.SetLeft(tb_Count, pnt.X);
            Canvas.SetTop(tb_Count, pnt.Y - tb_Count.FontSize - (1 / m_dScale));
        }

        public void OpenImage(string strPath)
        {
            if (!File.Exists(strPath))
            {
                m_ImgBrs = null;
                lib_Canvas.Background = Brushes.Transparent;
                return;
            }
            BitmapImage bmpImg = new BitmapImage();
            FileStream source = File.OpenRead(strPath);

            bmpImg.BeginInit();
            bmpImg.CacheOption = BitmapCacheOption.OnLoad;
            bmpImg.StreamSource = source;
            bmpImg.EndInit();

            WriteableBitmap wbm = new WriteableBitmap(bmpImg);

            m_ImgBrs = new ImageBrush(wbm);

            m_ImgBrs.Stretch = Stretch.None;
            lib_Canvas.Width = m_ImgBrs.ImageSource.Width;
            lib_Canvas.Height = m_ImgBrs.ImageSource.Height;
            lib_Canvas.Background = m_ImgBrs;
            m_nWidth = (int)Math.Round(m_ImgBrs.ImageSource.Width + 0.5);
            m_nHeight = (int)Math.Round(m_ImgBrs.ImageSource.Height + 0.5);
            m_nChannel = 1;

            m_dScale = myScaleTransform.ScaleX;
            m_lbWidth.Content = m_nWidth.ToString("0.00");
            m_lbHeight.Content = m_nHeight.ToString("0.00");
        }
    }
}
