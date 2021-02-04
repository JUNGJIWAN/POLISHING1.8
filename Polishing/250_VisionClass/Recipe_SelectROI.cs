using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WaferPolishingSystem.Vision
{
    /**
    @class	Recipe_SElectROI : IPropertyChaged
    @brief	레시피 선택영역 Binding 클래스
    @remark	
     - 
    @author	선경규(Kyeong Kyu - Seon)
    @date	2020/10/6  12:15
    */
    class Recipe_SelectROI : IPropertyChanged
    {
        public int SelMode = -1;
        public int SelIdx = -1;
        private double offset = 10.0;
        public double Offset
        {
            get
            {
                return offset;
            }
            set
            {
                offset = value;
                OnPropertyChanged("Offset");
            }
        }
        private string name = "";
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged("Name");
            }
        }

        private double x = 0;
        public double X
        {
            get 
            {
                return x;
            }
            set
            {
                x = value;
                OnPropertyChanged("X");
            }
        }

        private double y = 0;
        public double Y
        {
            get
            {
                return y;
            }
            set
            {
                y = value;
                OnPropertyChanged("Y");
            }
        }

        private double w = 0;
        public double W
        {
            get
            {
                return w;
            }
            set
            {
                w = value;
                OnPropertyChanged("W");
            }
        }

        private double h = 0;
        public double H
        {
            get
            {
                return h;
            }
            set
            {
                h = value;
                OnPropertyChanged("H");
            }
        }
    }
}
