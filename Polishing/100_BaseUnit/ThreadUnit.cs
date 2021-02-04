using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using static WaferPolishingSystem.FormMain;
using static WaferPolishingSystem.Define.UserClass;
using WaferPolishingSystem.Define;

namespace WaferPolishingSystem.BaseUnit
{
    class ThreadUnit
    {
        //
        const int MAX_THREAD        =  11; //Max Thread
                                    
        const int THREAD_INTERVAL0  =  10;
        const int THREAD_INTERVAL1  = 100; //Log
        const int THREAD_INTERVAL2  =  50; //MOTOR
        const int THREAD_INTERVAL3  =  20; //IO
        const int THREAD_INTERVAL4  =  20; //Load Cell Update
        const int THREAD_INTERVAL5  =  30; //PMC
        const int THREAD_INTERVAL6  =  100; //Slurry
        const int THREAD_INTERVAL7  =  50;  //RFID, REST
        
        //---------------------------------------------------------------------------
        // Vision
        const int THREAD_INTERVAL8  =  100;
        const int THREAD_INTERVAL9  =  5000;
                                    
        const int THREAD_INTERVAL10 =  10; //Watch Dog

        const int TH0  =  0;
        const int TH1  =  1;
        const int TH2  =  2;
        const int TH3  =  3;
        const int TH4  =  4;
        const int TH5  =  5;
        const int TH6  =  6;
        const int TH7  =  7;
        const int TH8  =  8;
        const int TH9  =  9;
        const int TH10 = 10;

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //Var
        bool m_bIOOn; 
        private bool[] m_bThreadRun     = new bool[MAX_THREAD];
        private bool[] m_bWatchDog      = new bool[MAX_THREAD]; //Thread Check
        private bool[] m_bWatchDog_1    = new bool[MAX_THREAD]; //Thread Check
        
        //Thread
        private Thread[] m_ThreaProcess = new Thread[MAX_THREAD]; 

        //Timer
        private uint[] m_nStartTime = new uint[MAX_THREAD];
        public  uint[] m_nScanTime  = new uint[MAX_THREAD];

        public  double[] m_dScanTime  = new double[MAX_THREAD];

        

        //
        Object Obj = new Object();

        public int m_TryCount_Cam = 0;

        //
        private TOnDelayTimer[] m_tDelayTimer = new TOnDelayTimer[MAX_THREAD];
        private TOnDelayTimer[] m_tChkTimer   = new TOnDelayTimer[MAX_THREAD];

        //Property
        public uint this[int index] 
        { 
            get { return m_nScanTime[index]; } 
            /*
            set 
            {
                if (index > m_nScanTime.Length) Array.Resize<uint>(ref m_nScanTime, index + 1);
                m_nScanTime[index] = value; 
            }
            */
        }



        //---------------------------------------------------------------------------
        //생성자
        public ThreadUnit()
        {
            for (int i=0; i<MAX_THREAD; i++)
            {
                m_bThreadRun [i] = false;
                m_tDelayTimer[i] = new TOnDelayTimer();
                m_tDelayTimer[i].Clear();

                m_nStartTime [i] = new uint();
                m_nScanTime  [i] = new uint();

                m_nStartTime [i] = 0;
                m_nScanTime  [i] = 0;

                m_bWatchDog  [i] = false;
                m_bWatchDog_1[i] = false;

                m_tChkTimer  [i] = new TOnDelayTimer();
                m_tChkTimer  [i].Clear();

                m_dScanTime  [i] = new double();
                m_dScanTime  [i] = 0.0;

            }

            
            //Main
            m_ThreaProcess[TH0] = new Thread(new ThreadStart(THREAD_PROCESS_0));
            m_ThreaProcess[TH0].IsBackground = false;

            //Log
            m_ThreaProcess[TH1] = new Thread(new ThreadStart(THREAD_PROCESS_1));
            m_ThreaProcess[TH1].IsBackground = false;

            //MOTOR
            m_ThreaProcess[TH2] = new Thread(new ThreadStart(THREAD_PROCESS_2));
            m_ThreaProcess[TH2].IsBackground = false;

            //IO
            m_ThreaProcess[TH3] = new Thread(new ThreadStart(THREAD_PROCESS_3));
            m_ThreaProcess[TH3].IsBackground = false;

            //LoadCell
            m_ThreaProcess[TH4] = new Thread(new ThreadStart(THREAD_PROCESS_4));
            m_ThreaProcess[TH4].IsBackground = false;

            //PMC
            m_ThreaProcess[TH5] = new Thread(new ThreadStart(THREAD_PROCESS_5));
            m_ThreaProcess[TH5].IsBackground = false;

            //PMC
            m_ThreaProcess[TH6] = new Thread(new ThreadStart(THREAD_PROCESS_6));
            m_ThreaProcess[TH6].IsBackground = false;

            //RFID, REST
            m_ThreaProcess[TH7] = new Thread(new ThreadStart(THREAD_PROCESS_7));
            m_ThreaProcess[TH7].IsBackground = false;


            // Light Connection.
            m_ThreaProcess[TH8] = new Thread(new ThreadStart(THREAD_PROCESS_8));
            m_ThreaProcess[TH8].IsBackground = false;
            // Camera Connection.
            m_ThreaProcess[TH9] = new Thread(new ThreadStart(THREAD_PROCESS_9));
            m_ThreaProcess[TH9].IsBackground = false;

            //Watch Dog
            m_ThreaProcess[TH10] = new Thread(new ThreadStart(THREAD_PROCESS_10));
            m_ThreaProcess[TH10].IsBackground = false;


            //
            m_bIOOn = false; 
        }
        //---------------------------------------------------------------------------
        //Start Thread
        public void fn_StartThread()
        {
            //Start 
            m_bThreadRun  [TH0] = true; //Main Process
            m_bThreadRun  [TH1] = true; //Log
            m_bThreadRun  [TH2] = true; //Motor
            m_bThreadRun  [TH3] = true; //IO
            m_bThreadRun  [TH4] = true; //LoadCell
            m_bThreadRun  [TH5] = true; //PMC
            m_bThreadRun  [TH6] = true; //Slurry
            m_bThreadRun  [TH7] = true; //RFID, REST

			m_bThreadRun  [TH8] = true; // Light
            m_bThreadRun  [TH9] = true; // Cam

            m_bThreadRun  [TH10] = false; // Watch Dog

            //Thread Start
            for (int i = 0; i < MAX_THREAD; i++)
            {
                if(m_bThreadRun[i]) m_ThreaProcess[i].Start();
                m_nStartTime[i] = (uint)Environment.TickCount;
            }

        }
        
        //---------------------------------------------------------------------------
        //End Thread
        public void fn_StopThread()
        {
            for (int t = 0; t < MAX_THREAD; t++)
            {
                if (m_bThreadRun[t])
                {
                    m_bThreadRun[t] = false;
                    if(!m_ThreaProcess[t].Join(1000))
                    {
                        m_ThreaProcess[t].Abort();
                    }
                }
            }
        }
        //-------------------------------------------------------------------------------------------------
        public double fn_GetScanTime(int thread)
        {
            return m_dScanTime[thread];
        }
        //---------------------------------------------------------------------------
        //Main Process
        private void THREAD_PROCESS_0()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;

            //
            try
            {
                while (m_bThreadRun[TH0])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        

                        //-----------------------------------------------
                        Monitor.Enter(Obj);
                        try
                        {
                            m_bWatchDog[TH0] = false;

                            //Main Sequence
                            if (m_bIOOn) SEQ.fn_Update();
                            //SEQ.fn_Update1();

                            m_bWatchDog[TH0] = true;
                        }
                        finally
                        {
                            Monitor.Exit(Obj);
                        }
                        //-----------------------------------------------

                        

                        bGo = true;
                        m_tDelayTimer[TH0].Clear();
                        

                    }

                    //if (Scan < THREAD_INTERVAL0)
                    //{
                    //    if (!m_tDelayTimer[TH0].OnDelay(bGo, THREAD_INTERVAL0 - Scan)) continue;
                    //}

                    bGo = false;

                    m_dScanTime[TH0] = TICK._GetTickTime() - Start;
                    Start            = TICK._GetTickTime();


                    //Scan Time Check
                    m_nScanTime [TH0] = (uint)Environment.TickCount - m_nStartTime[TH0];
                    m_nStartTime[TH0] = (uint)Environment.TickCount;

                }

            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH0] = false;

                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace("[THREAD_PROCESS_0]", ex);
            }

        }

        //---------------------------------------------------------------------------
        //Log Process
        private void THREAD_PROCESS_1()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;

            //
            try
            {
                while (m_bThreadRun[TH1])
                {
                    Thread.Sleep(30);

                    if (!bGo)
                    {
                        

                        m_bWatchDog[TH1] = false;

                        //Log
                        LOG.fn_UpdateLog();

                        m_bWatchDog[TH1] = true;

                        
                        bGo = true;
                        m_tDelayTimer[TH1].Clear();

                    }

                    //if(Scan < THREAD_INTERVAL1)
                    //{
                    //    if (!m_tDelayTimer[TH1].OnDelay(bGo, THREAD_INTERVAL1 - Scan)) continue;
                    //}

                    bGo = false;


                    m_dScanTime[TH1] = TICK._GetTickTime() - Start;
                    Start            = TICK._GetTickTime();


                    //Scan Time Check
                    m_nScanTime [TH1] = (uint)(TICK._GetTickTime() - m_nStartTime[TH1]); 
                    m_nStartTime[TH1] = (uint) TICK._GetTickTime();
                }
            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH1] = false;
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace("[THREAD_PROCESS_1]", ex);
            }

        }
        //---------------------------------------------------------------------------
        //MANUAL, Sequence
        private void THREAD_PROCESS_2()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;

            //
            try
            {
                while (m_bThreadRun[TH2])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        Start = TICK._GetTickTime();
                        
                        m_bWatchDog[TH2] = false;
                        
                        //Motor Data Update
                        MOTR.fn_Update(SEQ._bRun);

                        m_bWatchDog[TH2] = true;

                        Scan = TICK._GetTickTime() - Start; 
                        bGo = true;
                        m_tDelayTimer[TH2].Clear();

                    }

                    if (Scan < THREAD_INTERVAL2)
                    {
                        if (!m_tDelayTimer[TH2].OnDelay(bGo, THREAD_INTERVAL2 - Scan)) continue;
                    }

                    bGo = false;
                   

                    //Scan Time Check
                    m_nScanTime [TH2] = (uint)(TICK._GetTickTime() - m_nStartTime[TH2]); 
                    m_nStartTime[TH2] = (uint) TICK._GetTickTime() ;

                }
            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH2] = false;
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace("[THREAD_PROCESS_2]", ex);
            }
        }
        //---------------------------------------------------------------------------
        //IO Update
        private void THREAD_PROCESS_3()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;
            
            //
            try
            {
                while (m_bThreadRun[TH3])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        Start = TICK._GetTickTime();
                        m_bWatchDog[TH3] = false;

                        //IO Update
                        IO.fn_Update();

                        m_bWatchDog[TH3] = true;

                        m_bIOOn = true; 

                        Scan = TICK._GetTickTime() - Start;
                        bGo = true;
                        m_tDelayTimer[TH3].Clear();

                    }

                    if (Scan < THREAD_INTERVAL3)
                    {
                        if (!m_tDelayTimer[TH3].OnDelay(bGo, THREAD_INTERVAL3 - Scan)) continue;
                    }

                    bGo = false;
                    
                    //Scan Time Check
                    m_nScanTime [TH3] = (uint)(TICK._GetTickTime() - m_nStartTime[TH3]); 
                    m_nStartTime[TH3] = (uint) TICK._GetTickTime() ;
                
                }
            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH3] = false;
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace("[THREAD_PROCESS_3]", ex);
            }

        }

        //---------------------------------------------------------------------------
        //LoadCell
        private void THREAD_PROCESS_4()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;

            //
            try
            {
                while (m_bThreadRun[TH4])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        Start = TICK._GetTickTime();
                        m_bWatchDog[TH4] = false;

                        //Load Cell Update
                        LDCBTM.fn_Update();

                        //Top Cell Data
                        IO.fn_UpdateTopLDC();

                        m_bWatchDog[TH4] = true;

                        Scan = TICK._GetTickTime() - Start;
                        bGo = true;
                        m_tDelayTimer[TH4].Clear();
                        
                    }

                    if (Scan < THREAD_INTERVAL4)
                    {
                        if (!m_tDelayTimer[TH4].OnDelay(bGo, THREAD_INTERVAL4 - Scan)) continue;
                    }

                    bGo = false;

                    //Scan Time Check
                    m_nScanTime [TH4] = (uint)(TICK._GetTickTime() - m_nStartTime[TH4]); 
                    m_nStartTime[TH4] = (uint) TICK._GetTickTime() ;
                }
            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH4] = false;
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace("[THREAD_PROCESS_4]", ex);
            }
        }

        //---------------------------------------------------------------------------
        //PMC
        private void THREAD_PROCESS_5()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;

            
            //
            try
            {
                while (m_bThreadRun[TH5])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        Start = TICK._GetTickTime();
                        m_bWatchDog[TH5] = false;

                        //PMC Update
                        //if(FM.m_stMasterOpt.nUsePMC == 1) PMC.fn_Update();

                        IO.fn_UpdateACSReboot();

                        m_bWatchDog[TH5] = true;

                        Scan = TICK._GetTickTime() - Start;
                        bGo = true;
                        m_tDelayTimer[TH5].Clear();
                        

                    }

                    if (Scan < THREAD_INTERVAL5)
                    {
                        if (!m_tDelayTimer[TH5].OnDelay(bGo, THREAD_INTERVAL5 - Scan)) continue;
                    }

                    bGo = false;
                    


                    //Scan Time Check
                    m_nScanTime [TH5] = (uint)(TICK._GetTickTime() - m_nStartTime[TH5]); 
                    m_nStartTime[TH5] = (uint) TICK._GetTickTime() ;
                }
            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH5] = false;
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace("[THREAD_PROCESS_5]", ex);
            }
        }
        //---------------------------------------------------------------------------
        //Slurry
        private void THREAD_PROCESS_6()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;
            int nNo      = TH6;
            int nInteval = THREAD_INTERVAL6; 

            //
            try
            {
                while (m_bThreadRun[nNo])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        Start = TICK._GetTickTime();

                        m_bWatchDog[TH6] = false;

                        if (FM.m_stSystemOpt.nUseAutoSlurry == 1)
                        {
                            //Auto Supply Update
                            SUPPLY[0].fn_PlcUpdate(); //SLURRY
                            SUPPLY[1].fn_PlcUpdate(); //SOAP
                        }

                        m_bWatchDog[TH6] = true;

                        Scan = TICK._GetTickTime() - Start;
                        bGo = true;
                        m_tDelayTimer[nNo].Clear();

                    }

                    if (Scan < nInteval)
                    {
                        if (!m_tDelayTimer[nNo].OnDelay(bGo, nInteval - Scan)) continue;
                    }

                    bGo = false;
                    
                    //Scan Time Check
                    m_nScanTime [nNo] = (uint)(TICK._GetTickTime() - m_nStartTime[nNo]); 
                    m_nStartTime[nNo] = (uint) TICK._GetTickTime() ;
                }
            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH6] = false;
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace("[THREAD_PROCESS_6]", ex);
            }
        }
        //---------------------------------------------------------------------------
        private void THREAD_PROCESS_7()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;
            int idx = TH7;
            int interval = THREAD_INTERVAL7;

            //
            try
            {
                while (m_bThreadRun[idx])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        Start = TICK._GetTickTime();
                        m_bWatchDog[TH7] = false;

                        //RFID 
                        RFID.fn_Upate();
                        
                        //REST API
                        REST.fn_Upate();

                        m_bWatchDog[TH7] = true;

                        Scan = TICK._GetTickTime() - Start;
                        bGo = true;
                        m_tDelayTimer[idx].Clear();
                        

                    }

                    if (Scan < interval)
                    {
                        if (!m_tDelayTimer[idx].OnDelay(bGo, interval - Scan)) continue;
                    }

                    bGo = false;
                    

                    //Scan Time Check
                    m_nScanTime [idx] = (uint)(TICK._GetTickTime() - m_nStartTime[idx]); 
                    m_nStartTime[idx] = (uint) TICK._GetTickTime() ;
                }
            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH7] = false;
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace($"[THREAD_PROCESS_{interval}]", ex);
            }
        }

        //---------------------------------------------------------------------------
        private void THREAD_PROCESS_8()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;
            int idx = TH8;
            int interval = THREAD_INTERVAL8;

            //
            try
            {
                while (m_bThreadRun[idx])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        Start = TICK._GetTickTime();
                        m_bWatchDog[TH8] = false;

                        // Todo Light Conn


                        m_bWatchDog[TH8] = true;

                        Scan = TICK._GetTickTime() - Start;
                        bGo = true;
                        m_tDelayTimer[idx].Clear();
                        

                    }

                    if (Scan < interval)
                    {
                        if (!m_tDelayTimer[idx].OnDelay(bGo, interval - Scan)) continue;
                    }

                    bGo = false;
                    

                    //Scan Time Check
                    m_nScanTime[idx]  = (uint)(TICK._GetTickTime() - m_nStartTime[idx]); 
                    m_nStartTime[idx] = (uint) TICK._GetTickTime() ;
                }
            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH8] = false;
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace($"[THREAD_PROCESS_{interval}]", ex);
            }
        }
        //---------------------------------------------------------------------------
        private void THREAD_PROCESS_9()
        {
            bool   bGo   = false;
            double Start = 0;
            double Scan  = 0;
            int idx = TH9;
            int interval = THREAD_INTERVAL9;
            m_TryCount_Cam = 0;
            //
            try
            {
                while (m_bThreadRun[idx])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        Start = TICK._GetTickTime();
                        m_bWatchDog[TH9] = false;

                        // Cam Connection Try
                        if (g_VisionManager._CamManager.CameraRetryCount < 12)
                        {
                            if(g_VisionManager._CamManager.fn_OpenCam() == true)
                                g_VisionManager._CamManager.CameraRetryCount = 12;
                            else
                                g_VisionManager._CamManager.CameraRetryCount++;
                        }

                        m_bWatchDog[TH9] = true;

                        Scan = TICK._GetTickTime() - Start;
                        bGo = true;
                        m_tDelayTimer[idx].Clear();
                        
                    }

                    if (Scan < interval)
                    {
                        if (!m_tDelayTimer[idx].OnDelay(bGo, interval - Scan)) continue;
                    }

                    bGo = false;
                    


                    //Scan Time Check
                    m_nScanTime [idx] = (uint)(TICK._GetTickTime() - m_nStartTime[idx]); 
                    m_nStartTime[idx] = (uint) TICK._GetTickTime() ;
                }
            }
            catch (System.Exception ex)
            {
                m_bWatchDog[TH9] = false;
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace($"[THREAD_PROCESS_{interval}]", ex);
            }
        }

        //---------------------------------------------------------------------------
        private void THREAD_PROCESS_10()
        {
            bool   bGo      = false;
            double Start    = 0;
            double Scan     = 0;
            int    idx      = TH10;
            int    interval = THREAD_INTERVAL10;
            int    nIdex    = 0 ;
            
            //
            try
            {
                while (m_bThreadRun[idx])
                {
                    Thread.Sleep(1);

                    if (!bGo)
                    {
                        Start = TICK._GetTickTime();

                        //Check Thread
                        for (nIdex = 0; nIdex < MAX_THREAD-1; nIdex++)
                        {
                            if (!m_bThreadRun[nIdex]) continue;
                            
                            if (nIdex >= TH7) continue; 

                            m_tChkTimer[nIdex].OnDelay(!m_bWatchDog[nIdex] , 3000);
                            if(m_tChkTimer[nIdex].Out) 
                            {
                                //Console.WriteLine(string.Format($"THREAD ERROR_{nIdex}"));
                                //EPU.fn_SetErr(ERRID.EN_ERR_LIST.ERR_0120 + nIdex); //JUNG/200605
                            }

                        }


                        Scan = TICK._GetTickTime() - Start;
                        bGo = true;
                        m_tDelayTimer[idx].Clear();
                    }

                    if (Scan < interval)
                    {
                        if (!m_tDelayTimer[idx].OnDelay(bGo, interval - Scan)) continue;
                    }

                    bGo = false;

                    //Scan Time Check
                    m_nScanTime [idx] = (uint)(TICK._GetTickTime() - m_nStartTime[idx]); 
                    m_nStartTime[idx] = (uint) TICK._GetTickTime() ;
                }
            }
            catch (System.Exception ex)
            {
                System.Diagnostics.Trace.WriteLine(ex.Message);
                LOG.ExceptionTrace($"[THREAD_PROCESS_{interval}]", ex);
            }
        }



    }

}
