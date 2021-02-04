using System;
using System.Activities.Expressions;
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
using static WaferPolishingSystem.Define.UserClass;
using System.Threading;

namespace UserInterface
{
    public delegate void ReturnAngle(double angle);
    public delegate void ReturnUpdateRect(int index, Rect rect);
    public delegate void ReturnMilling(Rect[] rect);
    /// <summary>
    /// Align.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class AlignControl : UserControl
    {
        public ReturnAngle delAngle = null;
        public ReturnUpdateRect delUpdateRect = null;
        public ReturnMilling delMilling = null;
        string Title = "AlignControl";
        const double m_dResultFontSize = 100;
        public bool m_bLinkMil = true;
        private bool m_bPressMove = false;
        private bool m_bSearchLock = true;
        public bool SearchLock
        {
            get { return m_bSearchLock; }
            set { m_bSearchLock = value; }
        }

        public static readonly DependencyProperty AlignControlVisibleProperty
         = DependencyProperty.Register(
             "AlignControlVisible",
             typeof(Visibility),
             typeof(UserControl),
             new PropertyMetadata(Visibility.Visible)
             );
        public Visibility AlignControlVisible
        {
            get { return (Visibility)GetValue(AlignControlVisibleProperty); }
            set { SetValue(AlignControlVisibleProperty, value); }
        }
        #region define enum
        //---------------------------------------------------------------------------
        /**
        @enum   EnRoiMode	
        @brief	UserControl의 Draw 모드 구분 열거형.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:32
        */
        public enum EnRoiMode
        {
            ModeMove = 0,
            ModeDraw,
            ModeTheta,
            ModeDrawMove,
            ModeDrawMoveLT,
            ModeDrawMoveT,
            ModeDrawMoveRT,
            ModeDrawMoveL,
            ModeDrawMoveR,
            ModeDrawMoveLB,
            ModeDrawMoveB,
            ModeDrawMoveRB
        }

        //---------------------------------------------------------------------------
        /**
        @enum   EnObjectSelect	
        @brief	UserControl의 선택 모드 구분 열거형.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:32
        */
        public enum EnObjectSelect
        {
            SelModel = 8,
            SelSearch,
            SelMil1,
            SelMil2,
            SelMil3,
            SelMil4,
            SelMil5,
            SelMil6,
            SelMil7,
            SelMil8,
            SelMil9,
            SelMil10,
            SelAngle,
            SelShowAll
        }

        //---------------------------------------------------------------------------
        /**
        @enum   EnRectGrid	
        @brief	UserControl의 Grid 구분 열거형.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:32
        */
        public enum EnRectGrid
        {
            GridLT = 0,
            GridT,
            GridRT,
            GridL,
            GridR,
            GridLB,
            GridB,
            GridRB,
            GridALL
        }
        #endregion

        #region define Member Variable
        double m_dScale = 1.0;
        System.Windows.Point m_pntPrev;
        SolidColorBrush m_brush = new SolidColorBrush(Color.FromArgb(100, 200, 0, 0));
        SolidColorBrush m_brushSearch = new SolidColorBrush(Color.FromArgb(100, 200, 200, 0));
        SolidColorBrush[] m_brushMil = new SolidColorBrush[10];
        SolidColorBrush[] m_brushMilGrid = new SolidColorBrush[10];

        EnRoiMode m_enMode = 0;
        EnRoiMode m_enOldMode = 0;

        Cursor m_oldCursor;

        Canvas m_cvsResult = new Canvas();

        Grid m_gridOverray = new Grid();
        Grid m_gridRuler = new Grid();

        Label m_lbWidth = new Label();
        Label m_lbHeight = new Label();
        Label m_lbResolutionX = new Label();
        Label m_lbResolutionY = new Label();

        Rectangle m_rectModel = new Rectangle();
        Rectangle m_rectSel = new Rectangle();

        ImageBrush m_ImgBrsOrg;
        ImageBrush m_ImgBrs;

        const int RECT_GRID_COUNT = 9;
        const int RECT_OBJECT_COUNT = 12;
        Rect[] m_rectGrid = new Rect[Enum.GetNames(typeof(EnRectGrid)).Length];

        Rect[] m_rectROI = new Rect[RECT_OBJECT_COUNT];

        Rect m_rectInterlock = new Rect();

        bool m_bRuler = false;
        bool m_bShiftDown = false;

        private int m_nSelObj = (int)EnObjectSelect.SelModel;

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
        #endregion

        public AlignControl()
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

            //         0x99, 0x00, 0xC8, 0x00
            //         0x99, 0x00, 0x00, 0xC8
            //         0x99, 0xC8, 0x00, 0xC8
            //         0x99, 0x00, 0xC8, 0xC8
            //         0x99, 0xC8, 0xC8, 0x00
            //         0x99, 0xC8, 0x00, 0x00
            //         0x99, 0x64, 0x64, 0xC8
            //         0x99, 0xC8, 0x64, 0x00
            //         0x99, 0x64, 0x00, 0xC8
            //         0x99, 0x00, 0xC8, 0x64

            m_brushMil[0] = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0xC8, 0x00));
            m_brushMil[1] = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0x00, 0xC8));
            m_brushMil[2] = new SolidColorBrush(Color.FromArgb(0x99, 0xC8, 0x00, 0xC8));
            m_brushMil[3] = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0xC8, 0xC8));
            m_brushMil[4] = new SolidColorBrush(Color.FromArgb(0x99, 0xC8, 0xC8, 0x00));
            m_brushMil[5] = new SolidColorBrush(Color.FromArgb(0x99, 0xC8, 0x00, 0x00));
            m_brushMil[6] = new SolidColorBrush(Color.FromArgb(0x99, 0x64, 0x64, 0xC8));
            m_brushMil[7] = new SolidColorBrush(Color.FromArgb(0x99, 0xC8, 0x64, 0x00));
            m_brushMil[8] = new SolidColorBrush(Color.FromArgb(0x99, 0x64, 0x00, 0xC8));
            m_brushMil[9] = new SolidColorBrush(Color.FromArgb(0x99, 0x00, 0xC8, 0x64));

            m_brushMilGrid[0] = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xC8, 0x00));
            m_brushMilGrid[1] = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0x00, 0xC8));
            m_brushMilGrid[2] = new SolidColorBrush(Color.FromArgb(0xFF, 0xC8, 0x00, 0xC8));
            m_brushMilGrid[3] = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xC8, 0xC8));
            m_brushMilGrid[4] = new SolidColorBrush(Color.FromArgb(0xFF, 0xC8, 0xC8, 0x00));
            m_brushMilGrid[5] = new SolidColorBrush(Color.FromArgb(0xFF, 0xC8, 0x00, 0x00));
            m_brushMilGrid[6] = new SolidColorBrush(Color.FromArgb(0xFF, 0x64, 0x64, 0xC8));
            m_brushMilGrid[7] = new SolidColorBrush(Color.FromArgb(0xFF, 0xC8, 0x64, 0x00));
            m_brushMilGrid[8] = new SolidColorBrush(Color.FromArgb(0xFF, 0x64, 0x00, 0xC8));
            m_brushMilGrid[9] = new SolidColorBrush(Color.FromArgb(0xFF, 0x00, 0xC8, 0x64));


            // Grid Rectangle 추가
            for (int i = 0; i < 8; i++)
            {
                lib_Canvas.Children.Add(new Rectangle());
            }
            // Model Rectangle 추가
            lib_Canvas.Children.Add(m_rectModel);

            m_rectModel.StrokeThickness = m_dScale < 1 ? (1 / m_dScale) : 1;
            DoubleCollection dc1 = new DoubleCollection();
            dc1.Add(3 / m_dScale);
            dc1.Add(3 / m_dScale);
            m_rectModel.StrokeDashArray = dc1;
            m_rectModel.Stroke = Brushes.Red;
            //(lib_Canvas.Children[(int)EnObjectSelect.SelModel] as Rectangle).Fill = m_brush;

            // Search Rectangle 추가
            lib_Canvas.Children.Add(m_rectSel);

            m_rectSel.StrokeThickness = m_dScale < 1 ? (1 / m_dScale) : 1;
            DoubleCollection dc2 = new DoubleCollection();
            dc2.Add(3 / m_dScale);
            dc2.Add(3 / m_dScale);
            m_rectSel.StrokeDashArray = dc2;
            m_rectSel.Stroke = Brushes.Gold;
            //(lib_Canvas.Children[(int)EnObjectSelect.SelSearch] as Rectangle).Fill = m_brushSearch;

            // Milling Rectangle 추가
            for (int i = 0; i < 10; i++)
            {
                lib_Canvas.Children.Add(new Rectangle());

                (lib_Canvas.Children[(int)EnObjectSelect.SelMil1 + i] as Rectangle).StrokeThickness = m_dScale < 1 ? (1 / m_dScale) : 1;
                DoubleCollection dc3 = new DoubleCollection();
                dc3.Add(3 / m_dScale);
                dc3.Add(3 / m_dScale);
                (lib_Canvas.Children[(int)EnObjectSelect.SelMil1 + i] as Rectangle).StrokeDashArray = dc3;
                (lib_Canvas.Children[(int)EnObjectSelect.SelMil1 + i] as Rectangle).Stroke = m_brushMilGrid[i];
                (lib_Canvas.Children[(int)EnObjectSelect.SelMil1 + i] as Rectangle).Fill = m_brushMil[i];
            }
            // ThetaLine
            lib_Canvas.Children.Add(new Line());
            lib_Canvas.Children.Add(new Line());
            // Angle
            lib_Canvas.Children.Add(new TextBlock());

            for (int i = 0; i < lib_Canvas.Children.Count; i++)
            {
                lib_Canvas.Children[i].Visibility = Visibility.Hidden;
            }

            InitOveray();

            for (int i = 0; i < m_rectROI.Length; i++)
            {
                m_rectROI[i] = new Rect(0, 0, 0, 0);
            }
            for (int i = 0; i < 16; i++)
            {
                Grid grid = new Grid();

                grid.Children.Add(new Rectangle());
                grid.Children.Add(new TextBlock());
                (grid.Children[1] as TextBlock).VerticalAlignment = VerticalAlignment.Center;
                (grid.Children[1] as TextBlock).HorizontalAlignment = HorizontalAlignment.Center;
                (grid.Children[1] as TextBlock).FontSize = 50;
                m_cvsResult.Children.Add(grid);
            }
            lib_Canvas.Children.Add(m_cvsResult);

            // Milling ROI Interlock Test Code 20.05.08 22:00
            //             lib_Canvas.Children.Add(new Rectangle());
            //             Canvas.SetLeft(lib_Canvas.Children[lib_Canvas.Children.Count-1], 100);
            //             Canvas.SetTop(lib_Canvas.Children[lib_Canvas.Children.Count - 1], 100);
            //             (lib_Canvas.Children[lib_Canvas.Children.Count - 1] as Rectangle).Width = 3000;
            //             (lib_Canvas.Children[lib_Canvas.Children.Count - 1] as Rectangle).Height = 2000;
            // 
            //             (lib_Canvas.Children[lib_Canvas.Children.Count - 1] as Rectangle).Opacity = 0.3;
            //             (lib_Canvas.Children[lib_Canvas.Children.Count - 1] as Rectangle).Fill = Brushes.LightGray;
            //Grid grid = DeepClone(m_gridResult);
            if (m_ImgBrs != null)
                lib_Canvas.Background = m_ImgBrs;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void fn_SetResolution(double dResX, double dResY)
        @brief	Align Control에 Resolution Setting.
        @return	void
        @param	double dResX : X방향 Resolution.
        @param	double dResY : Y방향 Resolution.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:37
        */
        public void fn_SetResolution(double dResX, double dResY)
        {
            m_dResolutionX = dResX;
            m_dResolutionY = dResY;
            m_lbResolutionX.Content = m_dResolutionX.ToString("0.00");
            m_lbResolutionY.Content = m_dResolutionY.ToString("0.00");
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void OpenImage(string strPath)
        @brief	Align Control에 string path로 이미지 Open.
        @return	
        @param	
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:39
        */
        public void OpenImage(string strPath)
        {

            if (!File.Exists(strPath))
            {
                m_ImgBrs = null;
                m_ImgBrsOrg = null;
                lib_Canvas.Background = Brushes.Transparent;
                return;
            }
            //SetMove(EnRoiMode.ModeMove);
            //------------------------------------
            Byte[] buffer;
            buffer = System.IO.File.ReadAllBytes(strPath);
            MemoryStream ms = new MemoryStream(buffer);

            BitmapImage bmpImg = new BitmapImage();
            //FileStream source = File.OpenRead(strPath);

            bmpImg.BeginInit();
            bmpImg.CacheOption = BitmapCacheOption.OnLoad;
            //bmpImg.StreamSource = source;
            bmpImg.StreamSource = ms;
            bmpImg.EndInit();
            
            WriteableBitmap wbm = new WriteableBitmap(bmpImg);

            m_ImgBrs = new ImageBrush(wbm);
            m_ImgBrs.Stretch = Stretch.None;
            lib_Canvas.Width = m_ImgBrs.ImageSource.Width;
            lib_Canvas.Height = m_ImgBrs.ImageSource.Height;
            lib_Canvas.Background = m_ImgBrs;
            m_nWidth = (m_ImgBrs.ImageSource as WriteableBitmap).PixelWidth;
            m_nHeight = (m_ImgBrs.ImageSource as WriteableBitmap).PixelHeight;
            m_nChannel = 1;
            m_ImgBrsOrg = m_ImgBrs.Clone();

            m_dScale = myScaleTransform.ScaleX;
            m_lbWidth.Content = m_nWidth.ToString("0.00");
            m_lbHeight.Content = m_nHeight.ToString("0.00");

            //Dispatcher.Invoke((ThreadStart)(() => { }), DispatcherPriority.ApplicationIdle);
            //SetMove(EnRoiMode.ModeDraw);
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
                if (m_nWidth > 0 && m_nHeight > 0 && m_ImgBrs != null)
                {
                    dScaleX = ctrl_Grid.ActualWidth / m_ImgBrs.ImageSource.Width - 0.001;
                    dScaleY = ctrl_Grid.ActualHeight / m_ImgBrs.ImageSource.Height - 0.001;
                }
            }
            catch (System.Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            return (dScaleY > dScaleX) ? dScaleX : dScaleY;
        }

        public void SetFitScale()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate ()
            {
                sl_Scale.Value = GetFitScale();
                //DrawRectGrid();
            }));
        }

        public void SetScale(double dScale)
        {
            sl_Scale.Value = dScale;
            DrawRectGrid();
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
            if (m_ImgBrs != null && bUpdateOrg)
                m_ImgBrsOrg = m_ImgBrs.Clone();
            if (m_ImgBrs.Stretch == Stretch.None)
            {
                lib_Canvas.Width = m_ImgBrs.ImageSource.Width;
                lib_Canvas.Height = m_ImgBrs.ImageSource.Height;
            }

            m_nWidth = (m_ImgBrs.ImageSource as WriteableBitmap).PixelWidth;
            m_nHeight = (m_ImgBrs.ImageSource as WriteableBitmap).PixelHeight;
            m_nChannel = 1;

            lib_Canvas.Background = m_ImgBrs;
            m_dScale = myScaleTransform.ScaleX;

            m_lbWidth.Content = m_nWidth.ToString("0.00");
            m_lbHeight.Content = m_nHeight.ToString("0.00");
        }

        public void SetImage(byte[] buff, int width, int height)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {

                if (g_VisionManager._CamManager.IsGrabLive)
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
                }
                else
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

        public void fn_SetROIInterlock(Rect rect)
        {
            m_rectInterlock = rect;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void onMouseDown(object sender, MouseButtonEventArgs e)
        @brief	마우스 Down 이벤트.
        @return	void
        @param	object                  sender
        @param	MouseButtonEventArgs    e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:44
        */
        private void onMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (m_enMode == EnRoiMode.ModeMove)
                m_pntPrev = e.GetPosition(this);
            else if (m_enMode == EnRoiMode.ModeDraw)
            {
                m_pntPrev = e.GetPosition(lib_Canvas);

                if (lib_Canvas.Children.Count > 0)
                {
                    if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridLT], e.GetPosition(lib_Canvas))) m_enMode = EnRoiMode.ModeDrawMoveLT;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridT], e.GetPosition(lib_Canvas))) m_enMode = EnRoiMode.ModeDrawMoveT;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridRT], e.GetPosition(lib_Canvas))) m_enMode = EnRoiMode.ModeDrawMoveRT;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridL], e.GetPosition(lib_Canvas))) m_enMode = EnRoiMode.ModeDrawMoveL;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridR], e.GetPosition(lib_Canvas))) m_enMode = EnRoiMode.ModeDrawMoveR;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridLB], e.GetPosition(lib_Canvas))) m_enMode = EnRoiMode.ModeDrawMoveLB;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridB], e.GetPosition(lib_Canvas))) m_enMode = EnRoiMode.ModeDrawMoveB;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridRB], e.GetPosition(lib_Canvas))) m_enMode = EnRoiMode.ModeDrawMoveRB;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridALL], e.GetPosition(lib_Canvas))) m_enMode = EnRoiMode.ModeDrawMove;
                }
                // Double Clik Open시 Rectangle 변화 막기
                m_bPressMove = true;
            }
            else if (m_enMode == EnRoiMode.ModeTheta)
            {
                m_pntPrev = e.GetPosition(lib_Canvas);

            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void onMouseMove(object sender, MouseEventArgs e)
        @brief	마우스 Move 이벤트.
        @return	void
        @param	object          sender
        @param	MouseEventArgs  e
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:46
        */
        private void onMouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (m_enMode == EnRoiMode.ModeDraw)
                {
                    if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridLT], e.GetPosition(lib_Canvas))) this.Cursor = Cursors.SizeNWSE;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridT], e.GetPosition(lib_Canvas))) this.Cursor = Cursors.SizeNS;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridRT], e.GetPosition(lib_Canvas))) this.Cursor = Cursors.SizeNESW;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridL], e.GetPosition(lib_Canvas))) this.Cursor = Cursors.SizeWE;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridR], e.GetPosition(lib_Canvas))) this.Cursor = Cursors.SizeWE;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridLB], e.GetPosition(lib_Canvas))) this.Cursor = Cursors.SizeNESW;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridB], e.GetPosition(lib_Canvas))) this.Cursor = Cursors.SizeNS;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridRB], e.GetPosition(lib_Canvas))) this.Cursor = Cursors.SizeNWSE;
                    else if (CheckRectInsidePoint(m_rectGrid[(int)EnRectGrid.GridALL], e.GetPosition(lib_Canvas))) this.Cursor = Cursors.SizeAll;
                    else this.Cursor = m_oldCursor;
                }

                if (e.LeftButton == MouseButtonState.Pressed && m_bPressMove) // Double Clik Open시 Rectangle 변화 막기
                {
                    if (m_enMode == EnRoiMode.ModeMove)
                    {
                        double dOffsetX = lib_ScrollViewer.HorizontalOffset;
                        double dOffsetY = lib_ScrollViewer.VerticalOffset;
                        Point pnt = new Point();
                        pnt.X = m_pntPrev.X - e.GetPosition(this).X;
                        pnt.Y = m_pntPrev.Y - e.GetPosition(this).Y;
                        lib_ScrollViewer.ScrollToHorizontalOffset(dOffsetX + pnt.X);
                        lib_ScrollViewer.ScrollToVerticalOffset(dOffsetY + pnt.Y);
                        m_pntPrev = e.GetPosition(this);
                    }
                    else if (m_enMode == EnRoiMode.ModeTheta)
                    {

                        Point pnt = e.GetPosition(lib_Canvas);

                        DrawTheta(m_pntPrev, pnt);
                    }
                    else
                    {
                        // Rect Interlock
                        Rectangle selRect = (lib_Canvas.Children[m_nSelObj] as Rectangle);

                        Point pnt = e.GetPosition(lib_Canvas);

                        ViewContent(m_nSelObj, true);
                        //lib_Canvas.Children[m_nSelObj]
                        double dPointGapX = pnt.X - m_pntPrev.X;
                        double dPointGapY = pnt.Y - m_pntPrev.Y;

                        if (selRect.Width > Math.Abs(dPointGapX) && selRect.Height > Math.Abs(dPointGapY))
                        {
                            switch (m_enMode)
                            {
                                case EnRoiMode.ModeDrawMove:
                                    if (m_nSelObj == (int)EnObjectSelect.SelModel && m_bLinkMil)
                                    {
                                        Rect[] rectMil = new Rect[(int)EnObjectSelect.SelMil10 - (int)EnObjectSelect.SelMil1 + 1];
                                        for (int i = (int)EnObjectSelect.SelMil1; i <= (int)EnObjectSelect.SelMil10; i++)
                                        {
                                            rectMil[i - (int)EnObjectSelect.SelMil1].X = Canvas.GetLeft(lib_Canvas.Children[i]) + dPointGapX;
                                            rectMil[i - (int)EnObjectSelect.SelMil1].Y = Canvas.GetTop(lib_Canvas.Children[i]) + dPointGapY;
                                            rectMil[i - (int)EnObjectSelect.SelMil1].Width = (lib_Canvas.Children[i] as Rectangle).Width;
                                            rectMil[i - (int)EnObjectSelect.SelMil1].Height = (lib_Canvas.Children[i] as Rectangle).Height;
                                            if (lib_Canvas.Children[i].Visibility == Visibility.Visible)
                                            {
                                                Canvas.SetLeft(lib_Canvas.Children[i], rectMil[i - (int)EnObjectSelect.SelMil1].X);
                                                Canvas.SetTop(lib_Canvas.Children[i], rectMil[i - (int)EnObjectSelect.SelMil1].Y);
                                            }
                                        }
                                        delMilling?.Invoke(rectMil);
                                    }
                                    Canvas.SetLeft(selRect, Canvas.GetLeft(selRect) + dPointGapX);
                                    Canvas.SetTop(selRect, Canvas.GetTop(selRect) + dPointGapY);
                                    DrawRectGrid();
                                    m_pntPrev = pnt;
                                    return;
                                case EnRoiMode.ModeDrawMoveLT:
                                    Canvas.SetLeft(selRect, Canvas.GetLeft(selRect) + dPointGapX);
                                    Canvas.SetTop(selRect, Canvas.GetTop(selRect) + dPointGapY);
                                    (selRect).Width += dPointGapX * -1;
                                    (selRect).Height += dPointGapY * -1;
                                    DrawRectGrid();
                                    m_pntPrev = pnt;
                                    return;
                                case EnRoiMode.ModeDrawMoveT:
                                    Canvas.SetTop(selRect, Canvas.GetTop(selRect) + dPointGapY);
                                    (selRect).Height += dPointGapY * -1;
                                    DrawRectGrid();
                                    m_pntPrev = pnt;
                                    return;
                                case EnRoiMode.ModeDrawMoveRT:
                                    Canvas.SetTop(selRect, Canvas.GetTop(selRect) + dPointGapY);
                                    selRect.Width += dPointGapX;
                                    selRect.Height += dPointGapY * -1;
                                    DrawRectGrid();
                                    m_pntPrev = pnt;
                                    return;
                                case EnRoiMode.ModeDrawMoveL:
                                    Canvas.SetLeft(selRect,
                                        Canvas.GetLeft(selRect) + dPointGapX);
                                    selRect.Width += dPointGapX * -1;
                                    DrawRectGrid();
                                    m_pntPrev = pnt;
                                    return;
                                case EnRoiMode.ModeDrawMoveR:
                                    selRect.Width += dPointGapX;
                                    DrawRectGrid();
                                    m_pntPrev = pnt;
                                    return;
                                case EnRoiMode.ModeDrawMoveLB:
                                    Canvas.SetLeft(selRect, Canvas.GetLeft(selRect) + dPointGapX);
                                    selRect.Width += dPointGapX * -1;
                                    selRect.Height += dPointGapY;
                                    DrawRectGrid();
                                    m_pntPrev = pnt;
                                    return;
                                case EnRoiMode.ModeDrawMoveB:
                                    selRect.Height += dPointGapY;
                                    DrawRectGrid();
                                    m_pntPrev = pnt;
                                    return;
                                case EnRoiMode.ModeDrawMoveRB:
                                    selRect.Width += dPointGapX;
                                    selRect.Height += dPointGapY;
                                    DrawRectGrid();
                                    m_pntPrev = pnt;
                                    return;
                            }

                        }
                        #region DrawRect
                        if (dPointGapX > 0)
                        {
                            Canvas.SetLeft(selRect, m_pntPrev.X);
                            selRect.Width = dPointGapX;
                        }
                        else
                        {
                            Canvas.SetLeft(selRect, pnt.X);
                            selRect.Width = Math.Abs(dPointGapX);
                        }

                        if (dPointGapY > 0)
                        {
                            Canvas.SetTop(selRect, m_pntPrev.Y);
                            selRect.Height = dPointGapY;
                        }
                        else
                        {
                            Canvas.SetTop(selRect, pnt.Y);
                            selRect.Height = Math.Abs(dPointGapY);
                        }
                        DrawRectGrid();
                        #endregion
                    }
                }
                else if (e.LeftButton == MouseButtonState.Released)
                {
                    m_enMode = m_enOldMode;
                    m_bPressMove = false;
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
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
            //             m_enMode = m_enOldMode;
            //             if (delMilling != null && m_nSelObj > 7)
            //             {
            //                 RectD rect = new RectD();
            //                 int idx = m_nSelObj - 8;
            //                 rect.X = m_rectROI[idx].X;
            //                 rect.Y = m_rectROI[idx].Y;
            //                 rect.Width = m_rectROI[idx].Width;
            //                 rect.Height = m_rectROI[idx].Height;
            // 
            //                 delMilling(idx, rect);
            //             }
        }

        public void SetTheta(double Angle)
        {
            //              Point pnt1 = new Point();
            //              Point pnt2 = new Point();
            //              double length = 100.0;
            //              if (m_Theta == 360)
            //              {
            //                  pnt1.X = lib_ScrollViewer.HorizontalOffset + lib_ScrollViewer.ActualWidth / 2.0;
            //                  pnt1.Y = lib_ScrollViewer.VerticalOffset + lib_ScrollViewer.ActualHeight / 2.0;
            //                  pnt2.X = length * Math.Cos(Angle * Math.PI / 180) + pnt1.X;
            //                  pnt2.Y = length * Math.Sin(Angle * Math.PI / 180) + pnt1.Y;
            //              }
            //              else
            //              {
            //                  Line line = (lib_Canvas.Children[(int)EnObjectSelect.SelAngle] as Line);
            //                  pnt1.X = line.X1;
            //                  pnt1.Y = line.Y1;
            //                  pnt2.X = length * Math.Cos(Angle * Math.PI / 180) + pnt1.X;
            //                  pnt2.Y = length * Math.Sin(Angle * Math.PI / 180) + pnt1.Y;
            //  
            //              }
            //              DrawTheta(pnt1, pnt2, true);
        }

        public void DrawTheta(Point pnt1, Point pnt2, bool bText = false)
        {
            try
            {
                Line line = lib_Canvas.Children[(int)EnObjectSelect.SelAngle] as Line;
                double length = Math.Sqrt(Math.Pow(pnt2.X - pnt1.X, 2) + Math.Pow(pnt2.Y - pnt1.Y, 2));
                if (line != null)
                {
                    line.Visibility = Visibility.Visible;
                    line.X1 = pnt1.X;
                    line.Y1 = pnt1.Y;
                    line.X2 = pnt2.X;
                    line.Y2 = pnt2.Y;
                    line.Stroke = Brushes.Magenta;
                    line.StrokeThickness = 2 / m_dScale;
                }
                line = lib_Canvas.Children[(int)EnObjectSelect.SelAngle + 1] as Line;
                if (line != null)
                {
                    line.Visibility = Visibility.Visible;
                    line.X1 = pnt1.X;
                    line.Y1 = pnt1.Y;
                    line.X2 = pnt1.X + length;
                    line.Y2 = pnt1.Y;
                    line.Stroke = Brushes.Magenta;
                    line.StrokeThickness = 2 / m_dScale;
                }
                // Update Text
                TextBlock tb = lib_Canvas.Children[(int)EnObjectSelect.SelAngle + 2] as TextBlock;
                if (tb != null)
                {
                    tb.Visibility = Visibility.Visible;
                    tb.Foreground = Brushes.Magenta;
                    tb.FontWeight = FontWeights.Bold;
                    tb.FontSize = 20.0 / m_dScale;
                    m_Theta = Math.Acos((pnt2.X - pnt1.X) / Math.Sqrt(Math.Pow(pnt2.X - pnt1.X, 2) + Math.Pow(pnt2.Y - pnt1.Y, 2))) * 180.0 / Math.PI;
                    if (pnt2.Y - pnt1.Y > 0)
                    {
                        m_Theta *= -1;
                        Console.WriteLine($"Theta : {m_Theta}");
                    }
                    tb.Text = m_Theta.ToString("0.00˚");
                    //if(!bText)
                    delAngle?.Invoke(m_Theta);

                    //tb.Margin = new Thickness((pnt1.X + pnt2.X) / 2.0, (pnt1.Y + 3 * pnt2.Y) / 4.0, 0, 0);
                    tb.Margin = new Thickness(pnt2.X, pnt2.Y, 0, 0);
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		private void DrawRectGrid()
        @brief	Grid Draw 함수.
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:49
        */
        public void DrawRectGrid()
        {
            // Search Rectangle Exception
            if (m_bSearchLock)
            {
                if ((lib_Canvas.Children[(int)EnObjectSelect.SelSearch] as Rectangle).Width != 0 || (lib_Canvas.Children[(int)EnObjectSelect.SelSearch] as Rectangle).Height != 0)
                {
                    if (m_nSelObj == (int)EnObjectSelect.SelSearch || m_nSelObj == (int)EnObjectSelect.SelModel)
                    {
                        if ((lib_Canvas.Children[(int)EnObjectSelect.SelSearch] as Rectangle).Width <= (lib_Canvas.Children[(int)EnObjectSelect.SelModel] as Rectangle).Width)
                            (lib_Canvas.Children[(int)EnObjectSelect.SelSearch] as Rectangle).Width = (lib_Canvas.Children[(int)EnObjectSelect.SelModel] as Rectangle).Width;
                        if ((lib_Canvas.Children[(int)EnObjectSelect.SelSearch] as Rectangle).Height <= (lib_Canvas.Children[(int)EnObjectSelect.SelModel] as Rectangle).Height)
                            (lib_Canvas.Children[(int)EnObjectSelect.SelSearch] as Rectangle).Height = (lib_Canvas.Children[(int)EnObjectSelect.SelModel] as Rectangle).Height;
                    }
                }
            }
            // ~Select Rectangle Exception
            int nGridWidth = (int)(12 / m_dScale);
            int nGridHeight = (int)(12 / m_dScale);

            if (lib_Canvas.Children.Count > m_nSelObj)
            {
                (lib_Canvas.Children[m_nSelObj] as Rectangle).StrokeThickness = m_dScale < 1 ? (int)(1 / m_dScale) : 1;
                m_rectGrid[(int)EnRectGrid.GridALL].X = Canvas.GetLeft(lib_Canvas.Children[m_nSelObj]);
                m_rectGrid[(int)EnRectGrid.GridALL].Y = Canvas.GetTop(lib_Canvas.Children[m_nSelObj]);
                m_rectGrid[(int)EnRectGrid.GridALL].Width = (lib_Canvas.Children[m_nSelObj] as Rectangle).Width;
                m_rectGrid[(int)EnRectGrid.GridALL].Height = (lib_Canvas.Children[m_nSelObj] as Rectangle).Height;

                m_rectGrid[(int)EnRectGrid.GridLT].X = m_rectGrid[(int)EnRectGrid.GridALL].Left - nGridWidth / 2.0;
                m_rectGrid[(int)EnRectGrid.GridLT].Y = m_rectGrid[(int)EnRectGrid.GridALL].Top - nGridHeight / 2.0;

                m_rectGrid[(int)EnRectGrid.GridT].X = (m_rectGrid[(int)EnRectGrid.GridALL].Left + m_rectGrid[(int)EnRectGrid.GridALL].Right - nGridWidth) / 2.0;
                m_rectGrid[(int)EnRectGrid.GridT].Y = m_rectGrid[(int)EnRectGrid.GridALL].Top - nGridHeight / 2.0;

                m_rectGrid[(int)EnRectGrid.GridRT].X = m_rectGrid[(int)EnRectGrid.GridALL].Right - nGridWidth / 2.0;
                m_rectGrid[(int)EnRectGrid.GridRT].Y = m_rectGrid[(int)EnRectGrid.GridALL].Top - nGridHeight / 2.0;

                m_rectGrid[(int)EnRectGrid.GridL].X = m_rectGrid[(int)EnRectGrid.GridALL].Left - nGridWidth / 2.0;
                m_rectGrid[(int)EnRectGrid.GridL].Y = (m_rectGrid[(int)EnRectGrid.GridALL].Top + m_rectGrid[(int)EnRectGrid.GridALL].Bottom - nGridHeight) / 2.0;

                m_rectGrid[(int)EnRectGrid.GridR].X = m_rectGrid[(int)EnRectGrid.GridALL].Right - nGridWidth / 2.0;
                m_rectGrid[(int)EnRectGrid.GridR].Y = (m_rectGrid[(int)EnRectGrid.GridALL].Top + m_rectGrid[(int)EnRectGrid.GridALL].Bottom - nGridHeight) / 2.0;

                m_rectGrid[(int)EnRectGrid.GridLB].X = m_rectGrid[(int)EnRectGrid.GridALL].Left - nGridWidth / 2.0;
                m_rectGrid[(int)EnRectGrid.GridLB].Y = m_rectGrid[(int)EnRectGrid.GridALL].Bottom - nGridHeight / 2.0;

                m_rectGrid[(int)EnRectGrid.GridB].X = (m_rectGrid[(int)EnRectGrid.GridALL].Left + m_rectGrid[(int)EnRectGrid.GridALL].Right - nGridWidth) / 2.0;
                m_rectGrid[(int)EnRectGrid.GridB].Y = m_rectGrid[(int)EnRectGrid.GridALL].Bottom - nGridHeight / 2.0;

                m_rectGrid[(int)EnRectGrid.GridRB].X = m_rectGrid[(int)EnRectGrid.GridALL].Right - nGridWidth / 2.0;
                m_rectGrid[(int)EnRectGrid.GridRB].Y = m_rectGrid[(int)EnRectGrid.GridALL].Bottom - nGridHeight / 2.0;

                for (int i = 0; i < 8; i++)
                {
                    m_rectGrid[i].Width = nGridWidth;
                    m_rectGrid[i].Height = nGridWidth;
                    Rectangle re = lib_Canvas.Children[i] as Rectangle;
                    //re.Visibility = Visibility.Visible;
                    re.Width = nGridWidth;
                    re.Height = nGridHeight;
                    re.StrokeThickness = 0;
                    switch (m_nSelObj)
                    {
                        case (int)EnObjectSelect.SelModel:
                            re.Stroke = Brushes.Red;
                            re.Fill = Brushes.Red;
                            break;
                        case (int)EnObjectSelect.SelSearch:
                            re.Stroke = Brushes.Gold;
                            re.Fill = Brushes.Gold;
                            break;
                        default:
                            re.Stroke = m_brushMilGrid[m_nSelObj - (int)EnObjectSelect.SelMil1];
                            re.Fill = m_brushMilGrid[m_nSelObj - (int)EnObjectSelect.SelMil1];
                            break;
                    }

                    Canvas.SetLeft(re, m_rectGrid[i].X);
                    Canvas.SetTop(re, m_rectGrid[i].Y);
                }
                //---------------------------------------------------------------------------
                // Update Rect Info And Run CallBack
                // - Index를 8개 Grid를 제외하고 반환.
                try
                {
                    for (int i = 0; i < m_rectROI.Length; i++)
                    {
                        m_rectROI[i].X = Canvas.GetLeft(lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)]);
                        m_rectROI[i].Y = Canvas.GetTop(lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)]);
                        m_rectROI[i].Width = (lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)] as Rectangle).Width > 0
                            ? (lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)] as Rectangle).Width : 0;
                        m_rectROI[i].Height = (lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)] as Rectangle).Height > 0
                            ? (lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)] as Rectangle).Height : 0;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                if (delUpdateRect != null && m_nSelObj > 7)
                {
                    int idx = m_nSelObj - 8;
                    delUpdateRect?.Invoke(idx, m_rectROI[idx]);
                }
                //---------------------------------------------------------------------------
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private bool CheckRectInsidePoint(Rect rect, Point pnt)
        @brief	인자로 넘어온 사각형 안에 포인트가 속해 있는지 bool형으로 리턴.
        @return	bool : Point가 Rect 안에 있는지.
        @param	Rect rect : 확인할 영역.
        @param	Point pnt : 확인할 점.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:49
        */
        private bool CheckRectInsidePoint(Rect rect, Point pnt)
        {
            return (rect.Left <= pnt.X) && (rect.Top <= pnt.Y) && (rect.Right >= pnt.X) && (rect.Bottom >= pnt.Y);
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void ZoomScale(double dScale)
        @brief	Zoom Scale 설정.
        @return	void
        @param	double dScale : 실수형 줌 값.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:51
        */
        private void ZoomScale(double dScale)
        {
            if (myScaleTransform != null)
            {
                double dCurPosRateX = 0.0;
                double dCurPosRateY = 0.0;
                if (lib_ScrollViewer.ExtentWidth > 0)
                    dCurPosRateX = (lib_ScrollViewer.HorizontalOffset + lib_ScrollViewer.ActualWidth / 2) / lib_ScrollViewer.ExtentWidth;
                if (lib_ScrollViewer.ExtentHeight > 0)
                    dCurPosRateY = (lib_ScrollViewer.VerticalOffset + lib_ScrollViewer.ActualHeight / 2) / lib_ScrollViewer.ExtentHeight;

                myScaleTransform.ScaleX = dScale;
                myScaleTransform.ScaleY = dScale;
                m_dScale = dScale;

                double dOffsetX = (nWidth * dScale) * dCurPosRateX - lib_ScrollViewer.ActualWidth / 2;
                double dOffsetY = (nHeight * dScale) * dCurPosRateY - lib_ScrollViewer.ActualHeight / 2;
                lib_ScrollViewer.ScrollToHorizontalOffset(dOffsetX);
                lib_ScrollViewer.ScrollToVerticalOffset(dOffsetY);

                for (int i = (int)EnObjectSelect.SelModel; i < (int)EnObjectSelect.SelMil10; i++)
                {
                    (lib_Canvas.Children[i] as Rectangle).StrokeThickness = m_dScale < 1 ? (int)(1 / m_dScale) : 1;
                }
                DrawRectGrid();
                RulerEnable(m_bRuler);
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void RulerEnable(bool bRuler)
        @brief	Ruler 활성화.
        @return	void
        @param	bool bRuler : Ruler 활성화 여부.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:52
        */
        public void RulerEnable(bool bRuler)
        {
            try
            {
                m_bRuler = bRuler;
                if (m_bRuler)
                {
                    DeInitRuler();
                    InitRuler();
                    m_gridRuler.Visibility = Visibility.Visible;
                    m_gridOverray.Visibility = Visibility.Visible;
                }
                else
                {
                    m_gridRuler.Visibility = Visibility.Hidden;
                    m_gridOverray.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public double GetZoomScale()
        @brief	Control에 설정된 Zoom Scale 반환.
        @return	double : 현재 Zoom Scale.
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:52
        */
        public double GetZoomScale()
        {
            return m_dScale;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void SetMove(EnRoiMode enMove)
        @brief	Control의 Move(Draw)모드 설정.
        @return	void
        @param	EnRoiMode enMove : 설정할 모드.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:53
        */
        public void SetMove(EnRoiMode enMove)
        {
            m_enOldMode = m_enMode = enMove;
            switch (enMove)
            {
                case EnRoiMode.ModeMove:
                    m_oldCursor = this.Cursor = Cursors.ScrollAll;
                    for (int i = 0; i < (int)EnObjectSelect.SelModel; i++)
                    {
                        ViewContent(i, false);
                    }
                    bn_Mode.Content = "Edit Disabled";
                    this.Background = Brushes.Silver;
                    break;
                case EnRoiMode.ModeDraw:
                    m_oldCursor = this.Cursor = Cursors.Cross;
                    SelectObject((EnObjectSelect)m_nSelObj);
                    //ViewContent(m_nSelObj, true);
                    //for (int i = 0; i < (int)EnObjectSelect.SelMil10; i++)
                    //{
                    //    ViewContent(i, true);
                    //}
                    bn_Mode.Content = "Edit Enabled";
                    this.Background = Brushes.Transparent;
                    break;
                case EnRoiMode.ModeTheta:
                    m_oldCursor = this.Cursor = Cursors.Cross;
                    break;
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void ViewContent(int index, bool view)
        @brief	Canvas의 Children의 객체를 보여줄지 설정.
        @return	void
        @param	int     index   : Canvas의 Children 객체 Index
        @param	bool    view    : View 여부.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:55
        */
        public void ViewContent(int index, bool view)
        {
            if (index < lib_Canvas.Children.Count)
            {
                if (view)
                    lib_Canvas.Children[index].Visibility = Visibility.Visible;
                else
                    lib_Canvas.Children[index].Visibility = Visibility.Hidden;
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void SelectObject(EnObjectSelect nIdx)
        @brief	편집할 Object 선택.
        @return	void
        @param	EnObjectSelect nIdx : 편집 객체 ID
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  16:57
        */
        public void SelectObject(EnObjectSelect nIdx)
        {
            if ((int)nIdx == (int)EnObjectSelect.SelShowAll)
            {
                m_nSelObj = (int)EnObjectSelect.SelModel;

                for (int i = 0; i < (int)EnObjectSelect.SelModel; i++)
                {
                    lib_Canvas.Children[i].Visibility = Visibility.Hidden;
                }

                for (int i = (int)EnObjectSelect.SelModel; i < (int)EnObjectSelect.SelMil10; i++)
                {
                    lib_Canvas.Children[i].Visibility = Visibility.Visible;
                }
            }
            else
            {
                m_nSelObj = (int)nIdx;

                //                 for (int i = (int)EnObjectSelect.SelModel; i < (int)EnObjectSelect.SelMil10; i++)
                //                 {
                //                     lib_Canvas.Children[i].Visibility = Visibility.Hidden;
                //                 }

                switch(m_enMode)
                {
                    case EnRoiMode.ModeMove:
                        for (int i = 0; i < (int)EnObjectSelect.SelModel; i++)
                        {
                            ViewContent(i, false);
                        }
                        break;
                    case EnRoiMode.ModeDraw:
                        for (int i = 0; i < (int)EnObjectSelect.SelModel; i++)
                        {
                            ViewContent(i, true);
                        }
                        break;
                }
                lib_Canvas.Children[(int)nIdx].Visibility = Visibility.Visible;
                DrawRectGrid();
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public Rect GetRectModel()
        @brief	현재 설정된 Model ROI의 Rect 얻기.
        @return	Rect : Model ROI의 Rect 객체.
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  17:00
        */
        public Rect GetRectModel()
        {
            return m_rectROI[(int)EnObjectSelect.SelModel - (int)EnObjectSelect.SelModel];
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public Rect GetRectSearch()
        @brief	현재 설정된 Search ROI의 Rect 얻기.
        @return	Rect : Search ROI의 Rect 객체.
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  17:00
        */
        public Rect GetRectSearch()
        {
            return m_rectROI[(int)EnObjectSelect.SelSearch - (int)EnObjectSelect.SelModel];
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public Rect GetRectMilling(int idx = 0)
        @brief	현재 설정된 Milling ROI의 Rect 얻기.
        @return	Rect : Milling ROI의 Rect 객체.
        @param	int idx : 얻고자 하는 Milling ROI Index.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  17:00
        */
        public Rect GetRectMilling(int idx = 0)
        {
            return m_rectROI[(int)(EnObjectSelect.SelMil1 + idx) - (int)EnObjectSelect.SelModel];
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		void ResetRect(int idx = 0)
        @brief	ROI Zero Set.
        @return	void
        @param	int idx : ZeroSet할 객체 Index.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  17:02
        */
        public void ResetRect(int idx = 0)
        {
            try
            {
                m_rectROI[idx - (int)EnObjectSelect.SelModel].X = 0;
                m_rectROI[idx - (int)EnObjectSelect.SelModel].Y = 0;
                m_rectROI[idx - (int)EnObjectSelect.SelModel].Width = 0;
                m_rectROI[idx - (int)EnObjectSelect.SelModel].Height = 0;

                Rectangle rect = lib_Canvas.Children[idx] as Rectangle;
                Canvas.SetLeft(rect, m_rectROI[idx - (int)EnObjectSelect.SelModel].X);
                Canvas.SetTop(rect, m_rectROI[idx - (int)EnObjectSelect.SelModel].Y);
                rect.Width = m_rectROI[idx - (int)EnObjectSelect.SelModel].Width;
                rect.Height = m_rectROI[idx - (int)EnObjectSelect.SelModel].Height;
                DrawRectGrid();
            }
            catch (System.Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public CroppedBitmap GetModelImage()
        @brief	Model ROI에서 이미지 얻기.
        @return	CroppedBitmap : Model ROI Image
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  17:03
        */
        public CroppedBitmap GetModelImage()
        {
            Rect rectROI = m_rectROI[0];

            if (rectROI.Width > 0 && rectROI.Height > 0 && lib_Canvas.Background != null)
            {
                ImageBrush ib = lib_Canvas.Background as ImageBrush;
                if (ib != null)
                {
                    WriteableBitmap wb = ib.ImageSource as WriteableBitmap;

                    CroppedBitmap cb = new CroppedBitmap(
                        wb,
                        new Int32Rect((int)rectROI.X, (int)rectROI.Y,
                        (int)rectROI.Width, (int)rectROI.Height));       //select region rect

                    return cb;
                }
                else
                {
                    return null;
                }

            }
            else
                return null;
        }

        public void SetMatchingValue(double dValue)
        {
            //m_lbMatching.Content = dValue.ToString("0.00");
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void InitRuler()
        @brief	Ruler Init.
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:30
        */
        private void InitRuler()
        {
            //m_gridRuler
            Brush TextColor = Brushes.Cyan;
            int nFontSize = 16;
            bool bFive = false;
            double dCenterX = ctrl_Grid.ActualWidth / 2.0;
            double dCenterY = ctrl_Grid.ActualHeight / 2.0;
            double dWidth = ctrl_Grid.ActualWidth;
            double dHeight = ctrl_Grid.ActualHeight;

            m_gridRuler.Visibility = Visibility.Hidden;
            m_gridRuler.MouseMove += onMouseMove;
            m_gridRuler.MouseDown += onMouseDown;
            m_gridRuler.MouseUp += onMouseUp;

            Brush LineColor = Brushes.Cyan;

            Line line = new Line();
            line.Stroke = LineColor;
            line.X1 = 0;
            line.X2 = dWidth;
            line.Y1 = dCenterY;
            line.Y2 = dCenterY;
            m_gridRuler.Children.Add(line);


            Line line2 = new Line();
            line2.Stroke = LineColor;
            line2.X1 = dCenterX;
            line2.X2 = dCenterX;
            line2.Y1 = 0;
            line2.Y2 = dHeight;
            m_gridRuler.Children.Add(line2);

            List<Line> listLine = new List<Line>();
            List<Label> listLabel = new List<Label>();
            listLine.Clear();
            int nPitchCount = 0;
            if (m_dResolutionX == 0)
                m_dResolutionX = 1;
            if (m_dResolutionY == 0)
                m_dResolutionY = 1;
            if (dCenterX / (1000 / m_dResolutionX * m_dScale) > 5)
                bFive = true;
            else
                bFive = false;
            for (int i = (int)dCenterX; i < dWidth; i += (int)(1000 / m_dResolutionX * m_dScale))
            {
                if (i == (int)dCenterX)
                {
                    nPitchCount++;
                    continue;
                }
                listLine.Add(new Line());
                listLabel.Add(new Label());

                listLine[listLine.Count - 1].Stroke = LineColor;
                listLine[listLine.Count - 1].StrokeThickness = 1;
                listLine[listLine.Count - 1].X1 = i;
                listLine[listLine.Count - 1].X2 = i;
                if (nPitchCount % 5 == 0)
                {
                    listLine[listLine.Count - 1].Y1 = dCenterY - 10;
                    listLine[listLine.Count - 1].Y2 = dCenterY + 10;
                    listLabel[listLabel.Count - 1].Content = nPitchCount.ToString("0 mm");
                }
                else
                {
                    listLine[listLine.Count - 1].Y1 = dCenterY - 5;
                    listLine[listLine.Count - 1].Y2 = dCenterY + 5;
                    if (!bFive)
                        listLabel[listLabel.Count - 1].Content = nPitchCount.ToString("0 mm");
                }
                listLabel[listLabel.Count - 1].Margin = new Thickness(i, dCenterY, 0, 0);
                listLabel[listLabel.Count - 1].Foreground = TextColor;
                listLabel[listLabel.Count - 1].FontSize = nFontSize;

                m_gridRuler.Children.Add(listLabel[listLabel.Count - 1]);
                m_gridRuler.Children.Add(listLine[listLine.Count - 1]);
                nPitchCount++;
            }
            nPitchCount = 0;
            for (int i = (int)dCenterX; i > 0; i -= (int)(1000 / m_dResolutionX * m_dScale))
            {
                if (i == (int)dCenterX)
                {
                    nPitchCount++;
                    continue;
                }
                listLine.Add(new Line());
                listLabel.Add(new Label());

                listLine[listLine.Count - 1].Stroke = LineColor;
                listLine[listLine.Count - 1].StrokeThickness = 1;
                listLine[listLine.Count - 1].X1 = i;
                listLine[listLine.Count - 1].X2 = i;
                if (nPitchCount % 5 == 0)
                {
                    listLine[listLine.Count - 1].Y1 = dCenterY - 10;
                    listLine[listLine.Count - 1].Y2 = dCenterY + 10;
                    listLabel[listLabel.Count - 1].Content = nPitchCount.ToString("0 mm");
                }
                else
                {
                    listLine[listLine.Count - 1].Y1 = dCenterY - 5;
                    listLine[listLine.Count - 1].Y2 = dCenterY + 5;
                    if (!bFive)
                        listLabel[listLabel.Count - 1].Content = nPitchCount.ToString("0 mm");
                }
                listLabel[listLabel.Count - 1].Margin = new Thickness(i, dCenterY, 0, 0);
                listLabel[listLabel.Count - 1].Foreground = TextColor;
                listLabel[listLabel.Count - 1].FontSize = nFontSize;

                m_gridRuler.Children.Add(listLabel[listLabel.Count - 1]);
                m_gridRuler.Children.Add(listLine[listLine.Count - 1]);
                nPitchCount++;
            }

            nPitchCount = 0;

            for (int i = (int)dCenterY; i < dHeight; i += (int)(1000 / m_dResolutionY * m_dScale))
            {
                if (i == (int)dCenterY)
                {
                    nPitchCount++;
                    continue;
                }
                listLine.Add(new Line());
                listLabel.Add(new Label());

                listLine[listLine.Count - 1].Stroke = LineColor;
                listLine[listLine.Count - 1].StrokeThickness = 1;
                if (nPitchCount % 5 == 0)
                {
                    listLine[listLine.Count - 1].X1 = dCenterX - 10;
                    listLine[listLine.Count - 1].X2 = dCenterX + 10;
                    listLabel[listLabel.Count - 1].Content = nPitchCount.ToString("0 mm");
                }
                else
                {
                    listLine[listLine.Count - 1].X1 = dCenterX - 5;
                    listLine[listLine.Count - 1].X2 = dCenterX + 5;
                    if (!bFive)
                        listLabel[listLabel.Count - 1].Content = nPitchCount.ToString("0 mm");
                }

                listLine[listLine.Count - 1].Y1 = i;
                listLine[listLine.Count - 1].Y2 = i;
                listLabel[listLabel.Count - 1].Margin = new Thickness(dCenterX, i, 0, 0);

                listLabel[listLabel.Count - 1].Foreground = TextColor;
                listLabel[listLabel.Count - 1].FontSize = nFontSize;

                m_gridRuler.Children.Add(listLabel[listLabel.Count - 1]);
                m_gridRuler.Children.Add(listLine[listLine.Count - 1]);
                nPitchCount++;
            }
            nPitchCount = 0;
            for (int i = (int)dCenterY; i > 0; i -= (int)(1000 / m_dResolutionY * m_dScale))
            {
                if (i == (int)dCenterY)
                {
                    nPitchCount++;
                    continue;
                }
                listLine.Add(new Line());
                listLabel.Add(new Label());

                listLine[listLine.Count - 1].Stroke = LineColor;
                listLine[listLine.Count - 1].StrokeThickness = 1;
                if (nPitchCount % 5 == 0)
                {
                    listLine[listLine.Count - 1].X1 = dCenterX - 10;
                    listLine[listLine.Count - 1].X2 = dCenterX + 10;
                    listLabel[listLabel.Count - 1].Content = nPitchCount.ToString("0 mm");
                }
                else
                {
                    listLine[listLine.Count - 1].X1 = dCenterX - 5;
                    listLine[listLine.Count - 1].X2 = dCenterX + 5;
                    if (!bFive)
                        listLabel[listLabel.Count - 1].Content = nPitchCount.ToString("0 mm");
                }
                listLine[listLine.Count - 1].Y1 = i;
                listLine[listLine.Count - 1].Y2 = i;
                listLabel[listLabel.Count - 1].Margin = new Thickness(dCenterX, i, 0, 0);
                listLabel[listLabel.Count - 1].Foreground = TextColor;
                listLabel[listLabel.Count - 1].FontSize = nFontSize;

                m_gridRuler.Children.Add(listLabel[listLabel.Count - 1]);
                m_gridRuler.Children.Add(listLine[listLine.Count - 1]);
                nPitchCount++;
            }

            ctrl_Grid.Children.Add(m_gridRuler);
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void DeInitRuler()
        @brief	Ruler DeInit.
        @return	void
        @param	void
        @remark	
         - Ruler에 설정 하였던 마우스 이벤트 해제 및 객체 제거.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:31
        */
        private void DeInitRuler()
        {
            try
            {
                m_gridRuler.MouseMove -= onMouseMove;
                m_gridRuler.MouseDown -= onMouseDown;
                m_gridRuler.MouseUp -= onMouseUp;

                while (m_gridRuler.Children.Count != 0)
                {
                    m_gridRuler.Children.RemoveAt(0);
                }

                ctrl_Grid.Children.Remove(m_gridRuler);
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		private void InitOveray()
        @brief	Overay Init.
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:32
        */
        private void InitOveray()
        {
            Brush TextColor = Brushes.Cyan;
            int nFontSize = 14;

            m_gridOverray.Visibility = Visibility.Hidden;
            m_gridOverray.MouseMove += onMouseMove;
            m_gridOverray.MouseDown += onMouseDown;
            m_gridOverray.MouseUp += onMouseUp;
            m_gridOverray.ColumnDefinitions.Add(new ColumnDefinition());
            m_gridOverray.ColumnDefinitions.Add(new ColumnDefinition());
            m_gridOverray.ColumnDefinitions.Add(new ColumnDefinition());
            m_gridOverray.RowDefinitions.Add(new RowDefinition());
            m_gridOverray.RowDefinitions.Add(new RowDefinition());
            m_gridOverray.RowDefinitions.Add(new RowDefinition());
            m_gridOverray.RowDefinitions.Add(new RowDefinition());
            m_gridOverray.Width = 200;
            m_gridOverray.Height = 60;
            m_gridOverray.HorizontalAlignment = HorizontalAlignment.Left;
            m_gridOverray.VerticalAlignment = VerticalAlignment.Top;

            Label lb = new Label();
            lb.Content = "Width : ";
            lb.HorizontalContentAlignment = HorizontalAlignment.Right;
            lb.Padding = new Thickness(0);
            lb.Foreground = TextColor;
            lb.FontSize = nFontSize;
            m_gridOverray.Children.Add(lb);
            Grid.SetColumn(lb, 0);
            Grid.SetRow(lb, 0);

            lb = new Label();
            lb.Content = "Height : ";
            lb.HorizontalContentAlignment = HorizontalAlignment.Right;
            lb.Padding = new Thickness(0);
            lb.Foreground = TextColor;
            lb.FontSize = nFontSize;
            m_gridOverray.Children.Add(lb);
            Grid.SetColumn(lb, 0);
            Grid.SetRow(lb, 1);

            lb = new Label();
            lb.Content = "Resolution X : ";
            lb.HorizontalContentAlignment = HorizontalAlignment.Right;
            lb.Padding = new Thickness(0);
            lb.Foreground = TextColor;
            lb.FontSize = nFontSize;
            m_gridOverray.Children.Add(lb);
            Grid.SetColumn(lb, 0);
            Grid.SetRow(lb, 2);

            lb = new Label();
            lb.Content = "Resolution X : ";
            lb.HorizontalContentAlignment = HorizontalAlignment.Right;
            lb.Padding = new Thickness(0);
            lb.Foreground = TextColor;
            lb.FontSize = nFontSize;
            m_gridOverray.Children.Add(lb);
            Grid.SetColumn(lb, 0);
            Grid.SetRow(lb, 3);

            m_lbWidth.Padding = new Thickness(0);
            m_lbWidth.Foreground = TextColor;
            m_lbWidth.HorizontalContentAlignment = HorizontalAlignment.Center;
            m_lbWidth.FontSize = nFontSize;
            m_lbHeight.Padding = new Thickness(0);
            m_lbHeight.Foreground = TextColor;
            m_lbHeight.HorizontalContentAlignment = HorizontalAlignment.Center;
            m_lbHeight.FontSize = nFontSize;
            m_lbResolutionX.Content = m_dResolutionX.ToString("0.00");
            m_lbResolutionX.Padding = new Thickness(0);
            m_lbResolutionX.Foreground = TextColor;
            m_lbResolutionX.HorizontalContentAlignment = HorizontalAlignment.Center;
            m_lbResolutionX.FontSize = nFontSize;

            m_lbResolutionY.Content = m_dResolutionX.ToString("0.00");
            m_lbResolutionY.Padding = new Thickness(0);
            m_lbResolutionY.Foreground = TextColor;
            m_lbResolutionY.HorizontalContentAlignment = HorizontalAlignment.Center;
            m_lbResolutionY.FontSize = nFontSize;

            m_gridOverray.Children.Add(m_lbWidth);
            m_gridOverray.Children.Add(m_lbHeight);
            m_gridOverray.Children.Add(m_lbResolutionX);
            m_gridOverray.Children.Add(m_lbResolutionY);

            Grid.SetColumn(m_lbWidth, 1);
            Grid.SetColumn(m_lbHeight, 1);
            Grid.SetColumn(m_lbResolutionX, 1);
            Grid.SetColumn(m_lbResolutionY, 1);
            Grid.SetRow(m_lbWidth, 0);
            Grid.SetRow(m_lbHeight, 1);
            Grid.SetRow(m_lbResolutionX, 2);
            Grid.SetRow(m_lbResolutionY, 3);

            lb = new Label();
            lb.Content = "px";
            lb.HorizontalContentAlignment = HorizontalAlignment.Left;
            lb.Padding = new Thickness(0);
            lb.Foreground = TextColor;
            lb.FontSize = nFontSize;
            m_gridOverray.Children.Add(lb);
            Grid.SetColumn(lb, 3);
            Grid.SetRow(lb, 0);

            lb = new Label();
            lb.Content = "px";
            lb.HorizontalContentAlignment = HorizontalAlignment.Left;
            lb.Padding = new Thickness(0);
            lb.Foreground = TextColor;
            lb.FontSize = nFontSize;
            m_gridOverray.Children.Add(lb);
            Grid.SetColumn(lb, 3);
            Grid.SetRow(lb, 1);

            lb = new Label();
            lb.Content = "um";
            lb.HorizontalContentAlignment = HorizontalAlignment.Left;
            lb.Padding = new Thickness(0);
            lb.Foreground = TextColor;
            lb.FontSize = nFontSize;
            m_gridOverray.Children.Add(lb);
            Grid.SetColumn(lb, 3);
            Grid.SetRow(lb, 2);

            lb = new Label();
            lb.Content = "um";
            lb.HorizontalContentAlignment = HorizontalAlignment.Left;
            lb.Padding = new Thickness(0);
            lb.Foreground = TextColor;
            lb.FontSize = nFontSize;
            m_gridOverray.Children.Add(lb);
            Grid.SetColumn(lb, 3);
            Grid.SetRow(lb, 3);

            ctrl_Grid.Children.Add(m_gridOverray);
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public IntPtr? fn_GetIamgePtr()
        @brief	Image IntPtr 얻기.
        @return	IntPtr? : nullable Image Pointer
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:32
        */
        public IntPtr? fn_GetImagePtr()
        {
            if (m_ImgBrs != null)
            {
                try
                {
                    WriteableBitmap bmp = m_ImgBrs.ImageSource as WriteableBitmap;
                    return bmp.BackBuffer;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            return null;
        }

        /**	
        @fn		public IntPtr? fn_GetImgOrgPtr()
        @brief	원본 이미지 포인터 얻기.
        @return	IntPtr? : 원본 이미지 포인터.
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/7  16:21
        */
        public IntPtr? fn_GetImgOrgPtr()
        {
            if (m_ImgBrsOrg != null)
            {
                try
                {
                    WriteableBitmap bmp = m_ImgBrsOrg.ImageSource as WriteableBitmap;
                    return bmp.BackBuffer;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            return null;
        }

        /**	
        @fn		public void fn_OriginalImage()
        @brief	이미지 원본으로.
        @return	void
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/7  16:20
        */
        public void fn_OriginalImage(bool bFlag = false)
        {
            if (m_ImgBrsOrg != null)
            {
                if (!bFlag)
                {
                    lib_Canvas.Width = m_ImgBrsOrg.ImageSource.Width;
                    lib_Canvas.Height = m_ImgBrsOrg.ImageSource.Height;

                    m_nWidth = (m_ImgBrsOrg.ImageSource as WriteableBitmap).PixelWidth;
                    m_nHeight = (m_ImgBrsOrg.ImageSource as WriteableBitmap).PixelHeight;
                    m_nChannel = 1;
                }
                lib_Canvas.Background = m_ImgBrsOrg;
                m_dScale = myScaleTransform.ScaleX;
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public WriteableBitmap fn_GetImageStream()
        @brief	Image Steam 얻기.
        @return	WriteableBitmap : Control의 이미지.
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:33
        */
        public WriteableBitmap fn_GetImageStream(bool bOrg = false)
        {
            if (m_ImgBrs != null)
            {
                try
                {
                    WriteableBitmap bmp = null;
                    if (bOrg)
                        bmp = m_ImgBrsOrg.ImageSource as WriteableBitmap;
                    else
                        bmp = m_ImgBrs.ImageSource as WriteableBitmap;
                    return bmp;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    return null;
                }
            }
            return null;
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
        public void fn_SaveImage(string strPath, bool bOrigin = false)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Normal, new Action(delegate ()
            {
                if (m_ImgBrs != null)
                {
                    try
                    {
                        WriteableBitmap bmp;
                        if (bOrigin)
                            bmp = m_ImgBrsOrg.ImageSource as WriteableBitmap;
                        else
                            bmp = m_ImgBrs.ImageSource as WriteableBitmap;

                        using (FileStream stream = new FileStream(strPath, FileMode.Create))
                        {
                            //PngBitmapEncoder encoder = new PngBitmapEncoder();
                            BmpBitmapEncoder encoder = new BmpBitmapEncoder();

                            encoder.Frames.Add(BitmapFrame.Create(bmp));
                            encoder.Save(stream);
                        }
                    }
                    catch (Exception ex)
                    {
                        fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                    }
                }
            }));
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		void fn_SetModelRect(Rect rect)
        @brief	Control에 Model Rect 설정.
        @return	void
        @param	Rect rect : ROI 영역.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:35
        */
        public void fn_SetModelRect(Rect rect)
        {
            try
            {
                Rectangle rectangle = lib_Canvas.Children[m_nSelObj] as Rectangle;
                double dPointGapX = rect.X - Canvas.GetLeft(rectangle);
                double dPointGapY = rect.Y - Canvas.GetTop(rectangle);
                if (rectangle != null)
                {
                    if (m_nSelObj == (int)EnObjectSelect.SelModel && m_bLinkMil)
                    {
                        Rect[] rectMil = new Rect[(int)EnObjectSelect.SelMil10 - (int)EnObjectSelect.SelMil1 + 1];
                        for (int i = (int)EnObjectSelect.SelMil1; i <= (int)EnObjectSelect.SelMil10; i++)
                        {
                            rectMil[i - (int)EnObjectSelect.SelMil1].X = Canvas.GetLeft(lib_Canvas.Children[i]) + dPointGapX;
                            rectMil[i - (int)EnObjectSelect.SelMil1].Y = Canvas.GetTop(lib_Canvas.Children[i]) + dPointGapY;
                            rectMil[i - (int)EnObjectSelect.SelMil1].Width = (lib_Canvas.Children[i] as Rectangle).Width;
                            rectMil[i - (int)EnObjectSelect.SelMil1].Height = (lib_Canvas.Children[i] as Rectangle).Height;
                            if (lib_Canvas.Children[i].Visibility == Visibility.Visible)
                            {
                                Canvas.SetLeft(lib_Canvas.Children[i], rectMil[i - (int)EnObjectSelect.SelMil1].X);
                                Canvas.SetTop(lib_Canvas.Children[i], rectMil[i - (int)EnObjectSelect.SelMil1].Y);
                            }
                        }
                        delMilling?.Invoke(rectMil);
                    }
                    Canvas.SetLeft(rectangle, rect.X);
                    Canvas.SetTop(rectangle, rect.Y);
                    rectangle.Width = rect.Width;
                    rectangle.Height = rect.Height;
                    DrawRectGrid();
                }
            }
            catch (Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void fn_SetResult(ST_ALIGN_RESULT Result)
        @brief	Search 결과를 표시하기 위해 Control에 Data Set.
        @return	void
        @param	ST_ALIGN_RESULT Result : 결과 데이터.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:36
        */
        public void fn_SetResult(ST_ALIGN_RESULT Result, bool bMaxScore = false)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate ()
            {
                //if (this.IsLoaded)
                {
                    double dMaxScore = 0.0;
                    int nMaxScoreIdx = -1;
                    m_cvsResult.Width = lib_Canvas.ActualWidth;
                    m_cvsResult.Height = lib_Canvas.ActualHeight;

                    for (int i = 0; i < Result.stResult.Length; i++)
                    {
                        if (Result.stResult[i].dScore > dMaxScore)
                        {
                            nMaxScoreIdx = i;
                            dMaxScore = Result.stResult[i].dScore;
                        }
                        Canvas.SetLeft((m_cvsResult.Children[i] as Grid), Result.stResult[i].dX - Result.stResult[i].dWidth / 2.0);
                        Canvas.SetTop((m_cvsResult.Children[i] as Grid), Result.stResult[i].dY - Result.stResult[i].dHeight / 2.0);
                        //(m_cvsResult.Children[i] as Grid).Margin = new Thickness(Result.stResult[i].nX, Result.stResult[i].nY, 0, 0);
                        if (Result.stResult[i].dScore > 0)
                        {
                            (m_cvsResult.Children[i] as Grid).Width = Result.stResult[i].dWidth;
                            (m_cvsResult.Children[i] as Grid).Height = Result.stResult[i].dHeight;
                            (m_cvsResult.Children[i] as Grid).Visibility = Visibility.Visible;
                        }
                        else
                        {
                            (m_cvsResult.Children[i] as Grid).Width = 0;
                            (m_cvsResult.Children[i] as Grid).Height = 0;
                            (m_cvsResult.Children[i] as Grid).Visibility = Visibility.Hidden;
                        }
                        ((m_cvsResult.Children[i] as Grid).Children[0] as Rectangle).Stroke = Brushes.Magenta;
                        ((m_cvsResult.Children[i] as Grid).Children[0] as Rectangle).StrokeThickness = 2;
                        ((m_cvsResult.Children[i] as Grid).Children[1] as TextBlock).Foreground = Brushes.Magenta;
                        ((m_cvsResult.Children[i] as Grid).Children[1] as TextBlock).FontSize = m_dResultFontSize;
                        ((m_cvsResult.Children[i] as Grid).Children[1] as TextBlock).TextWrapping = TextWrapping.Wrap;

                        // Theta 처리.
                        // ±180
                        if (Result.stResult[i].dAngle > 180)
                            Result.stResult[i].dAngle -= 360;
                        // LEE/201120 [Modify]
                        // ±90
                        if (Result.stResult[i].dAngle > 90)
                            Result.stResult[i].dAngle -= 180;
                        if (Result.stResult[i].dAngle < -90)
                            Result.stResult[i].dAngle += 180;

                        ((m_cvsResult.Children[i] as Grid).Children[1] as TextBlock).Text = $"{Result.stResult[i].dScore:F2}% {Result.stResult[i].dAngle:F2}˚";
                    }

                    // Max Score View 처리.
                    if (bMaxScore)
                    {
                        if (nMaxScoreIdx > -1)
                        {
                            for (int i = 0; i < Result.stResult.Length; i++)
                            {
                                if (nMaxScoreIdx != i)
                                    (m_cvsResult.Children[i] as Grid).Visibility = Visibility.Hidden;
                            }
                        }
                    }
                }
            }));
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void fn_ClearResult()
        @brief	결과 초기화.
        @return	void
        @param	void
        @remark	
         - 결과 데이터를 초기화 하여. 화면에서 치움.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:39
        */
        public void fn_ClearResult()
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Render, new Action(delegate ()
            {
                for (int i = 0; i < m_cvsResult.Children.Count; i++)
                {
                    Canvas.SetLeft((m_cvsResult.Children[i] as Grid), 0);
                    Canvas.SetTop ((m_cvsResult.Children[i] as Grid), 0);
                    ((m_cvsResult.Children[i] as Grid).Children[0] as Rectangle).Stroke = Brushes.Magenta;
                    ((m_cvsResult.Children[i] as Grid).Children[0] as Rectangle).StrokeThickness = 2;
                    ((m_cvsResult.Children[i] as Grid).Children[1] as TextBlock).Text = "";
                    ( m_cvsResult.Children[i] as Grid).Visibility = Visibility.Hidden;
                }
            }));
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void SetRectangle(Rect[] rect)
        @brief	Rect 정보 Set.
        @return	void
        @param	Rect[] rect : Model, Search, Milling등의 정보.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:39
        */
        public void SetRectangle(Rect[] rect)
        {
            fn_ClearResult();
            if (rect != null)
            {
                for (int i = 0; i < rect.Length; i++)
                {
                    //lib_Canvas.Children[(int)EnObjectSelect.SelModel + i].TranslatePoint(new Point(0, 0), lib_Canvas);
                    Canvas.SetLeft(lib_Canvas.Children[(int)EnObjectSelect.SelModel + i], rect[i].X);
                    Canvas.SetTop(lib_Canvas.Children[(int)EnObjectSelect.SelModel + i], rect[i].Y);
                    (lib_Canvas.Children[(int)EnObjectSelect.SelModel + i] as Rectangle).Width = rect[i].Width;
                    (lib_Canvas.Children[(int)EnObjectSelect.SelModel + i] as Rectangle).Height = rect[i].Height;

                    m_rectROI[i].X = Canvas.GetLeft(lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)]);
                    m_rectROI[i].Y = Canvas.GetTop(lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)]);
                    m_rectROI[i].Width = (lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)] as Rectangle).Width;
                    m_rectROI[i].Height = (lib_Canvas.Children[(int)(EnObjectSelect.SelModel + i)] as Rectangle).Height;
                }
            }
        }



        private void UserControl_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            //             if (Keyboard.IsKeyDown(Key.LeftCtrl))
            //             {
            //                 SetMove(EnRoiMode.ModeMove);
            //             }
            // 
            //             if (Keyboard.IsKeyDown(Key.LeftShift))
            //             {
            //                 m_bShiftDown = true;
            //             }
        }

        private void UserControl_PreviewKeyUp(object sender, KeyEventArgs e)
        {
            //             SetMove(EnRoiMode.ModeDraw);
            //             m_bShiftDown = false;
        }

        private void UserControl_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (m_bShiftDown)
            {
                double dZoom = GetZoomScale();
                if (e.Delta > 0)
                {
                    ZoomScale(dZoom + 0.1);
                }
                else
                {
                    ZoomScale(dZoom - 0.1);
                }
            }
        }

        public void ClearCanvas()
        {
            if (lib_Canvas != null)
            {
                lib_Canvas.Background = Brushes.Transparent;
                m_ImgBrs = null;
                m_ImgBrsOrg = null;
            }
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

        private void bn_Scale_Click(object sender, RoutedEventArgs e)
        {
            SetFitScale();
        }

        private void bn_Mode_Click(object sender, RoutedEventArgs e)
        {
            switch(m_enMode)
            {
                case EnRoiMode.ModeDraw:
                    SetMove(EnRoiMode.ModeMove);
                    break;
                case EnRoiMode.ModeMove:
                    SetMove(EnRoiMode.ModeDraw);
                    break;
            }
        }

        public bool fn_CheckROIModelIntoSearch()
        {
            bool bRet = true;
            if (this.IsLoaded)
            {
                double dModelX = Canvas.GetLeft(lib_Canvas.Children[(int)EnObjectSelect.SelModel]);
                double dModelY = Canvas.GetTop(lib_Canvas.Children[(int)EnObjectSelect.SelModel]);
                double dModelW = (lib_Canvas.Children[(int)EnObjectSelect.SelModel] as Rectangle).Width;
                double dModelH = (lib_Canvas.Children[(int)EnObjectSelect.SelModel] as Rectangle).Height;
                double dSearchX = Canvas.GetLeft(lib_Canvas.Children[(int)EnObjectSelect.SelSearch]);
                double dSearchY = Canvas.GetTop(lib_Canvas.Children[(int)EnObjectSelect.SelSearch]);
                double dSearchW = (lib_Canvas.Children[(int)EnObjectSelect.SelSearch] as Rectangle).Width;
                double dSearchH = (lib_Canvas.Children[(int)EnObjectSelect.SelSearch] as Rectangle).Height;

                if (dModelX < dSearchX || dModelY < dSearchY || dModelX + dModelW > dSearchX + dSearchW || dModelY + dModelH > dSearchY + dSearchH)
                    bRet = false;

                // SearchROI 의 크기가 0인경우는 무시.
                if (dSearchW == 0 || dSearchH == 0)
                    bRet = true;
            }
            return bRet;
        }
        public void fn_SetLink(bool bLink)
        {
            m_bLinkMil = bLink;
        }

        public void fn_EnableDragButton(bool bEnable)
        {
            bn_Mode.IsEnabled = bEnable;
        }
    }
}
