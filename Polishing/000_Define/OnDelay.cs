using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;


namespace WaferPolishingSystem.Define
{
    /**
    @class     Tick Time Class
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/1/30  17:59
    */

    public class TickTime
    {
        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);
        //--------------------------------------------------------------------------
        public double _GetTickTime ()
        {
            long liEndCounter,liFrequency ;
            QueryPerformanceCounter    (out liEndCounter);
            QueryPerformanceFrequency  (out liFrequency );
            return ( ((double)liEndCounter / (double)liFrequency) * 1000.0 );
        }
        //--------------------------------------------------------------------------
        public double _GetTickTime_us ()
        {
            long liEndCounter,liFrequency ;
            QueryPerformanceCounter  (out liEndCounter);
            QueryPerformanceFrequency(out liFrequency );
            return ( ((double)liEndCounter / (double)liFrequency) * 1000000.0 );
        }
        //--------------------------------------------------------------------------
        public string ConvTimeTickToStr(double tickCnt)
        {
            int Hour, Minute, Sec;
            string temp;


            Hour     = (int)tickCnt / 3600000;
            tickCnt -= (Hour * 3600000);
            Minute   = (int)tickCnt / 60000;
            tickCnt -= (Minute * 60000);
            Sec      = (int)tickCnt / 1000;
            tickCnt -= (Sec * 1000);

            temp = string.Format("{0:00}:{1:00}:{2:00}", Hour, Minute, Sec);
            return temp;
        }
        //--------------------------------------------------------------------------
        public int ConvTimetoSec(double tickCnt)
        {
            int Hour, Minute, Sec;
            int rtnSec = 0; 
            
            Hour     = (int)tickCnt / 3600000;
            tickCnt -= (Hour * 3600000);
            Minute   = (int)tickCnt / 60000;
            tickCnt -= (Minute * 60000);
            Sec      = (int)tickCnt / 1000;
            tickCnt -= (Sec * 1000);

            rtnSec = Sec + (Minute * 60) + (Hour * 60 * 60);

            return rtnSec;
        }

    }

    //---------------------------------------------------------------------------
    /**
    @class     Delay Timer Class
    @brief     
    @remark    
    -    
    @author    정지완(JUNGJIWAN)
    @date      2020/1/30  17:59
    */

    public class TOnDelayTimer : TickTime
    {
        bool   bScan      ; //0 : TickCount사용,  1 : Scan 단위
        double PreTickTime;

        public bool   Out    ;
        public double SetTime; //[ms]설정 시간
        public double CurTime; //[ms]현재 시간 (0 -> 설정시간 )

        //~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
        //생성자 & 소멸자. (Constructor & Destructor)
        public TOnDelayTimer()
        {
            bScan       = false;
            PreTickTime = 0.0;
            Out         = false;
            Clear();
        }

        //---------------------------------------------------------------------------
        public TOnDelayTimer(bool _bScan  ) { bScan = _bScan; Clear(); }
        //---------------------------------------------------------------------------
        public void Clear        (        ) { OnDelay(false , 0); }
        //---------------------------------------------------------------------------
        public bool OnDelay      (bool SeqInput, double _SetTime) { SetTime = _SetTime; return OnDelay(SeqInput); }
        //---------------------------------------------------------------------------
        public bool OnDelay      (bool SeqInput)
        {
            double CurTick = _GetTickTime();
            if (SeqInput) {
                if (bScan)	CurTime++;
                else		CurTime += CurTick - PreTickTime;
                if (CurTime >= SetTime) { CurTime = SetTime; Out = true ; }
                else                                         Out = false;
                }
            else {
                CurTime = 0;
                Out = false;
                }
            PreTickTime = CurTick;
            return Out;
        }
    }

}
