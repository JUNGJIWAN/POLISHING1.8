using System;
using System.Windows.Media.Imaging;
using System.Runtime.InteropServices;
using WaferPolishingSystem.Define;
using static WaferPolishingSystem.Define.UserFunction;
using static WaferPolishingSystem.Define.UserFunctionVision;


namespace WaferPolishingSystem.Vision
{  
    public class AlignManager
    {
        public ST_ALIGN_RESULT _stResult = new ST_ALIGN_RESULT();
        public ModelList _recipe = new ModelList();
        IntPtr _PtrImage;
        IntPtr _PtrMark;
        string Title = "AlignManager";
        
        public AlignManager()
        {
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		public fn_SetMarkStream
        @brief	Mark 이미지 세팅
        @return	void
        @param	FileStream  ms
        @param	double         width
        @param	double         height
        @param	double         channel
        @remark	
        -   DLL에 Mark Image 세팅
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/9  20:01
        */
        public void fn_SetMarkStream(WriteableBitmap wb)
        {
            if (wb != null)
            {
                _PtrMark = wb.BackBuffer;
                if (_PtrMark != null)
                {
                    libSetMark(_PtrMark, wb.Width, wb.Height, 1);
                    fn_WriteLog(this.Title + " : Model(Mark) Set Lib.", UserEnum.EN_LOG_TYPE.ltVision);
                }
                else
                    fn_WriteLog(this.Title + " : Model(Mark) Set Lib Fail.", UserEnum.EN_LOG_TYPE.ltVision);
            }
            else
            {
                fn_WriteLog(this.Title + " : MarkImage Is Null.", UserEnum.EN_LOG_TYPE.ltVision);
            }
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		public fn_SetImageStream
        @brief	Search 이미지 세팅
        @return	void
        @param	FileStream  ms
        @param	double         width
        @param	double         height
        @param	double         channel
        @remark	
        -   DLL에 Search Image 세팅
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/9  20:01
        */
        public void fn_SetImageStream(WriteableBitmap wb)
        {
            if (wb != null)
            {
                _PtrImage = wb.BackBuffer;
                if (_PtrImage != null)
                {
                    libSetImage(_PtrImage, wb.Width, wb.Height, 1);
                    fn_WriteLog(this.Title + " : Image Set Lib.", UserEnum.EN_LOG_TYPE.ltVision);
                }
                else
                    fn_WriteLog(this.Title + " : Image Set Fail.", UserEnum.EN_LOG_TYPE.ltVision);
            }
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		public fn_SetParameter
        @brief	DLL에 파라메터 설정.
        @return	void
        @param	ST_PARAM_MODEL   model
        @param	ST_PARAM_PATTERN pattern
        @remark	
        -   DLL에 파라메터 설정.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/9  20:01
        */
        public void fn_SetParameter(ModelList recipe, bool bPre = false)
        {
            //fn_WriteLog("Set Align Param.", UserEnum.EN_LOG_TYPE.ltVision);
            _recipe = recipe;
            fn_WriteLog(this.Title + " : Set Param To Dll.", UserEnum.EN_LOG_TYPE.ltVision);
            if (bPre)
                fn_SetParamToDLLPreAlign();
            else
                fn_SetParamToDLL();
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		public fn_RunAlignment
        @brief	Align 실행.
        @return	void
        @param  void
        @remark	
        -   Mark Search 실행.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/9  20:01
        */
        public void fn_RunAlignment(bool bModel = true)
        {
            //fn_WriteLog("Align Start.", UserEnum.EN_LOG_TYPE.ltVision);
            if (bModel)
            {
                fn_WriteLog(this.Title + " : ModelFinder Run.", UserEnum.EN_LOG_TYPE.ltVision);
                libRunProcModel();
                fn_WriteLog(this.Title + " : ModelFinder End.", UserEnum.EN_LOG_TYPE.ltVision);
            }
            else
            {
                fn_WriteLog(this.Title + " : PatternMatching Run.", UserEnum.EN_LOG_TYPE.ltVision);
                libRunProcPattern();
                fn_WriteLog(this.Title + " : PatternMatching End.", UserEnum.EN_LOG_TYPE.ltVision);
            }
        }
        //---------------------------------------------------------------------------
        /**	
        @fn		private fn_SetParamToDLL()
        @brief	DLL에 파라메터 설정.
        @return	void
        @param  void
        @remark	
        -   DLL에 파라메터 세팅.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/9  20:01
        */
        private void fn_SetParamToDLL()
        {
            IntPtr pModel = Marshal.AllocHGlobal(Marshal.SizeOf(_recipe.ParamModel));
            IntPtr pPattern = Marshal.AllocHGlobal(Marshal.SizeOf(_recipe.ParamPattern));
            try
            {
                Marshal.StructureToPtr(_recipe.ParamModel, pModel, true);
                Marshal.StructureToPtr(_recipe.ParamPattern, pPattern, true);
                libSetParam(pModel, pPattern);
                fn_WriteLog(this.Title + " : Set Param Lib.", UserEnum.EN_LOG_TYPE.ltVision);
            }
            catch (System.Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            finally
            {
                Marshal.FreeHGlobal(pModel);
                Marshal.FreeHGlobal(pPattern);
            }
        }
        /**	
        @fn		private void fn_SetParamToDLLPreAlign()
        @brief	DLL에 파라메터 설정. (PreAlign)
        @return	void
        @param  void
        @remark	
        -   DLL에 파라메터 세팅.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/9  20:01
        */
        private void fn_SetParamToDLLPreAlign()
        {
            IntPtr pModel = Marshal.AllocHGlobal(Marshal.SizeOf(_recipe.LoadingParam));
            try
            {
                Marshal.StructureToPtr(_recipe.LoadingParam, pModel, true);
                libSetParam(pModel, IntPtr.Zero);
                fn_WriteLog(this.Title + " : Set Param Lib. (PreAlgin)", UserEnum.EN_LOG_TYPE.ltVision);
            }
            catch (System.Exception ex)
            {
                fn_WriteLog(this.Title + " : " + ex.Message, UserEnum.EN_LOG_TYPE.ltVision);
            }
            finally
            {
                Marshal.FreeHGlobal(pModel);
            }
        }

        //---------------------------------------------------------------------------
        /**	
        @fn		public fn_GetSearchResult
        @brief	Search 결과 얻기.
        @return	ST_ALIGN_RESULT
        @param  void
        @remark	
        -   Search 결과 얻기.
        @author	선경규(Kyeong Kyu - Seon)
        @date	2020/2/9  20:01
        */
        public ST_ALIGN_RESULT fn_GetSearchResult()
        {
            IntPtr pResult = Marshal.AllocHGlobal(Marshal.SizeOf(_stResult));
            Marshal.StructureToPtr(_stResult, pResult, true);
            libGetResult(pResult);
            fn_WriteLog(this.Title + " : Get Result.", UserEnum.EN_LOG_TYPE.ltVision);
            _stResult = (ST_ALIGN_RESULT)Marshal.PtrToStructure(pResult, typeof(ST_ALIGN_RESULT));
            return _stResult;
        }
        //---------------------------------------------------------------------------
    }
}

