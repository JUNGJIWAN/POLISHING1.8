using System;
using Basler.Pylon;
using static WaferPolishingSystem.Define.UserFunction;
using WaferPolishingSystem.Define;
using System.Threading;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;

namespace WaferPolishingSystem.Vision
{
    public delegate void del_Update(byte[] buff, int width, int height);
    public class CameraManager
    {
        Camera _camera;
        public double _Width = 0.0;
        public double _Height = 0.0;
        public IFloatParameter _paramGain, _paramExposure;
        public IIntegerParameter _paramGainRaw, _paramExposureRaw;
        public del_Update update = null;
        public bool _bConnect = false;
        private bool bGrabLive = false;
        public bool IsGrabLive
        {
            get { return bGrabLive; }
        }
        string Title = "CameraManager";
        byte[] FrameBuffer;
        WriteableBitmap m_SimulFrame = null;
        private int m_nCameraRetryCount = 0;
        public int CameraRetryCount
        {
            get { return m_nCameraRetryCount; }
            set
            {
                m_nCameraRetryCount = value;
            }
        }
        /**	
        @fn     Class 생성자
        @brief	
        @return	
        @param	
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/26  21:37
        */
        public CameraManager()
        {
            _bConnect = false;
        }

        /**	
        @fn		public bool fn_OpenCam()
        @brief	카메라 Open.
        @return	성공 여부.
        @param	void
        @remark	
         - 카메라 버퍼 셋팅   : 8192 byte
         - 타임아웃          : 1000 ms
         - Connection Lost 감지.
         - Exposure Time Parameter 얻음.
         - Grab Event Set
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  17:19
        */
        public bool fn_OpenCam()
        {
            bool bRet = false;
            if (!_bConnect)
            {
                try
                {
                    if (_camera == null)
                    {
                        _camera = new Camera(DeviceType.GigE, CameraSelectionStrategy.FirstFound);
                        _camera.CameraOpened += Configuration.AcquireContinuous;
                        fn_WriteLog(this.Title + " : OpenCam_Cam Created.", UserEnum.EN_LOG_TYPE.ltVision);
                    }
                    if (!_camera.IsOpen)
                    {
                        _camera.Open();
                        fn_WriteLog(this.Title + " : OpenCam_Cam Open OK.", UserEnum.EN_LOG_TYPE.ltVision);
                    }
                    else
                    {
                        fn_WriteLog(this.Title + " : OpenCam_Cam is Already Opened.", UserEnum.EN_LOG_TYPE.ltVision);
                    }
                    _camera.Parameters[PLTransportLayer.HeartbeatTimeout].TrySetValue(1000, IntegerValueCorrection.Nearest);  // 1000 ms timeout
                    _camera.ConnectionLost += fn_CamConnLost;
                    _camera.Parameters[PLCameraInstance.MaxNumBuffer].SetValue(5);
#if !DEBUG
                    _camera.Parameters[PLCamera.GevSCPSPacketSize].SetValue(9000);
                    _camera.Parameters[PLCamera.ReverseX].SetValue(true);
                    _camera.Parameters[PLCamera.ReverseY].SetValue(true);
#endif
                    double.TryParse(_camera.Parameters["Width"].ToString(), out _Width);
                    double.TryParse(_camera.Parameters["Height"].ToString(), out _Height);

                    _paramExposureRaw = _camera.Parameters[PLCamera.ExposureTimeRaw];
                    _paramGainRaw = _camera.Parameters[PLCamera.GainRaw];

                    //_paramExposure = _camera.Parameters[PLCamera.ExposureTime];
                    //_paramGain     = _camera.Parameters[PLCamera.Gain];
                    //_paramGain     = _camera.Parameters[PLCamera.GainAbs];

                    _camera.StreamGrabber.ImageGrabbed += OnImageGrabed;

                    _bConnect = true;
                }
                catch (Exception ex)
                {
                    fn_WriteLog(this.Title + " : OpenCam_Exception " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                    if (_camera != null)
                    {
                        _camera.ConnectionLost -= fn_CamConnLost;
                        _camera.Close();
                    }
                    _camera = null;
                    _bConnect = false;
                }
            }
            else
            {
                //fn_WriteLog(this.Title + " : OpenCam_Camera Already Connected.", UserEnum.EN_LOG_TYPE.ltVision);
            }
            return bRet;
        }

        /**	
        @fn		public bool fn_GrabStart(int mode = 0)
        @brief	Image Grab Start.
        @return	성공 여부.
        @param	int mode : Grab Mode ( 0 = Oneshot, 1 = Live )
        @remark	
         - mode에 따라 Grab 시작.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  18:10
        */
        public bool fn_GrabStart(int mode = 0)
        {
            bool bRet = false;
            // Grab모드를 member 변수에 저장.
            bGrabLive = mode == 1 ? true : false;
            if (_camera != null)
            {
                try
                {
                    if (!_camera.StreamGrabber.IsGrabbing)
                    {

                        switch (mode)
                        {
                            case 0:
                                _camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.SingleFrame);
                                _camera.StreamGrabber.Start(1, GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                                bRet = true;
                                fn_WriteLog(this.Title + " : GrabStart_OneShot", UserEnum.EN_LOG_TYPE.ltVision);
                                break;
                            case 1:
                                _camera.Parameters[PLCamera.AcquisitionMode].SetValue(PLCamera.AcquisitionMode.Continuous);
                                _camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
                                bRet = true;
                                fn_WriteLog(this.Title + " : GrabStart_Live", UserEnum.EN_LOG_TYPE.ltVision);
                                break;
                            default:
                                bRet = false;
                                fn_WriteLog(this.Title + " : GrabStart_Invalid Grab Mode", UserEnum.EN_LOG_TYPE.ltVision);
                                break;
                        }

                    }
                    else
                        fn_WriteLog(this.Title + " : GrabStart_Camera Is Grabbing.", UserEnum.EN_LOG_TYPE.ltVision);
                }
                catch (Exception ex)
                {
                    fn_WriteLog(this.Title + " : GrabStart_" + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
            else
                fn_WriteLog(this.Title + " : GrabStart_camera is Null.", UserEnum.EN_LOG_TYPE.ltVision);
            return bRet;
        }


        /**	
        @fn		public bool fn_GrabStop()
        @brief	Grab Stop.
        @return	성공 여부.
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  18:30
        */
        public bool fn_GrabStop()
        {
            bool bRet = false;
            if (_camera != null)
            {
                try
                {
                    if (_camera.IsOpen)
                    {
                        if (_camera.StreamGrabber.IsGrabbing)
                        {
                            bGrabLive = false;
                            _camera.StreamGrabber.Stop();
                            bRet = true;
                            fn_WriteLog(this.Title + " : GrabStop_GrabStop OK", UserEnum.EN_LOG_TYPE.ltVision);
                        }
                    }
                }
                catch (Exception ex)
                {
                    fn_WriteLog(this.Title + " : GrabStop_" + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
            else
            {
                fn_WriteLog(this.Title + " : GrabStop_Camera Is Null.", UserEnum.EN_LOG_TYPE.ltVision);
            }
            
            return bRet;
        }

        /**	
        @fn		public bool fn_CloseCam()
        @brief	카메라 객체 해제.
        @return	성공 여부.
        @param	void
        @remark	
         - Grab Event 해제 수행.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  18:31
        */
        public bool fn_CloseCam()
        {
            bool bRet = false;
            if (_camera != null)
            {
                try
                {
                    _camera.Close();
                    fn_WriteLog(this.Title + " : CloseCam_Close OK.", UserEnum.EN_LOG_TYPE.ltVision);
                    bRet = true;
                }
                catch (Exception ex)
                {
                    fn_WriteLog(this.Title + " : CloseCam_" + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
                }
                bGrabLive = false;
            }
            else
                fn_WriteLog(this.Title + " : CloseCam_Camera is Null.", UserEnum.EN_LOG_TYPE.ltVision);
            return bRet;
        }

        /**	
        @fn		public bool fn_SetExposure(double dValue)
        @brief	Exposure Time Set.
        @return	성공 여부.
        @param	double dValue : Exposure Time.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  18:35
        */
        public bool fn_SetExposure(double dValue)
        {
            bool bRet = false;
            if (_camera != null)
            {
                if (_camera.IsOpen)
                {
                    //_camera.Parameters[PLCamera.ExposureTimeMode].SetValue(PLCamera.ExposureTimeMode.Standard);
                    // Set the exposure time to 3500 microseconds
                    //_camera.Parameters[PLCamEmuCamera.ExposureTimeAbs]
                    _camera.Parameters[PLCamera.ExposureTimeRaw].SetValue((long)dValue);
                    //_paramExposureRaw.SetValue((long)dValue);
                    bRet = true;
                    fn_WriteLog(this.Title + " : SetExposure_OK. " + dValue, UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
            return bRet;
        }

        /**	
        @fn		public bool fn_SetGain(double dValue)
        @brief	Gain Set.
        @return	성공 여부.
        @param	double dValue : Gain Value.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  18:37
        */
        public bool fn_SetGain(double dValue)
        {
            bool bRet = false;
            if (_camera != null)
            {
                if (_camera.IsOpen)
                {
                    _camera.Parameters[PLCamera.GainRaw].SetValue((long)dValue);
                    //_paramGainRaw.SetValue((long)dValue);
                    bRet = true;
                    fn_WriteLog(this.Title + " : SetGain_OK. " + dValue, UserEnum.EN_LOG_TYPE.ltVision);
                }
            }
            return bRet;
        }

        /**	
        @fn		public void fn_CamConnLost(object sender, EventArgs e)
        @brief	Camera Connection Lost 함수.
        @return	void.
        @param	object      sender  : 
        @param	EventArgs   e       : 
        @remark	
         - Connection Lost시 Connection Flag False.
         - 연결시 Time Out 설정 되어 있어야함.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  18:38
        */
        public void fn_CamConnLost(object sender, EventArgs e)
        {
            fn_WriteLog(this.Title + " : CamConnLost_Lost Connection!", UserEnum.EN_LOG_TYPE.ltVision);
            CameraRetryCount = 0;
            _bConnect = false;
        }

        /**	
        @fn		public bool fn_GetGrabbingState()
        @brief	StreamGrabber의 IsGrabbing Flag 반환.
        @return	IsGrabbing Flag.
        @param	void.
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  18:39
        */
        public bool fn_GetGrabbingState()
        {
            bool bRet = false;
            if (_camera != null)
            {
                if (_camera.IsOpen)
                {
                    bRet = _camera.StreamGrabber.IsGrabbing;
                }
            }
            return bRet;
        }

        /**	
        @fn		public void fn_SetSimulFrame(WriteableBitmap wb)
        @brief	Simul Frame Setting
        @return	void
        @param	WriteableBitmap wb : Setting Image
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/5  16:21
        */
        public void fn_SetSimulFrame(WriteableBitmap wb)
        {
            if(wb != null)
                m_SimulFrame = wb.Clone();
        }

        /**
        @fn     public WriteableBitmap fn_GetSimulFrame()
        @brief	Setting 된 Simul Frame 얻기.
        @return	WriteableBitmap : Simul Frame
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/10/6  9:50
        */
        public WriteableBitmap fn_GetSimulFrame()
        {
            return m_SimulFrame;
        }
        /**	
        @fn		public WriteableBitmap fn_GetFrame()
        @brief	단일 프레임 얻기.
        @return	WriteableBitmap : 얻은 프레임.
        @param	void
        @remark	
         - 
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/4/5  18:41
        */
        public WriteableBitmap fn_GetFrame(bool bNewFrame = true)
        {
            WriteableBitmap wbm = null;
            if (bNewFrame)
            {
                if(!fn_GrabStart())
                {
                    fn_WriteLog($"{this.Title} : Grab Fail.", UserEnum.EN_LOG_TYPE.ltVision);
                    FrameBuffer = null;
                }
                Thread.Sleep(1000);
            }

            if (FrameBuffer != null)
            {
                wbm = new WriteableBitmap((int)_Width, (int)_Height, 96, 96, PixelFormats.Gray8, null);
                wbm.Lock();
                wbm.WritePixels(new Int32Rect(0, 0, (int)_Width, (int)_Height), FrameBuffer, (int)_Width, 0);
                wbm.AddDirtyRect(new Int32Rect(0, 0, (int)_Width, (int)_Height));
                wbm.Unlock();
            }

            return wbm;
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public void OnImageGrabed(object sender, ImageGrabbedEventArgs e)
        @brief	Basler Camera Callback Function.
        @return	void
        @param	object                  sender  : 
        @param	ImageGrabbedEventArgs   e       : Image Grab Result.
        @remark	
         - 이미지 Grab시 해당 Callback을 Camera에 Set 해야함.
         - 200428_수정.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/3/9  18:40
        */
        public void OnImageGrabed(object sender, ImageGrabbedEventArgs e)
        {
            try
            {
                IGrabResult result = e.GrabResult;
                if (result.IsValid)
                {
                    FrameBuffer = result.PixelData as byte[];

                    //isGrabbed = true;
                    update?.Invoke(FrameBuffer, result.Width, result.Height);
                    Console.WriteLine($"Cam Grabed!{FrameBuffer[0]}");
                    //GC.Collect();
                }
                else
                {
                    //isGrabbed = false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                e.DisposeGrabResultIfClone();
            }
        }
    }

}
